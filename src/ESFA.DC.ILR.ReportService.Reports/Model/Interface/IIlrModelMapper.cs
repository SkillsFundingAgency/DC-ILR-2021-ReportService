using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Model.Interface
{
    public interface IIlrModelMapper
    {
        ProviderSpecLearnerMonitoringModel MapProviderSpecLearnerMonitorings(IEnumerable<IProviderSpecLearnerMonitoring> monitorings);

        ProviderSpecDeliveryMonitoringModel MapProviderSpecDeliveryMonitorings(IEnumerable<IProviderSpecDeliveryMonitoring> monitorings);

        LearningDeliveryFAMsModel MapLearningDeliveryFAMs(IEnumerable<ILearningDeliveryFAM> learningDeliveryFams);

    }
}
