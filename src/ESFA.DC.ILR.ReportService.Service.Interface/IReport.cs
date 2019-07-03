using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ReportService.Service.Interface
{
    public interface IReport
    {
        string TaskName { get; }

        Task<IEnumerable<string>> GenerateAsync(IReportServiceContext reportServiceContext, IReportServiceDependentData reportsDependentData, CancellationToken cancellationToken);

        IEnumerable<Type> DependsOn { get; }
    }
}
