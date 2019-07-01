using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ReportService.Service.Interface.Providers
{
    public interface IExternalDataProvider
    {
        Task<object> ProvideAsync(IReportServiceContext reportServiceContext, CancellationToken cancellationToken);
    }
}
