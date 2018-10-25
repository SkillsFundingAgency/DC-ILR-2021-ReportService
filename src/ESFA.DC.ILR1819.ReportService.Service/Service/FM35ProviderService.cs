using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output;
using ESFA.DC.ILR1819.ReportService.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.JobContextManager.Model.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR1819.ReportService.Service.Service
{
    public class FM35ProviderService : IFM35ProviderService
    {
        private readonly ILogger _logger;

        private readonly IKeyValuePersistenceService _redis;
        private readonly IKeyValuePersistenceService _blob;

        private readonly IJsonSerializationService _jsonSerializationService;

        private readonly SemaphoreSlim _getDataLock;

        private bool _loadedDataAlready;

        private FM35Global _fundingOutputs;

        public FM35ProviderService(
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

        public async Task<FM35Global> GetFM35Data(IJobContextMessage jobContextMessage, CancellationToken cancellationToken)
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
                string fm35Filename = jobContextMessage.KeyValuePairs[JobContextMessageKey.FundingFm35Output].ToString();
                string fm35 = await _redis.GetAsync(fm35Filename, cancellationToken);

                if (string.IsNullOrEmpty(fm35))
                {
                    _fundingOutputs = null;
                    return _fundingOutputs;
                }

                // await _blob.SaveAsync($"{jobContextMessage.KeyValuePairs[JobContextMessageKey.UkPrn]}_{jobContextMessage.JobId.ToString()}_Fm35.json", fm35, cancellationToken);
                _fundingOutputs = _jsonSerializationService.Deserialize<FM35Global>(fm35);
            }
            catch (Exception ex)
            {
                // Todo: Check behaviour
                _logger.LogError("Failed to get & deserialise FM35 funding data", ex);
            }
            finally
            {
                _getDataLock.Release();
            }

            return _fundingOutputs;
        }
    }
}
