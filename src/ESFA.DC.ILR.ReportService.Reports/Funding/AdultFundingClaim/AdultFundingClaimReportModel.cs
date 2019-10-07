using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Funding.AdultFundingClaim.Model;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.AdultFundingClaim 
{
    public class AdultFundingClaimReportModel : AbstractReportHeaderFooterModel
    {
        public ActualEarnings AEBProgrammeFunding { get; set; }

        public ActualEarnings AEBLearningSupport { get; set; }

        public ActualEarnings AEBProgrammeFunding1924 { get; set; }

        public ActualEarnings AEBLearningSupport1924 { get; set; }

        public ActualEarnings ALBBursaryFunding { get; set; }

        public ActualEarnings ALBAreaCosts { get; set; }

        public ActualEarnings ALBExcessSupport { get; set; }

    }
}
