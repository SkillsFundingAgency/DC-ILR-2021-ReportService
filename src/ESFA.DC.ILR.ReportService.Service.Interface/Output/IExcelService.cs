using System.Threading;
using System.Threading.Tasks;
using Aspose.Cells;

namespace ESFA.DC.ILR.ReportService.Service.Interface.Output
{
    public interface IExcelService
    {
        Workbook NewWorkbook();

        Worksheet GetWorksheetFromWorkbook(Workbook workbook, int index);
        
        Task SaveWorkbookAsync(Workbook workbook, string fileName, string container, CancellationToken cancellationToken);

        void ApplyLicense();
    }
}
