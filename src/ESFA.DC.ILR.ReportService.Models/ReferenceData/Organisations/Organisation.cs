using System.Collections.Generic;

namespace ESFA.DC.ILR.ReportService.Models.ReferenceData.Organisations
{
    public class Organisation
    {
        public int UKPRN { get; set; }

        public string Name { get; set; }

        public List<OrganisationCoFRemoval> OrganisationCoFRemovals { get; set; }
    }
}
