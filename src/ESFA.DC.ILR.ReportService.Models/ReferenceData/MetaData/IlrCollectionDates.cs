using System.Collections.Generic;

namespace ESFA.DC.ILR.ReportService.Models.ReferenceData.MetaData
{
    public class IlrCollectionDates
    {
        public IReadOnlyCollection<ReturnPeriod> ReturnPeriods { get; set; }

        public IReadOnlyCollection<CensusDate> CensusDates { get; set; }
    }
}
