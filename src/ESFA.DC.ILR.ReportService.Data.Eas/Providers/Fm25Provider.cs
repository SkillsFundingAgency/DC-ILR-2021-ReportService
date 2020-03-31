using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Models.Fm25;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR1920.DataStore.EF.Interface;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.ILR.ReportService.Data.Eas.Providers
{
    public class Fm25Provider : IExternalDataProvider
    {
        private readonly Func<IILR1920_DataStoreEntities> _ilrContext;

        public Fm25Provider(Func<IILR1920_DataStoreEntities> ilrContext)
        {
            _ilrContext = ilrContext;
        }

        public async Task<object> ProvideAsync(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            using (var context = _ilrContext())
            {
                return new FM25Global()
                {
                    Learners = await context.FM25_Learners
                        .Where(fml => fml.UKPRN == reportServiceContext.Ukprn)
                        .Select(learner => new FM25Learner()
                        {
                            LearnRefNumber = learner.LearnRefNumber,
                            OnProgPayment = learner.OnProgPayment,
                            AreaCostFact1618Hist = learner.AreaCostFact1618Hist,
                            ConditionOfFundingEnglish = learner.ConditionOfFundingEnglish,
                            ConditionOfFundingMaths = learner.ConditionOfFundingMaths,
                            FundLine = learner.FundLine,
                            LearnerActEndDate = learner.LearnerActEndDate,
                            LearnerPlanEndDate = learner.LearnerPlanEndDate,
                            LearnerStartDate = learner.LearnerStartDate,
                            NatRate = learner.NatRate,
                            ProgWeightHist = learner.ProgWeightHist,
                            PrvDisadvPropnHist = learner.PrvDisadvPropnHist,
                            PrvHistLrgProgPropn = learner.PrvHistLrgProgPropn,
                            PrvRetentFactHist = learner.PrvRetentFactHist,
                            RateBand = learner.RateBand,
                            StartFund = learner.StartFund,
                            LearnerPeriodisedValues = learner
                                .FM25_FM35_Learner_PeriodisedValues
                                .Select(learnerPeriodisedValues => new LearnerPeriodisedValues()
                                {
                                    LearnRefNumber = learnerPeriodisedValues.LearnRefNumber,
                                    AttributeName = learnerPeriodisedValues.AttributeName,
                                    Period1 = learnerPeriodisedValues.Period_1,
                                    Period2 = learnerPeriodisedValues.Period_2,
                                    Period3 = learnerPeriodisedValues.Period_3,
                                    Period4 = learnerPeriodisedValues.Period_4,
                                    Period5 = learnerPeriodisedValues.Period_5,
                                    Period6 = learnerPeriodisedValues.Period_6,
                                    Period7 = learnerPeriodisedValues.Period_7,
                                    Period8 = learnerPeriodisedValues.Period_8,
                                    Period9 = learnerPeriodisedValues.Period_9,
                                    Period10 = learnerPeriodisedValues.Period_10,
                                    Period11 = learnerPeriodisedValues.Period_11,
                                    Period12 = learnerPeriodisedValues.Period_12
                                }).ToList()
                        }).ToListAsync(cancellationToken)
                };
            }
        }
    }
}
