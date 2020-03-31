using System.Collections.Generic;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Devolved.Model.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.Model.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Devolved.Desktop
{
    public class DesktopDevolvedAdultEducationFundingSummaryModelBuilder : DevolvedAdultEducationFundingSummaryReportModelBuilder
    {
        public DesktopDevolvedAdultEducationFundingSummaryModelBuilder(IDateTimeProvider dateTimeProvider, IAcademicYearService academicYearService) 
            : base(dateTimeProvider, academicYearService)
        {
        }

        protected override IDevolvedAdultEducationFundLineGroup BuildEasFm35FundLineGroup(int currentPeriod, IEnumerable<string> fundLines, IPeriodisedValuesLookup periodisedValues) => null;
    }
}
