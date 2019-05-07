using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Interface.Context;
using ESFA.DC.ILR.ReportService.Interface.Provider;
using ESFA.DC.ILR.ReportService.Model.Configuration;
using ESFA.DC.ILR.ReportService.Service.Provider.Abstract;
using ESFA.DC.ILR1819.DataStore.EF.Interface;
using ESFA.DC.ILR1819.DataStore.EF.Valid.Interface;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ReportService.Service.Provider
{
    public sealed class IlrFileServiceProvider : AbstractFundModelProviderService, IIlrProviderService
    {
        private readonly DataStoreConfiguration _dataStoreConfiguration;
        private readonly Func<IIlr1819ValidContext> _ilrValidContextFactory;
        private readonly Func<IIlr1819RulebaseContext> _ilrRulebaseContextFactory;
        private readonly SemaphoreSlim _getIlrLock = new SemaphoreSlim(1, 1);
        private Message _message;

        public IlrFileServiceProvider(
            ILogger logger,
            IStreamableKeyValuePersistenceService storage,
            IXmlSerializationService xmlSerializationService)
        : base(storage, xmlSerializationService, logger)
        {
        }

        public async Task<IMessage> GetIlrFile(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            await _getIlrLock.WaitAsync(cancellationToken);

            try
            {
                if (_message != null)
                {
                    return _message;
                }

                cancellationToken.ThrowIfCancellationRequested();

                string filename = reportServiceContext.Filename;
                using (MemoryStream ms = new MemoryStream())
                {
                    await _streamableKeyValuePersistenceService.GetAsync(filename, ms, cancellationToken);
                    ms.Seek(0, SeekOrigin.Begin);
                    _message = _serializationService.Deserialize<Message>(ms);
                }
            }
            finally
            {
                _getIlrLock.Release();
            }

            return _message;
        }
    }
}