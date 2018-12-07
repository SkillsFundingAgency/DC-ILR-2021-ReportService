using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR1819.ReportService.Interface.Context;

namespace ESFA.DC.ILR1819.ReportService.Interface.Service
{
    public interface IOrgProviderService
    {
        Task<string> GetProviderName(IReportServiceContext reportServiceContext, CancellationToken cancellationToken);

        Task<string> GetVersionAsync(CancellationToken cancellationToken);
    }
}
