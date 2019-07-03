using System.Collections.Generic;

namespace ESFA.DC.ILR.ReportService.Model.NonContractedAppsActivity
{
    public class NonContractedActivityRuleBaseInfo
    {
        public int UkPrn { get; set; }

        public List<PriceEpisodeInfo> PriceEpisodes { get; set; }

        public List<AECLearningDeliveryInfo> AECLearningDeliveries { get; set; }

    }
}
