using Aspose.Cells;
using Aspose.Cells.Tables;
using ESFA.DC.ILR.ReportService.Service.Interface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ESFA.DC.ILR.ReportService.Reports.Frm.Summary
{
    public class FrmSummaryReportRenderService : IRenderService<FrmSummaryReportModel>
    {
        public string[] TableColumnNames =>
            new string[]
            {
                "Report",
                "Title",
                "",
                "Number Of Queries"
            };

        public object[] HeaderRows =>
            new object[]
            {
                "Provider Name:",
                "UKPRN:",
                "ILR File:",
                "Last ILR File Update:",
                "Security Classification"
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

        public FrmSummaryReportRenderService()
        {
            var cellsFactory = new CellsFactory();

            _headerStyle = cellsFactory.CreateStyle();
            _tableTitleStyle = cellsFactory.CreateStyle();
            _tableHeaderStyle = cellsFactory.CreateStyle();
            _tableTotalStyle = cellsFactory.CreateStyle();
            _tableRowStyle = cellsFactory.CreateStyle();
            ConfigureStyles();
        }

        public Worksheet Render(FrmSummaryReportModel model, Worksheet worksheet)
        {
            worksheet.Cells.StandardHeight = 15;
            GenerateSummaryHeader(worksheet, model, 0, 0);
            GenerateSummaryTable(worksheet, model.FundingRulesMonitoring, 8);
            worksheet.AutoFitColumns(0, 1);
            return worksheet;
        }

        public Worksheet GenerateSummaryHeader(Worksheet worksheet, FrmSummaryReportModel frmSummaryReportModel, int row, int column)
        {
            worksheet.Cells.ImportObjectArray(HeaderRows, row, column, true);
            worksheet.Cells.ImportObjectArray(new object[]
                {
                    frmSummaryReportModel.ProviderName,
                    frmSummaryReportModel.UKPRN,
                    frmSummaryReportModel.ILRFileName,
                    frmSummaryReportModel.LastFileUpdate,
                    frmSummaryReportModel.SecurityClassification
                }, row, column + 1, true);

            ApplyStyleToRow(worksheet, row, column, HeaderRows.Length, 2, _headerStyle);
            worksheet.Cells["A7"].PutValue(TableHeading);
            ApplyStyleToRow(worksheet, 6, 0, 1, 1, _tableTitleStyle);
            return worksheet;
        }

        public Worksheet GenerateSummaryTable(Worksheet worksheet, List<FrmSummaryReportTableRow> frmSummaryReportTableRows, int row)
        {
            GenerateSummaryTableHeader(worksheet, row);

            GenerateSummaryTableRow(worksheet, frmSummaryReportTableRows, row+1);

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

        public Worksheet GenerateSummaryTableRow(Worksheet worksheet, List<FrmSummaryReportTableRow> frmSummaryReportTableRows, int row)
        {
            worksheet.Cells.Merge(row, 1, 1, 2);
            worksheet.Cells.Merge(row, 3, 1, 4);

            ImportTableOptions tableOptions = new ImportTableOptions();
            tableOptions.CheckMergedCells = true;
            tableOptions.IsFieldNameShown = false;

            worksheet.Cells.ImportCustomObjects(frmSummaryReportTableRows, row, 0, tableOptions);

            ApplyStyleToRow(worksheet, row, 0, frmSummaryReportTableRows.Count, 7, _tableRowStyle);
            ApplyStyleToRow(worksheet, row + frmSummaryReportTableRows.Count - 1 , 0, 1, 7, _tableTotalStyle);
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
