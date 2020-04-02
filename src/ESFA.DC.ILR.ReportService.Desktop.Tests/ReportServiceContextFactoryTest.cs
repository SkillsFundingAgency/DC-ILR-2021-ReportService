using System;
using ESFA.DC.ILR.Desktop.Interface;
using ESFA.DC.ILR.ReportService.Desktop.Context;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Desktop.Tests
{
    public class ReportServiceContextFactoryTest
    {
        [Fact]
        public void Factory_Returns_ExpectedType()
        {
            var mockDesktopContext = new Mock<IDesktopContext>();
            mockDesktopContext.Setup(x => x.DateTimeUtc).Returns(new DateTime(2019, 10, 10));
            var sut = new ReportServiceContextFactory();
            var reportServiceContext = sut.Build(mockDesktopContext.Object);
            reportServiceContext.Should().BeOfType<ReportServiceJobContextDesktopContext>();
            reportServiceContext.SubmissionDateTimeUtc.Should().Be(new DateTime(2019, 10, 10));
        }
    }
}
