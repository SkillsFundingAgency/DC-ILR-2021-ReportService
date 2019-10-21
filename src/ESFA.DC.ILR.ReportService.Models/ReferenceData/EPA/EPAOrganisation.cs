using ESFA.DC.ILR.ReportService.Models.ReferenceData.Abstract;

namespace ESFA.DC.ILR.ReportService.Models.ReferenceData.EPA
{
    public class EPAOrganisation : AbstractTimeBoundedEntity
    {
        public string ID { get; set; }

        public string Standard { get; set; }
    }
}
