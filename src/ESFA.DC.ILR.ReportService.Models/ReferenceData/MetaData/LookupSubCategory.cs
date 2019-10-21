using System;

namespace ESFA.DC.ILR.ReportService.Models.ReferenceData.MetaData
{
    public class LookupSubCategory
    {
        public string Code { get; set; }

        public DateTime? EffectiveFrom { get; set; }

        public DateTime? EffectiveTo { get; set; }
    }
}
