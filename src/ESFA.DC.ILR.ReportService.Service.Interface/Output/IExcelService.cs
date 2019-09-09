using System.Threading;
using System.Threading.Tasks;
using Aspose.Cells;

namespace ESFA.DC.ILR.ReportService.Service.Interface.Output
{
    public interface IExcelService
    {
        Workbook NewWorkbook();

        Workbook BindExcelTemplateToWorkbook<T>(T model, string templateName, string dataSource);

        Workbook GetWorkbookFromTemplate(string templateFileName);

        Worksheet GetWorksheetFromWorkbook(Workbook workbook, int index);

        Worksheet GetWorksheetFromWorkbook(Workbook workbook, string sheetName);

        Task SaveWorkbookAsync(Workbook workbook, string fileName, string container, CancellationToken cancellationToken);

        void ApplyLicense();
    }
}
