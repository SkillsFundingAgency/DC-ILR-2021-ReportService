using System.Collections.Generic;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model.Interface
{
    public interface IFundingSubCategory :  IFundingSummaryReportRow
    {
        IList<IFundLineGroup> FundLineGroups { get; }

        string FundingSubCategoryTitle { get; }
    }
}
