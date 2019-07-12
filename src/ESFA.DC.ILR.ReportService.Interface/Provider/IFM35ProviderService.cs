using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Interface.Provider
{
    public interface IFM35ProviderService
    {
        Task<FM35Global> GetFM35Data(IReportServiceContext reportServiceContext, CancellationToken cancellationToken);
    }
}