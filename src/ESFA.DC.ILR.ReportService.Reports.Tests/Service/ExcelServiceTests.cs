using Aspose.Cells;
using ESFA.DC.ILR.ReportService.Reports.Service;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Service
{
    public class ExcelServiceTests
    {
        [Fact]
        public void ApplyLicense()
        {
            var workbook = new Workbook();

            workbook.IsLicensed.Should().BeFalse();

            NewService().ApplyLicense();

            workbook.IsLicensed.Should().BeTrue();
        }

        private ExcelService NewService()
        {
            return new ExcelService(null);
        }
    }
}
