﻿namespace ESFA.DC.ILR1819.ReportService.Model.DasCommitments
{
    public interface IIdentifyCommitments
    {
        string LearnRefNumber { get; set; }

        int AimSeqNumber { get; set; }

        string PriceEpisodeIdentifier { get; set; }

        long Ukprn { get; set; }
    }
}
