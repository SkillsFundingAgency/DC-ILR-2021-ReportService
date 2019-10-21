using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ReportService.Models.ReferenceData.LARS.Abstract
{
    public class AbstractLARSFramework
    {
        public int FworkCode { get; set; }

        public int ProgType { get; set; }

        public int PwayCode { get; set; }

        public DateTime? EffectiveFromNullable { get; set; }

        public DateTime? EffectiveTo { get; set; }

        public List<LARSFrameworkCommonComponent> LARSFrameworkCommonComponents { get; set; }

        public List<LARSFrameworkApprenticeshipFunding> LARSFrameworkApprenticeshipFundings { get; set; }
    }
}
