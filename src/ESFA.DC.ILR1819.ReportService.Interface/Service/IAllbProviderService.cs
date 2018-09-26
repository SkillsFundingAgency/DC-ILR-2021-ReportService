using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.FundingService.ALB.FundingOutput.Model.Output;
using ESFA.DC.JobContext.Interface;

namespace ESFA.DC.ILR1819.ReportService.Interface.Service
{
    public interface IAllbProviderService
    {
        Task<ALBGlobal> GetAllbData(IJobContextMessage jobContextMessage, CancellationToken cancellationToken);
    }
}
