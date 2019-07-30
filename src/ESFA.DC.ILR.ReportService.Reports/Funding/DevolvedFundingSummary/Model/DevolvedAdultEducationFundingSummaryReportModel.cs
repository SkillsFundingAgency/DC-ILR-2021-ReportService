using System;
using System.Collections.Generic;
using System.Text;
using ESFA.DC.ILR.ReportService.Reports.Funding.DevolvedFundingSummary.Model.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.DevolvedFundingSummary.Model
{
    public class DevolvedAdultEducationFundingSummaryReportModel : IDevolvedAdultEducationFundingSummaryReport
    {
        public DevolvedAdultEducationFundingSummaryReportModel(List<IDevolvedAdultEducationFundingArea> devolvedFundingAreas)
        {
            DevolvedFundingAreas = devolvedFundingAreas ?? new List<IDevolvedAdultEducationFundingArea>();
        }

        public List<IDevolvedAdultEducationFundingArea> DevolvedFundingAreas { get; }
    }
}
