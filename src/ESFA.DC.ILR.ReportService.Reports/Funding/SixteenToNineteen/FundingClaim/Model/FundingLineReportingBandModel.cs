using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.HighNeedsStudentSummary.Model
{
    public class FundingLineReportingBandModel
    {
        public int Band5StudentNumbers { get; set; }
        public int Band4aStudentNumbers { get; set; }
        public int Band4bStudentNumbers { get; set; }
        public int Band3StudentNumbers { get; set; }
        public int Band2StudentNumbers { get; set; }
        public int Band1StudentNumbers { get; set; }
        public decimal Band5TotalFunding { get; set; }
        public decimal Band4aTotalFunding { get; set; }
        public decimal Band4bTotalFunding { get; set; }
        public decimal Band3TotalFunding { get; set; }
        public decimal Band2TotalFunding { get; set; }
        public decimal Band1TotalFunding { get; set; }
    }
}
