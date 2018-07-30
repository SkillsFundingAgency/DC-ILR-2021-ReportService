using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using ESFA.DC.ILR1819.ReportService.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR1819.ReportService.Service.Service
{
    public sealed class ValidLearnersService : IValidLearnersService
    {
        private readonly ILogger _logger;
        private readonly IKeyValuePersistenceService _redis;
        private readonly IJsonSerializationService _jsonSerializationService;

        private readonly SemaphoreSlim _getDataLock;

        private bool _loadedDataAlready;

        private List<string> _loadedData;

        public ValidLearnersService(
            ILogger logger,
            [KeyFilter(PersistenceStorageKeys.Redis)] IKeyValuePersistenceService redis,
            IJsonSerializationService jsonSerializationService)
        {
            _logger = logger;
            _redis = redis;
            _jsonSerializationService = jsonSerializationService;
            _loadedData = null;
            _getDataLock = new SemaphoreSlim(1, 1);
        }

        public async Task<List<string>> GetValidLearnersAsync(IJobContextMessage jobContextMessage)
        {
            await _getDataLock.WaitAsync();

            try
            {
                if (_loadedDataAlready)
                {
                    return _loadedData;
                }

                _loadedDataAlready = true;
                string learnersValidStr = await _redis.GetAsync(jobContextMessage.KeyValuePairs[JobContextMessageKey.ValidLearnRefNumbers].ToString());
                _loadedData = _jsonSerializationService.Deserialize<List<string>>(learnersValidStr);
            }
            catch (Exception ex)
            {
                // Todo: Check behaviour
                _logger.LogError("Failed to get & deserialise valid learners", ex);
            }
            finally
            {
                _getDataLock.Release();
            }

            return _loadedData;
        }
    }
}
