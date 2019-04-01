using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.ILR1819.ReportService.Interface.Service
{
    public interface IPostcodeProviderService
    {
        Task<string> GetVersionAsync(CancellationToken cancellationToken);
    }
}
