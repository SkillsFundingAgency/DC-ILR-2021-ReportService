using System;

namespace ESFA.DC.ILR.ReportService.Model.PeriodEnd.AppsMonthlyPayment
{
    public class AECApprenticeshipPriceEpisodeInfo
    {
        public int UkPrn { get; set; }

        public string LearnRefNumber { get; set; }

        public int AimSequenceNumber { get; set; }

        public DateTime? PriceEpisodeActualEndDate { get; set; }

        public string PriceEpisodeAgreeId { get; set; }
        
    }
}