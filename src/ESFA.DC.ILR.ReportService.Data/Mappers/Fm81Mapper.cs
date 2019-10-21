using System.Linq;
using ESFA.DC.ILR.ReportService.Data.Interface.Mappers;
using ESFA.DC.ILR.ReportService.Models.Fm81;

namespace ESFA.DC.ILR.ReportService.Data.Mappers
{
    public class Fm81Mapper : IMapper<FundingService.FM81.FundingOutput.Model.Output.FM81Global, FM81Global>
    {
        public FM81Global MapData(FundingService.FM81.FundingOutput.Model.Output.FM81Global fm81Global)
        {
            return MapFm81Global(fm81Global);
        }

        private FM81Global MapFm81Global(FundingService.FM81.FundingOutput.Model.Output.FM81Global fm81Global)
        {
            return new FM81Global()
            {
                UKPRN = fm81Global.UKPRN,
                CurFundYr = fm81Global.CurFundYr,
                LARSVersion = fm81Global.LARSVersion,
                RulebaseVersion = fm81Global.RulebaseVersion,
                Learners = fm81Global.Learners?.Select(MapFm81Learner).ToList()
            };
        }

        private FM81Learner MapFm81Learner(FundingService.FM81.FundingOutput.Model.Output.FM81Learner fm81Learner)
        {
            return new FM81Learner()
            {
                LearnRefNumber = fm81Learner.LearnRefNumber,
                LearningDeliveries = fm81Learner.LearningDeliveries?.Select(MapFm81LearningDelivery).ToList()
            };
        }

        private LearningDelivery MapFm81LearningDelivery(FundingService.FM81.FundingOutput.Model.Output.LearningDelivery fm81LearningDelivery)
        {
            return new LearningDelivery()
            {
                AimSeqNumber = fm81LearningDelivery.AimSeqNumber,
                LearningDeliveryValues = MapFm81LearningDeliveryValue(fm81LearningDelivery.LearningDeliveryValues),
                LearningDeliveryPeriodisedValues = fm81LearningDelivery.LearningDeliveryPeriodisedValues?.Select(Mapfm81LearningDeliveryPeriodisedValue).ToList()
            };
        }

