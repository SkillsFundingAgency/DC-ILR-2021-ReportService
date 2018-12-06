using ESFA.DC.ILR1819.ReportService.Interface.Context;

namespace ESFA.DC.ILR1819.ReportService.Interface.Service
{
    using System.Threading;
    using System.Threading.Tasks;
    using ESFA.DC.ILR.Model.Interface;
    using ESFA.DC.ILR1819.ReportService.Model.ILR;

    public interface IIlrProviderService
    {
        Task<IMessage> GetIlrFile(IReportServiceContext reportServiceContext, CancellationToken cancellationToken);

        Task<ILRSourceFileInfo> GetLastSubmittedIlrFile(IReportServiceContext reportServiceContext, CancellationToken cancellationToken);
    }
}
