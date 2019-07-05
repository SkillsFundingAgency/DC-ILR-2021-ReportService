using System.Collections.Generic;
using System.ComponentModel;
using Aspose.Cells;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding.FundingSummary
{
    public class FundingSummaryReportRenderService
    {
        private const string NotApplicable = "N/A";
        private const string DecimalFormat = "#,##0.00";

        private const int StartColumn = 0;
        private const int ColumnCount = 17;

        private readonly Style _defaultStyle;
        private readonly Style _fundingCategoryStyle;

       

        private readonly StyleFlag _styleFlag = new StyleFlag()
        {
            All = true
        };
        
        public FundingSummaryReportRenderService()
        {
            var cellsFactory = new CellsFactory();

            _defaultStyle = cellsFactory.CreateStyle();
            _fundingCategoryStyle = cellsFactory.CreateStyle();

            ConfigureStyles();
        }

        public Worksheet Render(IFundingSummaryReport fundingSummaryReport, Worksheet worksheet)
        {
            worksheet.Workbook.DefaultStyle = _defaultStyle;

            foreach (var fundingCategory in fundingSummaryReport.FundingCategories)
            {
                RenderFundingCategory(worksheet, fundingCategory);
            }

            return worksheet;
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
            
            //Style style2 = workbook.CreateStyle();
            //style2.ForegroundColor = System.Drawing.Color.FromArgb(216, 216, 216);
            //style2.Pattern = BackgroundType.Solid;
            //style2.Font.Size = 12;
            //style2.Font.IsBold = true;
            //style2.Font.Name = "Arial";
            //style2.SetCustom(Constants.FundingSummaryReportDecimalFormat, true);
            
            //Style style3 = workbook.CreateStyle();
            //style3.ForegroundColor = System.Drawing.Color.FromArgb(242, 242, 242);
            //style3.Pattern = BackgroundType.Solid;
            //style3.Font.Size = 11;
            //style3.Font.IsBold = true;
            //style3.Font.Name = "Arial";
            //style3.SetCustom(Constants.FundingSummaryReportDecimalFormat, true);
            
            //Style style4 = workbook.CreateStyle();
            //style4.Font.Size = 11;
            //style4.Font.IsBold = true;
            //style4.Font.Name = "Arial";
            //style4.SetCustom(Constants.FundingSummaryReportDecimalFormat, true);
            
            //Style style5 = workbook.CreateStyle();
            //style5.Font.Size = 10;
            //style5.Font.Name = "Arial";
            //style5.SetCustom(Constants.FundingSummaryReportDecimalFormat, true);
        }

        private void ApplyStyleToRow(Worksheet worksheet, int row, Style style)
        {
            worksheet.Cells.CreateRange(row, StartColumn, 1, ColumnCount).ApplyStyle(style, _styleFlag);
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

            foreach (var fundLineGroup in fundingSubCategory.FundLineGroups)
            {
                RenderFundLineGroup(worksheet, fundLineGroup);
            }

            RenderFundingSummaryReportRow(worksheet, NextRow(worksheet), fundingSubCategory);

            return worksheet;
        }

        private Worksheet RenderFundLineGroup(Worksheet worksheet, IFundLineGroup fundLineGroup)
        {
            foreach (var fundLine in fundLineGroup.FundLines)
            {
                RenderFundLine(worksheet, fundLine);
            }

            RenderFundingSummaryReportRow(worksheet, NextRow(worksheet), fundLineGroup);

            return worksheet;
        }

        private Worksheet RenderFundLine(Worksheet worksheet, IFundLine fundLine)
        {
            RenderFundingSummaryReportRow(worksheet, NextRow(worksheet), fundLine);

            return worksheet;
        }

        private int NextRow(Worksheet worksheet)
        {
            var maxRow = worksheet.Cells.MaxRow;

            maxRow = maxRow == -1 ? 0 : maxRow;

            return maxRow + 1;
        }
    }
}
