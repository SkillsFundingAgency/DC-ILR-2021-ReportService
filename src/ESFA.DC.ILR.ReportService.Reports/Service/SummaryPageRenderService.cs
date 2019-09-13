using Aspose.Cells;
using ESFA.DC.ILR.ReportService.Reports.Model.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Service
{
    public class SummaryPageRenderService : IRenderService<ISummaryPage>
    {
        private readonly Style _descriptionStyle;

        private readonly StyleFlag _styleFlag = new StyleFlag()
        {
            All = true,
        };

        public SummaryPageRenderService()
        {
            var cellsFactory = new CellsFactory();
            _descriptionStyle = cellsFactory.CreateStyle();
            ConfigureStyles();
        }

        public Worksheet Render(ISummaryPage model, Worksheet worksheet)
        {
            worksheet.Cells.Columns[0].Width = 30;
            worksheet.Cells.Columns[1].Width = 40;
            worksheet.Cells.CreateRange(0, 0, 13, 1).ApplyStyle(_descriptionStyle, _styleFlag);

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
            _descriptionStyle.Font.Size = 10;
            _descriptionStyle.Font.Name = "Arial";
            _descriptionStyle.Font.IsBold = true;
        }
    }
}
