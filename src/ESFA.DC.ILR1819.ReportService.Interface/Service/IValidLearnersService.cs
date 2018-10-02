using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.JobContextManager.Model.Interface;

namespace ESFA.DC.ILR1819.ReportService.Interface.Service
{
    public interface IValidLearnersService
    {
        Task<List<string>> GetLearnersAsync(IJobContextMessage jobContextMessage, CancellationToken cancellationToken);
    }
}
