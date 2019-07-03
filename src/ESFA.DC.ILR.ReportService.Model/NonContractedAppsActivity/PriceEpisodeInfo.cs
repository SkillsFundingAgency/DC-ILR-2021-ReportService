using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.ILR.ReportService.Model.NonContractedAppsActivity
{
   public class PriceEpisodeInfo
    {
        public PriceEpisodeValuesInfo PriceEpisodeValues { get; set; }

        public List<AECApprenticeshipPriceEpisodePeriodisedValuesInfo> AECApprenticeshipPriceEpisodePeriodisedValues { get; set; }
    }
}
