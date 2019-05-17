
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ReportService.Model.PeriodEnd.NonContractedAppsActivity
{
    public class NonContractedAppsActivityLearningDeliveryInfo
    {
        public int UKPRN { get; set; }

        public string LearnRefNumber { get; set; }

        public string LearnAimRef { get; set; }

        public int AimType { get; set; }

        public int AimSeqNumber { get; set; }

        public string SWSupAimId { get; set; }

        public DateTime LearnStartDate { get; set; }

        public int? ProgType { get; set; }

        public int? FworkCode { get; set; }

        public int? PwayCode { get; set; }

        public int? StdCode { get; set; }

        public DateTime? OriginalLearnStartDate { get; set; }

        public DateTime LearningPlannedEndDate { get; set; }

        public DateTime? LearnActualEndDate { get; set; }

        public ICollection<NonContractedAppsActivityProviderSpecDeliveryMonitoringInfo> ProviderSpecDeliveryMonitorings { get; set; }

        public ICollection<NonContractedAppsActivityLearningDeliveryFAMInfo> LearningDeliveryFams { get; set; }

        public string EPAOrganisation { get; set; }
        public int? PartnerUkPrn { get; set; }
       
    }
}