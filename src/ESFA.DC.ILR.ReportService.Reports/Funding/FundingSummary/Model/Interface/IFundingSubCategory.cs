using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model.Interface
{
    public interface IFundingSubCategory :  IFundingSummaryReportRow
    {
        string FundingSubCategoryTitle { get; }
    }
}
