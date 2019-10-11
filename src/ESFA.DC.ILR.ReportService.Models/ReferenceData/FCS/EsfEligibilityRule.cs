using System.Collections.Generic;

namespace ESFA.DC.ILR.ReportService.Models.ReferenceData.FCS
{
    public class EsfEligibilityRule
    {
        public bool? Benefits { get; set; }

        public int? CalcMethod { get; set; }

        public string TenderSpecReference { get; set; }

        public string LotReference { get; set; }

        public int? MinAge { get; set; }

        public int? MaxAge { get; set; }

        public int? MinLengthOfUnemployment { get; set; }

        public int? MaxLengthOfUnemployment { get; set; }

        public string MinPriorAttainment { get; set; }

        public string MaxPriorAttainment { get; set; }

        public List<EsfEligibilityRuleEmploymentStatus> EmploymentStatuses { get; set; }

        public List<EsfEligibilityRuleLocalAuthority> LocalAuthorities { get; set; }

        public List<EsfEligibilityRuleLocalEnterprisePartnership> LocalEnterprisePartnerships { get; set; }

        public List<EsfEligibilityRuleSectorSubjectAreaLevel> SectorSubjectAreaLevels { get; set; }
    }
}
