using System;

namespace ESFA.DC.ILR.ReportService.Models.Fm36
{
    public class PriceEpisodeValues
    {
        public DateTime? EpisodeStartDate { get; set; }

        public Decimal? PriceEpisodeUpperBandLimit { get; set; }

        public DateTime? PriceEpisodeActualEndDate { get; set; }

        public Decimal? PriceEpisodeTotalTNPPrice { get; set; }

        public Decimal? PriceEpisodeUpperLimitAdjustment { get; set; }

        public Decimal? PriceEpisodeCompletionElement { get; set; }

        public Decimal? PriceEpisodeCappedRemainingTNPAmount { get; set; }

        public int? PriceEpisodeAimSeqNumber { get; set; }

        public string PriceEpisodeFundLineType { get; set; }

        public string PriceEpisodeAgreeId { get; set; }
    }
}
