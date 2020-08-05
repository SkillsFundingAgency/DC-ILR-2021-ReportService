using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.Abstract;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.SummaryOfFundingByStudent
{
    public class SummaryOfFundingByStudentReportModel : AbstractSixteenToNineteenModel
    {
        public int? NonTLevelPlanLearnHours { get; set; }

        public int? NonTLevelPlanEEPHours { get; set; }

        public int? NonTLevelTotalPlannedHours { get; set; }

        public int? TLevelPlannedHours { get; set; }
    }
}
