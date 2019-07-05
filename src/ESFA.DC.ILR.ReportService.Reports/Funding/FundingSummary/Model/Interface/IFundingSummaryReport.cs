using System.Collections.Generic;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model.Interface
{
    public interface IFundingSummaryReport
    {
        List<IFundingCategory> FundingCategories { get; }
    }
}
