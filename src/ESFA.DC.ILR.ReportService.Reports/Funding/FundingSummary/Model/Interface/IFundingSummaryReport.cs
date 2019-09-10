using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Reports.Model.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model.Interface
{
    public interface IFundingSummaryReport
    {
        List<IFundingCategory> FundingCategories { get; }

        ISummaryPage SummaryPage { get; }
    }
}
