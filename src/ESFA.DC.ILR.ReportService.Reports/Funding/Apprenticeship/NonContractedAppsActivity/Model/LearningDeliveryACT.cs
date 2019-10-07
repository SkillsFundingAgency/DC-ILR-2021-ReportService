using System;
using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Apprenticeship.NonContractedAppsActivity.Model
{
    public class LearningDeliveryACT : ILearningDeliveryFAM
    {
        public string LearnDelFAMType { get; set; }

        public string LearnDelFAMCode { get; set; }

        public DateTime? LearnDelFAMDateFromNullable { get; set; }

        public DateTime? LearnDelFAMDateToNullable { get; set; }
    }
}
