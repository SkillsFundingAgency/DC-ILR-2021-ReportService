using System.Collections.Generic;

namespace ESFA.DC.ILR.ReportService.Model.NonContractedAppsActivity
{
    public class AECLearningDeliveryInfo
    {
        public int UKPRN { get; set; }

        public string LearnRefNumber { get; set; }

        public int AimSeqNumber { get; set; }

        public List<AECApprenticeshipLearningDeliveryPeriodisedValuesInfo> LearningDeliveryPeriodisedValues { get; set; }

    }
}
