
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
        public int TotalDirectFunded1416_WithEHCP { get; set; }
        public int TotalDirectFunded1416_WithoutEHCP { get; set; }
        public int TotalDirectFunded1416_HNSWithoutEHCP { get; set; }
        public int TotalDirectFunded1416_HNSWithEHCP { get; set; }
        public int TotalDirectFunded1416_EHCPWithoutHNS { get; set; }


        public int Total1619IncludingHNS_WithEHCP { get; set; }
        public int Total1619IncludingHNS_WithoutEHCP { get; set; }
        public int Total1619IncludingHNS_HNSWithoutEHCP { get; set; }
        public int Total1619IncludingHNS_HNSWithEHCP { get; set; }
        public int Total1619IncludingHNS_EHCPWithoutHNS { get; set; }


        public int Total1924WithEHCP_WithEHCP { get; set; }
        public int Total1924WithEHCP_WithoutEHCP { get; set; }
        public int Total1924WithEHCP_HNSWithoutEHCP { get; set; }
        public int Total1924WithEHCP_HNSWithEHCP { get; set; }
        public int Total1924WithEHCP_EHCPWithoutHNS { get; set; }


        public int Total19PlusWithoutEHCP_WithEHCP { get; set; }
        public int Total19PlusWithoutEHCP_WithoutEHCP { get; set; }
        public int Total19PlusWithoutEHCP_HNSWithoutEHCP { get; set; }
        public int Total19PlusWithoutEHCP_HNSWithEHCP { get; set; }
        public int Total19PlusWithoutEHCP_EHCPWithoutHNS { get; set; }

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
