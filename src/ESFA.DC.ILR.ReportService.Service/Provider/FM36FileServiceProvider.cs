using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using ESFA.DC.ILR.ReportService.Interface.Provider;
using ESFA.DC.ILR.ReportService.Model.NonContractedAppsActivity;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Provider.Abstract;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ReportService.Service.Provider
{
    public class FM36FileServiceProvider : AbstractFundModelProviderService, IFM36ProviderService, IFM36NonContractedActivityProviderService
    {
        private readonly SemaphoreSlim _getDataLock = new SemaphoreSlim(1, 1);
        private bool _loadedDataAlready;
        private FM36Global _fundingOutputs;

        public FM36FileServiceProvider(
            ILogger logger,
            IStreamableKeyValuePersistenceService storage,
            IJsonSerializationService jsonSerializationService)
        : base(storage, jsonSerializationService, logger)
        {
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

        public async Task<NonContractedActivityRuleBaseInfo> GetFM36InfoForNonContractedActivityReportAsync(
            List<string> validLearners,
            IReportServiceContext reportServiceContext,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var fm36Global = await GetFM36Data(reportServiceContext, cancellationToken);

            var nonContractedActivityRuleBaseInfo = new NonContractedActivityRuleBaseInfo()
            {
                UkPrn = reportServiceContext.Ukprn,
                PriceEpisodes = new List<PriceEpisodeInfo>(),
                AECLearningDeliveries = new List<AECLearningDeliveryInfo>()
            };

            FM36Learner[] fm36Learners = fm36Global.Learners?.Where(x => validLearners.Contains(x.LearnRefNumber)).ToArray();

            if (fm36Learners != null)
            {
                foreach (var learner in fm36Learners)
                {
                    var aecLearningDeliveryInfos = learner.LearningDeliveries.Select(x => new AECLearningDeliveryInfo()
                    {
                        AimSeqNumber = x.AimSeqNumber,
                        LearnRefNumber = learner.LearnRefNumber,
                        LearningDeliveryValues = new AECLearningDeliveryValuesInfo()
                        {
                            LearnDelMathEng = x.LearningDeliveryValues.LearnDelMathEng.GetValueOrDefault(),
                            LearnDelInitialFundLineType = x.LearningDeliveryValues.LearnDelInitialFundLineType,
                            LearnAimRef = x.LearningDeliveryValues.LearnAimRef,
                            AppAdjLearnStartDate = x.LearningDeliveryValues.AppAdjLearnStartDate.GetValueOrDefault(),
                            AgeAtProgStart = x.LearningDeliveryValues.AgeAtProgStart.GetValueOrDefault()
                        },
                        LearningDeliveryPeriodisedValues = x.LearningDeliveryPeriodisedValues.Select(y =>
                            new AECApprenticeshipLearningDeliveryPeriodisedValuesInfo()
                            {
                                AttributeName = y.AttributeName,
                                Periods = new[]
                                {
                                    y.Period1.GetValueOrDefault(),
                                    y.Period2.GetValueOrDefault(),
                                    y.Period3.GetValueOrDefault(),
                                    y.Period4.GetValueOrDefault(),
                                    y.Period5.GetValueOrDefault(),
                                    y.Period6.GetValueOrDefault(),
                                    y.Period7.GetValueOrDefault(),
                                    y.Period8.GetValueOrDefault(),
                                    y.Period9.GetValueOrDefault(),
                                    y.Period10.GetValueOrDefault(),
                                    y.Period11.GetValueOrDefault(),
                                    y.Period12.GetValueOrDefault(),
                                }
                            }).ToList(),
                        LearningDeliveryPeriodisedTextValues = x.LearningDeliveryPeriodisedTextValues.Where(y => y.AttributeName.Equals("FundingLineType")).Select(z =>
                            new AECApprenticeshipLearningDeliveryPeriodisedTextValuesInfo()
                            {
                                AttributeName = z.AttributeName,
                                Periods = new[]
                                {
                                   z.Period1,
                                   z.Period2,
                                   z.Period3,
                                   z.Period4,
                                   z.Period5,
                                   z.Period6,
                                   z.Period7,
                                   z.Period8,
                                   z.Period9,
                                   z.Period10,
                                   z.Period11,
                                   z.Period12,
                                }
                            }).SingleOrDefault()
                    }).ToList();

                    var priceEpisodes = learner.PriceEpisodes.Select(x => new PriceEpisodeInfo()
                    {
                        PriceEpisodeValues = new PriceEpisodeValuesInfo()
                        {
                            PriceEpisodeFundLineType = x.PriceEpisodeValues.PriceEpisodeFundLineType,
                            PriceEpisodeAimSeqNumber = x.PriceEpisodeValues.PriceEpisodeAimSeqNumber.GetValueOrDefault(),
                            EpisodeStartDate = x.PriceEpisodeValues.EpisodeStartDate.GetValueOrDefault(),
                            PriceEpisodeActualEndDate = x.PriceEpisodeValues.PriceEpisodeActualEndDate.GetValueOrDefault()
                        },
                        AECApprenticeshipPriceEpisodePeriodisedValues = x.PriceEpisodePeriodisedValues.Select(y =>
                            new AECApprenticeshipPriceEpisodePeriodisedValuesInfo()
                            {
                                LearnRefNumber = learner.LearnRefNumber,
                                PriceEpisodeIdentifier = x.PriceEpisodeIdentifier,
                                AttributeName = y.AttributeName,
                                Periods = new[]
                                {
                                    y.Period1.GetValueOrDefault(),
                                    y.Period2.GetValueOrDefault(),
                                    y.Period3.GetValueOrDefault(),
                                    y.Period4.GetValueOrDefault(),
                                    y.Period5.GetValueOrDefault(),
                                    y.Period6.GetValueOrDefault(),
                                    y.Period7.GetValueOrDefault(),
                                    y.Period8.GetValueOrDefault(),
                                    y.Period9.GetValueOrDefault(),
                                    y.Period10.GetValueOrDefault(),
                                    y.Period11.GetValueOrDefault(),
                                    y.Period12.GetValueOrDefault(),
                                }
                            }).ToList()
                    }).ToList();

                    nonContractedActivityRuleBaseInfo.AECLearningDeliveries.AddRange(aecLearningDeliveryInfos);
                    nonContractedActivityRuleBaseInfo.PriceEpisodes.AddRange(priceEpisodes);
                }
            }

            return nonContractedActivityRuleBaseInfo;
        }
    }
}