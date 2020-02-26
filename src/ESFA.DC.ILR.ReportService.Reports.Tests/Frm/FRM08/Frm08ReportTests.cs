using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Aspose.Cells;
using ESFA.DC.ILR.ReportService.Reports.Frm.FRM08;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Frm.FRM08
{
    public class Frm08ReportTests
    {
        [Fact]
        public void DependsOn()
        {
            var dependsOn = NewReport().DependsOn.ToList();

            dependsOn.Should().HaveCount(2);

            dependsOn.Should().Contain(DependentDataCatalog.ReferenceData);
            dependsOn.Should().Contain(DependentDataCatalog.ValidIlr);
        }

        [Fact]
        public void GenerateAsync()
        {
            var container = "Container";
            var sheetName = "FRM08";
            var fileName = "fileName";
            
            var cancellationToken = CancellationToken.None;

            Workbook workbook = new Workbook();
            Worksheet worksheet = workbook.Worksheets.Add(sheetName);

            var frm08ReportModelBuilderMock = new Mock<IModelBuilder<IEnumerable<Frm08ReportModel>>>();
            var reportServiceContextMock = new Mock<IReportServiceContext>();
            var frm08ReportRenderServiceMock = new Mock<IRenderService<IEnumerable<Frm08ReportModel>>>();

            var reportServiceDependentData = Mock.Of<IReportServiceDependentData>();
            var reportModels = Enumerable.Range(1, 1).Select(x => new Frm08ReportModel()).ToList();

            frm08ReportModelBuilderMock.Setup(b => b.Build(reportServiceContextMock.Object, reportServiceDependentData)).Returns(reportModels);

            var excelServiceMock = new Mock<IExcelService>();

            excelServiceMock.Setup(s => s.NewWorkbook()).Returns(workbook);
            excelServiceMock.Setup(s => s.GetWorksheetFromWorkbook(workbook, sheetName)).Returns(worksheet);

            var report = NewReport(excelServiceMock.Object, frm08ReportModelBuilderMock.Object, frm08ReportRenderServiceMock.Object);
            
            report.Generate(workbook, reportServiceContextMock.Object, reportServiceDependentData, cancellationToken);

            frm08ReportRenderServiceMock.Verify(s => s.Render(reportModels, worksheet));
        }

        private Frm08Report NewReport(
            IExcelService excelService = null,
            IModelBuilder<IEnumerable<Frm08ReportModel>> frm08ReportModelBuilder = null,
            IRenderService<IEnumerable<Frm08ReportModel>> frm08ReportRenderService = null)
        {
            return new Frm08Report(excelService, frm08ReportModelBuilder, frm08ReportRenderService);
        }
    }
}
