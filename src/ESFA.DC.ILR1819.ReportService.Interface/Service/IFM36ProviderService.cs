using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model;
using ESFA.DC.JobContext.Interface;

namespace ESFA.DC.ILR1819.ReportService.Interface.Service
{
    public interface IFM36ProviderService
    {
        Task<FM36FundingOutputs> GetFM36Data(IJobContextMessage jobContextMessage, CancellationToken cancellationToken);
    }
}