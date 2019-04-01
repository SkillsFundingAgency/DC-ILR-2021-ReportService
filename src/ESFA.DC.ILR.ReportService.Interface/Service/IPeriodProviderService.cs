using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Interface.Context;

namespace ESFA.DC.ILR.ReportService.Interface.Service
{
    public interface IPeriodProviderService
    {
        Task<int> GetPeriod(IReportServiceContext reportServiceContext, CancellationToken cancellationToken);

        int MonthFromPeriod(int period);
    }
}
