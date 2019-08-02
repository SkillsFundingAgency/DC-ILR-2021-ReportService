using System.Collections.Generic;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.DevolvedFundingSummary.Model.Interface
{
    public interface IDevolvedAdultEducationFundLineGroup : IDevolvedAdultEducationFundingSummaryReportRow
    {
        IList<IDevolvedAdultEducationFundLine> FundLines { get; }
    }
}
