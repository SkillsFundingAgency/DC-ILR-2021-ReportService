using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.ILR.ReportService.Model.NonContractedAppsActivity
{
   public class PriceEpisodeValuesInfo
    {
        public string PriceEpisodeFundLineType { get; set; }

        public long PriceEpisodeAimSeqNumber { get; set; }

        public DateTime EpisodeStartDate { get; set; }

        public DateTime PriceEpisodeActualEndDate { get; set; }
    }
}
