using System;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Interface.Reports
{
    public interface ILegacyReport
    {
        string ReportTaskName { get; }

        string ReportFileName { get; }

        string GetFilename(IReportServiceContext reportServiceContext);

        string GetZipFilename(IReportServiceContext reportServiceContext);

        Task GenerateReport(IReportServiceContext reportServiceContext, ZipArchive archive, bool isFis, CancellationToken cancellationToken);

        bool IsMatch(string reportTaskName);
    }
}
