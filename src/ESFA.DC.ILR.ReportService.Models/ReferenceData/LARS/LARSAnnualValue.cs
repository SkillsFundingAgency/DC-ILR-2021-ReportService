using System;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.Abstract;

namespace ESFA.DC.ILR.ReportService.Models.ReferenceData.LARS
{
    public class LARSAnnualValue : AbstractTimeBoundedEntity
    {
        public string LearnAimRef { get; set; }

        public int? BasicSkills { get; set; }

        public int? BasicSkillsType { get; set; }

        public int? FullLevel2EntitlementCategory { get; set; }

        public int? FullLevel3EntitlementCategory { get; set; }

        public Decimal? FullLevel2Percent { get; set; }

        public Decimal? FullLevel3Percent { get; set; }
    }
}
