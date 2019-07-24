using System.Drawing;
using Aspose.Cells;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary
{
    public class FundingSummaryReportRenderService : IRenderService<IFundingSummaryReport>
    {
        private const string NotApplicable = "N/A";
        private const string DecimalFormat = "#,##0.00";

        private const int StartColumn = 0;
        private const int ColumnCount = 17;

        private readonly Style _defaultStyle;
        private readonly Style _fundingCategoryStyle;
        private readonly Style _fundingSubCategoryStyle;
        private readonly Style _fundLineGroupStyle;

        private readonly StyleFlag _styleFlag = new StyleFlag()
        {
            All = true
        };
        
        public FundingSummaryReportRenderService()
        {
            var cellsFactory = new CellsFactory();

            _defaultStyle = cellsFactory.CreateStyle();
            _fundingCategoryStyle = cellsFactory.CreateStyle();
            _fundingSubCategoryStyle = cellsFactory.CreateStyle();
            _fundLineGroupStyle = cellsFactory.CreateStyle();

            ConfigureStyles();
        }

        public Worksheet Render(IFundingSummaryReport fundingSummaryReport, Worksheet worksheet)
        {
            worksheet.Workbook.DefaultStyle = _defaultStyle;
            worksheet.Cells.StandardWidth = 20;
            worksheet.Cells.Columns[0].Width = 65;

            foreach (var fundingCategory in fundingSummaryReport.FundingCategories)
            {
                RenderFundingCategory(worksheet, fundingCategory);
            }
            
            return worksheet;
        }

        private Worksheet RenderFundingCategory(Worksheet worksheet, IFundingCategory fundingCategory)
        {
            var row = NextRow(worksheet) + 1;

            worksheet.Cells.ImportObjectArray(new object[] { fundingCategory.FundingCategoryTitle}, row, 0, false);
            ApplyStyleToRow(worksheet, row, _fundingCategoryStyle);

            foreach (var fundingSubCategory in fundingCategory.FundingSubCategories)
            {
                RenderFundingSubCategory(worksheet, fundingSubCategory);
            }

            row = NextRow(worksheet);
            RenderFundingSummaryReportRow(worksheet, row, fundingCategory);
            ApplyStyleToRow(worksheet, row, _fundingCategoryStyle);

            row = NextRow(worksheet);
            worksheet.Cells.ImportObjectArray(
                new object[]
                {
                    fundingCategory.CumulativeFundingCategoryTitle,
                    fundingCategory.CumulativePeriod1,
                    fundingCategory.CumulativePeriod2,
                    fundingCategory.CumulativePeriod3,
                    fundingCategory.CumulativePeriod4,
                    fundingCategory.CumulativePeriod5,
                    fundingCategory.CumulativePeriod6,
                    fundingCategory.CumulativePeriod7,
                    fundingCategory.CumulativePeriod8,
                    fundingCategory.CumulativePeriod9,
                    fundingCategory.CumulativePeriod10,
                    fundingCategory.CumulativePeriod11,
                    fundingCategory.CumulativePeriod12,
                    NotApplicable,
                    NotApplicable,
                    NotApplicable,
                    NotApplicable,
                }, row, 0, false);
            ApplyStyleToRow(worksheet, row, _fundingCategoryStyle);

            return worksheet;
        }

        private Worksheet RenderFundingSummaryReportRow(Worksheet worksheet, int row, IFundingSummaryReportRow fundingSummaryReportRow)
        {
            worksheet.Cells.ImportObjectArray(new object[]
            {
                fundingSummaryReportRow.Title,
                fundingSummaryReportRow.Period1,
                fundingSummaryReportRow.Period2,
                fundingSummaryReportRow.Period3,
                fundingSummaryReportRow.Period4,
                fundingSummaryReportRow.Period5,
                fundingSummaryReportRow.Period6,
                fundingSummaryReportRow.Period7,
                fundingSummaryReportRow.Period8,
                fundingSummaryReportRow.Period9,
                fundingSummaryReportRow.Period10,
                fundingSummaryReportRow.Period11,
                fundingSummaryReportRow.Period12,
                fundingSummaryReportRow.Period1To8,
                fundingSummaryReportRow.Period9To12,
                fundingSummaryReportRow.YearToDate,
                fundingSummaryReportRow.Total,
            }, row, 0, false);

            return worksheet;
        }

        private Worksheet RenderFundingSubCategory(Worksheet worksheet, IFundingSubCategory fundingSubCategory)
        {
            var row = NextRow(worksheet) + 1;

            worksheet.Cells.ImportObjectArray(new object[]
            {
                fundingSubCategory.FundingSubCategoryTitle,
                "Aug-18",
                "Sep-18",
                "Oct-18",
                "Nov-18",
                "Dec-18",
                "Jan-19",
                "Feb-19",
                "Mar-19",
                "Apr-19",
                "May-19",
                "Jun-19",
                "Jul-19",
                "Aug - Mar",
                "Apr - Jul",
                "Year To Date",
                "Total",
            }, row, 0, false);
            ApplyStyleToRow(worksheet, row, _fundingSubCategoryStyle);

            foreach (var fundLineGroup in fundingSubCategory.FundLineGroups)
            {
                RenderFundLineGroup(worksheet, fundLineGroup);
            }

            row = NextRow(worksheet);
            RenderFundingSummaryReportRow(worksheet, row, fundingSubCategory);
            ApplyStyleToRow(worksheet, row, _fundingSubCategoryStyle);

            return worksheet;
        }

        private Worksheet RenderFundLineGroup(Worksheet worksheet, IFundLineGroup fundLineGroup)
        {
            foreach (var fundLine in fundLineGroup.FundLines)
            {
                RenderFundLine(worksheet, fundLine);
            }

            var row = NextRow(worksheet);
            RenderFundingSummaryReportRow(worksheet, row, fundLineGroup);
            ApplyStyleToRow(worksheet, row, _fundLineGroupStyle);

            return worksheet;
        }

        private Worksheet RenderFundLine(Worksheet worksheet, IFundLine fundLine)
        {
            RenderFundingSummaryReportRow(worksheet, NextRow(worksheet), fundLine);

            return worksheet;
        }

        private int NextRow(Worksheet worksheet)
        {
            return worksheet.Cells.MaxRow + 1;
        }

        private void ApplyStyleToRow(Worksheet worksheet, int row, Style style)
        {
            worksheet.Cells.CreateRange(row, StartColumn, 1, ColumnCount).ApplyStyle(style, _styleFlag);
        }

        private void ConfigureStyles()
        {
            _defaultStyle.Font.Size = 10;
            _defaultStyle.Font.IsBold = false;
            _defaultStyle.Font.Name = "Arial";
            _defaultStyle.SetCustom(DecimalFormat, false);

            _fundingCategoryStyle.ForegroundColor = System.Drawing.Color.FromArgb(191, 191, 191);
            _fundingCategoryStyle.Pattern = BackgroundType.Solid;
            _fundingCategoryStyle.Font.Size = 13;
            _fundingCategoryStyle.Font.IsBold = true;
            _fundingCategoryStyle.Font.Name = "Arial";
            _fundingCategoryStyle.SetCustom(DecimalFormat, false);

            _fundingSubCategoryStyle.ForegroundColor = System.Drawing.Color.FromArgb(242, 242, 242);
            _fundingSubCategoryStyle.Pattern = BackgroundType.Solid;
            _fundingSubCategoryStyle.Font.Size = 11;
            _fundingSubCategoryStyle.Font.IsBold = true;
            _fundingSubCategoryStyle.Font.Name = "Arial";
            _fundingSubCategoryStyle.SetCustom(DecimalFormat, false);

            _fundLineGroupStyle.Font.Size = 11;
            _fundLineGroupStyle.Font.IsBold = true;
            _fundLineGroupStyle.Font.Name = "Arial";
            _fundLineGroupStyle.SetCustom(DecimalFormat, false);
        }
    }
}
