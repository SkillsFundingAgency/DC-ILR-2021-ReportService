using System;

namespace ESFA.DC.ILR.ReportService.Models.Fm35
{
    public class LearningDeliveryValue
    {
        public Decimal? AchieveElement { get; set; }

        public Decimal? AimValue { get; set; }

        public DateTime? ApplicEmpFactDate { get; set; }

        public DateTime? ApplicFactDate { get; set; }

        public string ApplicProgWeightFact { get; set; }

        public Decimal? ApplicWeightFundRate { get; set; }

        public Decimal? AreaCostFactAdj { get; set; }

        public Decimal? CapFactor { get; set; }

        public Decimal? DisUpFactAdj { get; set; }

        public string FundLine { get; set; }

        public bool? FundStart { get; set; }

        public int? LargeEmployerID { get; set; }

        public Decimal? LargeEmployerFM35Fctr { get; set; }

        public Decimal? NonGovCont { get; set; }

        public int? PlannedNumOnProgInstalm { get; set; }

        public int? PlannedNumOnProgInstalmTrans { get; set; }

        public bool? PrscHEAim { get; set; }

        public Decimal? StartPropTrans { get; set; }

        public bool? TrnWorkPlaceAim { get; set; }

        public bool? TrnWorkPrepAim { get; set; }

        public Decimal? WeightedRateFromESOL { get; set; }
    }
}
