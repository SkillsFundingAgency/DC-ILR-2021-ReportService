using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model.LARS;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Devolved.Model
{
    public class DevolvedAdultEducationOccupancyReportModel
    {
        public ILearner Learner { get; set; }

        public ProviderSpecLearnerMonitoringModel ProviderSpecLearnerMonitoring { get; set; }

        public ILearningDelivery LearningDelivery { get; set; }

        public ProviderSpecDeliveryMonitoringModel ProviderSpecDeliveryMonitoring { get; set; }

        public LearningDeliveryFAMsModel LearningDeliveryFAMs { get; set; }
        
        public LearningDeliveryValue Fm35LearningDelivery { get; set; }

        public LearningDeliveryPeriodisedValuesModel PeriodisedValues { get; set; }

        public LARSLearningDelivery LarsLearningDelivery { get; set; }

        public string McaGlaShortCode { get; set; }
    }
}
