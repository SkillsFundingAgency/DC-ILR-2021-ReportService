using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Reports.Extensions;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Extensions
{
    public class IDictionaryExtensionsTests
    {
        [Fact]
        public void GetValueOrDefault_Match()
        {
            var dictionary = new Dictionary<int, int>()
            {
                [1] = 2,
                [2] = 1,
            };

            dictionary.GetValueOrDefault(1).Should().Be(2);
        }

        [Fact]
        public void GetValueOrDefault_NoMatch()
        {
            var dictionary = new Dictionary<int, int>()
            {
                [1] = 2,
                [2] = 1,
            };

            dictionary.GetValueOrDefault(3).Should().Be(0);
        }

        [Fact]
        public void GetValueOrDefault_Nullable()
        {
            var dictionary = new Dictionary<int, int?>()
            {
                [1] = 2,
                [2] = 1,
            };

            dictionary.GetValueOrDefault(3).Should().BeNull();
        }
    }
}
