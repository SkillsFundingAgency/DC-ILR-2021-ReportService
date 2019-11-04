using System.Collections.Generic;

namespace ESFA.DC.ILR.ReportService.Models.Fm99
{
    public class LearningDelivery
    {
        public int AimSeqNumber { get; set; }

        public LearningDeliveryValue LearningDeliveryValue { get; set; }

        public List<LearningDeliveryPeriodisedValue> LearningDeliveryPeriodisedValues { get; set; }
    }
}
