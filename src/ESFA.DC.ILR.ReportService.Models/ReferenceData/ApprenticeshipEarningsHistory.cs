using System;

namespace ESFA.DC.ILR.ReportService.Models.ReferenceData
{
    public class ApprenticeshipEarningsHistory
    {
        public string AppIdentifier { get; set; }

        public bool? AppProgCompletedInTheYearInput { get; set; }

        public string CollectionYear { get; set; }

        public string CollectionReturnCode { get; set; }

        public int? DaysInYear { get; set; }

        public int? FworkCode { get; set; }

        public DateTime? HistoricEffectiveTNPStartDateInput { get; set; }

        public long? HistoricEmpIdEndWithinYear { get; set; }

        public long? HistoricEmpIdStartWithinYear { get; set; }

        public bool? HistoricLearner1618StartInput { get; set; }

        public Decimal? HistoricPMRAmount { get; set; }

        public Decimal? HistoricTNP1Input { get; set; }

        public Decimal? HistoricTNP2Input { get; set; }

        public Decimal? HistoricTNP3Input { get; set; }

        public Decimal? HistoricTNP4Input { get; set; }

        public Decimal? HistoricTotal1618UpliftPaymentsInTheYearInput { get; set; }

        public Decimal? HistoricVirtualTNP3EndOfTheYearInput { get; set; }

        public Decimal? HistoricVirtualTNP4EndOfTheYearInput { get; set; }

        public DateTime? HistoricLearnDelProgEarliestACT2DateInput { get; set; }

        public bool LatestInYear { get; set; }

        public string LearnRefNumber { get; set; }

        public DateTime? ProgrammeStartDateIgnorePathway { get; set; }

        public DateTime? ProgrammeStartDateMatchPathway { get; set; }

        public int? ProgType { get; set; }

        public int? PwayCode { get; set; }

        public int? STDCode { get; set; }

        public Decimal? TotalProgAimPaymentsInTheYear { get; set; }

        public DateTime? UptoEndDate { get; set; }

        public int UKPRN { get; set; }

        public long ULN { get; set; }
    }
}
