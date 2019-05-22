﻿namespace ESFA.DC.ILR.ReportService.Model.NonContractedAppsActivity
{
    public class AECApprenticeshipPriceEpisodePeriodisedValuesInfo
    {
        public int UKPRN { get; set; }

        public string LearnRefNumber { get; set; }

        public int AimSeqNumber { get; set; }

        public string PriceEpisodeIdentifier { get; set; }

        public string AttributeName { get; set; }

        public decimal[] Periods { get; set; }
      
    }
}
