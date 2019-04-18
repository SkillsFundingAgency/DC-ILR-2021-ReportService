using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Interface.Context;
using ESFA.DC.ILR.ReportService.Interface.Reports;

namespace ESFA.DC.ILR.ReportService.Interface.Service
{
    public interface IReportsProvider
    {
        IEnumerable<IReport> ProvideReportsForContext(IReportServiceContext reportServiceContext);
    }
}
