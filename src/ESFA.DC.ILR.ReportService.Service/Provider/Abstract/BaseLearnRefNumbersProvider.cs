using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.ReportService.Interface.Configuration;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR1819.DataStore.EF.Valid;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.ILR.ReportService.Service.Provider.Abstract
{
    public abstract class BaseLearnRefNumbersProvider : AbstractFundModelProviderService
    {
        private readonly string _filename;
        private readonly ILogger _logger;
        private readonly IFileService _fileService;
        private readonly IStreamableKeyValuePersistenceService _storage;
        private readonly IJsonSerializationService _jsonSerializationService;
        private readonly IReportServiceConfiguration _reportServiceConfiguration;

        private readonly SemaphoreSlim _getDataLock = new SemaphoreSlim(1, 1);

        private bool _loadedDataAlready;

        private List<string> _loadedData;

        protected BaseLearnRefNumbersProvider(
            string key,
            ILogger logger,
            IFileService fileService,
            IJsonSerializationService jsonSerializationService,
            IReportServiceConfiguration reportServiceConfiguration)
        : base(fileService, jsonSerializationService, logger)
        {
            _filename = key;
            _logger = logger;
            _fileService = fileService;
            _jsonSerializationService = jsonSerializationService;
            _reportServiceConfiguration = reportServiceConfiguration;
        }

        public async Task<List<string>> GetLearnersAsync(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            await _getDataLock.WaitAsync(cancellationToken);

            try
            {
                if (_loadedDataAlready)
                {
                    return _loadedData;
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    return null;
                }

                _loadedData = await Provide<List<string>>(_filename, reportServiceContext.Container, cancellationToken);

                _loadedDataAlready = true;
            }
            catch (Exception ex)
            {
                // Todo: Check behaviour
                _logger.LogError($"Failed to get & deserialise {_filename}", ex);
            }
            finally
            {
                _getDataLock.Release();
            }

            return _loadedData;
        }
    }
}
