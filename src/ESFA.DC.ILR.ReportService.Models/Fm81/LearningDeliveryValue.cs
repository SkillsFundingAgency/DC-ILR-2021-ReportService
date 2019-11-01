using System;

namespace ESFA.DC.ILR.ReportService.Models.Fm81
{
    public class LearningDeliveryValue
    {
        public DateTime? AchApplicDate { get; set; }

        public Decimal? AchievementApplicVal { get; set; }

        public int? AgeStandardStart { get; set; }

        public DateTime? ApplicFundValDate { get; set; }

        public long? CoreGovContCapApplicVal { get; set; }

        public int? EmpIdAchDate { get; set; }

        public int? EmpIdFirstDayStandard { get; set; }

        public int? EmpIdFirstYoungAppDate { get; set; }

        public int? EmpIdSecondYoungAppDate { get; set; }

        public int? EmpIdSmallBusDate { get; set; }

        public string FundLine { get; set; }

        public bool? MathEngLSFFundStart { get; set; }

        public Decimal? SmallBusApplicVal { get; set; }

        public bool? SmallBusEligible { get; set; }

        public Decimal? YoungAppApplicVal { get; set; }

        public bool? YoungAppEligible { get; set; }
    }
}
