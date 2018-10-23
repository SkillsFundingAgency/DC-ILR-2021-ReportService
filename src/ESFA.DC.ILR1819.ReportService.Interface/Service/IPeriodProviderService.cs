using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.JobContextManager.Model.Interface;

namespace ESFA.DC.ILR1819.ReportService.Interface.Service
{
    public interface IPeriodProviderService
    {
        Task<int> GetPeriod(IJobContextMessage jobContextMessage, CancellationToken cancellationToken);

        int MonthFromPeriod(int period);
    }
}
