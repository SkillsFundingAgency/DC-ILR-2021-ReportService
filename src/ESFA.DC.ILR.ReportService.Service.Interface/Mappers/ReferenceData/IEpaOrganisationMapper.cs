using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.EPA;

namespace ESFA.DC.ILR.ReportService.Service.Interface.Mappers.ReferenceData
{
    public interface IEpaOrganisationMapper
    {
        IReadOnlyCollection<EPAOrganisation> MapData(IEnumerable<ReferenceDataService.Model.EPAOrganisations.EPAOrganisation> epaOrganisations);
    }
}
