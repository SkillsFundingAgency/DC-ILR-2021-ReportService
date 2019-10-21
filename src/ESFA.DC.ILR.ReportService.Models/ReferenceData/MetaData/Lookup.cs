﻿using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ReportService.Models.ReferenceData.MetaData
{
    public class Lookup
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public DateTime? EffectiveFrom { get; set; }

        public DateTime? EffectiveTo { get; set; }

        public List<LookupSubCategory> SubCategories { get; set; }
    }
}
