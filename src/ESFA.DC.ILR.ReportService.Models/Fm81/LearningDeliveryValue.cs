using System;

namespace ESFA.DC.ILR.ReportService.Models.Fm81
{
    public class LearningDeliveryValue
    {
        public DateTime? AchApplicDate { get; set; }

        public bool? AchEligible { get; set; }

        public bool? Achieved { get; set; }

        public Decimal? AchievementApplicVal { get; set; }

        public Decimal? AchPayment { get; set; }

        public int? ActualDaysIL { get; set; }

        public int? ActualNumInstalm { get; set; }

        public DateTime? AdjProgStartDate { get; set; }

        public DateTime? AdjStartDate { get; set; }

        public int? AgeStandardStart { get; set; }

        public DateTime? ApplicFundValDate { get; set; }

        public Decimal? CombinedAdjProp { get; set; }

        public long? CoreGovContCapApplicVal { get; set; }

        public Decimal? CoreGovContPayment { get; set; }

        public Decimal? CoreGovContUncapped { get; set; }

        public int? EmpIdAchDate { get; set; }

        public int? EmpIdFirstDayStandard { get; set; }

        public int? EmpIdFirstYoungAppDate { get; set; }

        public int? EmpIdSecondYoungAppDate { get; set; }

        public int? EmpIdSmallBusDate { get; set; }

        public string FundLine { get; set; }

        public int? InstPerPeriod { get; set; }

        public int? LearnDelDaysIL { get; set; }

        public int? LearnDelStandardAccDaysIL { get; set; }

        public int? LearnDelStandardPrevAccDaysIL { get; set; }

        public int? LearnDelStandardTotalDaysIL { get; set; }

        public bool? LearnSuppFund { get; set; }

        public Decimal? LearnSuppFundCash { get; set; }

        public Decimal? MathEngAimValue { get; set; }

        public Decimal? MathEngBalPayment { get; set; }

        public long? MathEngBalPct { get; set; }

        public bool? MathEngLSFFundStart { get; set; }

        public int? MathEngLSFThresholdDays { get; set; }

        public Decimal? MathEngOnProgPayment { get; set; }

        public int? MathEngOnProgPct { get; set; }

        public int? OutstandNumOnProgInstalm { get; set; }

        public int? PlannedNumOnProgInstalm { get; set; }

        public int? PlannedTotalDaysIL { get; set; }

        public DateTime? ProgStandardStartDate { get; set; }

        public Decimal? SmallBusApplicVal { get; set; }

        public bool? SmallBusEligible { get; set; }

        public Decimal? SmallBusPayment { get; set; }

        public int? SmallBusStatusFirstDayStandard { get; set; }

        public int? SmallBusStatusThreshold { get; set; }

        public DateTime? SmallBusThresholdDate { get; set; }

        public Decimal? YoungAppApplicVal { get; set; }

        public bool? YoungAppEligible { get; set; }

        public Decimal? YoungAppFirstPayment { get; set; }

        public DateTime? YoungAppFirstThresholdDate { get; set; }

        public Decimal? YoungAppPayment { get; set; }

        public Decimal? YoungAppSecondPayment { get; set; }

        public DateTime? YoungAppSecondThresholdDate { get; set; }
    }
}
