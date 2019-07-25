using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.ReportService.Reports.Service;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Desktop.Service
{
    public class DesktopFileNameService : FileNameService, IFileNameService
    {
        public DesktopFileNameService(IDateTimeProvider dateTimeProvider)
            : base(dateTimeProvider)
        {
        }

        protected override string GetPath(IReportServiceContext reportServiceContext) => string.Empty;
    }
}
