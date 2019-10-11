using System;

namespace ESFA.DC.ILR.ReportService.Models.ReferenceData.Abstract
{
    public class AbstractTimeBoundedEntity
    {
        public DateTime EffectiveFrom { get; set; }

        public DateTime? EffectiveTo { get; set; }
    }
}
