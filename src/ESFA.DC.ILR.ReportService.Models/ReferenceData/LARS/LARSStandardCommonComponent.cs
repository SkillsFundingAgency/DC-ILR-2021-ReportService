using ESFA.DC.ILR.ReportService.Models.ReferenceData.Abstract;

namespace ESFA.DC.ILR.ReportService.Models.ReferenceData.LARS
{
    public class LARSStandardCommonComponent : AbstractTimeBoundedEntity
    {
        public int CommonComponent { get; set; }
    }
}
