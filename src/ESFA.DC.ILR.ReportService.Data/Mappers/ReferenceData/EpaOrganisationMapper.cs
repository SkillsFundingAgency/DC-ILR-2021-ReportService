using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.EPA;
using ESFA.DC.ILR.ReportService.Service.Interface.Mappers.ReferenceData;

namespace ESFA.DC.ILR.ReportService.Data.Mappers.ReferenceData
{
    public class EpaOrganisationMapper : IEpaOrganisationMapper
    {
        public IReadOnlyCollection<EPAOrganisation> MapData(IEnumerable<ReferenceDataService.Model.EPAOrganisations.EPAOrganisation> epaOrganisations)
        {
            return epaOrganisations?.Select(MapEpaOrganisation).ToList();
        }

        private EPAOrganisation MapEpaOrganisation(ReferenceDataService.Model.EPAOrganisations.EPAOrganisation epaOrganisation)
        {
            return new EPAOrganisation()
            {
                ID = epaOrganisation.ID,
                Standard = epaOrganisation.Standard,
                EffectiveTo = epaOrganisation.EffectiveTo,
                EffectiveFrom = epaOrganisation.EffectiveFrom
            };
        }
    }
}
