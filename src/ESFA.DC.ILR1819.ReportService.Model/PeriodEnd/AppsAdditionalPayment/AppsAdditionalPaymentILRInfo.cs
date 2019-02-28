using System.Collections.Generic;

namespace ESFA.DC.ILR1819.ReportService.Model.PeriodEnd.AppsAdditionalPayment
{
    public class AppsAdditionalPaymentILRInfo
    {
        public int UkPrn { get; set; }

        public List<AppsAdditionalPaymentLearnerInfo> Learners { get; set; }
    }
}
