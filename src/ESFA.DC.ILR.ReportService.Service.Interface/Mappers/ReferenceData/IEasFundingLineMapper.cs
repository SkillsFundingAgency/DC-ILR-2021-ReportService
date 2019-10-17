using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.EAS;

namespace ESFA.DC.ILR.ReportService.Service.Interface.Mappers.ReferenceData
{
    public interface IEasFundingLineMapper
    {
        IReadOnlyCollection<EasFundingLine> MapData(IEnumerable<ReferenceDataService.Model.EAS.EasFundingLine> fundingLine);
    }
}
