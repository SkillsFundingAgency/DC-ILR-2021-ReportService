using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.Model.Interface;
using System.Collections.Generic;
using ESFA.DC.DateTimeProvider.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Desktop
{
    public class DesktopFundingSummaryReportModelBuilder : FundingSummaryReportModelBuilder
    {
        public DesktopFundingSummaryReportModelBuilder(IPeriodisedValuesLookupProvider periodisedValuesLookupProvider, IDateTimeProvider dateTimeProvider)
            : base(periodisedValuesLookupProvider, dateTimeProvider)
        {
            FundingDataSources = new[]
            {
                Funding.FundingDataSources.FM25,
                Funding.FundingDataSources.FM35,
                Funding.FundingDataSources.FM36,
                Funding.FundingDataSources.FM81,
                Funding.FundingDataSources.FM99,
            };
        }

        protected override IFundLineGroup BuildEasFm35FundLineGroup(string ageRange, string description, int currentPeriod, IEnumerable<string> fundLines, IPeriodisedValuesLookup periodisedValues) => null;

        protected override IFundLineGroup BuildEasAuthorisedClaimsExcessLearningSupportFundLineGroup(string ageRange, string description, int currentPeriod, IEnumerable<string> fundLines, IPeriodisedValuesLookup periodisedValues) => null;

        protected override IFundLineGroup BuildEasNonLevyApprenticeshipsFundLineGroup(string ageRange, int currentPeriod, IEnumerable<string> fundLines, IPeriodisedValuesLookup periodisedValues) => null;

        protected override IFundLineGroup BuildEasFm25FundLineGroup(int currentPeriod, IPeriodisedValuesLookup periodisedValues) => null;

        protected override IFundLineGroup BuildEasAebFundLineGroup(string ageRange, string description, int currentPeriod, IEnumerable<string> fundModels, IPeriodisedValuesLookup periodisedValues) => null;
        
        protected override IFundLineGroup BuildEasFm99FundLineGroup(int currentPeriod, IPeriodisedValuesLookup periodisedValues) => null;

        protected override IFundLineGroup BuildEasLevyApprenticeshipsFundLineGroup(string ageRange, string description, int currentPeriod, IEnumerable<string> fundLines, IPeriodisedValuesLookup periodisedValues) => null;

        protected override IFundLineGroup BuildEasCovidFundLineGroup(string ageRange, string description, int currentPeriod, IEnumerable<string> fundModels, IPeriodisedValuesLookup periodisedValues) => null;
    }
}
