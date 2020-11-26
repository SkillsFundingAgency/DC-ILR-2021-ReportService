using System.Collections.Generic;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Model.Interface
{
    public interface IPeriodisedValuesLookup
    {
        IEnumerable<decimal?[]> GetPeriodisedValues(FundingDataSources fundModel, IEnumerable<string> fundLines, IEnumerable<string> attributes);
    }
}
