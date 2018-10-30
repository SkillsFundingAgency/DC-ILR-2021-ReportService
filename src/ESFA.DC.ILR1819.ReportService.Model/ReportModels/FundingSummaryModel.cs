using System;
using ESFA.DC.ILR1819.ReportService.Model.Styling;

namespace ESFA.DC.ILR1819.ReportService.Model.ReportModels
{
    public sealed class FundingSummaryModel : ICloneable
    {
        public FundingSummaryModel(int excelHeaderStyle = 4, int excelRecordStyle = 4)
        {
            ExcelHeaderStyle = excelHeaderStyle;
            ExcelRecordStyle = excelRecordStyle;
        }

        public FundingSummaryModel(string title, HeaderType headerType = HeaderType.None, int excelHeaderStyle = 4)
        {
            ExcelHeaderStyle = excelHeaderStyle;
            ExcelRecordStyle = 4;
            Title = title;
            HeaderType = headerType;
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

        public HeaderType HeaderType { get; }

        /// <summary>
        /// Shallow copies this model (which is enough as it should only have value types)
        /// </summary>
        /// <returns>A shallow copy of this object.</returns>
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}