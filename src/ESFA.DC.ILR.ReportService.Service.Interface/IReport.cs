using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ReportService.Service.Interface
{
    public interface IReport
    {
        string ReportTaskName { get; }

        string ReportFileName { get; }

        Task<IEnumerable<string>> GenerateReportAsync(IReportServiceContext reportServiceContext, CancellationToken cancellationToken);
    }
}
