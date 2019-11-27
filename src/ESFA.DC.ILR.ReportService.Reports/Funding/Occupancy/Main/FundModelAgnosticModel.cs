using System;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Main
{
    public class FundModelAgnosticModel
    {
        public int? AimSequenceNumber { get; set; }

        public int? AimType { get; set; }

        public int? CompStatus { get; set; }

        public int FundModel { get; set; }

        public decimal? ApplicableFundingRate { get; set; }

        public DateTime? LearningStartDate { get; set; }

        public DateTime? LearningPlannedEndDate { get; set; }

        public DateTime? LearningActualEndDate { get; set; }

        public string FundLine { get; set; }
    }
}
