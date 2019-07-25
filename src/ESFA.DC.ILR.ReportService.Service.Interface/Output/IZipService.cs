using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ReportService.Service.Interface.Output
{
    public interface IZipService
    {
        Task CreateZipAsync(IReportServiceContext reportServiceContext, IEnumerable<string> fileNames, string container, CancellationToken cancellationToken);
    }
}
