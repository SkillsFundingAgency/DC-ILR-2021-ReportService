using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR1819.ReportService.Model.Lars;

namespace ESFA.DC.ILR1819.ReportService.Interface.Service
{
    public interface ILarsProviderService
    {
        Task<Dictionary<string, LarsLearningDelivery>> GetLearningDeliveries(List<string> validLearnerAimRefs, CancellationToken cancellationToken);

        Task<Dictionary<string, LarsFrameworkAim>> GetFrameworkAims(List<string> validLearnerAimRefs, CancellationToken cancellationToken);

        Task<string> GetVersionAsync(CancellationToken cancellationToken);
    }
}
