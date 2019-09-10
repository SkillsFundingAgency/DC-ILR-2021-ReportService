using System;
using System.Collections.Generic;
using System.Text;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model.LARS;
using ESFA.DC.ILR.ReportService.Reports.Funding.Apprenticeship.Model;
using ESFA.DC.ILR.ReportService.Reports.Model;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Apprenticeship.AppsIndicitave
{
    public class AppsIndicativeEarningsReportModel
    {
        public ILearner Learner { get; set; }

        public ProviderSpecLearnerMonitoringModel ProviderSpecLearnerMonitoring { get; set; }

        public ILearningDelivery LearningDelivery { get; set; }

        public LearningDeliveryFAMsModel LearningDeliveryFAMs { get; set; }

        public LARSLearningDelivery LarsLearningDelivery { get; set; }

        public ProviderSpecDeliveryMonitoringModel ProviderSpecDeliveryMonitoring { get; set; }

        public PeriodisedValuesModel PeriodisedValues { get; set; }

        public ILearnerEmploymentStatus EmploymentStatus { get; set; }

        public LearningDeliveryValues Fm36LearningDelivery { get; set; }

        public PriceEpisodeValues PriceEpisodeValues { get; set; }

        public string LearnRefNumber => Learner?.LearnRefNumber;

        public int? AimSeqNumber => LearningDelivery?.AimSeqNumber ?? 0;

        public string StandardNotionalEndLevel { get; set; }

        public int? EmpStatusMonitoringSmallEmployer { get; set; }

        public DateTime? PriceEpisodeStartDate { get; set; }

        public string FundingLineType { get; set; }

        public decimal TotalPRMPreviousFundingYear { get; set; }

        public decimal TotalPRMThisFundingYear { get; set; }

        public string LearningDeliveryFAMTypeApprenticeshipContractType { get; set; }

        public DateTime? LearningDeliveryFAMTypeACTDateAppliesFrom { get; set; }

        public DateTime? LearningDeliveryFAMTypeACTDateAppliesTo { get; set; }

        public string OfficialSensitive { get; }
    }
}
