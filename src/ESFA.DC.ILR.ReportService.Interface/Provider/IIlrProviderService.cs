using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Interface.Context;
using ESFA.DC.ILR.ReportService.Model.ILR;
using ESFA.DC.ILR.ReportService.Model.PeriodEnd.AppsAdditionalPayment;
using ESFA.DC.ILR.ReportService.Model.PeriodEnd.AppsCoInvestment;
using ESFA.DC.ILR.ReportService.Model.PeriodEnd.AppsMonthlyPayment;

namespace ESFA.DC.ILR.ReportService.Interface.Provider
{
    public interface IIlrProviderService
    {
        Task<IMessage> GetIlrFile(IReportServiceContext reportServiceContext, CancellationToken cancellationToken);
    }
}
