using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.LARS;

namespace ESFA.DC.ILR.ReportService.Data.Interface.Mappers.ReferenceData
{
    public interface ILarsLearningDeliveryMapper
    {
        IReadOnlyCollection<LARSLearningDelivery> MapData(IEnumerable<ReferenceDataService.Model.LARS.LARSLearningDelivery> larsLearningDeliveries);
    }
}
