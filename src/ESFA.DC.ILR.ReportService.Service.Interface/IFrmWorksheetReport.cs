using System;
using System.Collections.Generic;
using System.Threading;
using Aspose.Cells;

namespace ESFA.DC.ILR.ReportService.Service.Interface
{
    public interface IFrmWorksheetReport
    {
        string TaskName { get; }

        //todo: change return object to 
        IFrmSummaryTableRow Generate(Workbook workbook, IReportServiceContext reportServiceContext, IReportServiceDependentData reportsDependentData, CancellationToken cancellationToken, IList<IFrmSummaryTableRow> tableRows = null);

        IEnumerable<Type> DependsOn { get; }
    }
}
