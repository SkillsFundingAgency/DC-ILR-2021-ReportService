using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Data.LARS.Model;
using ESFA.DC.Data.LARS.Model.Interfaces;
using ESFA.DC.ILR1819.ReportService.Interface.Model;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.Lars;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.ILR1819.ReportService.Service.Service
{
    public sealed class LarsProviderService : ILarsProviderService
    {
        private readonly ILogger _logger;

        private readonly ILarsConfiguration _larsConfiguration;

        private readonly SemaphoreSlim _getDataLock = new SemaphoreSlim(1, 1);

        private bool _loadedDataAlready;

        private Dictionary<string, ILarsLearningDelivery> _loadedData;

        public LarsProviderService(ILogger logger, ILarsConfiguration larsConfiguration)
        {
            _logger = logger;
            _larsConfiguration = larsConfiguration;
        }

        public async Task<Dictionary<string, ILarsLearningDelivery>> GetLarsData(List<string> validLearnersList)
        {
            await _getDataLock.WaitAsync();

            try
            {
                if (_loadedDataAlready)
                {
                    return _loadedData;
                }

                _loadedDataAlready = true;
                ILARS larsContext = new LARS(_larsConfiguration.LarsConnectionString);
                _loadedData = larsContext.LARS_LearningDelivery.Where(x => validLearnersList.Contains(x.LearnAimRef)).ToDictionary(
                    k => k.LearnAimRef, v => (ILarsLearningDelivery)new LarsLearningDelivery
                    {
                        LearningAimTitle = v.LearnAimRefTitle,
                        NotionalNvqLevel = v.NotionalNVQLevel,
                        Tier2SectorSubjectArea = v.SectorSubjectAreaTier2
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to read ILR", ex);
            }
            finally
            {
                _getDataLock.Release();
            }

            return _loadedData;
        }
    }
}
