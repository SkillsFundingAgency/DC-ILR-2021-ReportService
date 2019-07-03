using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ReportService.Service.Interface
{
    public interface IReportsDependentDataPopulationService
    {
        Task<IReportServiceDependentData> PopulateAsync(IReportServiceContext reportServiceContext, IEnumerable<Type> dependentTypes, CancellationToken cancellationToken);
    }
}
