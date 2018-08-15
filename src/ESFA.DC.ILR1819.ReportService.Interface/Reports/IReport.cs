using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR1819.ReportService.Model.Report;
using ESFA.DC.JobContext.Interface;

namespace ESFA.DC.ILR1819.ReportService.Interface.Reports
{
    public interface IReport
    {
        ReportType ReportType { get; }

        string GetReportFilename(string ukPrn, long jobId);

        Task GenerateReport(IJobContextMessage jobContextMessage, ZipArchive archive,
            CancellationToken cancellationToken);
    }
}
