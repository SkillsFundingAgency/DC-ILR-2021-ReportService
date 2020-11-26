using System;
using System.Collections.Generic;
using System.Threading;
using Aspose.Cells;

namespace ESFA.DC.ILR.ReportService.Service.Interface
{
    public interface IFrmWorksheetReport
    {
        string TaskName { get; }

        IFrmSummary Generate(Workbook workbook, IReportServiceContext reportServiceContext, IReportServiceDependentData reportsDependentData, CancellationToken cancellationToken);

        IEnumerable<Type> DependsOn { get; }
    }
}
