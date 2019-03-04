using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.ILR1819.ReportService.Interface.Service
{
    public interface ILargeEmployerProviderService
    {
        Task<string> GetVersionAsync(CancellationToken cancellationToken);
    }
}
