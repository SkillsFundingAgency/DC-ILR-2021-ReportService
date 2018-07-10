using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using ESFA.DC.ILR.ValidationErrors.Interface.Models;
using ESFA.DC.ILR1819.ReportService.Interface;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.JobContext;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR1819.ReportService.Service
{
    public sealed class EntryPoint
    {
        private readonly ILogger _logger;
        private readonly IKeyValuePersistenceService _storage;
        private readonly IKeyValuePersistenceService _redis;
        private readonly IXmlSerializationService _xmlSerializationService;
        private readonly IJsonSerializationService _jsonSerializationService;

        public EntryPoint(
            ILogger logger,
            [KeyFilter(PersistenceStorageKeys.Blob)] IKeyValuePersistenceService storage,
            [KeyFilter(PersistenceStorageKeys.Redis)] IKeyValuePersistenceService redis,
            IXmlSerializationService xmlSerializationService,
            IJsonSerializationService jsonSerializationService)
        {
            _logger = logger;
            _storage = storage;
            _redis = redis;
            _xmlSerializationService = xmlSerializationService;
            _jsonSerializationService = jsonSerializationService;
        }

        public async Task<bool> Callback(JobContextMessage jobContextMessage, CancellationToken cancellationToken)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            List<ValidationErrorDto> validationErrorDtos =
                await ReadAndDeserialiseValidationErrorsAsync(jobContextMessage);
            await PeristValuesToStorage(jobContextMessage, validationErrorDtos);
            _logger.LogDebug($"Persisted validation errors to csv in: {stopWatch.ElapsedMilliseconds}");
            stopWatch.Restart();

            return true;
        }

        private async Task PeristValuesToStorage(IJobContextMessage jobContextMessage, List<ValidationErrorDto> validationErrorDtos)
        {
            string key = jobContextMessage.KeyValuePairs[JobContextMessageKey.ValidationErrors]
                .ToString();

            await _storage.SaveAsync($"{key}.json", _jsonSerializationService.Serialize(validationErrorDtos));
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

        private string GetValidationErrorParameters(List<ValidationErrorParameter> validationErrorParameters)
        {
            var result = new System.Text.StringBuilder();
            validationErrorParameters.ForEach(x =>
            {
                result.Append($"{x.PropertyName}={x.Value}");
                result.Append("|");
            });

            return result.ToString();
        }
    }
}
