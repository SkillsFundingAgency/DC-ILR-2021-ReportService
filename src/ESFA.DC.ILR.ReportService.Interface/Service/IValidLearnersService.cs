using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR1819.ReportService.Interface.Context;

namespace ESFA.DC.ILR1819.ReportService.Interface.Service
{
    public interface IValidLearnersService
    {
        Task<List<string>> GetLearnersAsync(IReportServiceContext reportServiceContext, CancellationToken cancellationToken);
    }
}
