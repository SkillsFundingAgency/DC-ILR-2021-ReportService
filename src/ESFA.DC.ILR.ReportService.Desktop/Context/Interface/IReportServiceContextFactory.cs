using ESFA.DC.ILR.Desktop.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Desktop.Context.Interface
{
    public interface IReportServiceContextFactory
    {
        IReportServiceContext Build(IDesktopContext desktopContext);
    }
}
