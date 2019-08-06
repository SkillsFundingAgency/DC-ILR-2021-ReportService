using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Reports.Funding;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Desktop;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.Model;
using ESFA.DC.ILR.ReportService.Reports.Service;
using ESFA.DC.ILR.ReportService.Reports.Tests.Stubs;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding.FundingSummary
{
    public class DesktopFundingSummaryReportTests
    {
        [Fact]
        public void DependsOn()
        {
            var dependsOn = NewReport().DependsOn.ToList();

            dependsOn.Should().HaveCount(5);

            dependsOn.Should().Contain(DependentDataCatalog.Fm25);
            dependsOn.Should().Contain(DependentDataCatalog.Fm35);
            dependsOn.Should().Contain(DependentDataCatalog.Fm36);
            dependsOn.Should().Contain(DependentDataCatalog.Fm81);
            dependsOn.Should().Contain(DependentDataCatalog.Fm99);
        }

        [Fact]
        public async Task SystemTest()
        {
            var container = "Output";

            var reportServiceDependentData = Mock.Of<IReportServiceDependentData>();
            var periodisedValuesLookupProvider = new Mock<IPeriodisedValuesLookupProvider>();

            periodisedValuesLookupProvider.Setup(p => p.Provide(It.IsAny<IEnumerable<FundingDataSources>>(), reportServiceDependentData)).Returns(new PeriodisedValuesLookup());

            var fundingSummaryReportModelBuilder = new DesktopFundingSummaryReportModelBuilder(periodisedValuesLookupProvider.Object);

            var reportServiceContextMock = new Mock<IReportServiceContext>();

            reportServiceContextMock.Setup(c => c.Container).Returns(container);

            var excelService = new ExcelService(new FileServiceStub());

            var fileNameServiceMock = new Mock<IFileNameService>();

            var fileName = "DesktopFundingSummaryReport.xlsx";
            fileNameServiceMock.Setup(s => s.GetFilename(reportServiceContextMock.Object, "Funding Summary Report", OutputTypes.Excel, true)).Returns(fileName);

            var fundingSummaryReportRenderService = new FundingSummaryReportRenderService();

            var report = NewReport(
                fileNameServiceMock.Object,
                fundingSummaryReportModelBuilder,
                excelService,
                fundingSummaryReportRenderService);

            var cancellationToken = CancellationToken.None;

            excelService.ApplyLicense();

            await report.GenerateAsync(reportServiceContextMock.Object, reportServiceDependentData, cancellationToken);
        }

        private DesktopFundingSummaryReport NewReport(
            IFileNameService fileNameService = null,
            IModelBuilder<IFundingSummaryReport> fundingSummaryReportModelBuilder = null,
            IExcelService excelService = null,
            IRenderService<IFundingSummaryReport> fundingSummaryReportRenderService = null)
        {
            return new DesktopFundingSummaryReport(fileNameService, fundingSummaryReportModelBuilder, excelService, fundingSummaryReportRenderService);
        }
    }
}
