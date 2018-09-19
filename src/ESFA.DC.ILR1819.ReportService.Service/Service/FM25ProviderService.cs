using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using ESFA.DC.ILR.FundingService.FM25.Model.Output;
using ESFA.DC.ILR1819.ReportService.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR1819.ReportService.Service.Service
{
    public class FM25ProviderService : IFM25ProviderService
    {
        private readonly ILogger _logger;

        private readonly IKeyValuePersistenceService _redis;
        private readonly IKeyValuePersistenceService _blob;

        private readonly IJsonSerializationService _jsonSerializationService;

        private readonly SemaphoreSlim _getDataLock;

        private bool _loadedDataAlready;

        private Global _fundingOutputs;

        public FM25ProviderService(
            ILogger logger,
            [KeyFilter(PersistenceStorageKeys.Redis)] IKeyValuePersistenceService redis,
            [KeyFilter(PersistenceStorageKeys.Blob)] IKeyValuePersistenceService blob,
            IJsonSerializationService jsonSerializationService)
        {
            _logger = logger;
            _redis = redis;
            _blob = blob;
            _jsonSerializationService = jsonSerializationService;
            _fundingOutputs = null;
            _getDataLock = new SemaphoreSlim(1, 1);
        }

        public async Task<Global> GetFM25Data(IJobContextMessage jobContextMessage, CancellationToken cancellationToken)
        {
            await _getDataLock.WaitAsync(cancellationToken);

            try
            {
                if (_loadedDataAlready)
                {
                    return _fundingOutputs;
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    return null;
                }

                _loadedDataAlready = true;
                string fm25Filename = jobContextMessage.KeyValuePairs[JobContextMessageKey.FundingFm25Output].ToString();
                string fm25 = await _redis.GetAsync(fm25Filename, cancellationToken);

                if (string.IsNullOrEmpty(fm25))
                {
                    _fundingOutputs = null;
                    return _fundingOutputs;
                }

                await _blob.SaveAsync($"{jobContextMessage.KeyValuePairs[JobContextMessageKey.UkPrn]}_{jobContextMessage.JobId.ToString()}_Fm25.json", fm25, cancellationToken);
                _fundingOutputs = _jsonSerializationService.Deserialize<Global>(fm25);
            }
            catch (Exception ex)
            {
                // Todo: Check behaviour
                _logger.LogError("Failed to get & deserialise FM25 funding data", ex);
            }
            finally
            {
                _getDataLock.Release();
            }

            return _fundingOutputs;
        }
    }
}
