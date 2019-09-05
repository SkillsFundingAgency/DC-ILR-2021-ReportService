using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.HighNeedsStudentSummary.Model;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.HighNeedsStudentSummary
{
    public class HighNeedsStudentSummaryReportModel 
    {
        //Header
        public string ProviderName { get; set; }

        public int Ukprn { get; set; }

        public string IlrFile { get; set; }

        public string Year { get; set; }

        //Body

        public FundingLineReportingBandStudentNumbers DirectFunded1416StudentsTotal { get; set; }
        public FundingLineReportingBandStudentNumbers IncludingHNS1619StudentsTotal { get; set; }
        public FundingLineReportingBandStudentNumbers EHCP1924StudentsTotal { get; set; }
        public FundingLineReportingBandStudentNumbers Continuing19PlusExcludingEHCPStudentsTotal { get; set; }


        // footer
        public string ComponentSetVersion { get; set; }

        public string ApplicationVersion { get; set; }

        public string FilePreparationDate { get; set; }

        public string LarsData { get; set; }

        public string PostcodeData { get; set; }

        public string OrganisationData { get; set; }

        public string LargeEmployerData { get; set; }

        public string ReportGeneratedAt { get; set; }
    }
}
