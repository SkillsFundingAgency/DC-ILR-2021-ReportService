using System.Collections.Generic;
using ESFA.DC.ILR.Desktop.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Desktop.Context
{
    public class ReportServiceContextFactory : IReportServiceContextFactory<IDesktopContext>
    {
        public IReportServiceContext Build(IDesktopContext desktopContext)
        {
            return new ReportServiceJobContextDesktopContext(desktopContext, BuildReportFilterQueries(desktopContext));
        }

        public IEnumerable<IReportFilterQuery> BuildReportFilterQueries(IDesktopContext desktopContext)
        {
            return new List<IReportFilterQuery>();
        }
    }
}
