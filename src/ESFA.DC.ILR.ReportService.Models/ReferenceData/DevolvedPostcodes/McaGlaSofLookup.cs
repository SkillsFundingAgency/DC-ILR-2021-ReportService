using ESFA.DC.ILR.ReportService.Models.ReferenceData.Abstract;

namespace ESFA.DC.ILR.ReportService.Models.ReferenceData.DevolvedPostcodes
{
    public class McaGlaSofLookup : AbstractTimeBoundedEntity
    {
        public string SofCode { get; set; }

        public string McaGlaShortCode { get; set; }

        public string McaGlaFullName { get; set; }
    }
}
