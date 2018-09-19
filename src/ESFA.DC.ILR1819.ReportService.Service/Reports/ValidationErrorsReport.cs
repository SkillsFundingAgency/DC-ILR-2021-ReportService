using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationErrors.Interface.Models;
using ESFA.DC.ILR1819.ReportService.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Configuration;
using ESFA.DC.ILR1819.ReportService.Interface.Reports;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;
using ESFA.DC.ILR1819.ReportService.Service.Helper;
using ESFA.DC.ILR1819.ReportService.Service.Mapper;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR1819.ReportService.Service.Reports
{
    public sealed class ValidationErrorsReport : AbstractReportBuilder, IReport
    {
        private readonly ILogger _logger;
        private readonly IStreamableKeyValuePersistenceService _storage;
        private readonly IKeyValuePersistenceService _redis;
        private readonly IJsonSerializationService _jsonSerializationService;
        private readonly IIlrProviderService _ilrProviderService;

        private string _externalFileName;
        private string _fileName;
        private IlrValidationResult _ilrValidationResult;

        public ValidationErrorsReport(
            ILogger logger,
            [KeyFilter(PersistenceStorageKeys.Redis)] IKeyValuePersistenceService redis,
            IStreamableKeyValuePersistenceService storage,
            IJsonSerializationService jsonSerializationService,
            IIlrProviderService ilrProviderService,
            IDateTimeProvider dateTimeProvider,
            ITopicAndTaskSectionOptions topicAndTaskSectionOptions)
        : base(dateTimeProvider)
        {
            _logger = logger;
            _storage = storage;
            _redis = redis;
            _jsonSerializationService = jsonSerializationService;
            _ilrProviderService = ilrProviderService;

            ReportFileName = "Validation Errors Report";
            ReportTaskName = topicAndTaskSectionOptions.TopicReports_TaskGenerateValidationReport;
        }

        public async Task GenerateReport(IJobContextMessage jobContextMessage, ZipArchive archive, CancellationToken cancellationToken)
        {
            Task<IMessage> ilrTask = _ilrProviderService.GetIlrFile(jobContextMessage, cancellationToken);
            Task<List<ValidationErrorDto>> validationErrorDtosTask = ReadAndDeserialiseValidationErrorsAsync(jobContextMessage);
            await Task.WhenAll(ilrTask, validationErrorDtosTask);

            long jobId = jobContextMessage.JobId;
            string ukPrn = jobContextMessage.KeyValuePairs[JobContextMessageKey.UkPrn].ToString();
            _externalFileName = GetExternalFilename(ukPrn, jobId, jobContextMessage.SubmissionDateTimeUtc);
            _fileName = GetFilename(ukPrn, jobId, jobContextMessage.SubmissionDateTimeUtc);

            List<ValidationErrorDto> validationErrorDtos = validationErrorDtosTask.Result;

            List<ValidationErrorModel> validationErrors = ValidationErrorModels(ilrTask.Result, validationErrorDtos);
            GenerateFrontEndValidationReport(jobContextMessage.KeyValuePairs, validationErrorDtos);

            await PeristValuesToStorage(validationErrors, archive, cancellationToken);
        }

        private void GenerateFrontEndValidationReport(
            IDictionary<string, object> keyValuePairs,
            List<ValidationErrorDto> validationErrorDtos)
        {
            var errors = validationErrorDtos.Where(x => string.Equals(x.Severity, "E", StringComparison.OrdinalIgnoreCase)).ToArray();
            var warnings = validationErrorDtos.Where(x => string.Equals(x.Severity, "W", StringComparison.OrdinalIgnoreCase)).ToArray();

            _ilrValidationResult = new IlrValidationResult
            {
                TotalLearners = GetNumberOfLearners(keyValuePairs),
                TotalErrors = errors.Length,
                TotalWarnings = warnings.Length,
                TotalWarningLearners = warnings.DistinctByCount(x => x.LearnerReferenceNumber),
                TotalErrorLearners = errors.DistinctByCount(x => x.LearnerReferenceNumber)
            };
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

                validationErrors?.ToList().ForEach(x =>
                    result.Add(new ValidationErrorDto
                    {
                        AimSequenceNumber = x.AimSequenceNumber,
                        LearnerReferenceNumber = x.LearnerReferenceNumber,
                        RuleName = x.RuleName,
                        Severity = x.Severity,
                        ErrorMessage = validationErrorMessageLookups?.SingleOrDefault(y => x.RuleName == y.RuleName)?.Message,
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
                try
                {
                    result = _jsonSerializationService.Deserialize<List<ValidationErrorDto>>(validationErrorsStr);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Old behaviour failed", ex);
                }
            }

            return result;
        }

        private async Task PeristValuesToStorage(List<ValidationErrorModel> validationErrors, ZipArchive archive, CancellationToken cancellationToken)
        {
            string csv = GetCsv(validationErrors);
            await _storage.SaveAsync($"{_externalFileName}.json", _jsonSerializationService.Serialize(_ilrValidationResult), cancellationToken);
            await _storage.SaveAsync($"{_externalFileName}.csv", csv, cancellationToken);

            await WriteZipEntry(archive, $"{_fileName}.csv", csv);
            using (MemoryStream ms = new MemoryStream())
            {
                BuildXlsReport(ms, new ValidationErrorMapper(), validationErrors);
                await _storage.SaveAsync($"{_externalFileName}.xlsx", ms, cancellationToken);
                await WriteZipEntry(archive, $"{_fileName}.xlsx", ms, cancellationToken);
            }
        }

        private string GetCsv(List<ValidationErrorModel> validationErrorModels)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BuildCsvReport<ValidationErrorMapper, ValidationErrorModel>(ms, validationErrorModels);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        private List<ValidationErrorModel> ValidationErrorModels(IMessage message, List<ValidationErrorDto> validationErrorDtos)
        {
            var validationErrors = new List<ValidationErrorModel>();
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
                    ILearner learner = message.Learners?.FirstOrDefault(x => x.LearnRefNumber == validationErrorDto.LearnerReferenceNumber);
                    if (learner == null)
                    {
                        _logger.LogWarning($"Can't find learner {validationErrorDto.LearnerReferenceNumber}");
                    }

                    ILearningDelivery learningDelivery = learner?.LearningDeliveries?.FirstOrDefault(x => x.AimSeqNumber == validationErrorDto.AimSequenceNumber);
                    if (learningDelivery == null)
                    {
                        _logger.LogWarning(
                            $"Can't find learning delivery {validationErrorDto.AimSequenceNumber} for learner {validationErrorDto.LearnerReferenceNumber}. This may be ok for some validation rules.");
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

        private int GetNumberOfLearners(IDictionary<string, object> keyValuePairs)
        {
            int ret = 0;
            try
            {
                if (keyValuePairs.ContainsKey(JobContextMessageKey.ValidLearnRefNumbersCount))
                {
                    ret = Convert.ToInt32(keyValuePairs[JobContextMessageKey.ValidLearnRefNumbersCount]);
                }

                if (keyValuePairs.ContainsKey(JobContextMessageKey.InvalidLearnRefNumbersCount))
                {
                    ret = ret + Convert.ToInt32(keyValuePairs[JobContextMessageKey.InvalidLearnRefNumbersCount]);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Can't read number of learners", ex);
            }

            return ret;
        }
    }
}
