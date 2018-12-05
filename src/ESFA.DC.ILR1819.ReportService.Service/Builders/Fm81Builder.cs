using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.FundingService.FM81.FundingOutput.Model.Output;
using ESFA.DC.ILR1819.ReportService.Interface.Builders;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;
using LearningDelivery = ESFA.DC.ILR.FundingService.FM81.FundingOutput.Model.Output.LearningDelivery;

namespace ESFA.DC.ILR1819.ReportService.Service.Builders
{
    public sealed class Fm81Builder : IFm81Builder
    {
        private readonly ITotalBuilder _totalBuilder;
        private readonly ICacheProviderService<LearningDelivery[]> _cacheProviderService;

        public Fm81Builder(ITotalBuilder totalBuilder, ICacheProviderService<LearningDelivery[]> cacheProviderService)
        {
            _totalBuilder = totalBuilder;
            _cacheProviderService = cacheProviderService;
        }

        public FundingSummaryModel BuildWithFundLine(string title, FM81Global fm81Global, List<string> validLearners, string fundLine, string[] attributes)
        {
            FundingSummaryModel fundingSummaryModel = new FundingSummaryModel(title);

            if (fm81Global?.Learners == null || validLearners == null)
            {
                return fundingSummaryModel;
            }

            LearningDelivery[] learningDeliveries = _cacheProviderService.Get(fundLine);
            if (learningDeliveries == null)
            {
                FM81Learner[] learners =
                    fm81Global.Learners.Where(x => validLearners.Contains(x.LearnRefNumber)).ToArray();

                learningDeliveries = learners.SelectMany(x => x.LearningDeliveries)
                    .Where(x => string.Equals(fundLine, x.LearningDeliveryValues.FundLine, StringComparison.OrdinalIgnoreCase)).ToArray();

                _cacheProviderService.Set(fundLine, learningDeliveries);
            }

            LearningDeliveryPeriodisedValue[] periodisedValues = learningDeliveries.SelectMany(x => x.LearningDeliveryPeriodisedValues)
                .Where(x => attributes.Contains(x.AttributeName)).ToArray();

            fundingSummaryModel.Period1 = periodisedValues.Sum(x => x.Period1 ?? 0);
            fundingSummaryModel.Period2 = periodisedValues.Sum(x => x.Period2 ?? 0);
            fundingSummaryModel.Period3 = periodisedValues.Sum(x => x.Period3 ?? 0);
            fundingSummaryModel.Period4 = periodisedValues.Sum(x => x.Period4 ?? 0);
            fundingSummaryModel.Period5 = periodisedValues.Sum(x => x.Period5 ?? 0);
            fundingSummaryModel.Period6 = periodisedValues.Sum(x => x.Period6 ?? 0);
            fundingSummaryModel.Period7 = periodisedValues.Sum(x => x.Period7 ?? 0);
            fundingSummaryModel.Period8 = periodisedValues.Sum(x => x.Period8 ?? 0);
            fundingSummaryModel.Period9 = periodisedValues.Sum(x => x.Period9 ?? 0);
            fundingSummaryModel.Period10 = periodisedValues.Sum(x => x.Period10 ?? 0);
            fundingSummaryModel.Period11 = periodisedValues.Sum(x => x.Period11 ?? 0);
            fundingSummaryModel.Period12 = periodisedValues.Sum(x => x.Period12 ?? 0);

            fundingSummaryModel.Period1_8 =
                fundingSummaryModel.Period1 + fundingSummaryModel.Period2 + fundingSummaryModel.Period3 +
                fundingSummaryModel.Period4 + fundingSummaryModel.Period5 + fundingSummaryModel.Period6 +
                fundingSummaryModel.Period7 + fundingSummaryModel.Period8;
            fundingSummaryModel.Period9_12 =
                fundingSummaryModel.Period9 + fundingSummaryModel.Period10 + fundingSummaryModel.Period11 +
                fundingSummaryModel.Period12;

            // fundingSummaryModel.YearToDate = GetYearToDate(fundingSummaryModel, period); // todo: pass period
            fundingSummaryModel.Total = GetYearToDate(fundingSummaryModel, 12);

            //foreach (LearningDelivery learningDelivery in learningDeliveries)
            //{
            //    foreach (LearningDeliveryPeriodisedValue learningDeliveryPeriodisedValue in learningDelivery.LearningDeliveryPeriodisedValues)
            //    {
            //        if (attributes.Contains(learningDeliveryPeriodisedValue.AttributeName))
            //        {
            //            fundingSummaryModel.Period1 = _totalBuilder.Total(fundingSummaryModel.Period1, learningDeliveryPeriodisedValue.Period1);
            //            fundingSummaryModel.Period2 = _totalBuilder.Total(fundingSummaryModel.Period2, learningDeliveryPeriodisedValue.Period2);
            //            fundingSummaryModel.Period3 = _totalBuilder.Total(fundingSummaryModel.Period3, learningDeliveryPeriodisedValue.Period3);
            //            fundingSummaryModel.Period4 = _totalBuilder.Total(fundingSummaryModel.Period4, learningDeliveryPeriodisedValue.Period4);
            //            fundingSummaryModel.Period5 = _totalBuilder.Total(fundingSummaryModel.Period5, learningDeliveryPeriodisedValue.Period5);
            //            fundingSummaryModel.Period6 = _totalBuilder.Total(fundingSummaryModel.Period6, learningDeliveryPeriodisedValue.Period6);
            //            fundingSummaryModel.Period7 = _totalBuilder.Total(fundingSummaryModel.Period7, learningDeliveryPeriodisedValue.Period7);
            //            fundingSummaryModel.Period8 = _totalBuilder.Total(fundingSummaryModel.Period8, learningDeliveryPeriodisedValue.Period8);
            //            fundingSummaryModel.Period9 = _totalBuilder.Total(fundingSummaryModel.Period9, learningDeliveryPeriodisedValue.Period9);
            //            fundingSummaryModel.Period10 = _totalBuilder.Total(fundingSummaryModel.Period10, learningDeliveryPeriodisedValue.Period10);
            //            fundingSummaryModel.Period11 = _totalBuilder.Total(fundingSummaryModel.Period11, learningDeliveryPeriodisedValue.Period11);
            //            fundingSummaryModel.Period12 = _totalBuilder.Total(fundingSummaryModel.Period12, learningDeliveryPeriodisedValue.Period12);

            //            fundingSummaryModel.Period1_8 = _totalBuilder.Total(fundingSummaryModel.Period1_8, learningDeliveryPeriodisedValue.Period1);
            //            fundingSummaryModel.Period1_8 = _totalBuilder.Total(fundingSummaryModel.Period1_8, learningDeliveryPeriodisedValue.Period2);
            //            fundingSummaryModel.Period1_8 = _totalBuilder.Total(fundingSummaryModel.Period1_8, learningDeliveryPeriodisedValue.Period3);
            //            fundingSummaryModel.Period1_8 = _totalBuilder.Total(fundingSummaryModel.Period1_8, learningDeliveryPeriodisedValue.Period4);
            //            fundingSummaryModel.Period1_8 = _totalBuilder.Total(fundingSummaryModel.Period1_8, learningDeliveryPeriodisedValue.Period5);
            //            fundingSummaryModel.Period1_8 = _totalBuilder.Total(fundingSummaryModel.Period1_8, learningDeliveryPeriodisedValue.Period6);
            //            fundingSummaryModel.Period1_8 = _totalBuilder.Total(fundingSummaryModel.Period1_8, learningDeliveryPeriodisedValue.Period7);
            //            fundingSummaryModel.Period1_8 = _totalBuilder.Total(fundingSummaryModel.Period1_8, learningDeliveryPeriodisedValue.Period8);

            //            fundingSummaryModel.Period9_12 = _totalBuilder.Total(fundingSummaryModel.Period9_12, learningDeliveryPeriodisedValue.Period9);
            //            fundingSummaryModel.Period9_12 = _totalBuilder.Total(fundingSummaryModel.Period9_12, learningDeliveryPeriodisedValue.Period10);
            //            fundingSummaryModel.Period9_12 = _totalBuilder.Total(fundingSummaryModel.Period9_12, learningDeliveryPeriodisedValue.Period11);
            //            fundingSummaryModel.Period9_12 = _totalBuilder.Total(fundingSummaryModel.Period9_12, learningDeliveryPeriodisedValue.Period12);

            //            fundingSummaryModel.Total = _totalBuilder.Total(fundingSummaryModel.Total, fundingSummaryModel.Period1_8);
            //            fundingSummaryModel.Total = _totalBuilder.Total(fundingSummaryModel.Total, fundingSummaryModel.Period9_12);
            //        }
            //    }
            //}

            return fundingSummaryModel;
        }

