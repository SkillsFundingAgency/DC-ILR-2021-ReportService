using System.Collections.Generic;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model.Interface
{
    public interface IFundingSummaryReport
    {
        List<IFundingCategory> FundingCategories { get; }

        IDictionary<string, string> HeaderData { get; }

        IDictionary<string, string> FooterData { get; }
    }
}
