using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.FundingService.ALB.FundingOutput.Model.Output;
using ESFA.DC.ILR1819.ReportService.Model.ILR;
using ESFA.DC.JobContextManager.Model.Interface;

namespace ESFA.DC.ILR1819.ReportService.Interface.Service
{
    public interface IAllbProviderService
    {
        Task<ALBGlobal> GetAllbData(IJobContextMessage jobContextMessage, CancellationToken cancellationToken);

        Task<List<ALBLearningDeliveryValues>> GetALBDataFromDataStore(IJobContextMessage jobContextMessage, CancellationToken cancellationToken);
    }
}
