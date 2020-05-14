using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Models.Fm36;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Data.Providers
{
    public class Fm36ProviderStub : IExternalDataProvider
    {
        public async Task<object> ProvideAsync(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            return new FM36Global
            {
                Learners = new List<FM36Learner>()
            };
        }
    }
}
