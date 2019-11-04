using System.Collections.Generic;

namespace ESFA.DC.ILR.ReportService.Models.Fm36
{
    public class PriceEpisode
    {
        public PriceEpisodeValues PriceEpisodeValues { get; set; }

        public List<PriceEpisodePeriodisedValues> PriceEpisodePeriodisedValues { get; set; }
    }
}
