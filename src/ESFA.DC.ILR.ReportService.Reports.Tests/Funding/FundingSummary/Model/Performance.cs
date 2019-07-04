using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model.Interface;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding.FundingSummary.Model
{
    public class Performance
    {
        [Fact]
        public void PerformanceTest()
        {
            var fundingCategory = new FundingCategory("Title", "FCTitle", "CFCTitle", Enumerable.Range(1, 20)
                .Select(k => new FundingSubCategory("SubTitle", "Title", Enumerable.Range(1, 20)
                    .Select(i => new FundLineGroup("FundLineGroup", Enumerable.Range(0, 5)
                        .Select(j => new FundLine(12, "Title", 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12)))))));

            var total = fundingCategory.YearToDate;
        }
    }
}
