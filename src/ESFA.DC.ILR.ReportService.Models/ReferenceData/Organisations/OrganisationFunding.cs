using ESFA.DC.ILR.ReportService.Models.ReferenceData.Abstract;

namespace ESFA.DC.ILR.ReportService.Models.ReferenceData.Organisations
{
    public class OrganisationFunding : AbstractTimeBoundedEntity
    {
        public string OrgFundFactor { get; set; }

        public string OrgFundFactType { get; set; }

        public string OrgFundFactValue { get; set; }
    }
}
