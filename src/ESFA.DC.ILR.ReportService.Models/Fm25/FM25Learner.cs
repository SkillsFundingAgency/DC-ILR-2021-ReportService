using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ReportService.Models.Fm25
{
    public class FM25Learner
    {
        public string LearnRefNumber { get; set; }

        public int? AcadMonthPayment { get; set; }

        public bool? AcadProg { get; set; }

        public int? ActualDaysILCurrYear { get; set; }

        public Decimal? AreaCostFact1618Hist { get; set; }

        public Decimal? Block1DisadvUpliftNew { get; set; }

        public Decimal? Block2DisadvElementsNew { get; set; }

        public string ConditionOfFundingEnglish { get; set; }

        public string ConditionOfFundingMaths { get; set; }

        public int? CoreAimSeqNumber { get; set; }

        public Decimal? FullTimeEquiv { get; set; }

        public string FundLine { get; set; }

        public DateTime? LearnerActEndDate { get; set; }

        public DateTime? LearnerPlanEndDate { get; set; }

        public DateTime? LearnerStartDate { get; set; }

        public Decimal? NatRate { get; set; }

        public Decimal? OnProgPayment { get; set; }

        public int? PlannedDaysILCurrYear { get; set; }

        public Decimal? ProgWeightHist { get; set; }

        public Decimal? ProgWeightNew { get; set; }

        public Decimal? PrvDisadvPropnHist { get; set; }

        public Decimal? PrvHistLrgProgPropn { get; set; }

        public Decimal? PrvRetentFactHist { get; set; }

        public string RateBand { get; set; }

        public Decimal? RetentNew { get; set; }

        public bool? StartFund { get; set; }

        public int? ThresholdDays { get; set; }

        public List<LearnerPeriod> LearnerPeriods { get; set; }

        public List<LearnerPeriodisedValues> LearnerPeriodisedValues { get; set; }
    }
}
