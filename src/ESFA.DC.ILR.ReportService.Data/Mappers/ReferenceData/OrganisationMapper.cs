using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ReportService.Data.Interface.Mappers.ReferenceData;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.Organisations;

namespace ESFA.DC.ILR.ReportService.Data.Mappers.ReferenceData
{
    public class OrganisationMapper : IOrganisationMapper
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
                PartnerUKPRN = organisation.PartnerUKPRN,
                LegalOrgType = organisation.LegalOrgType,
                CampusIdentifers = organisation.CampusIdentifers?.Select(MapOrganisationCampusIdentifier).ToList(),
                OrganisationFundings = organisation.OrganisationFundings?.Select(MapOrganisationFunding).ToList(),
                OrganisationCoFRemovals = organisation.OrganisationCoFRemovals?.Select(MapOrganisationCoFRemoval).ToList(),
            };
        }

        private OrganisationCampusIdentifier MapOrganisationCampusIdentifier(ReferenceDataService.Model.Organisations.OrganisationCampusIdentifier organisationCampusIdentifier)
        {
            return new OrganisationCampusIdentifier()
            {
                UKPRN = organisationCampusIdentifier.UKPRN,
                CampusIdentifier = organisationCampusIdentifier.CampusIdentifier,
                SpecialistResources = organisationCampusIdentifier.SpecialistResources?.Select(MapSpecialistResource).ToList()
            };
        }

        private SpecialistResource MapSpecialistResource(ReferenceDataService.Model.Organisations.SpecialistResource specialistResource)
        {
            return new SpecialistResource()
            {
                IsSpecialistResource = specialistResource.IsSpecialistResource,
                EffectiveTo = specialistResource.EffectiveTo,
                EffectiveFrom = specialistResource.EffectiveFrom
            };
        }

        private OrganisationFunding MapOrganisationFunding(ReferenceDataService.Model.Organisations.OrganisationFunding organisationFunding)
        {
            return new OrganisationFunding()
            {
                OrgFundFactor = organisationFunding.OrgFundFactor,
                OrgFundFactType = organisationFunding.OrgFundFactType,
                OrgFundFactValue = organisationFunding.OrgFundFactValue,
                EffectiveTo = organisationFunding.EffectiveTo,
                EffectiveFrom = organisationFunding.EffectiveFrom
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
