using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Models.Fm36;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR2021.DataStore.EF.Interface;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.ILR.ReportService.Data.Eas.Providers
{
    public class Fm36Provider : IExternalDataProvider
    {
        private readonly Func<IILR2021_DataStoreEntities> _ilrContext;

        public Fm36Provider(Func<IILR2021_DataStoreEntities> ilrContext)
        {
            _ilrContext = ilrContext;
        }

        public async Task<object> ProvideAsync(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            using (var context = _ilrContext())
            {
                return new FM36Global()
                {
                    Learners = await context.AEC_Learners
                        .Where(fml => fml.UKPRN == reportServiceContext.Ukprn)
                        .Select(learner => new FM36Learner()
                        {
                            LearnRefNumber = learner.LearnRefNumber,
                            PriceEpisodes = learner.AEC_ApprenticeshipPriceEpisodes.Select(ape => new PriceEpisode()
                            {
                                PriceEpisodeValues = new PriceEpisodeValues()
                                {
                                    EpisodeStartDate = ape.EpisodeStartDate,
                                    PriceEpisodeUpperBandLimit = ape.PriceEpisodeUpperBandLimit,
                                    PriceEpisodeActualEndDate = ape.PriceEpisodeActualEndDate,
                                    PriceEpisodeTotalTNPPrice = ape.PriceEpisodeTotalTNPPrice,
                                    PriceEpisodeUpperLimitAdjustment = ape.PriceEpisodeUpperLimitAdjustment,
                                    PriceEpisodeCompletionElement = ape.PriceEpisodeCompletionElement,
                                    PriceEpisodeCappedRemainingTNPAmount = ape.PriceEpisodeCappedRemainingTNPAmount,
                                    PriceEpisodeAimSeqNumber = ape.PriceEpisodeAimSeqNumber,
                                    PriceEpisodeFundLineType = ape.PriceEpisodeFundLineType,
                                },
                                PriceEpisodePeriodisedValues = ape.AEC_ApprenticeshipPriceEpisode_PeriodisedValues.Select(apepv => new PriceEpisodePeriodisedValues()
                                {
                                    AttributeName = apepv.AttributeName,
                                    Period1 = apepv.Period_1,
                                    Period2 = apepv.Period_2,
                                    Period3 = apepv.Period_3,
                                    Period4 = apepv.Period_4,
                                    Period5 = apepv.Period_5,
                                    Period6 = apepv.Period_6,
                                    Period7 = apepv.Period_7,
                                    Period8 = apepv.Period_8,
                                    Period9 = apepv.Period_9,
                                    Period10 = apepv.Period_10,
                                    Period11 = apepv.Period_11,
                                    Period12 = apepv.Period_12
                                }).ToList()
                            }).ToList(),
                            LearningDeliveries = learner.AEC_LearningDeliveries.Select(learningDelivery => new LearningDelivery()
                            {
                                AimSeqNumber = learningDelivery.AimSeqNumber,
                                LearningDeliveryValues = new LearningDeliveryValues()
                                {
                                    AgeAtProgStart = learningDelivery.AgeAtProgStart,
                                    AppAdjLearnStartDate = learningDelivery.AppAdjLearnStartDate,
                                    LearnDelApplicProv1618FrameworkUplift = learningDelivery.LearnDelApplicProv1618FrameworkUplift,
                                    LearnDelInitialFundLineType = learningDelivery.LearnDelInitialFundLineType,
                                    LearnDelMathEng = learningDelivery.LearnDelMathEng,
                                    PlannedNumOnProgInstalm = learningDelivery.PlannedNumOnProgInstalm,
                                },
                                LearningDeliveryPeriodisedValues = learningDelivery.AEC_LearningDelivery_PeriodisedValues.Select(ldpv => new LearningDeliveryPeriodisedValues()
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
                                }).ToList(),
                                LearningDeliveryPeriodisedTextValues = learningDelivery.AEC_LearningDelivery_PeriodisedTextValues.Select(ldptv => new LearningDeliveryPeriodisedTextValues()
                                {
                                    AttributeName = ldptv.AttributeName,
                                    Period1 = ldptv.Period_1,
                                    Period2 = ldptv.Period_2,
                                    Period3 = ldptv.Period_3,
                                    Period4 = ldptv.Period_4,
                                    Period5 = ldptv.Period_5,
                                    Period6 = ldptv.Period_6,
                                    Period7 = ldptv.Period_7,
                                    Period8 = ldptv.Period_8,
                                    Period9 = ldptv.Period_9,
                                    Period10 = ldptv.Period_10,
                                    Period11 = ldptv.Period_11,
                                    Period12 = ldptv.Period_12
                                }).ToList()
                            }).ToList(),
                        }).ToListAsync(cancellationToken)
                };
            }            
        }
    }
}
