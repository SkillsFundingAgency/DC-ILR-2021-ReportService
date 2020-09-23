using Aspose.Cells;
using Aspose.Cells.Tables;
using ESFA.DC.ILR.ReportService.Reports.Frm.Model;
using ESFA.DC.ILR.ReportService.Reports.Frm.Model.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ESFA.DC.ILR.ReportService.Reports.Frm
{
    public class FrmReportRenderService : IRenderService<IFrmSummaryReport>
    {
        public string[] TableColumnNames =>
            new string[]
            {
                "Report",
                "Title",
                "",
                "Number Of Queries"
            };


        private string TableHeading = "Funding Rules Monitoring";

        private readonly StyleFlag _styleFlag = new StyleFlag()
        {
            All = true
        };

        private readonly Style _headerStyle;
        private readonly Style _tableTitleStyle;
        private readonly Style _tableHeaderStyle;
        private readonly Style _tableTotalStyle;
        private readonly Style _tableRowStyle;

        public FrmReportRenderService()
        {
            var cellsFactory = new CellsFactory();

            _headerStyle = cellsFactory.CreateStyle();
            _tableTitleStyle = cellsFactory.CreateStyle();
            _tableHeaderStyle = cellsFactory.CreateStyle();
            _tableTotalStyle = cellsFactory.CreateStyle();
            _tableRowStyle = cellsFactory.CreateStyle();
            ConfigureStyles();
        }

        public Worksheet Render(IFrmSummaryReport model, Worksheet worksheet)
        {
            worksheet.Cells.StandardHeight = 15;
            GenerateSummaryHeader(worksheet, model.HeaderData, 0, 0);
            GenerateSummaryTable(worksheet, model, 8);
            worksheet.AutoFitColumns(0, 1);
            return worksheet;
        }

        public Worksheet GenerateSummaryHeader(Worksheet worksheet, IDictionary<string, string> frmSummaryReportModel, int row, int column)
        {
            foreach (var entry in frmSummaryReportModel)
            {
                worksheet.Cells.ImportTwoDimensionArray(new object[,]
                {
                    { entry.Key, entry.Value }
                }, row, 0);

                ApplyStyleToRow(worksheet, row, column, 1, 2, _headerStyle);

                row++;
            }
            worksheet.Cells["A7"].PutValue(TableHeading);
            ApplyStyleToRow(worksheet, 6, 0, 1, 1, _tableTitleStyle);
            return worksheet;
        }


        public Worksheet GenerateSummaryTable(Worksheet worksheet, IFrmSummaryReport frmSummaryReport, int row)
        {
            GenerateSummaryTableHeader(worksheet, row);

            GenerateSummaryTableRow(worksheet, frmSummaryReport, row + 1);

            return worksheet;
        }

        public Worksheet GenerateSummaryTableHeader(Worksheet worksheet, int row)
        {
            worksheet.Cells.Merge(row, 1, 1, 2);
            worksheet.Cells.Merge(row, 3, 1, 4);
            worksheet.Cells.ImportObjectArray(TableColumnNames, row, 0, false);
            ApplyStyleToRow(worksheet, row, 0, 1, TableColumnNames.Length, _tableHeaderStyle);
            return worksheet;
        }

        public Worksheet GenerateSummaryTableRow(Worksheet worksheet, IFrmSummaryReport frmSummaryReport, int row)
        {
            worksheet.Cells.Merge(row, 1, 1, 2);
            worksheet.Cells.Merge(row, 3, 1, 4);

            ImportTableOptions tableOptions = new ImportTableOptions();
            tableOptions.CheckMergedCells = true;
            tableOptions.IsFieldNameShown = false;

            worksheet.Cells.ImportCustomObjects(frmSummaryReport.SummaryTable.ToList(), row, 0, tableOptions);
            worksheet.Cells.ImportObjectArray(new object[] {
                "Total",
                "All Records",
                "",
                frmSummaryReport.TotalRowCount
            }, row + frmSummaryReport.SummaryTable.Count , 0, false);

            ApplyStyleToRow(worksheet, row, 0, frmSummaryReport.SummaryTable.Count, 7, _tableRowStyle);
            ApplyStyleToRow(worksheet, row + frmSummaryReport.SummaryTable.Count, 0, 1, 7, _tableTotalStyle);
            return worksheet;
        }

        private void ApplyStyleToRow(Worksheet worksheet, int row, int firstColumn, int totalRows, int totalColumns, Style style)
        {
            worksheet.Cells.CreateRange(row, firstColumn, totalRows, totalColumns).ApplyStyle(style, _styleFlag);
        }


        private void ConfigureStyles()
        {

            _headerStyle.Font.Size = 10;
            _headerStyle.Font.IsBold = true;
            _headerStyle.Font.Name = "Arial";


            _tableTitleStyle.Font.Size = 12;
            _tableTitleStyle.Font.IsBold = true;
            _tableTitleStyle.Font.Name = "Arial";


            _tableHeaderStyle.Font.Size = 9;
            _tableHeaderStyle.Font.IsBold = true;
            _tableHeaderStyle.Font.Name = "Arial";
            _tableHeaderStyle.HorizontalAlignment = TextAlignmentType.Center;
            _tableHeaderStyle.Pattern = BackgroundType.Solid;
            _tableHeaderStyle.Font.Color = Color.White;
            _tableHeaderStyle.ForegroundColor = Color.FromArgb(16, 78, 117);
            _tableHeaderStyle.SetBorder(BorderType.BottomBorder, CellBorderType.Thin, Color.Black);
            _tableHeaderStyle.SetBorder(BorderType.LeftBorder, CellBorderType.Thin, Color.Black);
            _tableHeaderStyle.SetBorder(BorderType.RightBorder, CellBorderType.Thin, Color.Black);
            _tableHeaderStyle.SetBorder(BorderType.TopBorder, CellBorderType.Thin, Color.Black);

            _tableRowStyle.Font.Size = 9;
            _tableRowStyle.Font.Name = "Arial";
            _tableRowStyle.SetBorder(BorderType.BottomBorder, CellBorderType.Thin, Color.Black);
            _tableRowStyle.SetBorder(BorderType.LeftBorder, CellBorderType.Thin, Color.Black);
            _tableRowStyle.SetBorder(BorderType.RightBorder, CellBorderType.Thin, Color.Black);
            _tableRowStyle.SetBorder(BorderType.TopBorder, CellBorderType.Thin, Color.Black);

            _tableTotalStyle.Font.Size = 9;
            _tableTotalStyle.Font.IsBold = true;
            _tableTotalStyle.Font.Name = "Arial";
            _tableTotalStyle.SetBorder(BorderType.BottomBorder, CellBorderType.Thin, Color.Black);
            _tableTotalStyle.SetBorder(BorderType.LeftBorder, CellBorderType.Thin, Color.Black);
            _tableTotalStyle.SetBorder(BorderType.RightBorder, CellBorderType.Thin, Color.Black);
            _tableTotalStyle.SetBorder(BorderType.TopBorder, CellBorderType.Thin, Color.Black);
        }

    }
}
