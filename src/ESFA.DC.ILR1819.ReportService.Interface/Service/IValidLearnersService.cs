using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.JobContext.Interface;

namespace ESFA.DC.ILR1819.ReportService.Interface.Service
{
    public interface IValidLearnersService
    {
        Task<List<string>> GetValidLearnersAsync(IJobContextMessage jobContextMessage);
    }
}
