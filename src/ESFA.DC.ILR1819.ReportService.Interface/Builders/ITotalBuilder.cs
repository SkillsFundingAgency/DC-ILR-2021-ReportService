using System.Collections.Generic;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;

namespace ESFA.DC.ILR1819.ReportService.Interface.Builders
{
    public interface ITotalBuilder
    {
        FundingSummaryModel TotalRecords(string title, params FundingSummaryModel[] fundingSummaryModels);

        decimal TotalRecords(params decimal?[] values);

        decimal? Total(decimal? original, decimal? value);

        FundingSummaryModel TotalRecordsCumulative(string title, FundingSummaryModel sourceFundingSummaryModel);
    }
}
