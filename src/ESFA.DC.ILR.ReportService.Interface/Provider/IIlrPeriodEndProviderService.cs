using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Model.PeriodEnd.AppsAdditionalPayment;
using ESFA.DC.ILR.ReportService.Model.PeriodEnd.AppsCoInvestment;
using ESFA.DC.ILR.ReportService.Model.PeriodEnd.AppsMonthlyPayment;

namespace ESFA.DC.ILR.ReportService.Interface.Provider
{
    public interface IIlrPeriodEndProviderService
    {
        Task<AppsCoInvestmentILRInfo> GetILRInfoForAppsCoInvestmentReportAsync(int ukPrn, CancellationToken cancellationToken);

        Task<AppsAdditionalPaymentILRInfo> GetILRInfoForAppsAdditionalPaymentsReportAsync(int ukPrn, CancellationToken cancellationToken);

        Task<AppsMonthlyPaymentILRInfo> GetILRInfoForAppsMonthlyPaymentReportAsync(int ukPrn, CancellationToken cancellationToken);
    }
}
