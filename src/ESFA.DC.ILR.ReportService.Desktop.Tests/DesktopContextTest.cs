using System.Collections.Generic;
using ESFA.DC.ILR.Desktop.Interface;
using ESFA.DC.ILR.ReportService.Desktop.Context;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Desktop.Tests
{
    public class DesktopContextTest
    {
        [Fact]
        public void FileNameGet()
        {
            Mock<IDesktopContext> mockDesktopContext = new Mock<IDesktopContext>();

            var fileName = "FilenameValue";

            var keyValuePairs = new Dictionary<string, object>
            {
                { "Filename", fileName },
            };

            mockDesktopContext.SetupGet(x => x.KeyValuePairs).Returns(keyValuePairs);

            var context = NewContext(mockDesktopContext.Object);

            context.Filename.Should().Be(fileName);
        }

        [Fact]
        public void FileNameSet()
        {
            Mock<IDesktopContext> mockDesktopContext = new Mock<IDesktopContext>();

            var fileName = "FilenameValue";
            var fileNameKey = "Filename";

            var keyValuePairs = new Dictionary<string, object>();


            mockDesktopContext.SetupGet(x => x.KeyValuePairs).Returns(keyValuePairs);

            var context = NewContext(mockDesktopContext.Object);

            context.Filename = fileName;

            keyValuePairs[fileNameKey].Should().Be(fileName);
        }

        private ReportServiceJobContextDesktopContext NewContext(IDesktopContext desktopContext)
        {
            return new ReportServiceJobContextDesktopContext(desktopContext);
        }
    }
}
