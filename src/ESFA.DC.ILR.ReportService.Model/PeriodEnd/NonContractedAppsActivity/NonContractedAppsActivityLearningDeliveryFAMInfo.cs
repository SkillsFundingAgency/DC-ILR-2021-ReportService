using System;

namespace ESFA.DC.ILR.ReportService.Model.PeriodEnd.NonContractedAppsActivity
{
    public class NonContractedAppsActivityLearningDeliveryFAMInfo
    {
        public int UKPRN { get; set; }

        public string LearnRefNumber { get; set; }

        public int AimSeqNumber { get; set; }

        public string LearnDelFAMType { get; set; }

        public string LearnDelFAMCode { get; set; }
        public DateTime? LearnDelFAMAppliesFrom { get; set; }
        public DateTime? LearnDelFAMAppliesTo { get; set; }
    }
}