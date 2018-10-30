using System;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.JobContextManager.Model.Interface;

namespace ESFA.DC.ILR1819.ReportService.Interface.Reports
{
    public interface IReport
    {
        string ReportTaskName { get; }

        string GetFilename(string ukPrn, long jobId, DateTime submissionDateTime);

        Task GenerateReport(IJobContextMessage jobContextMessage, ZipArchive archive, bool isFis, CancellationToken cancellationToken);

        bool IsMatch(string reportTaskName);
    }
}
