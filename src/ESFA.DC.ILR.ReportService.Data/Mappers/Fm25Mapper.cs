using System.Linq;
using ESFA.DC.ILR.ReportService.Data.Interface.Mappers;
using ESFA.DC.ILR.ReportService.Models.Fm25;

namespace ESFA.DC.ILR.ReportService.Data.Mappers
{
    public class Fm25Mapper : IFm25Mapper
    {
        public FM25Global MapData(FundingService.FM25.Model.Output.FM25Global fm25Global)
        {
            return MapFm25Global(fm25Global);
        }

        private FM25Global MapFm25Global(FundingService.FM25.Model.Output.FM25Global fm25Global)
        {
            return new FM25Global()
            {
                UKPRN = fm25Global.UKPRN,
                LARSVersion = fm25Global.LARSVersion,
                OrgVersion = fm25Global.OrgVersion,
                PostcodeDisadvantageVersion = fm25Global.PostcodeDisadvantageVersion,
                RulebaseVersion = fm25Global.RulebaseVersion,
                Learners = fm25Global.Learners?.Select(MapFm25Learner).ToList()
            };
        }

        private FM25Learner MapFm25Learner(FundingService.FM25.Model.Output.FM25Learner learner)
        {
            return new FM25Learner()
            {
                LearnRefNumber = learner.LearnRefNumber,
                AcadMonthPayment = learner.AcadMonthPayment,
                OnProgPayment = learner.OnProgPayment,
                AcadProg = learner.AcadProg,
                ActualDaysILCurrYear = learner.ActualDaysILCurrYear,
                AreaCostFact1618Hist = learner.AreaCostFact1618Hist,
                Block1DisadvUpliftNew = learner.Block1DisadvUpliftNew,
                Block2DisadvElementsNew = learner.Block2DisadvElementsNew,
                ConditionOfFundingEnglish = learner.ConditionOfFundingEnglish,
                ConditionOfFundingMaths = learner.ConditionOfFundingMaths,
                CoreAimSeqNumber = learner.CoreAimSeqNumber,
                FullTimeEquiv = learner.FullTimeEquiv,
                FundLine = learner.FundLine,
                LearnerActEndDate = learner.LearnerActEndDate,
                LearnerPlanEndDate = learner.LearnerPlanEndDate,
                LearnerStartDate = learner.LearnerStartDate,
                NatRate = learner.NatRate,
                PlannedDaysILCurrYear = learner.PlannedDaysILCurrYear,
                ProgWeightHist = learner.ProgWeightHist,
                ProgWeightNew = learner.ProgWeightNew,
                PrvDisadvPropnHist = learner.PrvDisadvPropnHist,
                PrvHistLrgProgPropn = learner.PrvHistLrgProgPropn,
                PrvRetentFactHist = learner.PrvRetentFactHist,
                RateBand = learner.RateBand,
                RetentNew = learner.RetentNew,
                StartFund = learner.StartFund,
                ThresholdDays = learner.ThresholdDays,
                LearnerPeriods = learner.LearnerPeriods?.Select(MapLearnerPeriod).ToList(),
                LearnerPeriodisedValues = learner.LearnerPeriodisedValues?.Select(MapLearnerPeriodisedValues).ToList()
            };
        }

        private LearnerPeriod MapLearnerPeriod(FundingService.FM25.Model.Output.LearnerPeriod learnerPeriod)
        {
            return new LearnerPeriod()
            {
                LearnRefNumber = learnerPeriod.LearnRefNumber,
                Period = learnerPeriod.Period.Value,
                LnrOnProgPay = learnerPeriod.LnrOnProgPay
            };
        }

        private LearnerPeriodisedValues MapLearnerPeriodisedValues(FundingService.FM25.Model.Output.LearnerPeriodisedValues learnerPeriodisedValues)
        {
            return new LearnerPeriodisedValues()
            {
                LearnRefNumber = learnerPeriodisedValues.LearnRefNumber,
                AttributeName = learnerPeriodisedValues.AttributeName,
                Period1 = learnerPeriodisedValues.Period1,
                Period2 = learnerPeriodisedValues.Period2,
                Period3 = learnerPeriodisedValues.Period3,
                Period4 = learnerPeriodisedValues.Period4,
                Period5 = learnerPeriodisedValues.Period5,
                Period6 = learnerPeriodisedValues.Period6,
                Period7 = learnerPeriodisedValues.Period7,
                Period8 = learnerPeriodisedValues.Period8,
                Period9 = learnerPeriodisedValues.Period9,
                Period10 = learnerPeriodisedValues.Period10,
                Period11 = learnerPeriodisedValues.Period11,
                Period12 = learnerPeriodisedValues.Period12
            };
        }
    }
}
