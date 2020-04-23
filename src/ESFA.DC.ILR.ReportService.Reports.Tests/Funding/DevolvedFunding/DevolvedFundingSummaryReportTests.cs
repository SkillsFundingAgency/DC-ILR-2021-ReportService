using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aspose.Cells;
using ESFA.DC.ExcelService.Interface;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.DevolvedPostcodes;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Devolved;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Devolved.Model;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Devolved.Model.Interface;
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

            dependsOn.Should().HaveCount(4);

            dependsOn.Should().Contain(DependentDataCatalog.Fm35);
            dependsOn.Should().Contain(DependentDataCatalog.ValidIlr);
            dependsOn.Should().Contain(DependentDataCatalog.ReferenceData);
            dependsOn.Should().Contain(DependentDataCatalog.Eas);
        }

        [Fact]
        public async Task GenerateAsync()
        {
            var container = "Container";

            var devolvedFundingSummaryReportModelBuilderMock = new Mock<IModelBuilder<IEnumerable<DevolvedAdultEducationFundingSummaryReportModel>>>();

            var sofLookup = new McaGlaSofLookup()
                { SofCode = "105", McaGlaShortCode = "ShortName", McaGlaFullName = "FullName" };

            var reportServiceContextMock = new Mock<IReportServiceContext>();
            reportServiceContextMock.Setup(c => c.Container).Returns(container);

            var reportServiceDependentData = Mock.Of<IReportServiceDependentData>();
            var devolvedFundingSummaryReportModel = new List<DevolvedAdultEducationFundingSummaryReportModel> { new DevolvedAdultEducationFundingSummaryReportModel(sofLookup, 1000000, "Provider ABC", "ILR-10000000-1920-20191204-164917-01.xml", "ILR-10000000-1920-20191204-164916-01.xml", DateTime.Now, "EasVersion","OrgVersion", "LarsVersion", "PostcodeVersion", "EmployersVersion", "ApplicationVersion", "ReportGeneratedAt", new List<IDevolvedAdultEducationFundingCategory>())};

            devolvedFundingSummaryReportModelBuilderMock.Setup(b => b.Build(reportServiceContextMock.Object, reportServiceDependentData))
                .Returns(devolvedFundingSummaryReportModel);

            Worksheet worksheet = null;
            Workbook workbook = new Workbook();

            var excelServiceMock = new Mock<IExcelFileService>();

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
            IExcelFileService excelService = null,
            IRenderService<IDevolvedAdultEducationFundingSummaryReport> devolvedFundingSummaryReportRenderService = null)
        {
            return new DevolvedAdultEducationFundingSummaryReport(fileNameService, devolvedFundingSummaryReportBuilder, excelService, devolvedFundingSummaryReportRenderService);
        }
    }
}
