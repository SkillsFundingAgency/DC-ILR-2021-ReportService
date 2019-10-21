using System.Linq;
using ESFA.DC.ILR.ReportService.Data.Interface.Mappers;
using ESFA.DC.ILR.ReportService.Models.Fm99;

namespace ESFA.DC.ILR.ReportService.Data.Mappers
{
    public class Fm99Mapper : IMapper<FundingService.ALB.FundingOutput.Model.Output.ALBGlobal, ALBGlobal>
    {
        public ALBGlobal MapData(FundingService.ALB.FundingOutput.Model.Output.ALBGlobal albGlobal)
        {
            return MapAlbGlobal(albGlobal);
        }

        private ALBGlobal MapAlbGlobal(FundingService.ALB.FundingOutput.Model.Output.ALBGlobal albGlobal)
        {
            return  new ALBGlobal()
            {
                UKPRN = albGlobal.UKPRN,
                LARSVersion = albGlobal.LARSVersion,
                PostcodeAreaCostVersion = albGlobal.PostcodeAreaCostVersion,
                RulebaseVersion = albGlobal.RulebaseVersion,
                Learners = albGlobal.Learners?.Select(MapAlbLearner).ToList()
            };
        }

        private ALBLearner MapAlbLearner(FundingService.ALB.FundingOutput.Model.Output.ALBLearner albLearner)
        {
            return new ALBLearner()
            {
                LearnRefNumber = albLearner.LearnRefNumber,
                LearningDeliveries = albLearner.LearningDeliveries?.Select(MapAlbLearningDelivery).ToList(),
                LearnerPeriodisedValues = albLearner.LearnerPeriodisedValues?.Select(MapAlbLearnerPeriodisedValue).ToList()
            };
        }

        private LearningDelivery MapAlbLearningDelivery(FundingService.ALB.FundingOutput.Model.Output.LearningDelivery albLearningDelivery)
        {
            return new LearningDelivery()
            {
                AimSeqNumber = albLearningDelivery.AimSeqNumber,
                LearningDeliveryValue = MapAlbLearningDeliveryValue(albLearningDelivery.LearningDeliveryValue),
                LearningDeliveryPeriodisedValues = albLearningDelivery.LearningDeliveryPeriodisedValues.Select(MapAlbLearningDeliveryPeriodisedValue).ToList()
            };
        }

        private LearningDeliveryValue MapAlbLearningDeliveryValue(FundingService.ALB.FundingOutput.Model.Output.LearningDeliveryValue albLearningDeliveryValue)
        {
            return new LearningDeliveryValue()
            {
                Achieved = albLearningDeliveryValue.Achieved,
                ActualNumInstalm = albLearningDeliveryValue.ActualNumInstalm,
                AdvLoan = albLearningDeliveryValue.AdvLoan,
                ApplicFactDate = albLearningDeliveryValue.ApplicFactDate,
                ApplicProgWeightFact = albLearningDeliveryValue.ApplicProgWeightFact,
                AreaCostFactAdj = albLearningDeliveryValue.AreaCostFactAdj,
                AreaCostInstalment = albLearningDeliveryValue.AreaCostInstalment,
                FundLine = albLearningDeliveryValue.FundLine,
                FundStart = albLearningDeliveryValue.FundStart,
                LiabilityDate = albLearningDeliveryValue.LiabilityDate,
                LoanBursAreaUplift = albLearningDeliveryValue.LoanBursAreaUplift,
                LoanBursSupp = albLearningDeliveryValue.LoanBursSupp,
                OutstndNumOnProgInstalm = albLearningDeliveryValue.OutstndNumOnProgInstalm,
                PlannedNumOnProgInstalm = albLearningDeliveryValue.PlannedNumOnProgInstalm,
                WeightedRate = albLearningDeliveryValue.WeightedRate,
            };
        }

        private LearningDeliveryPeriodisedValue MapAlbLearningDeliveryPeriodisedValue(FundingService.ALB.FundingOutput.Model.Output.LearningDeliveryPeriodisedValue albLearningDeliveryPeriodisedValue)
        {
            return new LearningDeliveryPeriodisedValue()
            {
                AttributeName = albLearningDeliveryPeriodisedValue.AttributeName,
                Period1 = albLearningDeliveryPeriodisedValue.Period1,
                Period2 = albLearningDeliveryPeriodisedValue.Period2,
                Period3 = albLearningDeliveryPeriodisedValue.Period3,
                Period4 = albLearningDeliveryPeriodisedValue.Period4,
                Period5 = albLearningDeliveryPeriodisedValue.Period5,
                Period6 = albLearningDeliveryPeriodisedValue.Period6,
                Period7 = albLearningDeliveryPeriodisedValue.Period7,
                Period8 = albLearningDeliveryPeriodisedValue.Period8,
                Period9 = albLearningDeliveryPeriodisedValue.Period9,
                Period10 = albLearningDeliveryPeriodisedValue.Period10,
                Period11 = albLearningDeliveryPeriodisedValue.Period11,
                Period12 = albLearningDeliveryPeriodisedValue.Period12
            };
        }

        private LearnerPeriodisedValue MapAlbLearnerPeriodisedValue(FundingService.ALB.FundingOutput.Model.Output.LearnerPeriodisedValue albLearnerPeriodisedValue)
        {
            return new LearnerPeriodisedValue()
            {
                AttributeName = albLearnerPeriodisedValue.AttributeName,
                Period1 = albLearnerPeriodisedValue.Period1,
                Period2 = albLearnerPeriodisedValue.Period2,
                Period3 = albLearnerPeriodisedValue.Period3,
                Period4 = albLearnerPeriodisedValue.Period4,
                Period5 = albLearnerPeriodisedValue.Period5,
                Period6 = albLearnerPeriodisedValue.Period6,
                Period7 = albLearnerPeriodisedValue.Period7,
                Period8 = albLearnerPeriodisedValue.Period8,
                Period9 = albLearnerPeriodisedValue.Period9,
                Period10 = albLearnerPeriodisedValue.Period10,
                Period11 = albLearnerPeriodisedValue.Period11,
                Period12 = albLearnerPeriodisedValue.Period12
            };
        }
    }
}
