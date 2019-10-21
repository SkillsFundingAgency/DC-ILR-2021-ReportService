using System;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Models.Fm36;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.LARS;
using ESFA.DC.ILR.ReportService.Reports.Model;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Apprenticeship.Abstract.Model
{
    public abstract class AbstractAppsReportModel
    {
        public ILearner Learner { get; set; }

        public ProviderSpecLearnerMonitoringModel ProviderSpecLearnerMonitoring { get; set; }

        public ILearningDelivery LearningDelivery { get; set; }

        public LearningDeliveryFAMsModel LearningDeliveryFAMs { get; set; }

        public LARSLearningDelivery LarsLearningDelivery { get; set; }

        public ProviderSpecDeliveryMonitoringModel ProviderSpecDeliveryMonitoring { get; set; }

        public LearningDeliveryValues Fm36LearningDelivery { get; set; }

        public PriceEpisodeValues PriceEpisodeValues { get; set; }

        public string LearnRefNumber => Learner?.LearnRefNumber;

        public int? AimSeqNumber => LearningDelivery?.AimSeqNumber ?? 0;

        public DateTime? PriceEpisodeStartDate { get; set; }

        public string FundingLineType { get; set; }

        public string OfficialSensitive { get; }
    }
}
