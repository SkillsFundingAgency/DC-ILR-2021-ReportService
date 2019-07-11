using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Interface.Service
{
    public interface IZipService
    {
        Task AddReportToArchiveAsync(ZipArchive zipArchive, ILegacyReport report, IReportServiceContext reportServiceContext, CancellationToken cancellationToken);
    }
}
