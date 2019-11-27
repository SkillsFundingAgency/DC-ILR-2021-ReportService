using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Reports.Funding.Model.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Interface
{
    public interface IPeriodisedValuesLookupProvider
    {
        IPeriodisedValuesLookup Provide(IEnumerable<FundingDataSources> fundingDataSources, IReportServiceDependentData reportServiceDependentData);
    }
}
