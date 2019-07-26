using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding.FundingSummary.Model
{
    public class FundingSubCategoryTests
    {
        [Fact]
        public void WithFundLineGroup()
        {
            var fundLineGroupOne = Mock.Of<IFundLineGroup>();
            var fundLineGroupTwo = Mock.Of<IFundLineGroup>();

            var fundingSubCategory = NewSubCategory()
                .WithFundLineGroup(fundLineGroupOne)
                .WithFundLineGroup(fundLineGroupTwo);

            fundingSubCategory.FundLineGroups.Should().HaveCount(2);
            fundingSubCategory.FundLineGroups[0].Should().BeSameAs(fundLineGroupOne);
            fundingSubCategory.FundLineGroups[1].Should().BeSameAs(fundLineGroupTwo);
        }

        [Fact]
        public void WithFundLineGroup_Null()
        {
            var fundingSubCategory = NewSubCategory()
                .WithFundLineGroup(null);

            fundingSubCategory.FundLineGroups.Should().BeEmpty();
        }

        private FundingSubCategory NewSubCategory(string title = "Title", int currentPeriod = 1)
        {
            return new FundingSubCategory(title, currentPeriod);
        }
    }
}
