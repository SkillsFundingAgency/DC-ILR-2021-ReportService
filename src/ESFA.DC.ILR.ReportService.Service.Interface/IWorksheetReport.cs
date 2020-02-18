using System;
using System.Collections.Generic;
using System.Threading;
using Aspose.Cells;

namespace ESFA.DC.ILR.ReportService.Service.Interface
{
    public interface IWorksheetReport
    {
        string TaskName { get; }

        void GenerateAsync(Workbook workbook, IReportServiceContext reportServiceContext, IReportServiceDependentData reportsDependentData, CancellationToken cancellationToken);

        IEnumerable<Type> DependsOn { get; }
    }
}
