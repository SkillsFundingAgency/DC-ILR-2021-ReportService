using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output;
using ESFA.DC.JobContextManager.Model.Interface;

namespace ESFA.DC.ILR1819.ReportService.Interface.Service
{
    public interface IFM35ProviderService
    {
        Task<FM35Global> GetFM35Data(IJobContextMessage jobContextMessage, CancellationToken cancellationToken);
    }
}