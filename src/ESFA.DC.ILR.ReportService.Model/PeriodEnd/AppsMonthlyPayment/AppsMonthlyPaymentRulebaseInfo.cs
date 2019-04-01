using System.Collections.Generic;

namespace ESFA.DC.ILR1819.ReportService.Model.PeriodEnd.AppsMonthlyPayment
{
    public class AppsMonthlyPaymentRulebaseInfo
    {
        public int UkPrn { get; set; }

        public string LearnRefNumber { get; set; }

        public ICollection<AECApprenticeshipPriceEpisodeInfo> AECApprenticeshipPriceEpisodes { get; set; }
    }
}
