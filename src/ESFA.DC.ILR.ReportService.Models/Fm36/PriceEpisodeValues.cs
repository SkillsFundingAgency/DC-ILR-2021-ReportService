using System;

namespace ESFA.DC.ILR.ReportService.Models.Fm36
{
    public class PriceEpisodeValues
    {
        public DateTime? EpisodeStartDate { get; set; }

        public Decimal? TNP1 { get; set; }

        public Decimal? TNP2 { get; set; }

        public Decimal? TNP3 { get; set; }

        public Decimal? TNP4 { get; set; }

        public DateTime? PriceEpisodeActualEndDateIncEPA { get; set; }

        public Decimal? PriceEpisode1618FUBalValue { get; set; }

        public Decimal? PriceEpisodeApplic1618FrameworkUpliftCompElement { get; set; }

        public Decimal? PriceEpisode1618FrameworkUpliftTotPrevEarnings { get; set; }

        public Decimal? PriceEpisode1618FrameworkUpliftRemainingAmount { get; set; }

        public Decimal? PriceEpisode1618FUMonthInstValue { get; set; }

        public Decimal? PriceEpisode1618FUTotEarnings { get; set; }

        public Decimal? PriceEpisodeUpperBandLimit { get; set; }

        public DateTime? PriceEpisodePlannedEndDate { get; set; }

        public DateTime? PriceEpisodeActualEndDate { get; set; }

        public Decimal? PriceEpisodeTotalTNPPrice { get; set; }

        public Decimal? PriceEpisodeUpperLimitAdjustment { get; set; }

        public int? PriceEpisodePlannedInstalments { get; set; }

        public int? PriceEpisodeActualInstalments { get; set; }

        public int? PriceEpisodeInstalmentsThisPeriod { get; set; }

        public Decimal? PriceEpisodeCompletionElement { get; set; }

        public Decimal? PriceEpisodePreviousEarnings { get; set; }

        public Decimal? PriceEpisodeInstalmentValue { get; set; }

        public Decimal? PriceEpisodeOnProgPayment { get; set; }

        public Decimal? PriceEpisodeTotalEarnings { get; set; }

        public Decimal? PriceEpisodeBalanceValue { get; set; }

        public Decimal? PriceEpisodeBalancePayment { get; set; }

        public bool? PriceEpisodeCompleted { get; set; }

        public Decimal? PriceEpisodeCompletionPayment { get; set; }

        public Decimal? PriceEpisodeRemainingTNPAmount { get; set; }

        public Decimal? PriceEpisodeRemainingAmountWithinUpperLimit { get; set; }

        public Decimal? PriceEpisodeCappedRemainingTNPAmount { get; set; }

        public Decimal? PriceEpisodeExpectedTotalMonthlyValue { get; set; }

        public int? PriceEpisodeAimSeqNumber { get; set; }

        public Decimal? PriceEpisodeFirstDisadvantagePayment { get; set; }

        public Decimal? PriceEpisodeSecondDisadvantagePayment { get; set; }

        public Decimal? PriceEpisodeApplic1618FrameworkUpliftBalancing { get; set; }

        public Decimal? PriceEpisodeApplic1618FrameworkUpliftCompletionPayment { get; set; }

        public Decimal? PriceEpisodeApplic1618FrameworkUpliftOnProgPayment { get; set; }

        public Decimal? PriceEpisodeSecondProv1618Pay { get; set; }

        public Decimal? PriceEpisodeFirstEmp1618Pay { get; set; }

        public Decimal? PriceEpisodeSecondEmp1618Pay { get; set; }

        public Decimal? PriceEpisodeFirstProv1618Pay { get; set; }

        public Decimal? PriceEpisodeLSFCash { get; set; }

        public string PriceEpisodeFundLineType { get; set; }

        public Decimal? PriceEpisodeSFAContribPct { get; set; }

        public int? PriceEpisodeLevyNonPayInd { get; set; }

        public DateTime? EpisodeEffectiveTNPStartDate { get; set; }

        public DateTime? PriceEpisodeFirstAdditionalPaymentThresholdDate { get; set; }

        public DateTime? PriceEpisodeSecondAdditionalPaymentThresholdDate { get; set; }

        public string PriceEpisodeContractType { get; set; }

        public Decimal? PriceEpisodePreviousEarningsSameProvider { get; set; }

        public Decimal? PriceEpisodeTotProgFunding { get; set; }

        public Decimal? PriceEpisodeProgFundIndMinCoInvest { get; set; }

        public Decimal? PriceEpisodeProgFundIndMaxEmpCont { get; set; }

        public Decimal? PriceEpisodeTotalPMRs { get; set; }

        public Decimal? PriceEpisodeCumulativePMRs { get; set; }

        public int? PriceEpisodeCompExemCode { get; set; }

        public DateTime? PriceEpisodeLearnerAdditionalPaymentThresholdDate { get; set; }

        public string PriceEpisodeAgreeId { get; set; }

        public DateTime? PriceEpisodeRedStartDate { get; set; }

        public int? PriceEpisodeRedStatusCode { get; set; }
    }
}
