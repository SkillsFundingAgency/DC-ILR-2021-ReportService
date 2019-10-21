using System;

namespace ESFA.DC.ILR.ReportService.Models.Fm35
{
    public class LearningDeliveryValue
    {
        public DateTime? AchApplicDate { get; set; }

        public bool? Achieved { get; set; }

        public Decimal? AchieveElement { get; set; }

        public bool? AchievePayElig { get; set; }

        public Decimal? AchievePayPctPreTrans { get; set; }

        public Decimal? AchPayTransHeldBack { get; set; }

        public int? ActualDaysIL { get; set; }

        public int? ActualNumInstalm { get; set; }

        public int? ActualNumInstalmPreTrans { get; set; }

        public int? ActualNumInstalmTrans { get; set; }

        public DateTime? AdjLearnStartDate { get; set; }

        public bool? AdltLearnResp { get; set; }

        public int? AgeAimStart { get; set; }

        public Decimal? AimValue { get; set; }

        public DateTime? AppAdjLearnStartDate { get; set; }

        public Decimal? AppAgeFact { get; set; }

        public bool? AppATAGTA { get; set; }

        public bool? AppCompetency { get; set; }

        public bool? AppFuncSkill { get; set; }

        public Decimal? AppFuncSkill1618AdjFact { get; set; }

        public bool? AppKnowl { get; set; }

        public DateTime? AppLearnStartDate { get; set; }

        public DateTime? ApplicEmpFactDate { get; set; }

        public DateTime? ApplicFactDate { get; set; }

        public DateTime? ApplicFundRateDate { get; set; }

        public string ApplicProgWeightFact { get; set; }

        public Decimal? ApplicUnweightFundRate { get; set; }

        public Decimal? ApplicWeightFundRate { get; set; }

        public bool? AppNonFund { get; set; }

        public Decimal? AreaCostFactAdj { get; set; }

        public int? BalInstalmPreTrans { get; set; }

        public Decimal? BaseValueUnweight { get; set; }

        public Decimal? CapFactor { get; set; }

        public Decimal? DisUpFactAdj { get; set; }

        public bool? EmpOutcomePayElig { get; set; }

        public Decimal? EmpOutcomePctHeldBackTrans { get; set; }

        public Decimal? EmpOutcomePctPreTrans { get; set; }

        public bool? EmpRespOth { get; set; }

        public bool? ESOL { get; set; }

        public bool? FullyFund { get; set; }

        public string FundLine { get; set; }

        public bool? FundStart { get; set; }

        public int? LargeEmployerID { get; set; }

        public Decimal? LargeEmployerFM35Fctr { get; set; }

        public DateTime? LargeEmployerStatusDate { get; set; }

        public string LearnDelFundOrgCode { get; set; }

        public Decimal? LTRCUpliftFctr { get; set; }

        public Decimal? NonGovCont { get; set; }

        public bool? OLASSCustody { get; set; }

        public Decimal? OnProgPayPctPreTrans { get; set; }

        public int? OutstndNumOnProgInstalm { get; set; }

        public int? OutstndNumOnProgInstalmTrans { get; set; }

        public int? PlannedNumOnProgInstalm { get; set; }

        public int? PlannedNumOnProgInstalmTrans { get; set; }

        public int? PlannedTotalDaysIL { get; set; }

        public int? PlannedTotalDaysILPreTrans { get; set; }

        public Decimal? PropFundRemain { get; set; }

        public Decimal? PropFundRemainAch { get; set; }

        public bool? PrscHEAim { get; set; }

        public bool? Residential { get; set; }

        public bool? Restart { get; set; }

        public Decimal? SpecResUplift { get; set; }

        public Decimal? StartPropTrans { get; set; }

        public int? ThresholdDays { get; set; }

        public bool? Traineeship { get; set; }

        public bool? Trans { get; set; }

        public DateTime? TrnAdjLearnStartDate { get; set; }

        public bool? TrnWorkPlaceAim { get; set; }

        public bool? TrnWorkPrepAim { get; set; }

        public Decimal? UnWeightedRateFromESOL { get; set; }

        public Decimal? UnweightedRateFromLARS { get; set; }

        public Decimal? WeightedRateFromESOL { get; set; }

        public Decimal? WeightedRateFromLARS { get; set; }
    }
}
