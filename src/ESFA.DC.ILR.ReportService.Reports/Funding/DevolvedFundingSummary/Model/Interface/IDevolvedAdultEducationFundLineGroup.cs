using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.DevolvedFundingSummary.Model.Interface
{
    public interface IDevolvedAdultEducationFundLineGroup : IDevolvedAdultEducationFundingSummaryReportRow
    {
        IList<IDevolvedAdultEducationFundLine> FundLines { get; }
    }
}
