using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Aspose.Cells;

namespace ESFA.DC.ILR.ReportService.Service.Interface.Output
{
    public interface IExcelService
    {
        Workbook NewWorkbook();

        Worksheet GetWorksheetFromWorkbook(Workbook workbook, int index);
        
        void WriteRowsToWorksheet<T>(Worksheet worksheet, IEnumerable<T> rows);

        Task WriteRowsAndSaveNewWorkbookAsync<T>(IEnumerable<T> rows, string fileName, string container, CancellationToken cancellationToken);

        Task SaveWorkbookAsync(Workbook workbook, string fileName, string container, CancellationToken cancellationToken);
    }
}
