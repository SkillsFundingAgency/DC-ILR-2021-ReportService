namespace ESFA.DC.ILR1819.ReportService.Interface.Service
{
    public interface IValidationStageOutputCache
    {
        int DataMatchProblemCount { get; set; }

        int DataMatchProblemLearnersCount { get; set; }
    }
}
