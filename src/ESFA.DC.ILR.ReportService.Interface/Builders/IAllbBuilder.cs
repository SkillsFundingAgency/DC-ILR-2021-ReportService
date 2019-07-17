using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Model.ReportModels;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Interface.Builders
{
    public interface IAllbBuilder
    {
        Task<List<FundingSummaryModel>> BuildAsync(IReportServiceContext reportServiceContext, CancellationToken cancellationToken);
    }
}
