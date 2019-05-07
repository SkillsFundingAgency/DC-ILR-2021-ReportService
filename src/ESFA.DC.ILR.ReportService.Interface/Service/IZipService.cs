using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Interface.Context;
using ESFA.DC.ILR.ReportService.Interface.Reports;

namespace ESFA.DC.ILR.ReportService.Interface.Service
{
    public interface IZipService
    {
        Task AddReportToArchiveAsync(ZipArchive zipArchive, IReport report, IReportServiceContext reportServiceContext, CancellationToken cancellationToken);
    }
}
