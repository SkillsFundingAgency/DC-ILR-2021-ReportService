using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ReportService.Model.PeriodEnd.NonContractedAppsActivity
{
    public class NonContractedAppsActivityLearnerInfo
    {
        public string LearnRefNumber { get; set; }

        public string CampId { get; set; }

        public long UniqueLearnerNumber { get; set; }

        public DateTime DateOfBirth { get; set; }

        public ICollection<NonContractedAppsActivityLearningDeliveryInfo> LearningDeliveries { get; set; }

        public ICollection<NonContractedAppsActivityProviderSpecLearnerMonitoringInfo> ProviderSpecLearnerMonitorings { get; set; }

    }
}