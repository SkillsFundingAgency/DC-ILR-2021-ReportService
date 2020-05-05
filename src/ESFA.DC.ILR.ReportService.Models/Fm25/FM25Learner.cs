using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ReportService.Models.Fm25
{
    public class FM25Learner
    {
        public string LearnRefNumber { get; set; }

        public Decimal? AreaCostFact1618Hist { get; set; }

        public string ConditionOfFundingEnglish { get; set; }

        public string ConditionOfFundingMaths { get; set; }

        public string FundLine { get; set; }

        public DateTime? LearnerActEndDate { get; set; }

        public DateTime? LearnerPlanEndDate { get; set; }

        public DateTime? LearnerStartDate { get; set; }

        public Decimal? NatRate { get; set; }

        public Decimal? OnProgPayment { get; set; }

        public Decimal? ProgWeightHist { get; set; }

        public Decimal? PrvDisadvPropnHist { get; set; }

        public Decimal? PrvHistLrgProgPropn { get; set; }

        public Decimal? PrvRetentFactHist { get; set; }

        public Decimal? PrvHistL3ProgMathEngProp { get; set; }

        public string RateBand { get; set; }

        public bool? StartFund { get; set; }

        public bool? TLevelStudent { get; set; }

        public List<LearnerPeriodisedValues> LearnerPeriodisedValues { get; set; }
    }
}
