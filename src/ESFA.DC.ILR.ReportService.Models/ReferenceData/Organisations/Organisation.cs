using System.Collections.Generic;

namespace ESFA.DC.ILR.ReportService.Models.ReferenceData.Organisations
{
    public class Organisation
    {
        public int UKPRN { get; set; }

        public string Name { get; set; }

        public bool? PartnerUKPRN { get; set; }

        public string LegalOrgType { get; set; }

        public List<OrganisationCampusIdentifier> CampusIdentifers { get; set; }

        public List<OrganisationFunding> OrganisationFundings { get; set; }

        public List<OrganisationCoFRemoval> OrganisationCoFRemovals { get; set; }
    }
}
