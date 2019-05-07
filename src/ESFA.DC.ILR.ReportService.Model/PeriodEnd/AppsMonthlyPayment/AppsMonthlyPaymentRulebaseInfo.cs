using System.Collections.Generic;

namespace ESFA.DC.ILR.ReportService.Model.PeriodEnd.AppsMonthlyPayment
{
    public class AppsMonthlyPaymentRulebaseInfo
    {
        public int UkPrn { get; set; }

        public string LearnRefNumber { get; set; }

        public List<AECApprenticeshipPriceEpisodeInfo> AECApprenticeshipPriceEpisodes { get; set; }
    }
}
