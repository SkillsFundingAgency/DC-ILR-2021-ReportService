using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.JobContextManager.Model;

namespace ESFA.DC.ILR.ReportService.Stateless.Context
{
    public class ReportServiceJobContextMessageContextFactory : IReportServiceContextFactory<JobContextMessage>
    {
        public IReportServiceContext Build(JobContextMessage Context)
        {
            return new ReportServiceJobContextMessageContext(Context);
        }
    }
}
