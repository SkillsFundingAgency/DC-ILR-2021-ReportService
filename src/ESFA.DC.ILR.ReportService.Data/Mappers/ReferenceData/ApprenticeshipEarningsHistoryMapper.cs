using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ReportService.Data.Interface.Mappers;
using ESFA.DC.ILR.ReportService.Models.ReferenceData;

namespace ESFA.DC.ILR.ReportService.Data.Mappers.ReferenceData
{
    public class ApprenticeshipEarningsHistoryMapper : IMapper<IEnumerable<ReferenceDataService.Model.AppEarningsHistory.ApprenticeshipEarningsHistory>, IReadOnlyCollection<ApprenticeshipEarningsHistory>>
    {
        public IReadOnlyCollection<ApprenticeshipEarningsHistory> MapData(IEnumerable<ReferenceDataService.Model.AppEarningsHistory.ApprenticeshipEarningsHistory> appsEarningsHistory)
        {
            return appsEarningsHistory?.Select(MapApprenticeshipEarningHistory).ToList();
        }

        private ApprenticeshipEarningsHistory MapApprenticeshipEarningHistory(ReferenceDataService.Model.AppEarningsHistory.ApprenticeshipEarningsHistory appsEarningsHistory)
        {
            return new ApprenticeshipEarningsHistory()
            {
                AppIdentifier = appsEarningsHistory.AppIdentifier,
                AppProgCompletedInTheYearInput = appsEarningsHistory.AppProgCompletedInTheYearInput,
                CollectionYear = appsEarningsHistory.CollectionYear,
                CollectionReturnCode = appsEarningsHistory.CollectionReturnCode,
                DaysInYear = appsEarningsHistory.DaysInYear,
                FworkCode = appsEarningsHistory.FworkCode,
                HistoricEffectiveTNPStartDateInput = appsEarningsHistory.HistoricEffectiveTNPStartDateInput,
                HistoricEmpIdEndWithinYear = appsEarningsHistory.HistoricEmpIdEndWithinYear,
                HistoricEmpIdStartWithinYear = appsEarningsHistory.HistoricEmpIdStartWithinYear,
                HistoricLearner1618StartInput = appsEarningsHistory.HistoricLearner1618StartInput,
                HistoricPMRAmount = appsEarningsHistory.HistoricPMRAmount,
                HistoricTNP1Input = appsEarningsHistory.HistoricTNP1Input,
                HistoricTNP2Input = appsEarningsHistory.HistoricTNP2Input,
                HistoricTNP3Input = appsEarningsHistory.HistoricTNP3Input,
                HistoricTNP4Input = appsEarningsHistory.HistoricTNP4Input,
                HistoricTotal1618UpliftPaymentsInTheYearInput = appsEarningsHistory.HistoricTotal1618UpliftPaymentsInTheYearInput,
                HistoricVirtualTNP3EndOfTheYearInput = appsEarningsHistory.HistoricVirtualTNP3EndOfTheYearInput,
                HistoricVirtualTNP4EndOfTheYearInput = appsEarningsHistory.HistoricVirtualTNP4EndOfTheYearInput,
                HistoricLearnDelProgEarliestACT2DateInput = appsEarningsHistory.HistoricLearnDelProgEarliestACT2DateInput,
                LatestInYear = appsEarningsHistory.LatestInYear,
                LearnRefNumber = appsEarningsHistory.LearnRefNumber,
                ProgrammeStartDateIgnorePathway = appsEarningsHistory.ProgrammeStartDateIgnorePathway,
                ProgrammeStartDateMatchPathway = appsEarningsHistory.ProgrammeStartDateMatchPathway,
                ProgType = appsEarningsHistory.ProgType,
                PwayCode = appsEarningsHistory.PwayCode,
                STDCode = appsEarningsHistory.STDCode,
                TotalProgAimPaymentsInTheYear = appsEarningsHistory.TotalProgAimPaymentsInTheYear,
                UptoEndDate = appsEarningsHistory.UptoEndDate,
                UKPRN = appsEarningsHistory.UKPRN,
                ULN = appsEarningsHistory.ULN
            };
        }
    }
}
