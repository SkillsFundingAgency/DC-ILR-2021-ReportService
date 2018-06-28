using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.JobContext;

namespace ESFA.DC.ILR1819.ReportService.Service
{
    public sealed class EntryPoint
    {
        public async Task<bool> Callback(JobContextMessage jobContextMessage, CancellationToken cancellationToken)
        {
            return true;
        }
    }
}
