namespace ESFA.DC.ILR1819.ReportService.Model.DasCommitments
{
    public sealed class DatalockValidationError : IIdentifyCommitments
    {
        public long Ukprn { get; set; }

        public string LearnRefNumber { get; set; }

        public int AimSeqNumber { get; set; }

        public string RuleId { get; set; }

        public string PriceEpisodeIdentifier { get; set; }
    }
}
