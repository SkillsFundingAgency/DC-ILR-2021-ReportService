using System.Collections.Generic;

namespace ESFA.DC.ILR1819.ReportService.Model.PeriodEnd.AppsMonthlyPayment
{
    public class AppsMonthlyPaymentDASInfo
    {
        public long UkPrn { get; set; }

        public List<AppsMonthlyPaymentDASPaymentInfo> Payments { get; set; }
    }
}
