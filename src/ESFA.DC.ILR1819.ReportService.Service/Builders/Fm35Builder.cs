using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output;
using ESFA.DC.ILR1819.ReportService.Interface.Builders;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;

namespace ESFA.DC.ILR1819.ReportService.Service.Builders
{
    public sealed class Fm35Builder : IFm35Builder
    {
        private readonly ITotalBuilder _totalBuilder;

        // Singleton
        public Fm35Builder(ITotalBuilder totalBuilder)
        {
            _totalBuilder = totalBuilder;
        }

        public IList<SummaryOfFm35FundingModel> BuildModel(LearningDelivery fundLineData)
        {
            var summaryOfFm35Funding = new List<SummaryOfFm35FundingModel>();

            for (var period = 1; period < 13; period++)
            {
                var onProgramme = fundLineData.LearningDeliveryPeriodisedValues
                    .Where(x => x.AttributeName == Constants.Fm35OnProgrammeAttributeName)
                    .Sum(x => (decimal?)x.GetType().GetProperty($"Period{period}")?.GetValue(x) ?? 0);

                var balancing = fundLineData.LearningDeliveryPeriodisedValues
                    .Where(x => x.AttributeName == Constants.Fm35BalancingAttributeName)
                    .Sum(x => (decimal?)x.GetType().GetProperty($"Period{period}")?.GetValue(x) ?? 0);

                var jobOutcomeAchievement = fundLineData.LearningDeliveryPeriodisedValues
                    .Where(x => x.AttributeName == Constants.Fm35JobOutcomeAchievementAttributeName)
                    .Sum(x => (decimal?)x.GetType().GetProperty($"Period{period}")?.GetValue(x) ?? 0);

                var aimAchievement = fundLineData.LearningDeliveryPeriodisedValues
                    .Where(x => x.AttributeName == Constants.Fm35AimAchievementAttributeName)
                    .Sum(x => (decimal?)x.GetType().GetProperty($"Period{period}")?.GetValue(x) ?? 0);

                var learningSupport = fundLineData.LearningDeliveryPeriodisedValues
                    .Where(x => x.AttributeName == Constants.Fm35LearningSupportAttributeName)
                    .Sum(x => (decimal?)x.GetType().GetProperty($"Period{period}")?.GetValue(x) ?? 0);

                var totalAchievement = jobOutcomeAchievement + aimAchievement;

                summaryOfFm35Funding.Add(new SummaryOfFm35FundingModel
                {
                    FundingLineType = fundLineData.LearningDeliveryValue.FundLine,
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

        public FundingSummaryModel BuildWithFundLine(string title, FM35Global fm35Global, List<string> validLearners, string fundLine, string[] attributes)
        {
            FundingSummaryModel fundingSummaryModel = new FundingSummaryModel { Title = title };

            FM35Learner[] learners = fm35Global.Learners.Where(x => validLearners.Contains(x.LearnRefNumber)).ToArray();

            LearningDelivery[] learningDeliveries = learners.SelectMany(x => x.LearningDeliveries).Where(x => string.Equals(x.LearningDeliveryValue.FundLine, fundLine, StringComparison.OrdinalIgnoreCase)).ToArray();

            foreach (LearningDelivery learningDelivery in learningDeliveries)
            {
                foreach (LearningDeliveryPeriodisedValue learningDeliveryPeriodisedValue in learningDelivery.LearningDeliveryPeriodisedValues)
                {
                    if (attributes.Contains(learningDeliveryPeriodisedValue.AttributeName))
                    {
                        fundingSummaryModel.Period1 = _totalBuilder.Total(fundingSummaryModel.Period1, learningDeliveryPeriodisedValue.Period1);
                        fundingSummaryModel.Period2 = _totalBuilder.Total(fundingSummaryModel.Period2, learningDeliveryPeriodisedValue.Period2);
                        fundingSummaryModel.Period3 = _totalBuilder.Total(fundingSummaryModel.Period3, learningDeliveryPeriodisedValue.Period3);
                        fundingSummaryModel.Period4 = _totalBuilder.Total(fundingSummaryModel.Period4, learningDeliveryPeriodisedValue.Period4);
                        fundingSummaryModel.Period5 = _totalBuilder.Total(fundingSummaryModel.Period5, learningDeliveryPeriodisedValue.Period5);
                        fundingSummaryModel.Period6 = _totalBuilder.Total(fundingSummaryModel.Period6, learningDeliveryPeriodisedValue.Period6);
                        fundingSummaryModel.Period7 = _totalBuilder.Total(fundingSummaryModel.Period7, learningDeliveryPeriodisedValue.Period7);
                        fundingSummaryModel.Period8 = _totalBuilder.Total(fundingSummaryModel.Period8, learningDeliveryPeriodisedValue.Period8);
                        fundingSummaryModel.Period9 = _totalBuilder.Total(fundingSummaryModel.Period9, learningDeliveryPeriodisedValue.Period9);
                        fundingSummaryModel.Period10 = _totalBuilder.Total(fundingSummaryModel.Period10, learningDeliveryPeriodisedValue.Period10);
                        fundingSummaryModel.Period11 = _totalBuilder.Total(fundingSummaryModel.Period11, learningDeliveryPeriodisedValue.Period11);
                        fundingSummaryModel.Period12 = _totalBuilder.Total(fundingSummaryModel.Period12, learningDeliveryPeriodisedValue.Period12);

                        fundingSummaryModel.Period1_8 = _totalBuilder.Total(fundingSummaryModel.Period1_8, learningDeliveryPeriodisedValue.Period1);
                        fundingSummaryModel.Period1_8 = _totalBuilder.Total(fundingSummaryModel.Period1_8, learningDeliveryPeriodisedValue.Period2);
                        fundingSummaryModel.Period1_8 = _totalBuilder.Total(fundingSummaryModel.Period1_8, learningDeliveryPeriodisedValue.Period3);
                        fundingSummaryModel.Period1_8 = _totalBuilder.Total(fundingSummaryModel.Period1_8, learningDeliveryPeriodisedValue.Period4);
                        fundingSummaryModel.Period1_8 = _totalBuilder.Total(fundingSummaryModel.Period1_8, learningDeliveryPeriodisedValue.Period5);
                        fundingSummaryModel.Period1_8 = _totalBuilder.Total(fundingSummaryModel.Period1_8, learningDeliveryPeriodisedValue.Period6);
                        fundingSummaryModel.Period1_8 = _totalBuilder.Total(fundingSummaryModel.Period1_8, learningDeliveryPeriodisedValue.Period7);
                        fundingSummaryModel.Period1_8 = _totalBuilder.Total(fundingSummaryModel.Period1_8, learningDeliveryPeriodisedValue.Period8);

                        fundingSummaryModel.Period9_12 = _totalBuilder.Total(fundingSummaryModel.Period9_12, learningDeliveryPeriodisedValue.Period9);
                        fundingSummaryModel.Period9_12 = _totalBuilder.Total(fundingSummaryModel.Period9_12, learningDeliveryPeriodisedValue.Period10);
                        fundingSummaryModel.Period9_12 = _totalBuilder.Total(fundingSummaryModel.Period9_12, learningDeliveryPeriodisedValue.Period11);
                        fundingSummaryModel.Period9_12 = _totalBuilder.Total(fundingSummaryModel.Period9_12, learningDeliveryPeriodisedValue.Period12);

                        fundingSummaryModel.Total = _totalBuilder.Total(fundingSummaryModel.Total, fundingSummaryModel.Period1_8);
                        fundingSummaryModel.Total = _totalBuilder.Total(fundingSummaryModel.Total, fundingSummaryModel.Period9_12);
                    }
                }
            }

            return fundingSummaryModel;
        }
    }
}
