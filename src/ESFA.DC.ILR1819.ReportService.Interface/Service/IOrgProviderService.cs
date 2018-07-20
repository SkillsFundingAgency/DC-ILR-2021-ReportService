using System.Threading.Tasks;
using ESFA.DC.JobContext.Interface;

namespace ESFA.DC.ILR1819.ReportService.Interface.Service
{
    public interface IOrgProviderService
    {
        Task<string> GetProviderName(IJobContextMessage jobContextMessage);
    }
}
