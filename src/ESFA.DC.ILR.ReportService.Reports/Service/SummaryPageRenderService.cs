using Aspose.Cells;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Model.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Service
{
    public class SummaryPageRenderService : IRenderService<ISummaryPage>
    {
        private readonly Style _defaultStyle;

        public SummaryPageRenderService()
        {
            var cellsFactory = new CellsFactory();
            _defaultStyle = cellsFactory.CreateStyle();
            ConfigureStyles();
        }

        public Worksheet Render(ISummaryPage model, Worksheet worksheet)
        {
            worksheet.Workbook.DefaultStyle = _defaultStyle;
            worksheet.Cells.StandardWidth = 20;
            worksheet.Cells.Columns[0].Width = 65;

            int row = 0;

            foreach (var entry in model.HeaderData)
            {
                worksheet.Cells.ImportTwoDimensionArray(new object[,]
                {
                    { entry.Key, entry.Value }
                }, row, 0);

                row++;
            }

            foreach (var entry in model.FooterData)
            {
                worksheet.Cells.ImportTwoDimensionArray(new object[,]
                {
                    { entry.Key, entry.Value }
                }, row, 0);

                row++;
            }

            return worksheet;
        }

        private void ConfigureStyles()
        {
            _defaultStyle.Font.Size = 10;
            _defaultStyle.Font.Name = "Arial";
            _defaultStyle.Font.IsBold = true;
        }
    }
}
