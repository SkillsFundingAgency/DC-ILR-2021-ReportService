using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.ILR.ReportService.Reports.Validation.Summary.Model
{
    public class LearnerDestinationProgressionSummary
    {
        public int Total { get; set; }
        public int ValidLearnerDestinationProgressions { get; set; }
        public int InValidLearnerDestinationProgressions { get; set; }
        public int LearnerDestinationProgressionsWithWarnings{ get; set; }
    }
}
