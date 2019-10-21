using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ReportService.Data.Interface.Mappers;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.LARS;

namespace ESFA.DC.ILR.ReportService.Data.Mappers.ReferenceData
{
    public class LarsLearningDeliveryMapper : IMapper<IEnumerable<ReferenceDataService.Model.LARS.LARSLearningDelivery>, IReadOnlyCollection<LARSLearningDelivery>>
    {
        public IReadOnlyCollection<LARSLearningDelivery> MapData(IEnumerable<ReferenceDataService.Model.LARS.LARSLearningDelivery> larsLearningDeliveries)
        {
            return larsLearningDeliveries?.Select(MapLarsLearningDelivery).ToList();
        }

        private LARSLearningDelivery MapLarsLearningDelivery(ReferenceDataService.Model.LARS.LARSLearningDelivery larsLearningDelivery)
        {
            return new LARSLearningDelivery()
            {
                LearnAimRef = larsLearningDelivery.LearnAimRef,
                LearnAimRefTitle = larsLearningDelivery.LearnAimRefTitle,
                NotionalNVQLevel = larsLearningDelivery.NotionalNVQLevel,
                NotionalNVQLevelv2 = larsLearningDelivery.NotionalNVQLevelv2,
                FrameworkCommonComponent = larsLearningDelivery.FrameworkCommonComponent,
                SectorSubjectAreaTier2 = larsLearningDelivery.SectorSubjectAreaTier2,
            };
        }
    }
}
