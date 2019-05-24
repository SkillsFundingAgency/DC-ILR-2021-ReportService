using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Interface.Context;
using ESFA.DC.ILR.ReportService.Model.NonContractedAppsActivity;

namespace ESFA.DC.ILR.ReportService.Interface.Provider
{
    public interface IIlrProviderService
    {
        Task<IMessage> GetIlrFile(IReportServiceContext reportServiceContext, CancellationToken cancellationToken);

        Task<NonContractedAppsActivityILRInfo> GetILRInfoForNonContractedAppsActivityReportAsync(IReportServiceContext reportServiceContext, CancellationToken cancellationToken);
    }
}
