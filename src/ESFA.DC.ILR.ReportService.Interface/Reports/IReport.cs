using System;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Interface.Context;

namespace ESFA.DC.ILR.ReportService.Interface.Reports
{
    public interface IReport
    {
        string ReportTaskName { get; }

        string ReportFileName { get; }

        string GetFilename(IReportServiceContext reportServiceContext);

        string GetZipFilename(IReportServiceContext reportServiceContext);

        Task GenerateReport(IReportServiceContext reportServiceContext, ZipArchive archive, bool isFis, CancellationToken cancellationToken);

        bool IsMatch(string reportTaskName);
    }
}
