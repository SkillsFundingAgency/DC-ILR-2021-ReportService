using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Data.LARS.Model;
using ESFA.DC.Data.LARS.Model.Interfaces;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.Configuration;
using ESFA.DC.ILR1819.ReportService.Model.Lars;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.ILR1819.ReportService.Service.Service
{
    public sealed class LarsProviderService : ILarsProviderService
    {
        private readonly ILogger _logger;

        private readonly LarsConfiguration _larsConfiguration;

        private readonly SemaphoreSlim _getLearningDeliveriesLock;

        private readonly SemaphoreSlim _getFrameworkAimsLock;

        private readonly SemaphoreSlim _getVersionLock;

        private readonly SemaphoreSlim _getStandardsLock;

        private Dictionary<string, LarsLearningDelivery> _loadedLearningDeliveries;

        private LARS_Standard _loadedStandard;

        private List<LearnerAndDeliveries> _loadedFrameworkAims;

        private string _version;

        public LarsProviderService(ILogger logger, LarsConfiguration larsConfiguration)
        {
            _logger = logger;
            _larsConfiguration = larsConfiguration;
            _loadedLearningDeliveries = null;
            _loadedFrameworkAims = null;
            _version = null;
            _getLearningDeliveriesLock = new SemaphoreSlim(1, 1);
            _getFrameworkAimsLock = new SemaphoreSlim(1, 1);
            _getVersionLock = new SemaphoreSlim(1, 1);
    }

        public async Task<Dictionary<string, LarsLearningDelivery>> GetLearningDeliveries(
            string[] validLearnerAimRefs,
            CancellationToken cancellationToken)
        {
            await _getLearningDeliveriesLock.WaitAsync(cancellationToken);

            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return null;
                }

                if (_loadedLearningDeliveries == null)
                {
                    ILARS larsContext = new LARS(_larsConfiguration.LarsConnectionString);
                    _loadedLearningDeliveries = await larsContext.LARS_LearningDelivery
                        .Where(
                            x => validLearnerAimRefs.Contains(x.LearnAimRef))
                        .ToDictionaryAsync(
                            k => k.LearnAimRef,
                            v => new LarsLearningDelivery
                            {
                                LearningAimTitle = v.LearnAimRefTitle,
                                NotionalNvqLevel = v.NotionalNVQLevel,
                                Tier2SectorSubjectArea = v.SectorSubjectAreaTier2,
                                FrameworkCommonComponent = v.FrameworkCommonComponent
                            },
                            cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get LARS learning deliveries", ex);
            }
            finally
            {
                _getLearningDeliveriesLock.Release();
            }

            return _loadedLearningDeliveries;
        }

        public async Task<LARS_Standard> GetStandard(
            int learningDeliveryStandardCode,
            CancellationToken cancellationToken)
        {
            await _getStandardsLock.WaitAsync(cancellationToken);

            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return null;
                }

                if (_loadedLearningDeliveries == null)
                {
                    ILARS larsContext = new LARS(_larsConfiguration.LarsConnectionString);
                    _loadedStandard = await larsContext.LARS_Standard
                        .SingleOrDefaultAsync(l => l.StandardCode == learningDeliveryStandardCode, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get LARS standards", ex);
            }
            finally
            {
                _getStandardsLock.Release();
            }

            return _loadedStandard;
        }

        public async Task<List<LearnerAndDeliveries>> GetFrameworkAims(
            string[] learnAimRefs,
            List<ILearner> learners,
            CancellationToken cancellationToken)
        {
            await _getFrameworkAimsLock.WaitAsync(cancellationToken);

            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return null;
                }

                if (_loadedFrameworkAims == null)
                {
                    _loadedFrameworkAims = new List<LearnerAndDeliveries>();
                    foreach (ILearner learner in learners)
                    {
                        List<LearningDelivery> learningDeliveries = new List<LearningDelivery>();
                        foreach (ILearningDelivery learningDelivery in learner.LearningDeliveries)
                        {
                            learningDeliveries.Add(new LearningDelivery(learningDelivery.LearnAimRef, learningDelivery.AimSeqNumber, learningDelivery.FworkCodeNullable, learningDelivery.ProgTypeNullable, learningDelivery.PwayCodeNullable, learningDelivery.LearnStartDate));
                        }

                        _loadedFrameworkAims.Add(new LearnerAndDeliveries(learner.LearnRefNumber, learningDeliveries));
                    }

                    ILARS larsContext = new LARS(_larsConfiguration.LarsConnectionString);
                    LarsFrameworkAim[] res = await larsContext.LARS_FrameworkAims
                        .Where(x => learnAimRefs.Contains(x.LearnAimRef))
                        .Select(x =>
                            new LarsFrameworkAim
                            {
                                FworkCode = x.FworkCode,
                                ProgType = x.ProgType,
                                PwayCode = x.PwayCode,
                                LearnAimRef = x.LearnAimRef,
                                EffectiveFrom = x.EffectiveFrom,
                                EffectiveTo = x.EffectiveTo ?? DateTime.MaxValue,
                                FrameworkComponentType = x.FrameworkComponentType
                            })
                        .OrderByDescending(x => x.EffectiveTo)
                        .ToArrayAsync(cancellationToken);

                    foreach (LearnerAndDeliveries learnerAndDelivery in _loadedFrameworkAims)
                    {
                        foreach (LearningDelivery learningDelivery in learnerAndDelivery.LearningDeliveries)
                        {
                            learningDelivery.FrameworkComponentType = res.FirstOrDefault(x =>
                                (learningDelivery.FworkCode == null || x.FworkCode == learningDelivery.FworkCode) &&
                                (learningDelivery.ProgType == null || x.ProgType == learningDelivery.ProgType) &&
                                (learningDelivery.PwayCode == null || x.PwayCode == learningDelivery.PwayCode) &&
                                x.LearnAimRef == learningDelivery.LearningDeliveryLearnAimRef &&
                                x.EffectiveFrom < learningDelivery.LearningDeliveryLearnStartDate &&
                                x.EffectiveTo > learningDelivery.LearningDeliveryLearnStartDate)?.FrameworkComponentType;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get LARS framework aims", ex);
            }
            finally
            {
                _getFrameworkAimsLock.Release();
            }

            return _loadedFrameworkAims;
        }

        public async Task<string> GetVersionAsync(CancellationToken cancellationToken)
        {
            await _getVersionLock.WaitAsync(cancellationToken);

            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return null;
                }

                if (string.IsNullOrEmpty(_version))
                {
                    ILARS larsContext = new LARS(_larsConfiguration.LarsConnectionString);
                    _version = (await larsContext.Current_Version.SingleAsync(cancellationToken)).CurrentVersion;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get LARS version information", ex);
            }
            finally
            {
                _getVersionLock.Release();
            }

            return _version;
        }
    }
}
