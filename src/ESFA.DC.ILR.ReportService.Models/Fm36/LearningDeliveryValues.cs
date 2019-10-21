using System;

namespace ESFA.DC.ILR.ReportService.Models.Fm36
{
    public class LearningDeliveryValues
    {
        public int? AgeAtProgStart { get; set; }

        public DateTime? AppAdjLearnStartDate { get; set; }

        public Decimal? LearnDelApplicProv1618FrameworkUplift { get; set; }

        public string LearnDelInitialFundLineType { get; set; }

        public bool? LearnDelMathEng { get; set; }

        public int? PlannedNumOnProgInstalm { get; set; }
    }
}
