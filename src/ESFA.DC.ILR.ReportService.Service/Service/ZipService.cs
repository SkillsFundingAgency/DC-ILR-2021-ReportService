using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Interface.Reports;
using ESFA.DC.ILR.ReportService.Interface.Service;

namespace ESFA.DC.ILR.ReportService.Service.Service
{
    public class ZipService : IZipService
    {
        public void AddReportsToArchive(ZipArchive zipArchive, IEnumerable<IReport> reports, CancellationToken cancellationToken)
        {
        }
    }
}
