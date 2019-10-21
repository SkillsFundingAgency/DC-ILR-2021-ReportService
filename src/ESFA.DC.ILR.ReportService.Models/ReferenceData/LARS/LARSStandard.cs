using ESFA.DC.ILR.ReportService.Models.ReferenceData.Abstract;

namespace ESFA.DC.ILR.ReportService.Models.ReferenceData.LARS
{
    public class LARSStandard : AbstractTimeBoundedEntity
    {
        public int StandardCode { get; set; }

        public string NotionalEndLevel { get; set; }
    }
}
