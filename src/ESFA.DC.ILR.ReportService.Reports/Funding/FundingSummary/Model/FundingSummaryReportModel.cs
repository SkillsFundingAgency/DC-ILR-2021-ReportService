using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model
{
    public class FundingSummaryReportModel : IFundingSummaryReport
    {
        public FundingSummaryReportModel(
            IDictionary<string, string> headerData,
            List<IFundingCategory> fundingCategories,
            IDictionary<string, string> footerData)
        {
            FundingCategories = fundingCategories ?? new List<IFundingCategory>();
            HeaderData = headerData ?? new Dictionary<string, string>();
            FooterData = footerData ?? new Dictionary<string, string>();
        }

        public IDictionary<string, string> HeaderData { get; }

        public List<IFundingCategory> FundingCategories { get; }

        public IDictionary<string, string> FooterData { get; }
    }
}