        private LearningDeliveryValue MapFm81LearningDeliveryValue(FundingService.FM81.FundingOutput.Model.Output.LearningDeliveryValue fm81LearningDeliveryValue)
        {
            return new LearningDeliveryValue()
            {
                AchApplicDate = fm81LearningDeliveryValue.AchApplicDate,
                AchEligible = fm81LearningDeliveryValue.AchEligible,
                Achieved = fm81LearningDeliveryValue.Achieved,
                AchievementApplicVal = fm81LearningDeliveryValue.AchievementApplicVal,
                AchPayment = fm81LearningDeliveryValue.AchPayment,
                ActualDaysIL = fm81LearningDeliveryValue.ActualDaysIL,
                ActualNumInstalm = fm81LearningDeliveryValue.ActualNumInstalm,
                AdjProgStartDate = fm81LearningDeliveryValue.AdjProgStartDate,
                AdjStartDate = fm81LearningDeliveryValue.AdjStartDate,
                AgeStandardStart = fm81LearningDeliveryValue.AgeStandardStart,
                ApplicFundValDate = fm81LearningDeliveryValue.ApplicFundValDate,
                CombinedAdjProp = fm81LearningDeliveryValue.CombinedAdjProp,
                CoreGovContCapApplicVal = fm81LearningDeliveryValue.CoreGovContCapApplicVal,
                CoreGovContPayment = fm81LearningDeliveryValue.CoreGovContPayment,
                CoreGovContUncapped = fm81LearningDeliveryValue.CoreGovContUncapped,
                EmpIdAchDate = fm81LearningDeliveryValue.EmpIdAchDate,
                EmpIdFirstDayStandard = fm81LearningDeliveryValue.EmpIdFirstDayStandard,
                EmpIdFirstYoungAppDate = fm81LearningDeliveryValue.EmpIdFirstYoungAppDate,
                EmpIdSecondYoungAppDate = fm81LearningDeliveryValue.EmpIdSecondYoungAppDate,
                EmpIdSmallBusDate = fm81LearningDeliveryValue.EmpIdSmallBusDate,
                FundLine = fm81LearningDeliveryValue.FundLine,
                InstPerPeriod = fm81LearningDeliveryValue.InstPerPeriod,
                LearnDelDaysIL = fm81LearningDeliveryValue.LearnDelDaysIL,
                LearnDelStandardAccDaysIL = fm81LearningDeliveryValue.LearnDelStandardAccDaysIL,
                LearnDelStandardPrevAccDaysIL = fm81LearningDeliveryValue.LearnDelStandardPrevAccDaysIL,
                LearnDelStandardTotalDaysIL = fm81LearningDeliveryValue.LearnDelStandardTotalDaysIL,
                LearnSuppFund = fm81LearningDeliveryValue.LearnSuppFund,
                LearnSuppFundCash = fm81LearningDeliveryValue.LearnSuppFundCash,
                MathEngAimValue = fm81LearningDeliveryValue.MathEngAimValue,
                MathEngBalPayment = fm81LearningDeliveryValue.MathEngBalPayment,
                MathEngBalPct = fm81LearningDeliveryValue.MathEngBalPct,
                MathEngLSFFundStart = fm81LearningDeliveryValue.MathEngLSFFundStart,
                MathEngLSFThresholdDays = fm81LearningDeliveryValue.MathEngLSFThresholdDays,
                MathEngOnProgPayment = fm81LearningDeliveryValue.MathEngOnProgPayment,
                MathEngOnProgPct = fm81LearningDeliveryValue.MathEngOnProgPct,
                OutstandNumOnProgInstalm = fm81LearningDeliveryValue.OutstandNumOnProgInstalm,
                PlannedNumOnProgInstalm = fm81LearningDeliveryValue.PlannedNumOnProgInstalm,
                PlannedTotalDaysIL = fm81LearningDeliveryValue.PlannedTotalDaysIL,
                ProgStandardStartDate = fm81LearningDeliveryValue.ProgStandardStartDate,
                SmallBusApplicVal = fm81LearningDeliveryValue.SmallBusApplicVal,
                SmallBusEligible = fm81LearningDeliveryValue.SmallBusEligible,
                SmallBusPayment = fm81LearningDeliveryValue.SmallBusPayment,
                SmallBusStatusFirstDayStandard = fm81LearningDeliveryValue.SmallBusStatusFirstDayStandard,
                SmallBusStatusThreshold = fm81LearningDeliveryValue.SmallBusStatusThreshold,
                SmallBusThresholdDate = fm81LearningDeliveryValue.SmallBusThresholdDate,
                YoungAppApplicVal = fm81LearningDeliveryValue.YoungAppApplicVal,
                YoungAppEligible = fm81LearningDeliveryValue.YoungAppEligible,
                YoungAppFirstPayment = fm81LearningDeliveryValue.YoungAppFirstPayment,
                YoungAppFirstThresholdDate = fm81LearningDeliveryValue.YoungAppFirstThresholdDate,
                YoungAppPayment = fm81LearningDeliveryValue.YoungAppPayment,
                YoungAppSecondPayment = fm81LearningDeliveryValue.YoungAppSecondPayment,
                YoungAppSecondThresholdDate = fm81LearningDeliveryValue.YoungAppSecondThresholdDate
            };
        }

        private LearningDeliveryPeriodisedValue Mapfm81LearningDeliveryPeriodisedValue(FundingService.FM81.FundingOutput.Model.Output.LearningDeliveryPeriodisedValue fm81LearningDeliveryPeriodisedValue)
        {
            return new LearningDeliveryPeriodisedValue()
            {
                AttributeName = fm81LearningDeliveryPeriodisedValue.AttributeName,
                Period1 = fm81LearningDeliveryPeriodisedValue.Period1,
                Period2 = fm81LearningDeliveryPeriodisedValue.Period2,
                Period3 = fm81LearningDeliveryPeriodisedValue.Period3,
                Period4 = fm81LearningDeliveryPeriodisedValue.Period4,
                Period5 = fm81LearningDeliveryPeriodisedValue.Period5,
                Period6 = fm81LearningDeliveryPeriodisedValue.Period6,
                Period7 = fm81LearningDeliveryPeriodisedValue.Period7,
                Period8 = fm81LearningDeliveryPeriodisedValue.Period8,
                Period9 = fm81LearningDeliveryPeriodisedValue.Period9,
                Period10 = fm81LearningDeliveryPeriodisedValue.Period10,
                Period11 = fm81LearningDeliveryPeriodisedValue.Period11,
                Period12 = fm81LearningDeliveryPeriodisedValue.Period12
            };
        }
    }
}
