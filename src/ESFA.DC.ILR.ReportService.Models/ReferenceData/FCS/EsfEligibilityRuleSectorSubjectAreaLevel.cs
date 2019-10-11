using System;

namespace ESFA.DC.ILR.ReportService.Models.ReferenceData.FCS
{
    public class EsfEligibilityRuleSectorSubjectAreaLevel
    {
        public Decimal? SectorSubjectAreaCode { get; set; }

        public string MinLevelCode { get; set; }

        public string MaxLevelCode { get; set; }
    }
}
