using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.HighNeedsStudentSummary.Model;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.HighNeedsStudentSummary
{
    public class HighNeedsStudentSummaryReportModel : AbstractReportHeaderFooterModel
    {
        public FundingLineReportingBandStudentNumbers DirectFunded1416StudentsTotal { get; set; }
        public FundingLineReportingBandStudentNumbers IncludingHNS1619StudentsTotal { get; set; }
        public FundingLineReportingBandStudentNumbers EHCP1924StudentsTotal { get; set; }
        public FundingLineReportingBandStudentNumbers Continuing19PlusExcludingEHCPStudentsTotal { get; set; }
    }
}
