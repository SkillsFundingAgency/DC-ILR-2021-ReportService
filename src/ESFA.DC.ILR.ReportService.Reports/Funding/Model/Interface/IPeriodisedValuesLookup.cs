using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Model.Interface
{
    public interface IPeriodisedValuesLookup
    {
        IEnumerable<decimal?[]> GetPeriodisedValues(FundingDataSources fundModel, IEnumerable<string> fundLines, IEnumerable<string> attributes);
    }
}
