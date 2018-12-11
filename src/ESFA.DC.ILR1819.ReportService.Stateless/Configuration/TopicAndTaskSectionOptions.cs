using ESFA.DC.ILR1819.ReportService.Interface.Configuration;

namespace ESFA.DC.ILR1819.ReportService.Stateless.Configuration
{
    public sealed class TopicAndTaskSectionOptions : ITopicAndTaskSectionOptions
    {
        public string TopicValidation { get; set; }

        public string TopicFunding { get; set; }

        public string TopicDeds { get; set; }

        public string TopicReports { get; set; }

        public string TopicReports_TaskGenerateValidationReport { get; set; }

        public string TopicReports_TaskGenerateAllbOccupancyReport { get; set; }

        public string TopicReports_TaskGenerateFundingSummaryReport { get; set; }

        public string TopicReports_TaskGenerateAdultFundingClaimReport { get; set; }

        public string TopicDeds_TaskPersistDataToDeds { get; set; }

        public string TopicReports_TaskGenerateMainOccupancyReport { get; set; }

        public string TopicReports_TaskGenerateSummaryOfFunding1619Report { get; set; }

        public string TopicReports_TaskGenerateMathsAndEnglishReport { get; set; }

        public string TopicReports_TaskGenerateSummaryOfFM35FundingReport { get; set; }

        public string TopicReports_TaskGenerateAppsAdditionalPaymentsReport { get; set; }

        public string TopicReports_TaskGenerateAppsIndicativeEarningsReport { get; set; }

        public string TopicReports_TaskGenerateDataMatchReport { get; set; }

        public string TopicReports_TaskGenerateTrailblazerEmployerIncentivesReport { get; set; }

        public string TopicReports_TaskGenerateFundingClaim1619Report { get; set; }
    }
}
