using ESFA.DC.ILR.Desktop.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Desktop.Context
{
    public class ReportServiceContextFactory : IReportServiceContextFactory<IDesktopContext>
    {
        public IReportServiceContext Build(IDesktopContext desktopContext)
        {
            return new ReportServiceJobContextDesktopContext(desktopContext);
        }
    }
}
