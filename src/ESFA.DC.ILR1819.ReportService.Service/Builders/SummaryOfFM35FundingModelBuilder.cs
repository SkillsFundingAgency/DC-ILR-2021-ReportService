using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Attribute;
using ESFA.DC.ILR1819.ReportService.Service.Models;

namespace ESFA.DC.ILR1819.ReportService.Service.Builders
{
    public class SummaryOfFM35FundingModelBuilder : ISummaryOfFM35FundingModelBuilder
    {
        private const string OnProgrammeAttributeName = "OnProgPayment";
        private const string BalancingAttributeName = "BalancePayment";
        private const string JobOutcomeAchievementAttributeName = "EmpOutcomePay";
        private const string AimAchievementAttributeName = "AchievePayment";
        private const string LearningSupportAttributeName = "LearnSuppFundCash";

        public IList<SummaryOfFm35FundingModel> BuildModel(LearningDeliveryAttribute fundLineData)
        {
            var summaryOfFm35Funding = new List<SummaryOfFm35FundingModel>();

            for (var period = 1; period < 13; period++)
            {
                var onProgramme = fundLineData.LearningDeliveryPeriodisedAttributes
                    .Where(x => x.AttributeName == OnProgrammeAttributeName)
                    .Sum(x => (decimal?)x.GetType().GetProperty($"Period{period}")?.GetValue(x) ?? 0);

                var balancing = fundLineData.LearningDeliveryPeriodisedAttributes
                    .Where(x => x.AttributeName == BalancingAttributeName)
                    .Sum(x => (decimal?)x.GetType().GetProperty($"Period{period}")?.GetValue(x) ?? 0);

                var jobOutcomeAchievement = fundLineData.LearningDeliveryPeriodisedAttributes
                    .Where(x => x.AttributeName == JobOutcomeAchievementAttributeName)
                    .Sum(x => (decimal?)x.GetType().GetProperty($"Period{period}")?.GetValue(x) ?? 0);

                var aimAchievement = fundLineData.LearningDeliveryPeriodisedAttributes
                    .Where(x => x.AttributeName == AimAchievementAttributeName)
                    .Sum(x => (decimal?)x.GetType().GetProperty($"Period{period}")?.GetValue(x) ?? 0);

                var learningSupport = fundLineData.LearningDeliveryPeriodisedAttributes
                    .Where(x => x.AttributeName == LearningSupportAttributeName)
                    .Sum(x => (decimal?)x.GetType().GetProperty($"Period{period}")?.GetValue(x) ?? 0);

                var totalAchievement = jobOutcomeAchievement + aimAchievement;

                summaryOfFm35Funding.Add(new SummaryOfFm35FundingModel
                {
                    FundingLineType = fundLineData.LearningDeliveryAttributeDatas.FundLine,
                    Period = period,
                    OnProgramme = onProgramme,
                    Balancing = balancing,
                    JobOutcomeAchievement = jobOutcomeAchievement,
                    AimAchievement = aimAchievement,
                    TotalAchievement = totalAchievement,
                    LearningSupport = learningSupport,
                    Total = onProgramme + balancing + totalAchievement + learningSupport
                });
            }

            return summaryOfFm35Funding;
        }
    }
}
