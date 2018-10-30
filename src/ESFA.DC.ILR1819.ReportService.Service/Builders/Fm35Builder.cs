using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output;
using ESFA.DC.ILR1819.ReportService.Interface.Builders;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;

namespace ESFA.DC.ILR1819.ReportService.Service.Builders
{
    public sealed class Fm35Builder : IFm35Builder
    {
        private readonly ITotalBuilder _totalBuilder;
        private readonly ICacheProviderService<LearningDelivery[]> _cacheProviderService;

        // Singleton
        public Fm35Builder(ITotalBuilder totalBuilder, ICacheProviderService<LearningDelivery[]> cacheProviderService)
        {
            _totalBuilder = totalBuilder;
            _cacheProviderService = cacheProviderService;
        }

        public IList<SummaryOfFm35FundingModel> BuildModel(LearningDelivery fundLineData)
        {
            SummaryOfFm35FundingModel[] summaryOfFm35Funding = new SummaryOfFm35FundingModel[12];
            decimal[] onProgrammes = new decimal[12];
            decimal[] balancing = new decimal[12];
            decimal[] jobOutcomeAchievement = new decimal[12];
            decimal[] aimAchievement = new decimal[12];
            decimal[] learningSupport = new decimal[12];

            foreach (LearningDeliveryPeriodisedValue learningDeliveryPeriodisedValue in fundLineData.LearningDeliveryPeriodisedValues)
            {
                if (string.Equals(learningDeliveryPeriodisedValue.AttributeName, Constants.Fm35OnProgrammeAttributeName, StringComparison.OrdinalIgnoreCase))
                {
                    BuildValues(learningDeliveryPeriodisedValue, ref onProgrammes);
                }
                else if (string.Equals(learningDeliveryPeriodisedValue.AttributeName, Constants.Fm35BalancingAttributeName, StringComparison.OrdinalIgnoreCase))
                {
                    BuildValues(learningDeliveryPeriodisedValue, ref balancing);
                }
                else if (string.Equals(learningDeliveryPeriodisedValue.AttributeName, Constants.Fm35JobOutcomeAchievementAttributeName, StringComparison.OrdinalIgnoreCase))
                {
                    BuildValues(learningDeliveryPeriodisedValue, ref jobOutcomeAchievement);
                }
                else if (string.Equals(learningDeliveryPeriodisedValue.AttributeName, Constants.Fm35AimAchievementAttributeName, StringComparison.OrdinalIgnoreCase))
                {
                    BuildValues(learningDeliveryPeriodisedValue, ref aimAchievement);
                }
                else if (string.Equals(learningDeliveryPeriodisedValue.AttributeName, Constants.Fm35LearningSupportAttributeName, StringComparison.OrdinalIgnoreCase))
                {
                    BuildValues(learningDeliveryPeriodisedValue, ref learningSupport);
                }
            }

            for (int i = 0; i < 12; i++)
            {
                decimal totalAchievement = jobOutcomeAchievement[i] + aimAchievement[i];
                summaryOfFm35Funding[i] = new SummaryOfFm35FundingModel
                {
                    FundingLineType = fundLineData.LearningDeliveryValue.FundLine,
                    Period = i + 1,
                    OnProgramme = onProgrammes[i],
                    Balancing = balancing[i],
                    JobOutcomeAchievement = jobOutcomeAchievement[i],
                    AimAchievement = aimAchievement[i],
                    TotalAchievement = totalAchievement,
                    LearningSupport = learningSupport[i],
                    Total = onProgrammes[i] + balancing[i] + learningSupport[i] + totalAchievement
                };
            }

            return summaryOfFm35Funding;
        }

        public FundingSummaryModel BuildWithFundLine(string title, FM35Global fm35Global, List<string> validLearners, string[] fundLines, string[] attributes)
        {
            FundingSummaryModel fundingSummaryModel = new FundingSummaryModel(title);

            if (fm35Global?.Learners == null)
            {
                return fundingSummaryModel;
            }

            LearningDelivery[] learningDeliveries = _cacheProviderService.Get(fundLines.GetHashCode());
            if (learningDeliveries == null)
            {
                FM35Learner[] learners = fm35Global.Learners.Where(x => validLearners.Contains(x.LearnRefNumber)).ToArray();

                learningDeliveries = learners.SelectMany(x => x.LearningDeliveries).Where(x => fundLines.Contains(x.LearningDeliveryValue.FundLine)).ToArray();

                _cacheProviderService.Set(fundLines.GetHashCode(), learningDeliveries);
            }

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

        private void BuildValues(LearningDeliveryPeriodisedValue learningDeliveryPeriodisedValue, ref decimal[] values)
        {
            for (int i = 0; i < 12; i++)
            {
                values[i] = GetPeriodValue(learningDeliveryPeriodisedValue, i + 1);
            }
        }

        private decimal GetPeriodValue(LearningDeliveryPeriodisedValue learningDeliveryPeriodisedValue, int period)
        {
            switch (period)
            {
                case 1:
                    return learningDeliveryPeriodisedValue.Period1 ?? 0;
                case 2:
                    return learningDeliveryPeriodisedValue.Period2 ?? 0;
                case 3:
                    return learningDeliveryPeriodisedValue.Period3 ?? 0;
                case 4:
                    return learningDeliveryPeriodisedValue.Period4 ?? 0;
                case 5:
                    return learningDeliveryPeriodisedValue.Period5 ?? 0;
                case 6:
                    return learningDeliveryPeriodisedValue.Period6 ?? 0;
                case 7:
                    return learningDeliveryPeriodisedValue.Period7 ?? 0;
                case 8:
                    return learningDeliveryPeriodisedValue.Period8 ?? 0;
                case 9:
                    return learningDeliveryPeriodisedValue.Period9 ?? 0;
                case 10:
                    return learningDeliveryPeriodisedValue.Period10 ?? 0;
                case 11:
                    return learningDeliveryPeriodisedValue.Period11 ?? 0;
                case 12:
                    return learningDeliveryPeriodisedValue.Period12 ?? 0;
                default:
                    throw new ArgumentOutOfRangeException(nameof(period));
            }
        }
    }
}
