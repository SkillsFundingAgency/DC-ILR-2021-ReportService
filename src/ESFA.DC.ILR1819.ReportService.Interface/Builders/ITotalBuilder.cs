using System.Collections.Generic;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;

namespace ESFA.DC.ILR1819.ReportService.Interface.Builders
{
    public interface ITotalBuilder
    {
        List<FundingSummaryModel> TotalRecords(string title, List<FundingSummaryModel> fundingSummaryModels);
    }
}
