using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.ILR1819.ReportService.Interface.Model;

namespace ESFA.DC.ILR1819.ReportService.Interface.Service
{
    public interface ILarsProviderService
    {
        Task<Dictionary<string, ILarsLearningDelivery>> GetLearningDeliveries(List<string> validLearnerAimRefs);

        Task<Dictionary<string, ILarsFrameworkAim>> GetFrameworkAims(List<string> validLearnerAimRefs);

        Task<string> GetVersionAsync();
    }
}
