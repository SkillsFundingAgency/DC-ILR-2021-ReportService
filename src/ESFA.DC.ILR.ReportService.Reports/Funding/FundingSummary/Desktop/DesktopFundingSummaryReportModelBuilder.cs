using ESFA.DC.ILR.ReportService.Reports.Funding.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Desktop
{
    public class DesktopFundingSummaryReportModelBuilder : FundingSummaryReportModelBuilder
    {
        public DesktopFundingSummaryReportModelBuilder(IPeriodisedValuesLookupProvider periodisedValuesLookupProvider)
            : base(periodisedValuesLookupProvider)
        {
        }
    }
}
