using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Aspose.Cells;
using ESFA.DC.ILR.ReportService.Reports.Funding.DevolvedFundingSummary;
using ESFA.DC.ILR.ReportService.Reports.Funding.DevolvedFundingSummary.Model;
using ESFA.DC.ILR.ReportService.Reports.Funding.DevolvedFundingSummary.Model.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding.DevolvedFunding
{
    public class DevolvedFundingSummaryReportTests
    {
        [Fact]
        public void DependsOn()
        {
            var dependsOn = NewReport().DependsOn.ToList();

            dependsOn.Should().HaveCount(1);

            dependsOn.Should().Contain(DependentDataCatalog.Fm35);
        }

        [Fact]
        public async Task GenerateAsync()
        {
            var container = "Container";

            var devolvedFundingSummaryReportModelBuilderMock = new Mock<IModelBuilder<IEnumerable<DevolvedAdultEducationFundingSummaryReportModel>>>();

            var reportServiceContextMock = new Mock<IReportServiceContext>();
            reportServiceContextMock.Setup(c => c.Container).Returns(container);

            var reportServiceDependentData = Mock.Of<IReportServiceDependentData>();
            var devolvedFundingSummaryReportModel = new List<DevolvedAdultEducationFundingSummaryReportModel> { new DevolvedAdultEducationFundingSummaryReportModel("105", 1000000, "Provider ABC", "ILR-10000000-1920-20191204-164917-01.xml", "ILR-10000000-1920-20191204-164916-01.xml", new List<IDevolvedAdultEducationFundingCategory>())};

            devolvedFundingSummaryReportModelBuilderMock.Setup(b => b.Build(reportServiceContextMock.Object, reportServiceDependentData)).Returns(devolvedFundingSummaryReportModel);

            Workbook workbook = null;
            Worksheet worksheet = null;

            var excelServiceMock = new Mock<IExcelService>();

            excelServiceMock.Setup(s => s.NewWorkbook()).Returns(workbook);
            excelServiceMock.Setup(s => s.GetWorksheetFromWorkbook(workbook, 0)).Returns(worksheet);

            var fileNameServiceMock = new Mock<IFileNameService>();

            var fileName = "FileName";
            fileNameServiceMock.Setup(s => s.GetFilename(reportServiceContextMock.Object, "Devolved Adult Education Funding Summary Report", OutputTypes.Excel, true)).Returns(fileName);

            var devolvedFundingSummaryReportRenderServiceMock = new Mock<IRenderService<IDevolvedAdultEducationFundingSummaryReport>>();

            var report = NewReport(fileNameServiceMock.Object, devolvedFundingSummaryReportModelBuilderMock.Object, excelServiceMock.Object, devolvedFundingSummaryReportRenderServiceMock.Object);

            var cancellationToken = CancellationToken.None;

            await report.GenerateAsync(reportServiceContextMock.Object, reportServiceDependentData, cancellationToken);

            excelServiceMock.Verify(s => s.SaveWorkbookAsync(workbook, fileName, container, cancellationToken));
            devolvedFundingSummaryReportRenderServiceMock.Verify(s => s.Render(devolvedFundingSummaryReportModel.First(), worksheet));
        }

        private DevolvedAdultEducationFundingSummaryReport NewReport(
            IFileNameService fileNameService = null,
            IModelBuilder<IEnumerable<DevolvedAdultEducationFundingSummaryReportModel>> devolvedFundingSummaryReportBuilder = null,
            IExcelService excelService = null,
            IRenderService<IDevolvedAdultEducationFundingSummaryReport> devolvedFundingSummaryReportRenderService = null)
        {
            return new DevolvedAdultEducationFundingSummaryReport(fileNameService, devolvedFundingSummaryReportBuilder, excelService, devolvedFundingSummaryReportRenderService);
        }
    }
}
