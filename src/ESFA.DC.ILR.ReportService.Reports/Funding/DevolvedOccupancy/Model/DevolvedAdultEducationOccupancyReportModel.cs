using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model.LARS;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.DevolvedOccupancy.Model
{
    public class DevolvedAdultEducationOccupancyReportModel
    {
        public ILearner Learner { get; set; }

        public ILearningDelivery LearningDelivery { get; set; }

        public LearningDelivery Fm35LearningDelivery { get; set; }

        public LARSLearningDelivery LarsLearningDelivery { get; set; }
    }
}
