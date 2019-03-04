using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Context;
using ESFA.DC.ILR1819.ReportService.Model.ILR;
using ESFA.DC.ILR1819.ReportService.Model.PeriodEnd.AppsAdditionalPayment;
using ESFA.DC.ILR1819.ReportService.Model.PeriodEnd.AppsCoInvestment;

namespace ESFA.DC.ILR1819.ReportService.Interface.Service
{
    public interface IIlrProviderService
    {
        Task<IMessage> GetIlrFile(IReportServiceContext reportServiceContext, CancellationToken cancellationToken);

        Task<ILRSourceFileInfo> GetLastSubmittedIlrFile(IReportServiceContext reportServiceContext, CancellationToken cancellationToken);

        Task<AppsCoInvestmentILRInfo> GetILRInfoForAppsCoInvestmentReportAsync(int ukPrn, CancellationToken cancellationToken);

        Task<AppsAdditionalPaymentILRInfo> GetILRInfoForAppsAdditionalPaymentsReportAsync(int ukPrn, CancellationToken cancellationToken);
    }
}
