using ESFA.DC.ILR.ReportService.Reports.Service.Converters;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Service.Converters
{
    public class IlrBooleanConverterTests
    {
        [Fact]
        public void Y()
        {
            NewConverter().ConvertToString(true, null, null).Should().Be("Y");
        }

        [Fact]
        public void N()
        {
            NewConverter().ConvertToString(false, null, null).Should().Be("N");
        }

        [Fact]
        public void Null()
        {
            NewConverter().ConvertToString(null, null, null).Should().BeEmpty();
        }

        [Fact]
        public void NotABoolean()
        {
            NewConverter().ConvertToString("NotABoolean", null, null).Should().BeEmpty();
        }

        private IlrBooleanConverter NewConverter() => new IlrBooleanConverter();
    }
}
