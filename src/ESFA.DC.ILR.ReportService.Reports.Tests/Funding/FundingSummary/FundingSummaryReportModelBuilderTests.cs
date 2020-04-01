using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary;
using ESFA.DC.ILR.ReportService.Reports.Funding.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding.FundingSummary
{
    public class FundingSummaryReportModelBuilderTests
    {
        private FundingSummaryReportModelBuilder NewBuilder(IPeriodisedValuesLookupProvider periodisedValuesLookupProvider = null, IDateTimeProvider dateTimeProvider = null)
        {
            return new FundingSummaryReportModelBuilder(periodisedValuesLookupProvider, dateTimeProvider);
        }
    }
}
