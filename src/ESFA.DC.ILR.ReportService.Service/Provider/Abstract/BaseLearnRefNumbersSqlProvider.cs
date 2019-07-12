using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Interface.Configuration;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR1819.DataStore.EF.Valid;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.ILR.ReportService.Service.Provider.Abstract
{
    public abstract class BaseLearnRefNumbersSqlProvider
    {
        private readonly string _filename;
        private readonly ILogger _logger;
        private readonly IReportServiceConfiguration _reportServiceConfiguration;

        private readonly SemaphoreSlim _getDataLock = new SemaphoreSlim(1, 1);

        private bool _loadedDataAlready;

        private List<string> _loadedData;

        protected BaseLearnRefNumbersSqlProvider(
            string key,
            ILogger logger,
            IReportServiceConfiguration reportServiceConfiguration)
        {
            _filename = key;
            _logger = logger;
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

                _loadedDataAlready = true;
                int ukPrn = reportServiceContext.Ukprn;

                var validLearnersList = new List<string>();

                DbContextOptions<ILR1819_DataStoreEntitiesValid> validContextOptions = new DbContextOptionsBuilder<ILR1819_DataStoreEntitiesValid>().UseSqlServer(_reportServiceConfiguration.ILRDataStoreValidConnectionString).Options;
                using (var ilrValidContext = new ILR1819_DataStoreEntitiesValid(validContextOptions))
                {
                    validLearnersList = ilrValidContext.Learners.Where(x => x.UKPRN == ukPrn).Select(x => x.LearnRefNumber).ToList();
                }

                _loadedData = validLearnersList;
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
