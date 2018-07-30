using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using ESFA.DC.ILR.FundingService.ALB.FundingOutput.Model;
using ESFA.DC.ILR.FundingService.ALB.FundingOutput.Model.Interface;
using ESFA.DC.ILR1819.ReportService.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR1819.ReportService.Service.Service
{
    public sealed class AllbProviderService : IAllbProviderService
    {
        private readonly ILogger _logger;

        private readonly IKeyValuePersistenceService _redis;

        private readonly IJsonSerializationService _jsonSerializationService;

        private readonly SemaphoreSlim _getDataLock;

        private bool _loadedDataAlready;

        private IFundingOutputs _fundingOutputs;

        public AllbProviderService(
            ILogger logger,
            [KeyFilter(PersistenceStorageKeys.Redis)] IKeyValuePersistenceService redis,
            IJsonSerializationService jsonSerializationService)
        {
            _logger = logger;
            _redis = redis;
            _jsonSerializationService = jsonSerializationService;
            _fundingOutputs = null;
            _getDataLock = new SemaphoreSlim(1, 1);
        }

        public async Task<IFundingOutputs> GetAllbData(IJobContextMessage jobContextMessage)
        {
            await _getDataLock.WaitAsync();

            try
            {
                if (_loadedDataAlready)
                {
                    return _fundingOutputs;
                }

                _loadedDataAlready = true;
                string albFilename = jobContextMessage.KeyValuePairs[JobContextMessageKey.FundingAlbOutput].ToString();
                string alb = await _redis.GetAsync(albFilename);

                _fundingOutputs = _jsonSerializationService.Deserialize<FundingOutputs>(alb);
            }
            catch (Exception ex)
            {
                // Todo: Check behaviour
                _logger.LogError("Failed to get & deserialise ALB funding data", ex);
            }
            finally
            {
                _getDataLock.Release();
            }

            return _fundingOutputs;
        }
    }
}
