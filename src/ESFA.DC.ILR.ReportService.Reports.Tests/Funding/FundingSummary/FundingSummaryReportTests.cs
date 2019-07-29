using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Aspose.Cells;
using ESFA.DC.ILR.ReportService.Reports.Funding;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.Model.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding.FundingSummary
{
    public class FundingSummaryReportTests
    {
        [Fact]
        public void DependsOn()
        {
            var dependsOn = NewReport().DependsOn.ToList();

            dependsOn.Should().HaveCount(6);

            dependsOn.Should().Contain(DependentDataCatalog.Fm25);
            dependsOn.Should().Contain(DependentDataCatalog.Fm35);
            dependsOn.Should().Contain(DependentDataCatalog.Fm36);
            dependsOn.Should().Contain(DependentDataCatalog.Fm81);
            dependsOn.Should().Contain(DependentDataCatalog.Fm99);
            dependsOn.Should().Contain(DependentDataCatalog.ReferenceData);
        }

        [Fact]
        public async Task GenerateAsync()
        {
            var container = "Container";

            var fundingSummaryReportModelBuilderMock = new Mock<IModelBuilder<IFundingSummaryReport>>();

            var reportServiceContextMock = new Mock<IReportServiceContext>();

            reportServiceContextMock.Setup(c => c.Container).Returns(container);

            var reportServiceDependentData = Mock.Of<IReportServiceDependentData>();
            var fundingSummaryReportModel = Mock.Of<IFundingSummaryReport>();

            fundingSummaryReportModelBuilderMock.Setup(b => b.Build(reportServiceContextMock.Object, reportServiceDependentData)).Returns(fundingSummaryReportModel);

            Workbook workbook = null;
            Worksheet worksheet = null;

            var excelServiceMock = new Mock<IExcelService>();

            excelServiceMock.Setup(s => s.NewWorkbook()).Returns(workbook);
            excelServiceMock.Setup(s => s.GetWorksheetFromWorkbook(workbook, 0)).Returns(worksheet);

            var fileNameServiceMock = new Mock<IFileNameService>();

            var fileName = "FileName";
            fileNameServiceMock.Setup(s => s.GetFilename(reportServiceContextMock.Object, "Funding Summary Report", OutputTypes.Excel, true)).Returns(fileName);

            var fundingSummaryReportRenderServiceMock = new Mock<IRenderService<IFundingSummaryReport>>();
            
            var report = NewReport(fileNameServiceMock.Object, fundingSummaryReportModelBuilderMock.Object, excelServiceMock.Object, fundingSummaryReportRenderServiceMock.Object);

            var cancellationToken = CancellationToken.None;

            await report.GenerateAsync(reportServiceContextMock.Object, reportServiceDependentData, cancellationToken);
            
            excelServiceMock.Verify(s => s.SaveWorkbookAsync(workbook, fileName, container, cancellationToken));
            fundingSummaryReportRenderServiceMock.Verify(s => s.Render(fundingSummaryReportModel, worksheet));
        }

        private FundingSummaryReport NewReport(
            IFileNameService fileNameService = null,
            IModelBuilder<IFundingSummaryReport> fundingSummaryReportModelBuilder = null,
            IExcelService excelService = null,
            IRenderService<IFundingSummaryReport> fundingSummaryReportRenderService = null)
        {
            return new FundingSummaryReport(fileNameService, fundingSummaryReportModelBuilder, excelService, fundingSummaryReportRenderService);
        }
    }
}
