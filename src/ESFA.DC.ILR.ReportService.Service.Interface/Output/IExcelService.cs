using System.Threading;
using System.Threading.Tasks;
using Aspose.Cells;

namespace ESFA.DC.ILR.ReportService.Service.Interface.Output
{
    public interface IExcelService
    {
        Task WriteWorkbookAsync(Workbook workbook, string fileName, string container, CancellationToken cancellationToken);
    }
}
