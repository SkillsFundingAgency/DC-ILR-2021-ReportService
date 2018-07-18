using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR1819.ReportService.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR1819.ReportService.Service.Service
{
    public sealed class IlrProviderService : IIlrProviderService
    {
        private readonly ILogger _logger;

        private readonly IKeyValuePersistenceService _storage;

        private readonly IXmlSerializationService _xmlSerializationService;

        private readonly SemaphoreSlim _getIlrLock = new SemaphoreSlim(1, 1);

        private bool _loadedIlr;

        private Message _message;

        public IlrProviderService(
            ILogger logger,
            [KeyFilter(PersistenceStorageKeys.Blob)] IKeyValuePersistenceService storage,
            IXmlSerializationService xmlSerializationService)
        {
            _logger = logger;
            _storage = storage;
            _xmlSerializationService = xmlSerializationService;
        }

        public async Task<IMessage> GetIlrFile(IJobContextMessage jobContextMessage)
        {
            await _getIlrLock.WaitAsync();

            try
            {
                if (_loadedIlr)
                {
                    return _message;
                }

                _loadedIlr = true;
                string ilr =
                    await _storage.GetAsync(jobContextMessage.KeyValuePairs[JobContextMessageKey.Filename].ToString());

                _message = _xmlSerializationService.Deserialize<Message>(ilr);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to read ILR", ex);
            }
            finally
            {
                _getIlrLock.Release();
            }

            return _message;
        }
    }
}
