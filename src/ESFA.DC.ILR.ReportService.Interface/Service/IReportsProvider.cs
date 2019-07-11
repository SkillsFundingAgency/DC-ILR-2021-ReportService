using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Interface.Reports;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Interface.Service
{
    public interface IReportsProvider
    {
        IEnumerable<ILegacyReport> ProvideReportsForContext(IReportServiceContext reportServiceContext);
    }
}
