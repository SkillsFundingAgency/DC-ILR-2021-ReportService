using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.LARS;

namespace ESFA.DC.ILR.ReportService.Service.Interface.Mappers.ReferenceData
{
    public interface ILarsStandardMapper
    {
        IReadOnlyCollection<LARSStandard> MapData(IEnumerable<ReferenceDataService.Model.LARS.LARSStandard> larsStandards);
    }
}
