using System.Collections.Generic;
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

        public List<FundingSummaryModel> TotalRecords(string title, List<FundingSummaryModel> fundingSummaryModels)
        {
            FundingSummaryModel fundingSummaryModel = new FundingSummaryModel();
            fundingSummaryModel.Title = title;

            foreach (FundingSummaryModel summaryModel in fundingSummaryModels)
            {
                fundingSummaryModel.Period1 = Total(fundingSummaryModel.Period1, summaryModel.Period1);
            }

            return new List<FundingSummaryModel>
            {
                fundingSummaryModel
            };
        }

        private decimal? Total(decimal? original, decimal? value)
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
