using Aspose.Cells;
using ESFA.DC.ExcelService.Interface;
using ESFA.DC.ILR.ReportService.Reports.Frm;
using ESFA.DC.ILR.ReportService.Reports.Frm.Model.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Frm.Summary
{
    public class FrmSummaryReportTests
    {
        [Fact]
        public async Task GenerateAsync()
        {
            //Data
            var sheetName = "Summary";
            var fileName = "FileName";
            Workbook workbook = new Workbook();
            Worksheet worksheet = workbook.Worksheets.Add(sheetName);
            List<IFrmWorksheetReport> list = new List<IFrmWorksheetReport>();

            //Mocks
            var fundingSummaryReportModelBuilderMock = new Mock<IModelBuilder<IFrmSummaryReport>>();
            var reportServiceContextMock = new Mock<IReportServiceContext>();
            var reportServiceDependentData = Mock.Of<IReportServiceDependentData>();
            var fundingSummaryReportModel = Mock.Of<IFrmSummaryReport>();
            var excelServiceMock = new Mock<IExcelFileService>();
            var fileNameServiceMock = new Mock<IFileNameService>();
            var fundingSummaryReportRenderServiceMock = new Mock<IRenderService<IFrmSummaryReport>>();
            var cancellationToken = CancellationToken.None;

            //Setup
            fundingSummaryReportModelBuilderMock.Setup(b => b.Build(reportServiceContextMock.Object, reportServiceDependentData)).Returns(fundingSummaryReportModel);
            fileNameServiceMock.Setup(s => s.GetFilename(reportServiceContextMock.Object, "Frm Summary Report", OutputTypes.Excel, true)).Returns(fileName);
            excelServiceMock.Setup(s => s.NewWorkbook()).Returns(workbook);
            excelServiceMock.Setup(s => s.GetWorksheetFromWorkbook(workbook, sheetName)).Returns(worksheet);

            await NewReport(list, fileNameServiceMock.Object, excelServiceMock.Object, fundingSummaryReportModelBuilderMock.Object, fundingSummaryReportRenderServiceMock.Object).GenerateAsync(reportServiceContextMock.Object, reportServiceDependentData, cancellationToken);

            fundingSummaryReportModelBuilderMock.Verify(x => x.Build(reportServiceContextMock.Object, reportServiceDependentData));
            fundingSummaryReportRenderServiceMock.Verify(s => s.Render(fundingSummaryReportModel, worksheet));
        }
        private FrmReport NewReport(IList<IFrmWorksheetReport> frmReports = null,
            IFileNameService fileNameService = null,
            IExcelFileService excelFileService = null,
            IModelBuilder<IFrmSummaryReport> modelBuilder = null,
            IRenderService<IFrmSummaryReport> renderService = null)
        {
            return new FrmReport(frmReports, fileNameService, excelFileService, modelBuilder, renderService);
        }

    }
}
