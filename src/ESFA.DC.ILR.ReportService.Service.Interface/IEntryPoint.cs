using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ReportService.Service.Interface
{
    public interface IEntryPoint
    {
        Task<List<string>> Callback(IReportServiceContext reportServiceContext, CancellationToken cancellationToken);
    }
}
