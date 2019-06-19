using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ReportService.Service.Interface.Providers
{
    public interface IFileProviderService<T>
    {
        Task<T> ProvideAsync(IReportServiceContext fundingServiceContext, CancellationToken cancellationToken);
    }
}
