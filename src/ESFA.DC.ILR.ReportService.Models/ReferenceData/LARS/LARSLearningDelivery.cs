using System;
using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.Abstract;

namespace ESFA.DC.ILR.ReportService.Models.ReferenceData.LARS
{
    public class LARSLearningDelivery : AbstractTimeBoundedEntity
    {
        public string LearnAimRef { get; set; }

        public string LearnAimRefTitle { get; set; }

        public string LearnAimRefTypeDesc { get; set; }

        public string NotionalNVQLevel { get; set; }

        public string NotionalNVQLevelv2 { get; set; }

        public int? FrameworkCommonComponent { get; set; }

        public Decimal? SectorSubjectAreaTier2 { get; set; }

        public IReadOnlyCollection<LARSLearningDeliveryCategory> LARSLearningDeliveryCategories { get; set; }
    }
}
