using System;

namespace ESFA.DC.ILR.ReportService.Models.Fm99
{
    public class LearningDeliveryValue
    {
        public DateTime? ApplicFactDate { get; set; }

        public string ApplicProgWeightFact { get; set; }

        public Decimal? AreaCostFactAdj { get; set; }

        public string FundLine { get; set; }

        public DateTime? LiabilityDate { get; set; }

        public int? PlannedNumOnProgInstalm { get; set; }

        public Decimal? WeightedRate { get; set; }
    }
}
