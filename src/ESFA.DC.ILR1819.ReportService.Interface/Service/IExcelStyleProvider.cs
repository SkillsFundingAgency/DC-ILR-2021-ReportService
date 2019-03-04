using Aspose.Cells;
using ESFA.DC.ILR1819.ReportService.Model.Styling;

namespace ESFA.DC.ILR1819.ReportService.Interface.Service
{
    public interface IExcelStyleProvider
    {
        CellStyle[] GetFundingSummaryStyles(Workbook workbook);

        CellStyle GetCellStyle(CellStyle[] cellStyles, int index);
    }
}
