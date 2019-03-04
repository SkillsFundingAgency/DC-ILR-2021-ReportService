using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR1819.ReportService.Model.PeriodEnd.AppsAdditionalPayment;
using ESFA.DC.ILR1819.ReportService.Model.PeriodEnd.AppsCoInvestment;

namespace ESFA.DC.ILR1819.ReportService.Interface.Service
{
    public interface IDASPaymentsProviderService
    {
        Task<AppsCoInvestmentPaymentsInfo> GetPaymentsInfoForAppsCoInvestmentReportAsync(int ukPrn, CancellationToken cancellationToken);

        Task<AppsAdditionalPaymentDasPaymentsInfo> GetPaymentsInfoForAppsAdditionalPaymentsReportAsync(int ukPrn, CancellationToken cancellationToken);
    }
}
