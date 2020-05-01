using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.HighNeedsStudentSummary.Model;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.HighNeedsStudentSummary
{
    public class HighNeedsStudentSummaryReportModel : AbstractReportHeaderFooterModel
    {
        public HNSSummaryLearnerGroup StudyProgramme { get; set; }

        public HNSSummaryLearnerGroup TLevel { get; set; }
    }
}
