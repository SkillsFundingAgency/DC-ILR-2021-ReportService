using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Models.Fm81;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR2021.DataStore.EF.Interface;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.ILR.ReportService.Data.Eas.Providers
{
    public class Fm81Provider : IExternalDataProvider
    {
        private readonly Func<IILR2021_DataStoreEntities> _ilrContext;

        public Fm81Provider(Func<IILR2021_DataStoreEntities> ilrContext)
        {
            _ilrContext = ilrContext;
        }

        public async Task<object> ProvideAsync(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            using (var context = _ilrContext())
            {
                return new FM81Global()
                {
                    Learners = await context.TBL_Learners
                        .Where(fml => fml.UKPRN == reportServiceContext.Ukprn)
                        .Select(learner => new FM81Learner()
                        {
                            LearnRefNumber = learner.LearnRefNumber,
                            LearningDeliveries = learner.TBL_LearningDeliveries.Select(learningDelivery => new LearningDelivery()
                            {
                                AimSeqNumber = learningDelivery.AimSeqNumber,
                                LearningDeliveryValues = new LearningDeliveryValue()
                                {
                                    AchApplicDate = learningDelivery.AchApplicDate,
                                    AchievementApplicVal = learningDelivery.AchievementApplicVal,
                                    AgeStandardStart = learningDelivery.AgeStandardStart,
                                    ApplicFundValDate = learningDelivery.ApplicFundValDate,
                                    CoreGovContCapApplicVal = learningDelivery.CoreGovContCapApplicVal,
                                    EmpIdAchDate = learningDelivery.EmpIdAchDate,
                                    EmpIdFirstDayStandard = learningDelivery.EmpIdFirstDayStandard,
                                    EmpIdFirstYoungAppDate = learningDelivery.EmpIdFirstYoungAppDate,
                                    EmpIdSecondYoungAppDate = learningDelivery.EmpIdSecondYoungAppDate,
                                    EmpIdSmallBusDate = learningDelivery.EmpIdSmallBusDate,
                                    FundLine = learningDelivery.FundLine,
                                    MathEngLSFFundStart = learningDelivery.MathEngLSFFundStart,
                                    SmallBusApplicVal = learningDelivery.SmallBusApplicVal,
                                    SmallBusEligible = learningDelivery.SmallBusEligible,
                                    YoungAppApplicVal = learningDelivery.YoungAppApplicVal,
                                    YoungAppEligible = learningDelivery.YoungAppEligible,
                                },
                                LearningDeliveryPeriodisedValues = learningDelivery.TBL_LearningDelivery_PeriodisedValues.Select(ldpv => new LearningDeliveryPeriodisedValue
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
                                }).ToList()
                            }).ToList()
                        }).ToListAsync(cancellationToken)
                };
            }
        }
    }
}
