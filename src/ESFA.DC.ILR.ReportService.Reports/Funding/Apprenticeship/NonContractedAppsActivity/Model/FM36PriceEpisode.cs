using System;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Apprenticeship.NonContractedAppsActivity.Model
{
    public class FM36PriceEpisode
    {
        public string LearnRefNumber { get; set; }

        public int PriceEpisodeAimSeqNumber { get; set; }

        public DateTime? EpisodeStartDate { get; set; }

        public DateTime? PriceEpisodeActualEndDateIncEPA { get; set; }

        public string PriceEpisodeFundLineType { get; set; }

        public decimal? AugustTotal { get; set; }

        public decimal? SeptemberTotal { get; set; }

        public decimal? OctoberTotal { get; set; }

        public decimal? NovemberTotal { get; set; }

        public decimal? DecemberTotal { get; set; }

        public decimal? JanuaryTotal { get; set; }

        public decimal? FebruaryTotal { get; set; }

        public decimal? MarchTotal { get; set; }

        public decimal? AprilTotal { get; set; }

        public decimal? MayTotal { get; set; }

        public decimal? JuneTotal { get; set; }

        public decimal? JulyTotal { get; set; }
    }
}
