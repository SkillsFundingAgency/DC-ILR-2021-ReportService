using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using CsvHelper;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationErrors.Interface.Models;
using ESFA.DC.ILR1819.ReportService.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Reports;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.Report;
using ESFA.DC.ILR1819.ReportService.Service.Helper;
using ESFA.DC.ILR1819.ReportService.Service.Mapper;
using ESFA.DC.ILR1819.ReportService.Service.Model;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.Jobs.Model.Reports.ValidationReport;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR1819.ReportService.Service.Reports
{
    public sealed class ValidationErrorsReport : IReport
    {
        private readonly ILogger _logger;
        private readonly IKeyValuePersistenceService _storage;
        private readonly IKeyValuePersistenceService _redis;
        private readonly IXmlSerializationService _xmlSerializationService;
        private readonly IJsonSerializationService _jsonSerializationService;
        private readonly IIlrProviderService _ilrProviderService;
        private readonly IValidLearnersService _validLearnersService;

        public ValidationErrorsReport(
            ILogger logger,
            [KeyFilter(PersistenceStorageKeys.Blob)] IKeyValuePersistenceService storage,
            [KeyFilter(PersistenceStorageKeys.Redis)] IKeyValuePersistenceService redis,
            IXmlSerializationService xmlSerializationService,
            IJsonSerializationService jsonSerializationService,
            IIlrProviderService ilrProviderService,
            IValidLearnersService validLearnersService)
        {
            _logger = logger;
            _storage = storage;
            _redis = redis;
            _xmlSerializationService = xmlSerializationService;
            _jsonSerializationService = jsonSerializationService;
            _ilrProviderService = ilrProviderService;
            _validLearnersService = validLearnersService;
        }

        public ReportType ReportType { get; } = ReportType.ValidationErrors;

        public async Task GenerateReport(IJobContextMessage jobContextMessage)
        {
            IMessage message = await _ilrProviderService.GetIlrFile(jobContextMessage);
            string key = jobContextMessage.KeyValuePairs[JobContextMessageKey.ValidationErrors].ToString();
            //string validLearnersStr = await _redis.GetAsync(jobContextMessage
            //    .KeyValuePairs[JobContextMessageKey.ValidLearnRefNumbers]
            //    .ToString());

            List<ValidationErrorDto> validationErrorDtos = await ReadAndDeserialiseValidationErrorsAsync(jobContextMessage);
            List<ValidationErrorModel> validationErrors = ValidationErrorModels(validationErrorDtos, message);
            IlrValidationReport ilrValidationReport = PersistFrontEndValidationReport(jobContextMessage, message, validationErrorDtos);
            await PeristValuesToStorage(key, validationErrors, ilrValidationReport);
        }

        private IlrValidationReport PersistFrontEndValidationReport(IJobContextMessage jobContextMessage, IMessage message, List<ValidationErrorDto> validationErrorDtos)
        {
            //int validLearners = Convert.ToInt32(jobContextMessage.KeyValuePairs[JobContextMessageKey.ValidLearnRefNumbersCount]);
            //int invalidLearners = Convert.ToInt32(jobContextMessage.KeyValuePairs[JobContextMessageKey.InvalidLearnRefNumbersCount]);

            var errors = validationErrorDtos.Where(x => string.Equals(x.Severity, "E", StringComparison.OrdinalIgnoreCase)).ToArray();
            var warnings = validationErrorDtos.Where(x => string.Equals(x.Severity, "W", StringComparison.OrdinalIgnoreCase)).ToArray();

            IlrValidationReport validationReport = new IlrValidationReport
            {
                TotalErrors = errors.Length,
                TotalWarnings = warnings.Length,
                WarningLearners = warnings.DistinctBy(x => x.LearnerReferenceNumber).Count(),
                ErrorLearners = errors.DistinctBy(x => x.LearnerReferenceNumber).Count()
            };

            return validationReport;
        }

        private async Task<List<ValidationErrorDto>> ReadAndDeserialiseValidationErrorsAsync(IJobContextMessage jobContextMessage)
        {
            string validationErrorsStr = await _redis.GetAsync(jobContextMessage.KeyValuePairs[JobContextMessageKey.ValidationErrors]
                .ToString());

            string validationErrorLookups = await _redis.GetAsync(jobContextMessage.KeyValuePairs[JobContextMessageKey.ValidationErrorLookups]
                .ToString());

            List<ValidationErrorDto> result = new List<ValidationErrorDto>();
            try
            {
                List<ValidationError> validationErrors = _jsonSerializationService.Deserialize<List<ValidationError>>(validationErrorsStr);

                List<ValidationErrorMessageLookup> validationErrorMessageLookups =
                    _jsonSerializationService.Deserialize<List<ValidationErrorMessageLookup>>(validationErrorLookups);

                validationErrors.ToList().ForEach(x =>
                    result.Add(new ValidationErrorDto
                    {
                        AimSequenceNumber = x.AimSequenceNumber,
                        LearnerReferenceNumber = x.LearnerReferenceNumber,
                        RuleName = x.RuleName,
                        Severity = x.Severity,
                        ErrorMessage = validationErrorMessageLookups.Single(y => x.RuleName == y.RuleName).Message,
                        FieldValues = x.ValidationErrorParameters == null ? string.Empty : GetValidationErrorParameters(x.ValidationErrorParameters.ToList()),
                    }));
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to merge validation error messages", ex);
            }

            if (result.Count == 0)
            {
                _logger.LogError("Falling back to old behaviour");
                result = _jsonSerializationService.Deserialize<List<ValidationErrorDto>>(validationErrorsStr);
            }

            return result;
        }

        private async Task PeristValuesToStorage(string key, List<ValidationErrorModel> validationErrorModels, IlrValidationReport ilrValidationReport)
        {
            await _storage.SaveAsync($"{key}.json", _jsonSerializationService.Serialize(ilrValidationReport));
            await _storage.SaveAsync($"{key}.csv", GetCsv(validationErrorModels));
        }

        private string GetCsv(List<ValidationErrorModel> validationErrorModels)
        {
            StringBuilder sb = new StringBuilder();

            using (TextWriter textWriter = new StringWriter(sb))
            {
                using (CsvWriter csvWriter = new CsvWriter(textWriter))
                {
                    csvWriter.Configuration.RegisterClassMap<ValidationErrorMapper>();
                    csvWriter.WriteHeader<ValidationErrorModel>();
                    csvWriter.NextRecord();
                    csvWriter.WriteRecords(validationErrorModels);
                }
            }

            return sb.ToString();
        }

        private List<ValidationErrorModel> ValidationErrorModels(List<ValidationErrorDto> validationErrorDtos, IMessage message)
        {
            List<ValidationErrorModel> validationErrors = new List<ValidationErrorModel>(validationErrorDtos.Count);
            foreach (ValidationErrorDto validationErrorDto in validationErrorDtos)
            {
                if (message == null)
                {
                    validationErrors.Add(new ValidationErrorModel(
                        validationErrorDto.Severity,
                        validationErrorDto.LearnerReferenceNumber,
                        validationErrorDto.RuleName,
                        validationErrorDto.FieldValues,
                        validationErrorDto.ErrorMessage,
                        validationErrorDto.AimSequenceNumber));
                }
                else
                {
                    ILearner learner = message.Learners.FirstOrDefault(x => x.LearnRefNumber == validationErrorDto.LearnerReferenceNumber);
                    if (learner == null)
                    {
                        _logger.LogWarning($"Can't find learner {validationErrorDto.LearnerReferenceNumber}");
                    }

                    ILearningDelivery learningDelivery = learner?.LearningDeliveries.FirstOrDefault(x => x.AimSeqNumber == validationErrorDto.AimSequenceNumber);
                    if (learningDelivery == null)
                    {
                        _logger.LogWarning(
                            $"Can't find learning delivery {validationErrorDto.AimSequenceNumber} for learner {validationErrorDto.LearnerReferenceNumber}");
                    }

                    validationErrors.Add(new ValidationErrorModel(
                        validationErrorDto.Severity,
                        validationErrorDto.LearnerReferenceNumber,
                        validationErrorDto.RuleName,
                        validationErrorDto.FieldValues,
                        validationErrorDto.ErrorMessage,
                        validationErrorDto.AimSequenceNumber,
                        learningDelivery?.LearnAimRef,
                        learningDelivery?.SWSupAimId,
                        learningDelivery?.FundModel ?? -1,
                        learningDelivery?.PartnerUKPRNNullable,
                        learner?.ProviderSpecLearnerMonitorings?.FirstOrDefault(x => x.ProvSpecLearnMonOccur == "A")?.ProvSpecLearnMon,
                        learner?.ProviderSpecLearnerMonitorings?.FirstOrDefault(x => x.ProvSpecLearnMonOccur == "B")?.ProvSpecLearnMon,
                        learningDelivery?.ProviderSpecDeliveryMonitorings?.FirstOrDefault(x => x.ProvSpecDelMonOccur == "A")?.ProvSpecDelMon,
                        learningDelivery?.ProviderSpecDeliveryMonitorings?.FirstOrDefault(x => x.ProvSpecDelMonOccur == "B")?.ProvSpecDelMon,
                        learningDelivery?.ProviderSpecDeliveryMonitorings?.FirstOrDefault(x => x.ProvSpecDelMonOccur == "C")?.ProvSpecDelMon,
                        learningDelivery?.ProviderSpecDeliveryMonitorings?.FirstOrDefault(x => x.ProvSpecDelMonOccur == "D")?.ProvSpecDelMon));
                }
            }

            return validationErrors;
        }

        private string GetValidationErrorParameters(List<ValidationErrorParameter> validationErrorParameters)
        {
            StringBuilder result = new StringBuilder();
            validationErrorParameters.ForEach(x =>
            {
                result.Append($"{x.PropertyName}={x.Value}|");
            });

            return result.ToString();
        }
    }
}
