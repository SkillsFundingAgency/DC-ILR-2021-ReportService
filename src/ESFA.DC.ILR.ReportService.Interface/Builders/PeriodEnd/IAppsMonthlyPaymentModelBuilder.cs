using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Model.PeriodEnd.AppsMonthlyPayment;
using ESFA.DC.ILR.ReportService.Model.ReportModels.PeriodEnd;

namespace ESFA.DC.ILR.ReportService.Interface.Builders.PeriodEnd
{
    public interface IAppsMonthlyPaymentModelBuilder
    {
        IReadOnlyList<AppsMonthlyPaymentModel> BuildModel(
            AppsMonthlyPaymentILRInfo appsMonthlyPaymentIlrInfo,
            AppsMonthlyPaymentRulebaseInfo appsMonthlyPaymentRulebaseInfo,
            AppsMonthlyPaymentDASInfo appsMonthlyPaymentDasInfo,
            IReadOnlyList<AppsMonthlyPaymentLarsLearningDeliveryInfo> appsMonthlyPaymentLarsLearningDeliveryInfoList);
    }
}