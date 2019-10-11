using ESFA.DC.ILR.ReportService.Models.ReferenceData.LARS.Abstract;

namespace ESFA.DC.ILR.ReportService.Models.ReferenceData.LARS
{
    public class LARSStandardApprenticeshipFunding : AbstractLARSApprenticeshipFunding
    {
        public int ProgType { get; set; }

        public int? PwayCode { get; set; }
    }
}
