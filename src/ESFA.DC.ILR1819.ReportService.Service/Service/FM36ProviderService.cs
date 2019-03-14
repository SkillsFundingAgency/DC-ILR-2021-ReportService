using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using ESFA.DC.ILR1819.DataStore.EF;
using ESFA.DC.ILR1819.DataStore.EF.Interface;
using ESFA.DC.ILR1819.ReportService.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Context;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.PeriodEnd.AppsAdditionalPayment;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.ILR1819.ReportService.Service.Service
{
    public class FM36ProviderService : IFM36ProviderService
    {
        private readonly ILogger _logger;

        private readonly IKeyValuePersistenceService _redis;
        private readonly IKeyValuePersistenceService _blob;
        private readonly IJsonSerializationService _jsonSerializationService;
        private readonly IIntUtilitiesService _intUtilitiesService;
        private readonly Func<IIlr1819RulebaseContext> _ilrRulebaseContextFactory;
        private readonly SemaphoreSlim _getDataLock;
        private bool _loadedDataAlready;
        private FM36Global _fundingOutputs;

        public FM36ProviderService(
            ILogger logger,
            [KeyFilter(PersistenceStorageKeys.Redis)] IKeyValuePersistenceService redis,
            [KeyFilter(PersistenceStorageKeys.Blob)] IKeyValuePersistenceService blob,
            IJsonSerializationService jsonSerializationService,
            IIntUtilitiesService intUtilitiesService,
            Func<IIlr1819RulebaseContext> ilrRulebaseContextFactory)
        {
            _logger = logger;
            _redis = redis;
            _blob = blob;
            _jsonSerializationService = jsonSerializationService;
            _intUtilitiesService = intUtilitiesService;
            _ilrRulebaseContextFactory = ilrRulebaseContextFactory;
            _fundingOutputs = null;
            _getDataLock = new SemaphoreSlim(1, 1);
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

                int ukPrn = reportServiceContext.Ukprn;
                if (string.Equals(reportServiceContext.CollectionName, "ILR1819", StringComparison.OrdinalIgnoreCase))
                {
                    string fm36Filename = reportServiceContext.FundingFM36OutputKey;
                    string fm36 = await _redis.GetAsync(fm36Filename, cancellationToken);

                    if (string.IsNullOrEmpty(fm36))
                    {
                        _fundingOutputs = null;
                        return _fundingOutputs;
                    }

                    // await _blob.SaveAsync($"{jobContextMessage.KeyValuePairs[JobContextMessageKey.UkPrn]}_{jobContextMessage.JobId.ToString()}_Fm36.json", fm36, cancellationToken);
                    _fundingOutputs = _jsonSerializationService.Deserialize<FM36Global>(fm36);
                }
                else
                {
                    FM36Global fm36Global = new FM36Global();
                    using (var ilrContext = _ilrRulebaseContextFactory())
                    {
                        var fm36GlobalDb = await ilrContext.AecGlobals.FirstOrDefaultAsync(x => x.Ukprn == ukPrn, cancellationToken);
                        //AEC_LearningDelivery[] res = await ilrContext.AEC_LearningDelivery_Period.Where(x => x.UKPRN == ukPrn).Select(x => x.AEC_LearningDelivery)
                        //    .Include(x => x.AEC_LearningDelivery_PeriodisedValues).ToArrayAsync(cancellationToken);

                        AecLearningDelivery[] res = await ilrContext.AecLearningDeliveries.Where(x => x.Ukprn == ukPrn)
                            .Include(x => x.AecLearningDeliveryPeriodisedValues).ToArrayAsync(cancellationToken);

                        IGrouping<string, AecLearningDelivery>[] learners = res.GroupBy(x => x.LearnRefNumber).ToArray();

                        fm36Global.Learners = new List<FM36Learner>();

                        foreach (IGrouping<string, AecLearningDelivery> albLearningDeliveries in learners)
                        {
                            var learningDeliveryDto = new List<ILR.FundingService.FM36.FundingOutput.Model.Output.LearningDelivery>();
                            foreach (var ld in albLearningDeliveries)
                            {
                                var ldPeriodisedValues = ld.AecLearningDeliveryPeriodisedValues.Select(ldpv => new LearningDeliveryPeriodisedValues()
                                {
                                    AttributeName = ldpv.AttributeName,
                                    Period1 = ldpv.Period1,
                                    Period2 = ldpv.Period2,
                                    Period3 = ldpv.Period3,
                                    Period4 = ldpv.Period4,
                                    Period5 = ldpv.Period5,
                                    Period6 = ldpv.Period6,
                                    Period7 = ldpv.Period7,
                                    Period8 = ldpv.Period8,
                                    Period9 = ldpv.Period9,
                                    Period10 = ldpv.Period10,
                                    Period11 = ldpv.Period11,
                                    Period12 = ldpv.Period12
                                }).ToList();

                                learningDeliveryDto.Add(new LearningDelivery()
                                {
                                    AimSeqNumber = ld.AimSeqNumber,
                                    LearningDeliveryPeriodisedValues = ldPeriodisedValues,
                                    LearningDeliveryValues = new LearningDeliveryValues()
                                    {
                                        LearnDelInitialFundLineType = ld.LearnDelInitialFundLineType // todo : rest of the properties
                                    }
                                });
                            }

                            FM36Learner learner = new FM36Learner()
                            {
                                LearnRefNumber = albLearningDeliveries.Key,
                                LearningDeliveries = learningDeliveryDto
                            };

                            fm36Global.Learners.Add(learner);
                        }

                        if (fm36GlobalDb != null)
                        {
                            fm36Global.LARSVersion = fm36GlobalDb.Larsversion;
                            fm36Global.RulebaseVersion = fm36GlobalDb.RulebaseVersion;
                            fm36Global.UKPRN = fm36GlobalDb.Ukprn;
                        }
                    }

                    _fundingOutputs = fm36Global;
                }
            }
            finally
            {
                _getDataLock.Release();
            }

            return _fundingOutputs;
        }

        public async Task<AppsAdditionalPaymentRulebaseInfo> GetFM36DataForAppsAdditionalPaymentReportAsync(int ukPrn, CancellationToken cancellationToken)
        {
            var appsAdditionalPaymentRulebaseInfo = new AppsAdditionalPaymentRulebaseInfo()
            {
                UkPrn = ukPrn,
                AECApprenticeshipPriceEpisodePeriodisedValues = new List<AECApprenticeshipPriceEpisodePeriodisedValuesInfo>(),
                AECLearningDeliveries = new List<AECLearningDeliveryInfo>()
            };

            cancellationToken.ThrowIfCancellationRequested();

            List<AecApprenticeshipPriceEpisodePeriodisedValue> aecApprenticeshipPriceEpisodePeriodisedValues;
            List<AecLearningDelivery> aecLearningDeliveries;

            using (var ilrContext = _ilrRulebaseContextFactory())
            {
                aecApprenticeshipPriceEpisodePeriodisedValues = await ilrContext.AecApprenticeshipPriceEpisodePeriodisedValues.Where(x => x.Ukprn == ukPrn).ToListAsync(cancellationToken);
                aecLearningDeliveries = await ilrContext.AecLearningDeliveries.Where(x => x.Ukprn == ukPrn).ToListAsync(cancellationToken);
            }

            foreach (var aecApprenticeshipPriceEpisodePeriodisedValue in aecApprenticeshipPriceEpisodePeriodisedValues)
            {
                var periodisedValue = new AECApprenticeshipPriceEpisodePeriodisedValuesInfo()
                {
                    UKPRN = ukPrn,
                    LearnRefNumber = aecApprenticeshipPriceEpisodePeriodisedValue.LearnRefNumber,
                    AttributeName = aecApprenticeshipPriceEpisodePeriodisedValue.AttributeName,
                    Period_1 = aecApprenticeshipPriceEpisodePeriodisedValue.Period1,
                    Period_2 = aecApprenticeshipPriceEpisodePeriodisedValue.Period2,
                    Period_3 = aecApprenticeshipPriceEpisodePeriodisedValue.Period3,
                    Period_4 = aecApprenticeshipPriceEpisodePeriodisedValue.Period4,
                    Period_5 = aecApprenticeshipPriceEpisodePeriodisedValue.Period5,
                    Period_6 = aecApprenticeshipPriceEpisodePeriodisedValue.Period6,
                    Period_7 = aecApprenticeshipPriceEpisodePeriodisedValue.Period7,
                    Period_8 = aecApprenticeshipPriceEpisodePeriodisedValue.Period8,
                    Period_9 = aecApprenticeshipPriceEpisodePeriodisedValue.Period9,
                    Period_10 = aecApprenticeshipPriceEpisodePeriodisedValue.Period10,
                    Period_11 = aecApprenticeshipPriceEpisodePeriodisedValue.Period11,
                    Period_12 = aecApprenticeshipPriceEpisodePeriodisedValue.Period12
                };
                appsAdditionalPaymentRulebaseInfo.AECApprenticeshipPriceEpisodePeriodisedValues.Add(periodisedValue);
            }

            foreach (var aecLearningDelivery in aecLearningDeliveries)
            {
                var aecLearningDeliveryInfo = new AECLearningDeliveryInfo()
                {
                    UKPRN = ukPrn,
                    LearnRefNumber = aecLearningDelivery.LearnRefNumber,
                    AimSeqNumber = aecLearningDelivery.AimSeqNumber,
                    LearnDelEmpIdFirstAdditionalPaymentThreshold = aecLearningDelivery.LearnDelEmpIdFirstAdditionalPaymentThreshold,
                    LearnDelEmpIdSecondAdditionalPaymentThreshold = aecLearningDelivery.LearnDelEmpIdSecondAdditionalPaymentThreshold
                };

                appsAdditionalPaymentRulebaseInfo.AECLearningDeliveries.Add(aecLearningDeliveryInfo);
            }

            return appsAdditionalPaymentRulebaseInfo;
        }
    }
}