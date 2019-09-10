using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Aspose.Cells;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReferenceDataService.Model.MetaData;
using ESFA.DC.ILR.ReferenceDataService.Model.MetaData.ReferenceDataVersions;
using ESFA.DC.ILR.ReferenceDataService.Model.Organisations;
using ESFA.DC.ILR.ReportService.Reports.Funding;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.Model;
using ESFA.DC.ILR.ReportService.Reports.Model.Interface;
using ESFA.DC.ILR.ReportService.Reports.Service;
using ESFA.DC.ILR.ReportService.Reports.Tests.Stubs;
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

            Workbook workbook = new Workbook();
            Worksheet worksheet = workbook.Worksheets.Add("Test");

            var excelServiceMock = new Mock<IExcelService>();

            excelServiceMock.Setup(s => s.NewWorkbook()).Returns(workbook);
            excelServiceMock.Setup(s => s.GetWorksheetFromWorkbook(workbook, 0)).Returns(worksheet);

            var fileNameServiceMock = new Mock<IFileNameService>();

            var fileName = "FileName";
            fileNameServiceMock.Setup(s => s.GetFilename(reportServiceContextMock.Object, "Funding Summary Report", OutputTypes.Excel, true)).Returns(fileName);

            var fundingSummaryReportRenderServiceMock = new Mock<IRenderService<IFundingSummaryReport>>();
            var summaryPageRenderServiceMock = new Mock<IRenderService<ISummaryPage>>();
            
            var report = NewReport(fileNameServiceMock.Object, fundingSummaryReportModelBuilderMock.Object, excelServiceMock.Object, fundingSummaryReportRenderServiceMock.Object, summaryPageRenderServiceMock.Object);

            var cancellationToken = CancellationToken.None;

            await report.GenerateAsync(reportServiceContextMock.Object, reportServiceDependentData, cancellationToken);
            
            excelServiceMock.Verify(s => s.SaveWorkbookAsync(workbook, fileName, container, cancellationToken));
            fundingSummaryReportRenderServiceMock.Verify(s => s.Render(fundingSummaryReportModel, worksheet));
        }

        [Fact]
        public async Task SystemTest()
        {
            var container = "Output";
            var ukprn = 123456789;
            var reportGeneratedAt = new DateTime(2019, 03, 03);

            var refDataRoot = new ReferenceDataRoot()
            {
                Organisations = new List<Organisation>()
                {
                    new Organisation()
                    {
                        UKPRN = ukprn,
                        Name = "OrganisationName"
                    }
                },
                MetaDatas = new MetaData()
                {
                    ReferenceDataVersions = new ReferenceDataVersion()
                    {
                        OrganisationsVersion = new OrganisationsVersion("orgVersion"),
                        EasUploadDateTime = new EasUploadDateTime(new DateTime(2019, 01, 01)),
                        LarsVersion = new LarsVersion("larsVersion"),
                        PostcodesVersion = new PostcodesVersion("postcodeVersion"),
                        Employers = new EmployersVersion("employersVersion")
                    }
                }
            };

            var message = new Message()
            {
                Header = new MessageHeader()
                {
                    CollectionDetails = new MessageHeaderCollectionDetails()
                    {
                        FilePreparationDate = new DateTime(2019, 01, 02)
                    }
                }
            };

            var reportServiceDependentDataMock = new Mock<IReportServiceDependentData>();
            reportServiceDependentDataMock.Setup(x => x.Get<ReferenceDataRoot>()).Returns(refDataRoot);
            reportServiceDependentDataMock.Setup(x => x.Get<IMessage>()).Returns(message);

            var periodisedValuesLookupProvider = new Mock<IPeriodisedValuesLookupProvider>();

            periodisedValuesLookupProvider.Setup(p => p.Provide(It.IsAny<IEnumerable<FundingDataSources>>(), reportServiceDependentDataMock.Object)).Returns(new PeriodisedValuesLookup());

            var dateTimeProviderMock = new Mock<IDateTimeProvider>();
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(reportGeneratedAt);
            dateTimeProviderMock.Setup(x => x.ConvertUtcToUk(reportGeneratedAt)).Returns(reportGeneratedAt);

            var fundingSummaryReportModelBuilder = new FundingSummaryReportModelBuilder(periodisedValuesLookupProvider.Object, dateTimeProviderMock.Object);

            var reportServiceContextMock = new Mock<IReportServiceContext>();
            reportServiceContextMock.Setup(c => c.Container).Returns(container);
            reportServiceContextMock.Setup(c => c.Ukprn).Returns(ukprn);
            reportServiceContextMock.Setup(c => c.OriginalFilename).Returns("ILR-11111111-1920-20190507-152000-01");
            reportServiceContextMock.Setup(c => c.ServiceReleaseVersion).Returns("ServiceReleaseVersion");

            var excelService = new ExcelService(new FileServiceStub());

            var fileNameServiceMock = new Mock<IFileNameService>();

            var fileName = "FundingSummaryReport.xlsx";
            fileNameServiceMock.Setup(s => s.GetFilename(reportServiceContextMock.Object, "Funding Summary Report", OutputTypes.Excel, true)).Returns(fileName);

            var fundingSummaryReportRenderService = new FundingSummaryReportRenderService();
            var summaryPageRenderService = new SummaryPageRenderService();

            var report = NewReport(
                fileNameServiceMock.Object,
                fundingSummaryReportModelBuilder,
                excelService,
                fundingSummaryReportRenderService,
                summaryPageRenderService);

            var cancellationToken = CancellationToken.None;

            excelService.ApplyLicense();
            
            await report.GenerateAsync(reportServiceContextMock.Object, reportServiceDependentDataMock.Object, cancellationToken);
        }

        private FundingSummaryReport NewReport(
            IFileNameService fileNameService = null,
            IModelBuilder<IFundingSummaryReport> fundingSummaryReportModelBuilder = null,
            IExcelService excelService = null,
            IRenderService<IFundingSummaryReport> fundingSummaryReportRenderService = null,
            IRenderService<ISummaryPage> summaryPageRenderService = null)
        {
            return new FundingSummaryReport(fileNameService, fundingSummaryReportModelBuilder, excelService, fundingSummaryReportRenderService, summaryPageRenderService);
        }
    }
}
