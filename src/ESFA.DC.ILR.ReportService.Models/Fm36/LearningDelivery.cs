using System.Collections.Generic;

namespace ESFA.DC.ILR.ReportService.Models.Fm36
{
    public class LearningDelivery
    {
        public int AimSeqNumber { get; set; }

        public LearningDeliveryValues LearningDeliveryValues { get; set; }

        public List<LearningDeliveryPeriodisedValues> LearningDeliveryPeriodisedValues { get; set; }

        public List<LearningDeliveryPeriodisedTextValues> LearningDeliveryPeriodisedTextValues { get; set; }
    }
}
