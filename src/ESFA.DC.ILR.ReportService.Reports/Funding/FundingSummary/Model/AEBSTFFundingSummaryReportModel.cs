using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model
{
    public class AEBSTFFundingSummaryReportModel : FundingSummaryReportModel
    {
        public AEBSTFFundingSummaryReportModel(
            IDictionary<string, string> headerData, 
            List<IFundingCategory> fundingCategories, 
            IDictionary<string, string> footerData) : base(headerData, fundingCategories, footerData)
        {
        }
    }
}
