using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Data.LARS.Model;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR1819.ReportService.Model.Lars;

namespace ESFA.DC.ILR1819.ReportService.Interface.Service
{
    public interface ILarsProviderService
    {
        Task<Dictionary<string, LarsLearningDelivery>> GetLearningDeliveries(
            string[] validLearnerAimRefs,
            CancellationToken cancellationToken);

        Task<List<LearnerAndDeliveries>> GetFrameworkAims(
            string[] learnAimRefs,
            List<ILearner> learners,
            CancellationToken cancellationToken);

        Task<string> GetVersionAsync(CancellationToken cancellationToken);

        Task<LARS_Standard> GetStandard(
            int learningDeliveryStandardCode,
            CancellationToken cancellationToken);
    }
}
