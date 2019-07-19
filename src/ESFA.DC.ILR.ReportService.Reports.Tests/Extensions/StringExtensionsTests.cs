using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Service.Extensions;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Extensions
{
    public class StringExtensionsTests
    {
        [Theory]
        [InlineData("A", "a")]
        [InlineData("a", "a")]
        [InlineData("A", "A")]
        [InlineData("a", "A")]
        [InlineData(null, null)]
        public void CaseInsensitiveEquals_True(string left, string right)
        {
            left.CaseInsensitiveEquals(right).Should().BeTrue();
        }

        [Theory]
        [InlineData("a", "b")]
        [InlineData("a", null)]
        [InlineData(null, "a")]
        public void CaseInsensitiveEquals_False(string left, string right)
        {
            left.CaseInsensitiveEquals(right).Should().BeFalse();
        }
    }
}
