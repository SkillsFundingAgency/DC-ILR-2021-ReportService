using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ReportService.Interface.Provider
{
    public interface ILargeEmployerProviderService
    {
        Task<string> GetVersionAsync(CancellationToken cancellationToken);
    }
}
