using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Aspose.Cells;
using ESFA.DC.ILR.ReportService.Reports.Frm.FRM06;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Frm.FRM06
{
    public class Frm06ReportTests
    {
        [Fact]
        public void DependsOn()
        {
            var dependsOn = NewReport().DependsOn.ToList();

            dependsOn.Should().HaveCount(3);

            dependsOn.Should().Contain(DependentDataCatalog.Frm);
            dependsOn.Should().Contain(DependentDataCatalog.ValidIlr);
            dependsOn.Should().Contain(DependentDataCatalog.ReferenceData);
        }

        [Fact]
        public void GenerateAsync()
        {
            var sheetName = "FRM06";
            
            var cancellationToken = CancellationToken.None;

            Workbook workbook = new Workbook();
            Worksheet worksheet = workbook.Worksheets.Add(sheetName);

            var frm06ReportModelBuilderMock = new Mock<IModelBuilder<IEnumerable<Frm06ReportModel>>>();
            var reportServiceContextMock = new Mock<IReportServiceContext>();
            var frm06ReportRenderServiceMock = new Mock<IRenderService<IEnumerable<Frm06ReportModel>>>();

            var reportServiceDependentData = Mock.Of<IReportServiceDependentData>();
            var reportModels = Enumerable.Range(1, 1).Select(x => new Frm06ReportModel()).ToList();

            frm06ReportModelBuilderMock.Setup(b => b.Build(reportServiceContextMock.Object, reportServiceDependentData)).Returns(reportModels);

            var excelServiceMock = new Mock<IExcelService>();

            excelServiceMock.Setup(s => s.NewWorkbook()).Returns(workbook);
            excelServiceMock.Setup(s => s.GetWorksheetFromWorkbook(workbook, sheetName)).Returns(worksheet);

            var report = NewReport(excelServiceMock.Object, frm06ReportModelBuilderMock.Object, frm06ReportRenderServiceMock.Object);
            
            report.Generate(workbook, reportServiceContextMock.Object, reportServiceDependentData, cancellationToken);

            frm06ReportRenderServiceMock.Verify(s => s.Render(reportModels, worksheet));
        }

        private Frm06Report NewReport(
            IExcelService excelService = null,
            IModelBuilder<IEnumerable<Frm06ReportModel>> frm06ReportModelBuilder = null,
            IRenderService<IEnumerable<Frm06ReportModel>> frm06ReportRenderService = null)
        {
            return new Frm06Report(excelService, frm06ReportModelBuilder, frm06ReportRenderService);
        }
    }
}
