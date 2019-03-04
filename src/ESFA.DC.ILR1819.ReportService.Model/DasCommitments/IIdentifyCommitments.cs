namespace ESFA.DC.ILR1819.ReportService.Interface.DataMatch
{
    public interface IIdentifyCommitments
    {
        string LearnRefNumber { get; set; }

        int AimSeqNumber { get; set; }

        string PriceEpisodeIdentifier { get; set; }

        long Ukprn { get; set; }
    }
}
