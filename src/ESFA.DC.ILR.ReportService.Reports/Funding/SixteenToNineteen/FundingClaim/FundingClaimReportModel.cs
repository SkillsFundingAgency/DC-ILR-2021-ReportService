using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.FundingClaim.Model;
using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.HighNeedsStudentSummary.Model;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.FundingClaim
{
    public class FundingClaimReportModel : AbstractReportHeaderFooterModel
    {
        public FundingFactorModel FundingFactor { get; set; }
        public FundingLineReportingBandModel DirectFundingStudents { get; set; }
        public FundingLineReportingBandModel StudentsIncludingHNS { get; set; }
        public FundingLineReportingBandModel StudentsWithEHCPlan { get; set; }
        public FundingLineReportingBandModel ContinuingStudentsExcludingEHCPlan { get; set; }
        public decimal CofRemoval { get; set; }
    }
}
