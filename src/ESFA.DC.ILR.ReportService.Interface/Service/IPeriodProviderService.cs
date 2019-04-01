using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR1819.ReportService.Interface.Context;

namespace ESFA.DC.ILR1819.ReportService.Interface.Service
{
    public interface IPeriodProviderService
    {
        Task<int> GetPeriod(IReportServiceContext reportServiceContext, CancellationToken cancellationToken);

        int MonthFromPeriod(int period);
    }
}
