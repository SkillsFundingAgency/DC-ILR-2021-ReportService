using System.Collections.Generic;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model.Interface
{
    public interface IFundLineGroup : IFundingSummaryReportRow
    {
        IList<IFundLine> FundLines { get; }
    }
}
