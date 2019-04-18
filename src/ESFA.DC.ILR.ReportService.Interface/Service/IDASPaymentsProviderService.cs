using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Model.PeriodEnd.AppsAdditionalPayment;
using ESFA.DC.ILR.ReportService.Model.PeriodEnd.AppsCoInvestment;
using ESFA.DC.ILR.ReportService.Model.PeriodEnd.AppsMonthlyPayment;

namespace ESFA.DC.ILR.ReportService.Interface.Service
{
    public interface IDASPaymentsProviderService
    {
        Task<AppsCoInvestmentPaymentsInfo> GetPaymentsInfoForAppsCoInvestmentReportAsync(int ukPrn, CancellationToken cancellationToken);

        Task<AppsAdditionalPaymentDasPaymentsInfo> GetPaymentsInfoForAppsAdditionalPaymentsReportAsync(int ukPrn, CancellationToken cancellationToken);

        Task<AppsMonthlyPaymentDASInfo> GetPaymentsInfoForAppsMonthlyPaymentReportAsync(int ukPrn, CancellationToken cancellationToken);
    }
}
