using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR1819.DataStore.EF;
using ESFA.DC.ILR1819.DataStore.EF.Valid;
using ESFA.DC.ILR1819.ReportService.Interface.Context;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.Configuration;
using ESFA.DC.ILR1819.ReportService.Model.ILR;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR1819.ReportService.Service.Service
{
    public sealed class IlrProviderService : IIlrProviderService
    {
        private readonly ILogger _logger;

        private readonly IStreamableKeyValuePersistenceService _storage;

        private readonly IXmlSerializationService _xmlSerializationService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IIntUtilitiesService _intUtilitiesService;
        private readonly DataStoreConfiguration _dataStoreConfiguration;

        private readonly SemaphoreSlim _getIlrLock;

        private Message _message;

        public IlrProviderService(
            ILogger logger,
            IStreamableKeyValuePersistenceService storage,
            IXmlSerializationService xmlSerializationService,
            IDateTimeProvider dateTimeProvider,
            IIntUtilitiesService intUtilitiesService,
            DataStoreConfiguration dataStoreConfiguration)
        {
            _logger = logger;
            _storage = storage;
            _xmlSerializationService = xmlSerializationService;
            _dateTimeProvider = dateTimeProvider;
            _intUtilitiesService = intUtilitiesService;
            _dataStoreConfiguration = dataStoreConfiguration;
            _message = null;
            _getIlrLock = new SemaphoreSlim(1, 1);
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

                if (cancellationToken.IsCancellationRequested)
                {
                    return null;
                }

                string filename = reportServiceContext.Filename;
                int ukPrn = reportServiceContext.Ukprn;
                if (await _storage.ContainsAsync(filename, cancellationToken))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        await _storage.GetAsync(filename, ms, cancellationToken);
                        ms.Seek(0, SeekOrigin.Begin);
                        _message = _xmlSerializationService.Deserialize<Message>(ms);
                    }
                }
                else
                {
                    _message = new Message
                    {
                        Header = new MessageHeader
                        {
                            Source = new MessageHeaderSource
                            {
                                UKPRN = ukPrn
                            }
                        }
                    };

                    using (var ilrContext = new ILR1819_DataStoreEntities(_dataStoreConfiguration.ILRDataStoreConnectionString))
                    {
                        _message.Header.Source.DateTime = ilrContext.FileDetails.SingleOrDefault(x => x.UKPRN == ukPrn)?.SubmittedTime ?? _dateTimeProvider.ConvertUtcToUk(_dateTimeProvider.GetNowUtc());
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get and deserialise ILR from storage, key: {reportServiceContext.Filename}", ex);
            }
            finally
            {
                _getIlrLock.Release();
            }

            return _message;
        }

        public async Task<ILRSourceFileInfo> GetLastSubmittedIlrFile(
           IReportServiceContext reportServiceContext,
           CancellationToken cancellationToken)
        {
            await _getIlrLock.WaitAsync(cancellationToken);
            var ilrFileDetail = new ILRSourceFileInfo();
            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return null;
                }

                var ukPrn = reportServiceContext.Ukprn;

                using (var ilrContext = new ILR1819_DataStoreEntities(_dataStoreConfiguration.ILRDataStoreConnectionString))
                {
                    var fileDetail = await ilrContext.FileDetails.Where(x => x.UKPRN == ukPrn).OrderByDescending(x => x.ID).FirstOrDefaultAsync(cancellationToken);
                    if (fileDetail != null)
                    {
                        var filename = fileDetail.Filename.Contains('/') ? fileDetail.Filename.Split('/')[1] : fileDetail.Filename;

                        ilrFileDetail.UKPRN = fileDetail.UKPRN;
                        ilrFileDetail.Filename = filename;
                        ilrFileDetail.SubmittedTime = fileDetail.SubmittedTime;
                    }
                }

                using (var ilrContext = new ILR1819_DataStoreEntitiesValid(_dataStoreConfiguration.ILRDataStoreValidConnectionString))
                {
                    var collectionDetail = await ilrContext.CollectionDetails.FirstOrDefaultAsync(x => x.UKPRN == ukPrn, cancellationToken);
                    if (collectionDetail != null)
                    {
                      ilrFileDetail.FilePreparationDate = collectionDetail.FilePreparationDate;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get Last Submitted ILR file details ", ex);
            }
            finally
            {
                _getIlrLock.Release();
            }

            return ilrFileDetail;
        }
    }
}