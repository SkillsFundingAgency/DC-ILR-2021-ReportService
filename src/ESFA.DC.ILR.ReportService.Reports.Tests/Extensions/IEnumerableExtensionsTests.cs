using System;
using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Reports.Extensions;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Extensions
{
    public class IEnumerableExtensionsTests
    {
        [Fact]
        public void ToFixedLengthArray_Smaller()
        {
            var input = new List<int>() {  1, 2, 3 };

            var output = input.ToFixedLengthArray(2);

            output.Should().HaveCount(2);
            output[0].Should().Be(1);
            output[1].Should().Be(2);
        }

        [Fact]
        public void ToFixedLengthArray_Larger()
        {
            var input = new List<int>() { 1, 2, 3 };

            var output = input.ToFixedLengthArray(5);

            output.Should().HaveCount(5);
            output[0].Should().Be(1);
            output[1].Should().Be(2);
            output[2].Should().Be(3);
            output[3].Should().Be(0);
            output[4].Should().Be(0);
        }

        [Fact]
        public void ToFixedLengthArray_Null()
        {
            IEnumerable<int> nullEnumerable = null;

            var output = nullEnumerable.ToFixedLengthArray(3);

            output.Should().HaveCount(3);
            output[0].Should().Be(0);
            output[1].Should().Be(0);
            output[2].Should().Be(0);
        }

        [Fact]
        public void ToFixedLengthArray_String()
        {
            var input = new List<string>() { "string" };

            var output = input.ToFixedLengthArray(2);

            output.Should().HaveCount(2);
            output[0].Should().Be("string");
            output[1].Should().BeNull();

        }

        [Fact]
        public void MinOrDefault_Null()
        {
            IEnumerable<int> nullEnumerable = null;

            Action action = () => nullEnumerable.MinOrDefault(i => i);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void MinOrDefault_Empty()
        {
            IEnumerable<int> emptyEnumerable = new List<int>();

            emptyEnumerable.MinOrDefault(i => i).Should().Be(0);
        }

        [Fact]
        public void MinOrDefault()
        {
            IEnumerable<int> enumerable = new List<int>() { 1, 2, 3, };

            enumerable.MinOrDefault(i => i).Should().Be(1);
        }

        [Fact]
        public void MinOrDefault_Nullable()
        {
            IEnumerable<int?> enumerable = new List<int?>();

            enumerable.MinOrDefault(i => i).Should().Be(null);
        }


        [Fact]
        public void MaxOrDefault_Null()
        {
            IEnumerable<int> nullEnumerable = null;

            Action action = () => nullEnumerable.MaxOrDefault(i => i);
            Action actionNoFunc = () => nullEnumerable.MaxOrDefault();

            action.Should().Throw<ArgumentNullException>();
            actionNoFunc.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void MaxOrDefault_Empty()
        {
            IEnumerable<int> emptyEnumerable = new List<int>();

            emptyEnumerable.MaxOrDefault(i => i).Should().Be(0);
            emptyEnumerable.MaxOrDefault().Should().Be(0);
        }

        [Fact]
        public void MaxOrDefault()
        {
            IEnumerable<int> enumerable = new List<int>() { 1, 2, 3, };

            enumerable.MaxOrDefault(i => i).Should().Be(3);
            enumerable.MaxOrDefault().Should().Be(3);
        }

        [Fact]
        public void MaxOrDefault_Nullable()
        {
            IEnumerable<int?> enumerable = new List<int?>();

            enumerable.MaxOrDefault(i => i).Should().BeNull();
            enumerable.MaxOrDefault().Should().BeNull();
        }
    }
}
