using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using ESFA.DC.ILR1819.ReportService.Interface.Builders;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;

namespace ESFA.DC.ILR1819.ReportService.Service.Builders
{
    public sealed class Fm36Builder : IFm36Builder
    {
        private readonly ITotalBuilder _totalBuilder;
        private readonly ICacheProviderService<LearningDelivery[]> _cacheProviderService;

        public Fm36Builder(ITotalBuilder totalBuilder, ICacheProviderService<LearningDelivery[]> cacheProviderService)
        {
            _totalBuilder = totalBuilder;
            _cacheProviderService = cacheProviderService;
        }

        public FundingSummaryModel BuildWithFundLine(string title, FM36Global fm36Global, List<string> validLearners, string[] fundLines, string[] attributes)
        {
            FundingSummaryModel fundingSummaryModel = new FundingSummaryModel(title);

            if (fm36Global?.Learners == null)
            {
                return fundingSummaryModel;
            }

            LearningDelivery[] learningDeliveries = _cacheProviderService.Get(fundLines.GetHashCode());
            if (learningDeliveries == null)
            {
                FM36Learner[] learners = fm36Global.Learners.Where(x => validLearners.Contains(x.LearnRefNumber)).ToArray();

                learningDeliveries = learners.SelectMany(x => x.LearningDeliveries).Where(x => fundLines.Contains(x.LearningDeliveryValues.LearnDelInitialFundLineType)).ToArray();

                _cacheProviderService.Set(fundLines.GetHashCode(), learningDeliveries);
            }

            foreach (LearningDelivery learningDelivery in learningDeliveries)
            {
                foreach (LearningDeliveryPeriodisedValues learningDeliveryPeriodisedValue in learningDelivery.LearningDeliveryPeriodisedValues)
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
