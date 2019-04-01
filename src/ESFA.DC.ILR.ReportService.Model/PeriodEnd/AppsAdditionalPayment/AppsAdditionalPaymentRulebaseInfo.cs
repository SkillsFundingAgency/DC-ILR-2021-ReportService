using System.Collections.Generic;

namespace ESFA.DC.ILR1819.ReportService.Model.PeriodEnd.AppsAdditionalPayment
{
    public class AppsAdditionalPaymentRulebaseInfo
    {
        public int UkPrn { get; set; }

        public List<AECApprenticeshipPriceEpisodePeriodisedValuesInfo> AECApprenticeshipPriceEpisodePeriodisedValues { get; set; }

        public List<AECLearningDeliveryInfo> AECLearningDeliveries { get; set; }
    }
}
