using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.FundingService.FM81.FundingOutput.Model.Output;
using ESFA.DC.JobContextManager.Model.Interface;

namespace ESFA.DC.ILR1819.ReportService.Interface.Service
{
    public interface IFM81TrailBlazerProviderService
    {
        Task<FM81Global> GetFM81Data(IJobContextMessage jobContextMessage, CancellationToken cancellationToken);
    }
}
