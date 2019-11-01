using System.Collections.Generic;

namespace ESFA.DC.ILR.ReportService.Models.Fm81
{
    public class LearningDelivery
    {
        public int? AimSeqNumber { get; set; }

        public LearningDeliveryValue LearningDeliveryValues { get; set; }

        public List<LearningDeliveryPeriodisedValue> LearningDeliveryPeriodisedValues { get; set; }
    }
}
