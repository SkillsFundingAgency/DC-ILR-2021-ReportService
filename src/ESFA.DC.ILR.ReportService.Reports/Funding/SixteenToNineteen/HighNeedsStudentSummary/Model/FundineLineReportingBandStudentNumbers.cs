using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.HighNeedsStudentSummary.Model
{
    public class FundingLineReportingBandStudentNumbers
    {
        public int WithEHCP { get; set; }
        public int WithoutEHCP { get; set; }
        public int HNSWithoutEHCP { get; set; }
        public int EHCPWithHNS { get; set; }
        public int EHCPWithoutHNS { get; set; }
    }
}
