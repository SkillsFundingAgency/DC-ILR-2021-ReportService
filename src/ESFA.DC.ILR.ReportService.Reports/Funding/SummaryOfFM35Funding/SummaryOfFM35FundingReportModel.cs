namespace ESFA.DC.ILR.ReportService.Reports.Funding.SummaryOfFM35Funding
{
    public class SummaryOfFM35FundingReportModel
    {
        public int UKPRN { get; set; }

        public string FundingLineType { get; set; }

        public int Period { get; set; }

        public decimal? OnProgramme { get; set; }

        public decimal? Balancing { get; set; }

        public decimal? JobOutcomeAchievement { get; set; }

        public decimal? AimAchievement { get; set; }

        public decimal? TotalAchievement => SumTotalAchievement();

        public decimal? LearningSupport { get; set; }

        public decimal? Total => SumTotal();

        private decimal SumTotalAchievement()
        {
            return JobOutcomeAchievement + AimAchievement ?? 0m;
        }

        private decimal SumTotal()
        {
            return OnProgramme + Balancing + TotalAchievement + LearningSupport ?? 0m;
        }
    }
}
