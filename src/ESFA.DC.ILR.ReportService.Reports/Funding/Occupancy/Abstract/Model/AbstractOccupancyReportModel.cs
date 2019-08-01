using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model.LARS;
using ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Devolved.Model;
using ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Interface;
using ESFA.DC.ILR.ReportService.Reports.Model;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Abstract.Model
{
    public abstract class AbstractOccupancyReportModel : IOrderableOccupancyReportModel
    {
        public ILearner Learner { get; set; }

        public ProviderSpecLearnerMonitoringModel ProviderSpecLearnerMonitoring { get; set; }

        public ILearningDelivery LearningDelivery { get; set; }

        public ProviderSpecDeliveryMonitoringModel ProviderSpecDeliveryMonitoring { get; set; }

        public LearningDeliveryFAMsModel LearningDeliveryFAMs { get; set; }

        public LARSLearningDelivery LarsLearningDelivery { get; set; }

        public LearningDeliveryPeriodisedValuesModel PeriodisedValues { get; set; }

        public string LearnRefNumber => Learner?.LearnRefNumber;

        public int AimSeqNumber => LearningDelivery?.AimSeqNumber ?? 0;
    }
}
