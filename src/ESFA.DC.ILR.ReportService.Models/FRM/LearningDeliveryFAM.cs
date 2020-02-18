using System;

namespace ESFA.DC.ILR.ReportService.Models.FRM
{
    public class LearningDeliveryFAM
    {
        public string LearnDelFAMType { get; set; }

        public string LearnDelFAMCode { get; set; }

        public DateTime? LearnDelFAMDateFrom { get; set; }

        public DateTime? LearnDelFAMDateTo { get; set; }
    }
}
