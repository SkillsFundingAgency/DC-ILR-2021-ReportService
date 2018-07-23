using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.ILR1819.ReportService.Interface.Model;

namespace ESFA.DC.ILR1819.ReportService.Interface.Service
{
    public interface ILarsProviderService
    {
        Task<Dictionary<string, ILarsLearningDelivery>> GetLarsData(List<string> validLearnersList);

        Task<string> GetVersionAsync();
    }
}
