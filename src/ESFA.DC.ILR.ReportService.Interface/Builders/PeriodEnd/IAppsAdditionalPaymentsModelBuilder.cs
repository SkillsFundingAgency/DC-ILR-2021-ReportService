﻿using System.Collections.Generic;
using ESFA.DC.ILR1819.ReportService.Model.PeriodEnd.AppsAdditionalPayment;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels.PeriodEnd;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Model.ReportModels.PeriodEnd;

namespace ESFA.DC.ILR.ReportService.Interface.Builders.PeriodEnd
{
   public interface IAppsAdditionalPaymentsModelBuilder
    {
        List<AppsAdditionalPaymentsModel> BuildModel(AppsAdditionalPaymentILRInfo appsAdditionalPaymentIlrInfo, AppsAdditionalPaymentRulebaseInfo appsAdditionalPaymentRulebaseInfo, AppsAdditionalPaymentDasPaymentsInfo appsAdditionalPaymentDasPaymentsInfo);
    }
}