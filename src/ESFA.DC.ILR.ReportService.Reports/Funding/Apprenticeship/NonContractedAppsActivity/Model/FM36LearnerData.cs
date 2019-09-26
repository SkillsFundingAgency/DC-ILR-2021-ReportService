using System;
using System.Collections;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Reports.Model;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Apprenticeship.NonContractedAppsActivity.Model
{
    public class FM36LearnerData
    {
        public ILearner Learner { get; set; }

        public ProviderSpecLearnerMonitoringModel ProviderSpecLearnerMonitoringModels { get; set; }

        public string LearnRefNumber => Learner.LearnRefNumber;

        public long ULN => Learner.ULN;

        public DateTime? DateOfBirth => Learner?.DateOfBirthNullable;

        public string CampusIdentifier => Learner?.CampId;

        public ICollection<FM36LearningDeliveryData> LearningDeliveries { get; set; }
    }
}
