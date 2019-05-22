using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Interface.Context;
using ESFA.DC.ILR.ReportService.Model.NonContractedAppsActivity;

namespace ESFA.DC.ILR.ReportService.Interface.Provider
{
    public interface IFM36NonContractedActivityProviderService
    {
        Task<NonContractedActivityRuleBaseInfo> GetFM36InfoForNonContractedActivityReportAsync(List<string> validLearners, IReportServiceContext reportServiceContext, CancellationToken cancellationToken);
    }
}