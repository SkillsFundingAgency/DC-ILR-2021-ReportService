﻿
using System.Collections.Generic;

namespace ESFA.DC.ILR.ReportService.Model.PeriodEnd.AppsAdditionalPayment
{
    public class AppsAdditionalPaymentLearnerInfo
    {
        public string LearnRefNumber { get; set; }

        public virtual ICollection<AppsAdditionalPaymentProviderSpecLearnerMonitoringInfo> ProviderSpecLearnerMonitorings { get; set; }
    }
}