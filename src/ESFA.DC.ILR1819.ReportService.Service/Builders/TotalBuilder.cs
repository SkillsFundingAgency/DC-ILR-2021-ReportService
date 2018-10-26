using ESFA.DC.ILR1819.ReportService.Interface.Builders;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;

namespace ESFA.DC.ILR1819.ReportService.Service.Builders
{
    public sealed class TotalBuilder : ITotalBuilder
    {
        // Registered as singleton because no parameters
        public TotalBuilder()
        {
        }

        public FundingSummaryModel TotalRecords(string title, params FundingSummaryModel[] fundingSummaryModels)
        {
            FundingSummaryModel fundingSummaryModel = new FundingSummaryModel { Title = title };

            foreach (FundingSummaryModel summaryModel in fundingSummaryModels)
            {
                fundingSummaryModel.Period1 = Total(fundingSummaryModel.Period1, summaryModel.Period1);
                fundingSummaryModel.Period2 = Total(fundingSummaryModel.Period2, summaryModel.Period2);
                fundingSummaryModel.Period3 = Total(fundingSummaryModel.Period3, summaryModel.Period3);
                fundingSummaryModel.Period4 = Total(fundingSummaryModel.Period4, summaryModel.Period4);
                fundingSummaryModel.Period5 = Total(fundingSummaryModel.Period5, summaryModel.Period5);
                fundingSummaryModel.Period6 = Total(fundingSummaryModel.Period6, summaryModel.Period6);
                fundingSummaryModel.Period7 = Total(fundingSummaryModel.Period7, summaryModel.Period7);
                fundingSummaryModel.Period8 = Total(fundingSummaryModel.Period8, summaryModel.Period8);
                fundingSummaryModel.Period9 = Total(fundingSummaryModel.Period9, summaryModel.Period9);
                fundingSummaryModel.Period10 = Total(fundingSummaryModel.Period10, summaryModel.Period10);
                fundingSummaryModel.Period11 = Total(fundingSummaryModel.Period11, summaryModel.Period11);
                fundingSummaryModel.Period12 = Total(fundingSummaryModel.Period12, summaryModel.Period12);

                fundingSummaryModel.Period1_8 = Total(fundingSummaryModel.Period1_8, summaryModel.Period1_8);
                fundingSummaryModel.Period9_12 = Total(fundingSummaryModel.Period9_12, summaryModel.Period9_12);
                fundingSummaryModel.Total = Total(fundingSummaryModel.Total, summaryModel.Total);
            }

            return fundingSummaryModel;
        }

        public decimal? Total(decimal? original, decimal? value)
        {
            if (original == null && value == null)
            {
                return null;
            }

            if (value == null)
            {
                return original;
            }

            return original.GetValueOrDefault(0) + value.GetValueOrDefault(0);
        }
    }
}
