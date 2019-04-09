using System.Collections.Generic;
using System.IO.Compression;
using System.Threading;
using ESFA.DC.ILR.ReportService.Interface.Reports;

namespace ESFA.DC.ILR.ReportService.Interface.Service
{
    public interface IZipService
    {
        void AddReportsToArchive(ZipArchive zipArchive, IEnumerable<IReport> reports, CancellationToken cancellationToken);
    }
}
