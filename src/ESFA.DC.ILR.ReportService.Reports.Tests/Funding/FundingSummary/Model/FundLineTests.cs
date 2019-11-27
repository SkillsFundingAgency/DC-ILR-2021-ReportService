using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding.FundingSummary.Model
{
    public class FundLineTests
    {
        [Fact]
        public void SumPeriods()
        {
            var fundLine = new FundLine(
                5,
                "FundLine",
                1,
                2,
                3,
                4,
                5,
                6,
                7,
                8,
                9,
                10, 
                11,
                12);

            fundLine.Period1To8.Should().Be(36);
            fundLine.Period9To12.Should().Be(42);
            fundLine.YearToDate.Should().Be(15);
            fundLine.Total.Should().Be(78);
        }
    }
}
