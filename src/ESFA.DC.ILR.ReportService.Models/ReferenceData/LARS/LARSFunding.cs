using System;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.Abstract;

namespace ESFA.DC.ILR.ReportService.Models.ReferenceData.LARS
{
    public class LARSFunding : AbstractTimeBoundedEntity
    {
        public string LearnAimRef { get; set; }

        public string FundingCategory { get; set; }

        public Decimal? RateUnWeighted { get; set; }

        public Decimal? RateWeighted { get; set; }

        public string WeightingFactor { get; set; }
    }
}
