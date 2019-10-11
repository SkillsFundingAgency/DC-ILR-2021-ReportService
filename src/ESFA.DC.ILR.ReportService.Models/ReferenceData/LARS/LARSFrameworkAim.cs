using ESFA.DC.ILR.ReportService.Models.ReferenceData.Abstract;

namespace ESFA.DC.ILR.ReportService.Models.ReferenceData.LARS
{
    public class LARSFrameworkAim : AbstractTimeBoundedEntity
    {
        public string LearnAimRef { get; set; }

        public int? FrameworkComponentType { get; set; }
    }
}
