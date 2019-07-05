using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model
{
    public class FundingSummaryReport : IFundingSummaryReport
    {
        public FundingSummaryReport(List<IFundingCategory> fundingCategories)
        {
            FundingCategories = fundingCategories ?? new List<IFundingCategory>();
        }

        public List<IFundingCategory> FundingCategories { get; }
    }
}
