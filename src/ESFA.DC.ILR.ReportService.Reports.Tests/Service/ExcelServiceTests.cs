using System;
using Aspose.Cells;
using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.FundingClaim;
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

        [Theory]
        [InlineData("FundingClaim1619ReportTemplate.xlsx")]
        [InlineData("HNSSummaryReportTemplate.xlsx")]
        public void GetTemplateFromWorkbook_Returns_Workbook_For_ValidTemplate(string template)
        {
            var workbook = NewService().GetWorkbookFromTemplate(template);
            workbook.Should().NotBeNull();
            workbook.Worksheets.Count.Should().Be(1);
        }

        [Fact]
        public void GetTemplateFromWorkbook_ThrowsException_For_InvalidTemplate()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => NewService().GetWorkbookFromTemplate("invalid.xlsx"));
            Assert.Equal("Sequence contains no matching element", ex.Message);
        }

        [Fact]
        public void BindExcelTemplateToWorkbook_Returns_Workbook_For_ValidTemplate()
        {
            var workbook = NewService().BindExcelTemplateToWorkbook(new FundingClaimReportModel(), "HNSSummaryReportTemplate.xlsx", "HNSSummary");
            workbook.Should().NotBeNull();
            workbook.Worksheets.Count.Should().Be(1);
        }

        private ExcelService NewService()
        {
            return new ExcelService(null);
        }
    }
}
