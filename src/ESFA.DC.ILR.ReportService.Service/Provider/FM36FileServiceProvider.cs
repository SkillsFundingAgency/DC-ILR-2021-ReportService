using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Interface.Context;
using ESFA.DC.ILR.ReportService.Interface.Provider;
using ESFA.DC.ILR.ReportService.Model.NonContractedAppsActivity;
using ESFA.DC.ILR.ReportService.Service.Provider.Abstract;
using ESFA.DC.ILR1819.DataStore.EF.Interface;
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
                AECApprenticeshipPriceEpisodePeriodisedValues = new List<AECApprenticeshipPriceEpisodePeriodisedValuesInfo>(),
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
                            }).ToList()
                    }).ToList();

                    foreach (var priceEpisode in learner.PriceEpisodes)
                    {
                        var aecApprenticeshipPriceEpisodePeriodisedValuesInfos = priceEpisode
                            .PriceEpisodePeriodisedValues.Select(x =>
                                new AECApprenticeshipPriceEpisodePeriodisedValuesInfo()
                                {
                                    LearnRefNumber = learner.LearnRefNumber,
                                    PriceEpisodeIdentifier = priceEpisode.PriceEpisodeIdentifier,
                                    AttributeName = x.AttributeName,
                                    Periods = new[]
                                    {
                                        x.Period1.GetValueOrDefault(),
                                        x.Period2.GetValueOrDefault(),
                                        x.Period3.GetValueOrDefault(),
                                        x.Period4.GetValueOrDefault(),
                                        x.Period5.GetValueOrDefault(),
                                        x.Period6.GetValueOrDefault(),
                                        x.Period7.GetValueOrDefault(),
                                        x.Period8.GetValueOrDefault(),
                                        x.Period9.GetValueOrDefault(),
                                        x.Period10.GetValueOrDefault(),
                                        x.Period11.GetValueOrDefault(),
                                        x.Period12.GetValueOrDefault(),
                                    }
                                }).ToList();
                        nonContractedActivityRuleBaseInfo.AECApprenticeshipPriceEpisodePeriodisedValues.AddRange(
                            aecApprenticeshipPriceEpisodePeriodisedValuesInfos);
                    }

                    nonContractedActivityRuleBaseInfo.AECLearningDeliveries.AddRange(aecLearningDeliveryInfos);
                }
            }

            return nonContractedActivityRuleBaseInfo;
        }
    }
}