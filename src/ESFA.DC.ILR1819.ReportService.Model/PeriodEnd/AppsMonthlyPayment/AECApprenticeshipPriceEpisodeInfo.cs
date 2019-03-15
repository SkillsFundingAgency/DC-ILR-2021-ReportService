using System;

namespace ESFA.DC.ILR1819.ReportService.Model.PeriodEnd.AppsMonthlyPayment
{
    public class AECApprenticeshipPriceEpisodeInfo
    {
        public string LearnRefNumber { get; set; }

        public DateTime? PriceEpisodeActualEndDate { get; set; }

        public string PriceEpisodeAgreeId { get; set; }
    }
}