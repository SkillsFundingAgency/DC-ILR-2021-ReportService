using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.Abstract;

namespace ESFA.DC.ILR.ReportService.Models.ReferenceData.Organisations
{
    public class OrganisationCampusIdentifier : AbstractTimeBoundedEntity
    {
        public long UKPRN { get; set; }

        public string CampusIdentifier { get; set; }

        public List<SpecialistResource> SpecialistResources { get; set; }
    }
}
