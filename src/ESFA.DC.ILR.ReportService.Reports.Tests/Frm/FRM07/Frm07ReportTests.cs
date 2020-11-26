using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Aspose.Cells;
using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Models.ReferenceData;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.LARS;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.MetaData;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.Organisations;
using ESFA.DC.ILR.ReportService.Reports.Frm.FRM07;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Frm.FRM07
{
    public class Frm07ReportTests
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
            var sheetName = "FRM07";
            var fileName = "fileName";
            
            var cancellationToken = CancellationToken.None;

            Workbook workbook = new Workbook();
            Worksheet worksheet = workbook.Worksheets.Add(sheetName);

            var frm07ReportModelBuilderMock = new Mock<IModelBuilder<IEnumerable<Frm07ReportModel>>>();
            var reportServiceContextMock = new Mock<IReportServiceContext>();
            var frm07ReportRenderServiceMock = new Mock<IRenderService<IEnumerable<Frm07ReportModel>>>();

            var reportServiceDependentData = Mock.Of<IReportServiceDependentData>();
            var reportModels = Enumerable.Range(1, 1).Select(x => new Frm07ReportModel()).ToList();

            frm07ReportModelBuilderMock.Setup(b => b.Build(reportServiceContextMock.Object, reportServiceDependentData)).Returns(reportModels);

            var excelServiceMock = new Mock<IExcelService>();

            excelServiceMock.Setup(s => s.NewWorkbook()).Returns(workbook);
            excelServiceMock.Setup(s => s.GetWorksheetFromWorkbook(workbook, sheetName)).Returns(worksheet);

            var report = NewReport(excelServiceMock.Object, frm07ReportModelBuilderMock.Object, frm07ReportRenderServiceMock.Object);
            
            report.Generate(workbook, reportServiceContextMock.Object, reportServiceDependentData, cancellationToken);

            frm07ReportRenderServiceMock.Verify(s => s.Render(reportModels, worksheet));
        }
        
        private Frm07Report NewReport(
            IExcelService excelService = null,
            IModelBuilder<IEnumerable<Frm07ReportModel>> frm07ReportModelBuilder = null,
            IRenderService<IEnumerable<Frm07ReportModel>> frm07ReportRenderService = null)
        {
            return new Frm07Report(excelService, frm07ReportModelBuilder, frm07ReportRenderService);
        }
    }
}
