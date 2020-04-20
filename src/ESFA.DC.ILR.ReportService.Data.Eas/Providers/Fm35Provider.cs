using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Models.Fm35;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR2021.DataStore.EF.Interface;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.ILR.ReportService.Data.Eas.Providers
{
    public class Fm35Provider : IExternalDataProvider
    {
        private readonly Func<IILR2021_DataStoreEntities> _ilrContext;

        public Fm35Provider(Func<IILR2021_DataStoreEntities> ilrContext)
        {
            _ilrContext = ilrContext;
        }

        public async Task<object> ProvideAsync(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            using (var context = _ilrContext())
            {
                return new FM35Global()
                {
                    Learners = await context.FM35_Learners
                        .Where(fml => fml.UKPRN == reportServiceContext.Ukprn)
                        .Select(learner => new FM35Learner()
                        {
                            LearnRefNumber = learner.LearnRefNumber,
                            LearningDeliveries = learner.FM35_LearningDeliveries.Select(learningDelivery => new LearningDelivery
                            {
                                AimSeqNumber = learningDelivery.AimSeqNumber,
                                LearningDeliveryValue = new LearningDeliveryValue()
                                {
                                    AchieveElement = learningDelivery.AchieveElement,
                                    AimValue = learningDelivery.AimValue,
                                    ApplicEmpFactDate = learningDelivery.ApplicEmpFactDate,
                                    ApplicFactDate = learningDelivery.ApplicFactDate,
                                    ApplicProgWeightFact = learningDelivery.ApplicProgWeightFact,
                                    ApplicWeightFundRate = learningDelivery.ApplicWeightFundRate,
                                    AreaCostFactAdj = learningDelivery.AreaCostFactAdj,
                                    CapFactor = learningDelivery.CapFactor,
                                    DisUpFactAdj = learningDelivery.DisUpFactAdj,
                                    FundLine = learningDelivery.FundLine,
                                    LargeEmployerID = learningDelivery.LargeEmployerID,
                                    LargeEmployerFM35Fctr = learningDelivery.LargeEmployerFM35Fctr,
                                    NonGovCont = learningDelivery.NonGovCont,
                                    PlannedNumOnProgInstalm = learningDelivery.PlannedNumOnProgInstalm,
                                    PlannedNumOnProgInstalmTrans = learningDelivery.PlannedNumOnProgInstalmTrans,
                                    PrscHEAim = learningDelivery.PrscHEAim,
                                    StartPropTrans = learningDelivery.StartPropTrans,
                                    TrnWorkPlaceAim = learningDelivery.TrnWorkPlaceAim,
                                    TrnWorkPrepAim = learningDelivery.TrnWorkPrepAim,
                                    WeightedRateFromESOL = learningDelivery.WeightedRateFromESOL,
                                },
                                LearningDeliveryPeriodisedValues = learningDelivery.FM35_LearningDelivery_PeriodisedValues.Select(ldpv => new LearningDeliveryPeriodisedValue()
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
