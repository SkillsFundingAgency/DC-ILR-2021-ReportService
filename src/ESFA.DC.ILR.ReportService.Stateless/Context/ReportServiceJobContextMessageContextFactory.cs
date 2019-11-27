using ESFA.DC.ILR.ReportService.Interface.Configuration;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.JobContextManager.Model;

namespace ESFA.DC.ILR.ReportService.Stateless.Context
{
    public class ReportServiceJobContextMessageContextFactory : IReportServiceContextFactory<JobContextMessage>
    {
        private readonly IVersionInfo _versionInfo;

        public ReportServiceJobContextMessageContextFactory(IVersionInfo versionInfo)
        {
            _versionInfo = versionInfo;
        }

        public IReportServiceContext Build(JobContextMessage Context)
        {
            return new ReportServiceJobContextMessageContext(Context, _versionInfo);
        }
    }
}
