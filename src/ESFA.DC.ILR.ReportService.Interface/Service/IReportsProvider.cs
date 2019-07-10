using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Service.Interface;
using IReport = ESFA.DC.ILR.ReportService.Interface.Reports.IReport;

namespace ESFA.DC.ILR.ReportService.Interface.Service
{
    public interface IReportsProvider
    {
        IEnumerable<IReport> ProvideReportsForContext(IReportServiceContext reportServiceContext);
    }
}
