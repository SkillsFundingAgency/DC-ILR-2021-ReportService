﻿using System.Collections.Generic;

namespace ESFA.DC.ILR.ReportService.Model.PeriodEnd.AppsAdditionalPayment
{
    public class AppsAdditionalPaymentDasPaymentsInfo
    {
        public long UkPrn { get; set; }

        public List<DASPaymentInfo> Payments { get; set; }
    }
}
