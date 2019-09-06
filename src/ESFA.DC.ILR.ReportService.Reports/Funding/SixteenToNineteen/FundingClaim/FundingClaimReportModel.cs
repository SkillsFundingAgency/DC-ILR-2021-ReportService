using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.FundingClaim.Model;
using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.HighNeedsStudentSummary.Model;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.FundingClaim
{
    public class FundingClaimReportModel
    {
        //Header
        public string ProviderName { get; set; }

        public int Ukprn { get; set; }

        public string IlrFile { get; set; }

        public string Year { get; set; }

        //Body
        public FundingFactorModel FundingFactor { get; set; }
        public FundingLineReportingBandModel DirectFundingStudents { get; set; }
        public FundingLineReportingBandModel StudentsIncludingHNS { get; set; }
        public FundingLineReportingBandModel StudentsWithEHCPlan { get; set; }
        public FundingLineReportingBandModel ContinuingStudentsExcludingEHCPlan { get; set; }
        public decimal CofRemoval { get; set; }

        // footer
        public string ComponentSetVersion { get; set; }

        public string ApplicationVersion { get; set; }

        public string FilePreparationDate { get; set; }

        public string LarsData { get; set; }

        public string PostcodeData { get; set; }

        public string OrganisationData { get; set; }

        public string LargeEmployerData { get; set; }

        public string ReportGeneratedAt { get; set; }

        public string CofRemovalData { get; set; }
    }
}
