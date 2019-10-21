using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.Organisations;

namespace ESFA.DC.ILR.ReportService.Data.Interface.Mappers.ReferenceData
{
    public interface IOrganisationMapper
    {
        IReadOnlyCollection<Organisation> MapData(IEnumerable<ReferenceDataService.Model.Organisations.Organisation> organisations);
    }
}
