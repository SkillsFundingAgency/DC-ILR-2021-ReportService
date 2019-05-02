using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using ESFA.DC.ILR.ReportService.Interface.Context;
using ESFA.DC.ILR.ReportService.Interface.Provider;
using ESFA.DC.ILR.ReportService.Service.Provider.Abstract;
using ESFA.DC.ILR1819.DataStore.EF.Interface;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ReportService.Service.Provider
{
    public class FM36FileServiceProvider : AbstractFundModelProviderService, IFM36ProviderService
    {
        private readonly Func<IIlr1819RulebaseContext> _ilrRulebaseContextFactory;
        private readonly SemaphoreSlim _getDataLock = new SemaphoreSlim(1, 1);
        private bool _loadedDataAlready;
        private FM36Global _fundingOutputs;

        public FM36FileServiceProvider(
            ILogger logger,
            IStreamableKeyValuePersistenceService storage,
            IJsonSerializationService jsonSerializationService,
            Func<IIlr1819RulebaseContext> ilrRulebaseContextFactory)
        : base(storage, jsonSerializationService, logger)
        {
            _ilrRulebaseContextFactory = ilrRulebaseContextFactory;
        }

        public async Task<FM36Global> GetFM36Data(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            await _getDataLock.WaitAsync(cancellationToken);

            try
            {
                if (_loadedDataAlready)
                {
                    return _fundingOutputs;
                }

                cancellationToken.ThrowIfCancellationRequested();
                _loadedDataAlready = true;

                string fm36Filename = reportServiceContext.FundingFM36OutputKey;
                string fm36 = await _streamableKeyValuePersistenceService.GetAsync(fm36Filename, cancellationToken);

                if (string.IsNullOrEmpty(fm36))
                {
                    _fundingOutputs = null;
                    return _fundingOutputs;
                }

                _fundingOutputs = _serializationService.Deserialize<FM36Global>(fm36);
            }
            finally
            {
                _getDataLock.Release();
            }

            return _fundingOutputs;
        }
    }
}