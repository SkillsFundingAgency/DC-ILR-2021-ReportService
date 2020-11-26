using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aspose.Cells;
using ESFA.DC.ExcelService.Interface;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Funding.CommunityLearning;
using ESFA.DC.ILR.ReportService.Reports.Funding.CommunityLearning.Model;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding.CommunityLearning
{
    public class CommunityLearningReportTests
    {
        [Fact]
        public void DependsOn()
        {
            var dependsOn = NewReport().DependsOn.ToList();

            dependsOn.Should().HaveCount(2);
            dependsOn.Should().Contain(DependentDataCatalog.ValidIlr);
            dependsOn.Should().Contain(DependentDataCatalog.ReferenceData);
        }

        [Fact]
        public async Task GenerateAsync()
        {
            var container = "Container";
            var sheetName = "CommunityLearningReport";

            var communityLearningReportModelBuilderMock = new Mock<IModelBuilder<CommunityLearningReportModel>>();

            var reportServiceContextMock = new Mock<IReportServiceContext>();

            reportServiceContextMock.Setup(c => c.Container).Returns(container);

            var reportServiceDependentData = Mock.Of<IReportServiceDependentData>();
            var communityLearningReportModel = Mock.Of<CommunityLearningReportModel>();

            communityLearningReportModelBuilderMock.Setup(b => b.Build(reportServiceContextMock.Object, reportServiceDependentData)).Returns(communityLearningReportModel);

            Workbook workbook = new Workbook();
            Worksheet worksheet = workbook.Worksheets.Add(sheetName);

            var excelServiceMock = new Mock<IExcelFileService>();

            var fileNameServiceMock = new Mock<IFileNameService>();

            var fileName = "FileName";
            fileNameServiceMock.Setup(s => s.GetFilename(reportServiceContextMock.Object, "CL Summary of Learners by Non-Single Budget Category Report", OutputTypes.Excel, true)).Returns(fileName);
            
            var report = NewReport(fileNameServiceMock.Object, communityLearningReportModelBuilderMock.Object, excelServiceMock.Object);

            var cancellationToken = CancellationToken.None;

            await report.GenerateAsync(reportServiceContextMock.Object, reportServiceDependentData, cancellationToken);

            excelServiceMock.VerifyAll();
        }

        private CommunityLearningReport NewReport(
            IFileNameService fileNameService = null,
            IModelBuilder<CommunityLearningReportModel> communityLearningReportModelBuilder = null,
            IExcelFileService excelService = null)
        {
            return new CommunityLearningReport(fileNameService, communityLearningReportModelBuilder, excelService);
        }
    }
}
