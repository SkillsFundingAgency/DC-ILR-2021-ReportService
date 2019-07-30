using System.Collections.Generic;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.DevolvedFundingSummary.Model.Interface
{
    public interface IDevolvedAdultEducationFundingSummaryReport
    {
        List<IDevolvedAdultEducationFundingArea> DevolvedFundingAreas { get; }
    }
}
