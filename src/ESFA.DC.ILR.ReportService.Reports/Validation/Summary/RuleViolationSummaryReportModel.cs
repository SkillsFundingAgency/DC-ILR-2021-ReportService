using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Validation.Summary.Model;

namespace ESFA.DC.ILR.ReportService.Reports.Validation.Summary
{
    public class RuleViolationSummaryReportModel : AbstractReportHeaderFooterModel
    {
        public int TotalNoOfErrors { get; set; }
        public int TotalNoOfWarnings { get; set; }
        public int TotalNoOfLearners { get; set; }
        public int TotalNoOfLearnersWithWarnings { get; set; }
        public RuleViolationsTotalModel FullyValidLearners { get; set; }
        public RuleViolationsTotalModel InvalidLearners { get; set; }
        public RuleViolationsTotalModel LearningDeliveries { get; set; }
        public LearnerDestinationProgressionSummary LearnerDestinationProgressionSummary { get; set; }
        public List<ValidationErrorMessageModel> Errors { get; set; }
        public List<ValidationErrorMessageModel> Warnings { get; set; }
        public string DevolvedPostcodesData { get; set; }
    }
}
