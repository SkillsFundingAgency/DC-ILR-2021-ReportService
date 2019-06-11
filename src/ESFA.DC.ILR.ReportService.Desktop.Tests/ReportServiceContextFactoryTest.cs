using ESFA.DC.ILR.Desktop.Interface;
using ESFA.DC.ILR.ReportService.Desktop.Context.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Desktop.Tests
{
    public class ReportServiceContextFactoryTest
    {
        [Fact]
        public void FactoryNotNull()
        {
            var desktopContext = new Mock<IDesktopContext>();
            var mockDesktopContextFactory = new Mock<IReportServiceContextFactory>();
            mockDesktopContextFactory.Setup(x => x.Build(desktopContext.Object));
            mockDesktopContextFactory.Should().NotBeNull();
        }
    }
}
