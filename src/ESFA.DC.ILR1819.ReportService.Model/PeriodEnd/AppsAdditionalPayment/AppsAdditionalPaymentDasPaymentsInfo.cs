using System.Collections.Generic;

namespace ESFA.DC.ILR1819.ReportService.Model.PeriodEnd.AppsAdditionalPayment
{
    public class AppsAdditionalPaymentDasPaymentsInfo
    {
        public long UkPrn { get; set; }

        public List<DASPaymentInfo> Payments { get; set; }
    }
}
