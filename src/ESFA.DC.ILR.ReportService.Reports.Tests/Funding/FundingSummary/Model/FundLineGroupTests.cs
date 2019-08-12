using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Reports.Funding;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model;
using ESFA.DC.ILR.ReportService.Reports.Funding.Model;
using ESFA.DC.ILR.ReportService.Reports.Funding.Model.Interface;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding.FundingSummary.Model
{
    public class FundLineGroupTests
    {
        [Fact]
        public void BuildFundLine()
        {
            var title = "title";
            var currentPeriod = 3;

            var fundModel = FundingDataSources.FM35;
            var fundLine = new[] { "fundLine" };
            var attribute1 = "attribute1";
            var attribute2 = "attribute2";

            var missing = "missing";
            var attributes = new[] { attribute1, attribute2, missing };

            var dictionary = new PeriodisedValuesLookup()
            {
                [fundModel] = new Dictionary<string, Dictionary<string, decimal?[][]>>()
                {
                    [fundLine[0]] = new Dictionary<string, decimal?[][]>()
                    {
                        [attribute1] = new decimal?[][]
                        {
                            new decimal?[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                            new decimal?[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                            new decimal?[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                            new decimal?[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                            new decimal?[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                        },
                        [attribute2] = new decimal?[][]
                        {
                            new decimal?[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                        }
                    }
                }
            };

            var result = NewGroup(title, currentPeriod, fundModel, fundLine, dictionary).BuildFundLine(title, attributes);

            result.Period1.Should().Be(6);
            result.Period2.Should().Be(6);
            result.Period3.Should().Be(6);
            result.Period4.Should().Be(6);
            result.Period5.Should().Be(6);
            result.Period6.Should().Be(6);
            result.Period7.Should().Be(6);
            result.Period8.Should().Be(6);
            result.Period9.Should().Be(6);
            result.Period10.Should().Be(6);
            result.Period11.Should().Be(6);
            result.Period12.Should().Be(6);
            result.Period1To8.Should().Be(48);
            result.Period9To12.Should().Be(24);
            result.Total.Should().Be(72);
            result.YearToDate.Should().Be(18);
        }

        [Fact]
        public void BuildFundLine_Performance()
        {
            var title = "title";
            var currentPeriod = 3;

            var fundModel = FundingDataSources.FM35;
            var fundLine = new[] { "fundLine" };
            var attribute1 = "attribute1";

            var attributes = new[] { attribute1, };

            var dictionary = new PeriodisedValuesLookup()
            {
                [fundModel] = new Dictionary<string, Dictionary<string, decimal?[][]>>()
                {
                    [fundLine[0]] = new Dictionary<string, decimal?[][]>()
                    {
                        [attribute1] = Enumerable.Range(0, 1000000)
                            .Select(i => new decimal?[] { 1, null, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 })
                            .ToArray()
                    }
                }
            };

            var result = NewGroup("FundGroupTitle", currentPeriod, fundModel, fundLine, dictionary).BuildFundLine(title, attributes);

            result.Period1.Should().Be(1000000);
            result.Period2.Should().Be(0);
        }
        
        [Fact]
        public void BuildFundLine_NotIncludedInTotals_Zero()
        {
            var title = "title";
            var currentPeriod = 3;

            var fundModel = FundingDataSources.FM35;
            var fundLine = new[] { "fundLine" };
            var attribute1 = "attribute1";

            var attributes = new[] { attribute1, };

            var dictionary = new PeriodisedValuesLookup()
            {
                [fundModel] = new Dictionary<string, Dictionary<string, decimal?[][]>>()
                {
                    [fundLine[0]] = new Dictionary<string, decimal?[][]>()
                    {
                        [attribute1] = Enumerable.Range(0, 100)
                            .Select(i => new decimal?[] { 1, null, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 })
                            .ToArray()
                    }
                }
            };

            var result = NewGroup("FundGroupTitle", currentPeriod, fundModel, fundLine, dictionary).WithFundLine(title, attributes, false);

            result.Period1.Should().Be(0);
            result.Period2.Should().Be(0);
            result.Total.Should().Be(0);
        }

        [Fact]
        public void BuildFundLine_NotIncludedInTotals_Mix()
        {
            var title = "title";
            var currentPeriod = 3;

            var fundModel = FundingDataSources.FM35;
            var fundLine = new[] { "fundLine" };
            var attribute1 = "attribute1";

            var attributes = new[] { attribute1, };

            var dictionary = new PeriodisedValuesLookup()
            {
                [fundModel] = new Dictionary<string, Dictionary<string, decimal?[][]>>()
                {
                    [fundLine[0]] = new Dictionary<string, decimal?[][]>()
                    {
                        [attribute1] = Enumerable.Range(0, 100)
                            .Select(i => new decimal?[] { 1, null, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 })
                            .ToArray()
                    }
                }
            };

            var result = NewGroup("FundGroupTitle", currentPeriod, fundModel, fundLine, dictionary)
                .WithFundLine(title, attributes, false)
                .WithFundLine(title, attributes, true);

            result.Period1.Should().Be(100);
            result.Period2.Should().Be(0);
            result.Total.Should().Be(1100);
        }

        [Fact]
        public void WithFundLine_Instance()
        {
            var group = NewGroup();

            group.WithFundLine("title", new List<string>()).Should().BeSameAs(group);
        }

        private FundLineGroup NewGroup(string title = "Title", int currentPeriod = 12, FundingDataSources fundModel = FundingDataSources.FM35, IEnumerable<string> fundLines = null, IPeriodisedValuesLookup periodisedValues = null)
        {
            return new FundLineGroup(title, currentPeriod, fundModel, fundLines ?? new List<string>(), periodisedValues ?? new PeriodisedValuesLookup());
        }
    }
}