        private decimal? GetYearToDate(FundingSummaryModel fundingSummaryModel, int period)
        {
            decimal total = 0;
            for (int i = 0; i < period; i++)
            {
                switch (i)
                {
                    case 0:
                        total += fundingSummaryModel.Period1 ?? 0;
                        break;
                    case 1:
                        total += fundingSummaryModel.Period2 ?? 0;
                        break;
                    case 2:
                        total += fundingSummaryModel.Period3 ?? 0;
                        break;
                    case 3:
                        total += fundingSummaryModel.Period4 ?? 0;
                        break;
                    case 4:
                        total += fundingSummaryModel.Period5 ?? 0;
                        break;
                    case 5:
                        total += fundingSummaryModel.Period6 ?? 0;
                        break;
                    case 6:
                        total += fundingSummaryModel.Period7 ?? 0;
                        break;
                    case 7:
                        total += fundingSummaryModel.Period8 ?? 0;
                        break;
                    case 8:
                        total += fundingSummaryModel.Period9 ?? 0;
                        break;
                    case 9:
                        total += fundingSummaryModel.Period10 ?? 0;
                        break;
                    case 10:
                        total += fundingSummaryModel.Period11 ?? 0;
                        break;
                    case 11:
                        total += fundingSummaryModel.Period12 ?? 0;
                        break;
                }
            }

            return total;
        }
    }
}
