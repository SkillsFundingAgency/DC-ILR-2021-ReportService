using System.Threading.Tasks;
using ESFA.DC.JobContextManager.Model.Interface;

namespace ESFA.DC.ILR1819.ReportService.Interface.Service
{
    public interface IPeriodProviderService
    {
        Task<int> GetPeriod(IJobContextMessage jobContextMessage);
    }
}
