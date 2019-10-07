using System.Collections.Generic;
using System.Linq;
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
            return desktopContext
                .ReportFilterQueries?
                .Select(r => new ReportFilterQuery()
                       {
                           ReportName = r.ReportName,
                           Properties = r.FilterProperties?
                                            .Select(p => new ReportFilterPropertyQuery()
                           {
                               PropertyName = p.Name,
                               Value = p.Value,
                           }) ?? Enumerable.Empty<IReportFilterPropertyQuery>()
                        })
                ?? Enumerable.Empty<IReportFilterQuery>();
        }
    }
}
