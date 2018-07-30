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
using ESFA.DC.ILR1819.ReportService.Service.Mapper;
using ESFA.DC.ILR1819.ReportService.Service.Model;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.JobContext.Interface;
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

        public ValidationErrorsReport(
            ILogger logger,
            [KeyFilter(PersistenceStorageKeys.Blob)] IKeyValuePersistenceService storage,
            [KeyFilter(PersistenceStorageKeys.Redis)] IKeyValuePersistenceService redis,
            IXmlSerializationService xmlSerializationService,
            IJsonSerializationService jsonSerializationService,
            IIlrProviderService ilrProviderService)
        {
            _logger = logger;
            _storage = storage;
            _redis = redis;
            _xmlSerializationService = xmlSerializationService;
            _jsonSerializationService = jsonSerializationService;
            _ilrProviderService = ilrProviderService;
        }

        public ReportType ReportType { get; } = ReportType.ValidationErrors;

        public async Task GenerateReport(IJobContextMessage jobContextMessage)
        {
            List<ValidationErrorDto> validationErrors = await ReadAndDeserialiseValidationErrorsAsync(jobContextMessage);
            await PeristValuesToStorage(jobContextMessage, validationErrors);
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

        private async Task PeristValuesToStorage(IJobContextMessage jobContextMessage, List<ValidationErrorDto> validationErrorDtos)
        {
            string key = jobContextMessage.KeyValuePairs[JobContextMessageKey.ValidationErrors]
                .ToString();

            await _storage.SaveAsync($"{key}.json", _jsonSerializationService.Serialize(validationErrorDtos));
            await _storage.SaveAsync($"{key}.csv", await GetCsv(validationErrorDtos, jobContextMessage));
        }

        private async Task<string> GetCsv(List<ValidationErrorDto> validationErrorDtos, IJobContextMessage jobContextMessage)
        {
            IMessage message = await _ilrProviderService.GetIlrFile(jobContextMessage);

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
                    ILearner learner = message.Learners
                        .First(x => x.LearnRefNumber == validationErrorDto.LearnerReferenceNumber);
                    ILearningDelivery learningDelivery =
                        learner.LearningDeliveries.First(x => x.AimSeqNumber == validationErrorDto.AimSequenceNumber);
                    validationErrors.Add(new ValidationErrorModel(
                        validationErrorDto.Severity,
                        validationErrorDto.LearnerReferenceNumber,
                        validationErrorDto.RuleName,
                        validationErrorDto.FieldValues,
                        validationErrorDto.ErrorMessage,
                        validationErrorDto.AimSequenceNumber,
                        learningDelivery.LearnAimRef,
                        learningDelivery.SWSupAimId,
                        learningDelivery.FundModel,
                        learningDelivery.PartnerUKPRNNullable,
                        learner.ProviderSpecLearnerMonitorings?.FirstOrDefault(x => x.ProvSpecLearnMonOccur == "A")?.ProvSpecLearnMon,
                        learner.ProviderSpecLearnerMonitorings?.FirstOrDefault(x => x.ProvSpecLearnMonOccur == "B")?.ProvSpecLearnMon,
                        learningDelivery.ProviderSpecDeliveryMonitorings?.FirstOrDefault(x => x.ProvSpecDelMonOccur == "A")?.ProvSpecDelMon,
                        learningDelivery.ProviderSpecDeliveryMonitorings?.FirstOrDefault(x => x.ProvSpecDelMonOccur == "B")?.ProvSpecDelMon,
                        learningDelivery.ProviderSpecDeliveryMonitorings?.FirstOrDefault(x => x.ProvSpecDelMonOccur == "C")?.ProvSpecDelMon,
                        learningDelivery.ProviderSpecDeliveryMonitorings?.FirstOrDefault(x => x.ProvSpecDelMonOccur == "D")?.ProvSpecDelMon));
                }
            }

            StringBuilder sb = new StringBuilder();

            using (TextWriter textWriter = new StringWriter(sb))
            {
                using (CsvWriter csvWriter = new CsvWriter(textWriter))
                {
                    csvWriter.Configuration.RegisterClassMap<ValidationErrorMapper>();
                    csvWriter.WriteHeader<ValidationErrorModel>();
                    csvWriter.NextRecord();
                    csvWriter.WriteRecords(validationErrors);
                }
            }

            return sb.ToString();
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
