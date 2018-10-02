using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using ESFA.DC.JobContextManager.Model.Interface;

namespace ESFA.DC.ILR1819.ReportService.Interface.Service
{
    public interface IFM36ProviderService
    {
        Task<FM36Global> GetFM36Data(IJobContextMessage jobContextMessage, CancellationToken cancellationToken);
    }
}