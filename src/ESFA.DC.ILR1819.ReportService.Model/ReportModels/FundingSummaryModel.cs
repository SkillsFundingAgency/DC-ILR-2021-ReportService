﻿namespace ESFA.DC.ILR1819.ReportService.Model.ReportModels
{
    public sealed class FundingSummaryModel
    {
        public FundingSummaryModel(int excelHeaderStyle = 4, int excelRecordStyle = 4)
        {
            ExcelHeaderStyle = excelHeaderStyle;
            ExcelRecordStyle = excelRecordStyle;
        }

        public FundingSummaryModel(string title, bool titleOnly = false, int excelHeaderStyle = 4)
        {
            ExcelHeaderStyle = excelHeaderStyle;
            ExcelRecordStyle = 4;
            Title = title;
            TitleOnly = titleOnly;
        }

        public string Title { get; set; }

        public decimal? Period1 { get; set; }

        public decimal? Period2 { get; set; }

        public decimal? Period3 { get; set; }

        public decimal? Period4 { get; set; }

        public decimal? Period5 { get; set; }

        public decimal? Period6 { get; set; }

        public decimal? Period7 { get; set; }

        public decimal? Period8 { get; set; }

        public decimal? Period9 { get; set; }

        public decimal? Period10 { get; set; }

        public decimal? Period11 { get; set; }

        public decimal? Period12 { get; set; }

        public decimal? Period1_8 { get; set; }

        public decimal? Period9_12 { get; set; }

        public decimal? YearToDate { get; set; }

        public decimal? Total { get; set; }

        public int ExcelHeaderStyle { get; set; }

        public int ExcelRecordStyle { get; set; }

        public bool TitleOnly { get; }
    }
}