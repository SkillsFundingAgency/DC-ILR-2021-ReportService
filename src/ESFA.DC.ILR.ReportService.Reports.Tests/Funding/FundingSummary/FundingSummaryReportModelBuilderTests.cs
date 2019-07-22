using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ReportService.Reports.Funding;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary;
using ESFA.DC.ILR.ReportService.Reports.Funding.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.Model;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding.FundingSummary
{
    public class FundingSummaryReportModelBuilderTests
    {
        private FundingSummaryReportModelBuilder NewBuilder(IPeriodisedValuesLookupProvider periodisedValuesLookupProvider = null)
        {
            return new FundingSummaryReportModelBuilder(periodisedValuesLookupProvider);
        }
    }
}
