using System;

namespace ESFA.DC.ILR.ReportService.Models.Fm99
{
    public class LearningDeliveryValue
    {
        public bool? Achieved { get; set; }

        public int? ActualNumInstalm { get; set; }

        public bool? AdvLoan { get; set; }

        public DateTime? ApplicFactDate { get; set; }

        public string ApplicProgWeightFact { get; set; }

        public Decimal? AreaCostFactAdj { get; set; }

        public Decimal? AreaCostInstalment { get; set; }

        public string FundLine { get; set; }

        public bool? FundStart { get; set; }

        public DateTime? LiabilityDate { get; set; }

        public bool? LoanBursAreaUplift { get; set; }

        public bool? LoanBursSupp { get; set; }

        public int? OutstndNumOnProgInstalm { get; set; }

        public int? PlannedNumOnProgInstalm { get; set; }

        public Decimal? WeightedRate { get; set; }
    }
}
