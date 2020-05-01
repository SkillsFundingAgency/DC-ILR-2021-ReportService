namespace ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.HighNeedsStudentSummary.Model
{
    public class HNSSummaryLearnerGroup
    {
        public FundingLineReportingBandStudentNumbers DirectFunded1416StudentsTotal { get; set; }
        public FundingLineReportingBandStudentNumbers IncludingHNS1619StudentsTotal { get; set; }
        public FundingLineReportingBandStudentNumbers EHCP1924StudentsTotal { get; set; }
        public FundingLineReportingBandStudentNumbers Continuing19PlusExcludingEHCPStudentsTotal { get; set; }
    }
}
