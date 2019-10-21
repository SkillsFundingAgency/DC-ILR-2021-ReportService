using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ReportService.Data.Interface.Mappers;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.LARS;

namespace ESFA.DC.ILR.ReportService.Data.Mappers.ReferenceData
{
    public class LarsStandardMapper : IMapper<IEnumerable<ReferenceDataService.Model.LARS.LARSStandard>, IReadOnlyCollection<LARSStandard>>
    {
        public IReadOnlyCollection<LARSStandard> MapData(IEnumerable<ReferenceDataService.Model.LARS.LARSStandard> larsStandards)
        {
            return larsStandards?.Select(MapLarsStandard).ToList();
        }

        private LARSStandard MapLarsStandard(ReferenceDataService.Model.LARS.LARSStandard larsStandard)
        {
            return new LARSStandard()
            {
                StandardCode = larsStandard.StandardCode,
                NotionalEndLevel = larsStandard.NotionalEndLevel,
                EffectiveFrom = larsStandard.EffectiveFrom,
                EffectiveTo = larsStandard.EffectiveTo,
            };
        }
    }
}
