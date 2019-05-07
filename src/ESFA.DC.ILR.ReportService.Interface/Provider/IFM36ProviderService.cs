using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using ESFA.DC.ILR.ReportService.Interface.Context;
using ESFA.DC.ILR.ReportService.Model.PeriodEnd.AppsAdditionalPayment;
using ESFA.DC.ILR.ReportService.Model.PeriodEnd.AppsMonthlyPayment;

namespace ESFA.DC.ILR.ReportService.Interface.Provider
{
    public interface IFM36ProviderService
    {
        Task<FM36Global> GetFM36Data(IReportServiceContext reportServiceContext, CancellationToken cancellationToken);
    }
}