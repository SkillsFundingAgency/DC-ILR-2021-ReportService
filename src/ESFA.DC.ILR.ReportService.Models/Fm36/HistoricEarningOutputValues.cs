using System;

namespace ESFA.DC.ILR.ReportService.Models.Fm36
{
    public class HistoricEarningOutputValues
    {
        public string AppIdentifierOutput { get; set; }

        public bool? AppProgCompletedInTheYearOutput { get; set; }

        public int? HistoricDaysInYearOutput { get; set; }

        public DateTime? HistoricEffectiveTNPStartDateOutput { get; set; }

        public int? HistoricEmpIdEndWithinYearOutput { get; set; }

        public int? HistoricEmpIdStartWithinYearOutput { get; set; }

        public int? HistoricFworkCodeOutput { get; set; }

        public bool? HistoricLearner1618AtStartOutput { get; set; }

        public Decimal? HistoricPMRAmountOutput { get; set; }

        public DateTime? HistoricProgrammeStartDateIgnorePathwayOutput { get; set; }

        public DateTime? HistoricProgrammeStartDateMatchPathwayOutput { get; set; }

        public int? HistoricProgTypeOutput { get; set; }

        public int? HistoricPwayCodeOutput { get; set; }

        public int? HistoricSTDCodeOutput { get; set; }

        public Decimal? HistoricTNP1Output { get; set; }

        public Decimal? HistoricTNP2Output { get; set; }

        public Decimal? HistoricTNP3Output { get; set; }

        public Decimal? HistoricTNP4Output { get; set; }

        public Decimal? HistoricTotal1618UpliftPaymentsInTheYear { get; set; }

        public Decimal? HistoricTotalProgAimPaymentsInTheYear { get; set; }

        public long? HistoricULNOutput { get; set; }

        public DateTime? HistoricUptoEndDateOutput { get; set; }

        public Decimal? HistoricVirtualTNP3EndofThisYearOutput { get; set; }

        public Decimal? HistoricVirtualTNP4EndofThisYearOutput { get; set; }

        public DateTime? HistoricLearnDelProgEarliestACT2DateOutput { get; set; }
    }
}
