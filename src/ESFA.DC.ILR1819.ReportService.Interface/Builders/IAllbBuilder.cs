using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;
using ESFA.DC.JobContextManager.Model.Interface;

namespace ESFA.DC.ILR1819.ReportService.Interface.Builders
{
    public interface IAllbBuilder
    {
        Task<List<FundingSummaryModel>> BuildAsync(IJobContextMessage jobContextMessage, CancellationToken cancellationToken);
    }
}
