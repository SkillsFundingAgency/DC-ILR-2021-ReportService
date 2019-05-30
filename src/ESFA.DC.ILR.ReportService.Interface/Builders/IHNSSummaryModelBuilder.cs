using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.FundingService.FM25.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Model.ReportModels;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.ILR.ReportService.Interface.Builders
{
    public interface IHNSSummaryModelBuilder
    {
        HNSSummaryModel BuildHNSSummaryModel(
            ILogger logger,
            Task<IMessage> ilrFileTask,
            List<string> validLearners,
            FM25Global fm25Data,
            CancellationToken cancellationToken);
    }
}
