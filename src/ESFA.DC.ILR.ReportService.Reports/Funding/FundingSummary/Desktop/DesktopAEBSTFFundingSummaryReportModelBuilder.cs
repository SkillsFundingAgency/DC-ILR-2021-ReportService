using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.Model.Interface;
using System.Collections.Generic;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.AEBSTF;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Desktop
{
    public class DesktopAEBSTFFundingSummaryReportModelBuilder : AEBSTFFundingSummaryReportModelBuilder
    {
        public DesktopAEBSTFFundingSummaryReportModelBuilder(IPeriodisedValuesLookupProvider periodisedValuesLookupProvider, IDateTimeProvider dateTimeProvider)
            : base(periodisedValuesLookupProvider, dateTimeProvider)
        {
            FundingDataSources = new[]
            {
                Funding.FundingDataSources.FM35
            };
        }

        protected override IFundLineGroup BuildEasStfiFundLineGroup(string description, int currentPeriod, IEnumerable<string> fundLines, IPeriodisedValuesLookup periodisedValues) => null;
    }
}
