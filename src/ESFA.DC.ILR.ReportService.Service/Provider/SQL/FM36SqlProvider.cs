using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using ESFA.DC.ILR.ReportService.Interface.Provider;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Provider.Abstract;
using ESFA.DC.ILR1819.DataStore.EF;
using ESFA.DC.ILR1819.DataStore.EF.Interface;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.ILR.ReportService.Service.Provider.SQL
{
    public class FM36SqlProvider : IFM36ProviderService
    {
        private readonly Func<IIlr1819RulebaseContext> _ilrRulebaseContextFactory;
        private readonly SemaphoreSlim _getDataLock = new SemaphoreSlim(1, 1);
        private bool _loadedDataAlready;
        private FM36Global _fundingOutputs;

        public FM36SqlProvider(
            Func<IIlr1819RulebaseContext> ilrRulebaseContextFactory)
        {
            _ilrRulebaseContextFactory = ilrRulebaseContextFactory;
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
                FM36Global fm36Global = new FM36Global();
                using (var ilrContext = _ilrRulebaseContextFactory())
                {
                    var fm36GlobalDb = await ilrContext.AEC_globals.FirstOrDefaultAsync(x => x.UKPRN == ukPrn, cancellationToken);
                    //AEC_LearningDelivery[] res = await ilrContext.AEC_LearningDelivery_Period.Where(x => x.UKPRN == ukPrn).Select(x => x.AEC_LearningDelivery)
                    //    .Include(x => x.AEC_LearningDelivery_PeriodisedValues).ToArrayAsync(cancellationToken);

                    AEC_LearningDelivery[] res = await ilrContext.AEC_LearningDeliveries.Where(x => x.UKPRN == ukPrn)
                        .Include(x => x.AEC_LearningDelivery_PeriodisedValues).ToArrayAsync(cancellationToken);

                    IGrouping<string, AEC_LearningDelivery>[] learners = res.GroupBy(x => x.LearnRefNumber).ToArray();

                    fm36Global.Learners = new List<FM36Learner>();

                    foreach (IGrouping<string, AEC_LearningDelivery> albLearningDeliveries in learners)
                    {
                        var learningDeliveryDto = new List<ILR.FundingService.FM36.FundingOutput.Model.Output.LearningDelivery>();
                        foreach (var ld in albLearningDeliveries)
                        {
                            var ldPeriodisedValues = ld.AEC_LearningDelivery_PeriodisedValues.Select(ldpv => new LearningDeliveryPeriodisedValues()
                            {
                                AttributeName = ldpv.AttributeName,
                                Period1 = ldpv.Period_1,
                                Period2 = ldpv.Period_2,
                                Period3 = ldpv.Period_3,
                                Period4 = ldpv.Period_4,
                                Period5 = ldpv.Period_5,
                                Period6 = ldpv.Period_6,
                                Period7 = ldpv.Period_7,
                                Period8 = ldpv.Period_8,
                                Period9 = ldpv.Period_9,
                                Period10 = ldpv.Period_10,
                                Period11 = ldpv.Period_11,
                                Period12 = ldpv.Period_12
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
                        fm36Global.LARSVersion = fm36GlobalDb.LARSVersion;
                        fm36Global.RulebaseVersion = fm36GlobalDb.RulebaseVersion;
                        fm36Global.UKPRN = fm36GlobalDb.UKPRN;
                    }
                }

                _fundingOutputs = fm36Global;
            }
            finally
            {
                _getDataLock.Release();
            }

            return _fundingOutputs;
        }
    }
}