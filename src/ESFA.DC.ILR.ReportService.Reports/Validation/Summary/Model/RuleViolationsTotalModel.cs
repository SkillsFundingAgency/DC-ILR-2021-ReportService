using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.ILR.ReportService.Reports.Validation.Summary.Model
{
    public class RuleViolationsTotalModel
    {
        public int Total { get; set; }
        public int Apprenticeships { get; set; }
        public int Funded1619 { get; set; }
        public int AdultSkilledFunded { get; set; }
        public int CommunityLearningFunded { get; set; }
        public int ESFFunded { get; set; }
        public int OtherAdultFunded { get; set; }
        public int Other1619Funded { get; set; }
        public int NonFunded { get; set; }
        public int AdvancedLoanLearningDeliveries { get; set; }
    }
}
