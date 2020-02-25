using ESFA.DC.ILR.ReportService.Models.ReferenceData.Abstract;

namespace ESFA.DC.ILR.ReportService.Models.ReferenceData.LARS
{
    public class LARSLearningDeliveryCategory : AbstractTimeBoundedEntity
    {
        public string LearnAimRef { get; set; }

        public int CategoryRef { get; set; }
    }
}
