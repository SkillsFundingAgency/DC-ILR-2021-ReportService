using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.FundingService.ALB.FundingOutput.Model.Output;
using ESFA.DC.ILR.ReportService.Model.ILR;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Interface.Provider
{
    public interface IAllbProviderService
    {
        Task<ALBGlobal> GetAllbData(IReportServiceContext reportServiceContext, CancellationToken cancellationToken);

        Task<List<ALBLearningDeliveryValues>> GetALBDataFromDataStore(IReportServiceContext reportServiceContext, CancellationToken cancellationToken);
    }
}
