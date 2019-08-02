using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ReportService.Service.Interface
{
    public interface IAsyncModelBuilder<T>
    {
        Task<T> Build(IReportServiceContext reportServiceContext, IReportServiceDependentData reportServiceDependentData, CancellationToken cancellationToken);
    }
}
