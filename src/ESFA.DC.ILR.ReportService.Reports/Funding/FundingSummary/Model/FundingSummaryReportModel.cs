using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model.Interface;
using ESFA.DC.ILR.ReportService.Reports.Model;
using ESFA.DC.ILR.ReportService.Reports.Model.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model
{
    public class FundingSummaryReportModel : IFundingSummaryReport
    {
        public FundingSummaryReportModel(List<IFundingCategory> fundingCategories, ISummaryPage summaryPage)
        {
            FundingCategories = fundingCategories ?? new List<IFundingCategory>();
            SummaryPage = summaryPage ?? new SummaryPageModel();
        }

        public List<IFundingCategory> FundingCategories { get; }

        public ISummaryPage SummaryPage { get; }
    }
}
