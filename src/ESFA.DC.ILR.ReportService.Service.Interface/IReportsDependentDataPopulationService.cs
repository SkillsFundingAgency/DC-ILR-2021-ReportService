using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Service.Model;

namespace ESFA.DC.ILR.ReportService.Service.Interface
{
    public interface IReportsDependentDataPopulationService
    {
        Task<ReportServiceDependentData> PopulateAsync(IReportServiceContext reportServiceContext, IEnumerable<Type> dependentTypes, CancellationToken cancellationToken);
    }
}
