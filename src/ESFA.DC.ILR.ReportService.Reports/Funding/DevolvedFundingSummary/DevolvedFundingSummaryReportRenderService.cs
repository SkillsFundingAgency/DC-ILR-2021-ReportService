using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Aspose.Cells;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Funding.DevolvedFundingSummary.Model.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.DevolvedFundingSummary
{
    public class DevolvedFundingSummaryReportRenderService : IRenderService<IDevolvedAdultEducationFundingSummaryReport>
    {
        private const string ProviderName = "Provider Name:";
        private const string UKPRN = "UKPRN:";
        private const string ILRFile = "ILR File:";
        private const string LastILRFileUpdate = "Last ILR File Update:";
        private const string LastEASUpdate = "Last EAS Update:";
        private const string SourceOfFunding = "Source of Funding:";
        private const string SecurityClassification = "Security Classification:";

        private const int StartYear = 18;
        private const int EndYear = 19;

        private const string NotApplicable = "N/A";
        private const string DecimalFormat = "#,##0.00";

        private const int StartColumn = 0;
        private const int ColumnCount = 17;

        private readonly Style _defaultStyle;
        private readonly Style _futureMonthStyle;
        private readonly Style _fundingCategoryStyle;
        private readonly Style _fundingSubCategoryStyle;
        private readonly Style _fundLineGroupStyle;
        private readonly Style _headerStyle;

        private readonly StyleFlag _styleFlag = new StyleFlag()
        {
            All = true,
        };

        private readonly StyleFlag _italicStyleFlag = new StyleFlag()
        {
            FontItalic = true
        };

        public DevolvedFundingSummaryReportRenderService()
        {
            var cellsFactory = new CellsFactory();

            _defaultStyle = cellsFactory.CreateStyle();
            _futureMonthStyle = cellsFactory.CreateStyle();
            _fundingCategoryStyle = cellsFactory.CreateStyle();
            _fundingSubCategoryStyle = cellsFactory.CreateStyle();
            _fundLineGroupStyle = cellsFactory.CreateStyle();
            _headerStyle = cellsFactory.CreateStyle();

            ConfigureStyles();
        }

        public Worksheet Render(IDevolvedAdultEducationFundingSummaryReport fundingSummaryReport, Worksheet worksheet)
        {
            worksheet.Name = fundingSummaryReport.SofCode;
            worksheet.Workbook.DefaultStyle = _defaultStyle;
            worksheet.Cells.StandardWidth = 20;
            worksheet.Cells.Columns[0].Width = 65;

            RenderHeader(worksheet, NextRow(worksheet), fundingSummaryReport);

            foreach (var fundingCategory in fundingSummaryReport.FundingCategories)
            {
                RenderFundingCategory(worksheet, fundingCategory);
            }

            return worksheet;
        }

        private Worksheet RenderHeader(Worksheet worksheet, int row, IDevolvedAdultEducationFundingSummaryReport fundingSummaryReport)
        {
            worksheet.Cells.ImportTwoDimensionArray(new object[,]
            {
                { ProviderName, fundingSummaryReport.ProviderName },
                { UKPRN, fundingSummaryReport.Ukprn.ToString() },
                { ILRFile, fundingSummaryReport.IlrFile },
                { LastILRFileUpdate, fundingSummaryReport.LastSubmittedIlrFileName },
                { LastEASUpdate, "" },
                { SourceOfFunding, fundingSummaryReport.SofCode },
                { SecurityClassification, ReportingConstants.OfficialSensitive }
            },row,0 );

            ApplyStyleToRows(worksheet, row, 7, _headerStyle);

            return worksheet;
        }

        private Worksheet RenderFundingSummaryReportRow(Worksheet worksheet, int row, IDevolvedAdultEducationFundingSummaryReportRow fundingSummaryReportRow)
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

        private Worksheet RenderFundingSummaryCumulativeReportRow(Worksheet worksheet, int row, IDevolvedAdultEducationFundingCategory fundingCategory)
        {
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

            return worksheet;
        }

        private Worksheet RenderFundingCategory(Worksheet worksheet, IDevolvedAdultEducationFundingCategory fundingSubCategory)
        {
            var row = NextRow(worksheet) + 1;

            worksheet.Cells.ImportObjectArray(new object[]
            {
                fundingSubCategory.FundingCategoryTitle,
                $"Aug-{StartYear}",
                $"Sep-{StartYear}",
                $"Oct-{StartYear}",
                $"Nov-{StartYear}",
                $"Dec-{StartYear}",
                $"Jan-{EndYear}",
                $"Feb-{EndYear}",
                $"Mar-{EndYear}",
                $"Apr-{EndYear}",
                $"May-{EndYear}",
                $"Jun-{EndYear}",
                $"Jul-{EndYear}",
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
            ApplyFutureMonthStyleToRow(worksheet, row, fundingSubCategory.CurrentPeriod);

            row = NextRow(worksheet);
            RenderFundingSummaryCumulativeReportRow(worksheet, row, fundingSubCategory);
            ApplyStyleToRow(worksheet, row, _fundingSubCategoryStyle);
            ApplyFutureMonthStyleToRow(worksheet, row, fundingSubCategory.CurrentPeriod);

            return worksheet;
        }

        private Worksheet RenderFundLineGroup(Worksheet worksheet, IDevolvedAdultEducationFundLineGroup fundLineGroup)
        {
            foreach (var fundLine in fundLineGroup.FundLines)
            {
                RenderFundLine(worksheet, fundLine);
            }

            var row = NextRow(worksheet);
            RenderFundingSummaryReportRow(worksheet, row, fundLineGroup);
            ApplyStyleToRow(worksheet, row, _fundLineGroupStyle);
            ApplyFutureMonthStyleToRow(worksheet, row, fundLineGroup.CurrentPeriod);

            return worksheet;
        }

        private Worksheet RenderFundLine(Worksheet worksheet, IDevolvedAdultEducationFundLine fundLine)
        {
            var row = NextRow(worksheet);

            RenderFundingSummaryReportRow(worksheet, row, fundLine);
            ApplyFutureMonthStyleToRow(worksheet, row, fundLine.CurrentPeriod);

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

        private void ApplyStyleToRows(Worksheet worksheet, int startRow, int rowCount, Style style)
        {
            worksheet.Cells.CreateRange(startRow, StartColumn, rowCount, ColumnCount).ApplyStyle(style, _styleFlag);
        }

        private void ApplyFutureMonthStyleToRow(Worksheet worksheet, int row, int currentPeriod)
        {
            var columnCount = 12 - currentPeriod;

            if (columnCount > 0)
            {
                worksheet.Cells.CreateRange(row, currentPeriod + 1, 1, 12 - currentPeriod).ApplyStyle(_futureMonthStyle, _italicStyleFlag);
            }
        }

        private void ConfigureStyles()
        {
            _defaultStyle.Font.Size = 10;
            _defaultStyle.Font.Name = "Arial";
            _defaultStyle.SetCustom(DecimalFormat, false);

            _futureMonthStyle.Font.IsItalic = true;

            _fundingCategoryStyle.ForegroundColor = Color.FromArgb(191, 191, 191);
            _fundingCategoryStyle.Pattern = BackgroundType.Solid;
            _fundingCategoryStyle.Font.Size = 13;
            _fundingCategoryStyle.Font.IsBold = true;
            _fundingCategoryStyle.Font.Name = "Arial";
            _fundingCategoryStyle.SetCustom(DecimalFormat, false);

            _fundingSubCategoryStyle.ForegroundColor = Color.FromArgb(242, 242, 242);
            _fundingSubCategoryStyle.Pattern = BackgroundType.Solid;
            _fundingSubCategoryStyle.Font.Size = 11;
            _fundingSubCategoryStyle.Font.IsBold = true;
            _fundingSubCategoryStyle.Font.Name = "Arial";
            _fundingSubCategoryStyle.SetCustom(DecimalFormat, false);

            _fundLineGroupStyle.Font.Size = 11;
            _fundLineGroupStyle.Font.IsBold = true;
            _fundLineGroupStyle.Font.Name = "Arial";
            _fundLineGroupStyle.SetCustom(DecimalFormat, false);

            _headerStyle.Font.Size = 10;
            _headerStyle.Font.Name = "Arial";
            _headerStyle.Font.IsBold = true;
        }
    }
}
