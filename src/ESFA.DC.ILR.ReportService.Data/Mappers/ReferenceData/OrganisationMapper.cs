using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ReportService.Data.Interface.Mappers;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.Organisations;

namespace ESFA.DC.ILR.ReportService.Data.Mappers.ReferenceData
{
    public class OrganisationMapper : IMapper<IEnumerable<ReferenceDataService.Model.Organisations.Organisation>, IReadOnlyCollection<Organisation>>
    {
        public IReadOnlyCollection<Organisation> MapData(IEnumerable<ReferenceDataService.Model.Organisations.Organisation> organisations)
        {
            return organisations?.Select(MapOrganisation).ToList();
        }

        private Organisation MapOrganisation(ReferenceDataService.Model.Organisations.Organisation organisation)
        {
            return new Organisation()
            {
                UKPRN = organisation.UKPRN,
                Name = organisation.Name,
                OrganisationCoFRemovals = organisation.OrganisationCoFRemovals?.Select(MapOrganisationCoFRemoval).ToList(),
            };
        }

        private OrganisationCoFRemoval MapOrganisationCoFRemoval(ReferenceDataService.Model.Organisations.OrganisationCoFRemoval organisationCoFRemoval)
        {
            return new OrganisationCoFRemoval()
            {
                CoFRemoval = organisationCoFRemoval.CoFRemoval,
                EffectiveTo = organisationCoFRemoval.EffectiveTo,
                EffectiveFrom = organisationCoFRemoval.EffectiveFrom
            };
        }
    }
}
