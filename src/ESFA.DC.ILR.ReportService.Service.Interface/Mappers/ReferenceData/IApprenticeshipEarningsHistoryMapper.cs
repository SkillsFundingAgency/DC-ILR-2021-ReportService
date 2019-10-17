using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Models.ReferenceData;

namespace ESFA.DC.ILR.ReportService.Service.Interface.Mappers.ReferenceData
{
    public interface IApprenticeshipEarningsHistoryMapper
    {
        IReadOnlyCollection<ApprenticeshipEarningsHistory> MapData(IEnumerable<ReferenceDataService.Model.AppEarningsHistory.ApprenticeshipEarningsHistory> appsEarningsHistory);
    }
}
