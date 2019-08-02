using System.Collections.Generic;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.DevolvedFundingSummary.Model.Interface
{
    public interface IDevolvedAdultEducationFundingCategory : IDevolvedAdultEducationFundingSummaryReportRow
    {
        string FundingCategoryTitle { get; }

        IList<IDevolvedAdultEducationFundLineGroup> FundLineGroups { get; }

        string CumulativeFundingCategoryTitle { get; }

        decimal CumulativePeriod1 { get; }

        decimal CumulativePeriod2 { get; }

        decimal CumulativePeriod3 { get; }

        decimal CumulativePeriod4 { get; }

        decimal CumulativePeriod5 { get; }

        decimal CumulativePeriod6 { get; }

        decimal CumulativePeriod7 { get; }

        decimal CumulativePeriod8 { get; }

        decimal CumulativePeriod9 { get; }

        decimal CumulativePeriod10 { get; }

        decimal CumulativePeriod11 { get; }

        decimal CumulativePeriod12 { get; }
    }
}
