using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model;
using ESFA.DC.JobContext.Interface;

namespace ESFA.DC.ILR1819.ReportService.Interface.Service
{
    public interface IFM35ProviderService
    {
        Task<FM35FundingOutputs> GetFM35Data(IJobContextMessage jobContextMessage, CancellationToken cancellationToken);
    }
}