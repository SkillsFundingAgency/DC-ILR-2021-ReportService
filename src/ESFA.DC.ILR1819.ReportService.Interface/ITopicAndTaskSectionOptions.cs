namespace ESFA.DC.ILR1819.ReportService.Interface
{
    public interface ITopicAndTaskSectionOptions
    {
        string TopicValidation { get; set; }

        string TopicFunding { get; set; }

        string TopicDeds { get; set; }

        string TopicReports { get; set; }

        string TopicReports_TaskGenerateValidationReport { get; set; }

        string TopicDeds_TaskPersistDataToDeds { get; set; }
    }
}
