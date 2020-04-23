using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aspose.Cells;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ExcelService.Interface;
using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Models.ReferenceData;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.MetaData;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.Organisations;
using ESFA.DC.ILR.ReportService.Reports.Funding;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Desktop;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.Model;
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
                        OrganisationsVersion = new OrganisationsVersion { Version = "orgVersion" },
                        EasUploadDateTime = new EasUploadDateTime { UploadDateTime = new DateTime(2019, 01, 01)},
                        LarsVersion = new LarsVersion { Version = "larsVersion" },
                        PostcodesVersion = new PostcodesVersion { Version = "postcodeVersion" },
                        Employers = new EmployersVersion { Version = "employersVersion" }
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

            var fundingSummaryReportModelBuilder = new DesktopFundingSummaryReportModelBuilder(periodisedValuesLookupProvider.Object, dateTimeProviderMock.Object);

            var reportServiceContextMock = new Mock<IReportServiceContext>();
            reportServiceContextMock.Setup(c => c.Container).Returns(container);
            reportServiceContextMock.Setup(c => c.Ukprn).Returns(ukprn);
            reportServiceContextMock.Setup(c => c.IlrReportingFilename).Returns("ILR-11111111-1920-20190507-152000-01");
            reportServiceContextMock.Setup(c => c.ServiceReleaseVersion).Returns("ServiceReleaseVersion");

            var workBook = new Workbook();
            workBook.Worksheets.Add("FundingSummaryReport");
            var excelService = new Mock<IExcelFileService>();
            excelService.Setup(es => es.NewWorkbook()).Returns(workBook);
            excelService.Setup(es => es.GetWorksheetFromWorkbook(workBook, "FundingSummaryReport")).Returns(workBook.Worksheets["FundingSummaryReport"]);

            var fileNameServiceMock = new Mock<IFileNameService>();

            var fileName = "DesktopFundingSummaryReport.xlsx";
            fileNameServiceMock.Setup(s => s.GetFilename(reportServiceContextMock.Object, "Funding Summary Report", OutputTypes.Excel, true)).Returns(fileName);

            var fundingSummaryReportRenderService = new FundingSummaryReportRenderService();

            var report = NewReport(
                fileNameServiceMock.Object,
                fundingSummaryReportModelBuilder,
                excelService.Object,
                fundingSummaryReportRenderService);

            var cancellationToken = CancellationToken.None;

            await report.GenerateAsync(reportServiceContextMock.Object, reportServiceDependentDataMock.Object, cancellationToken);
        }

        private DesktopFundingSummaryReport NewReport(
            IFileNameService fileNameService = null,
            IModelBuilder<IFundingSummaryReport> fundingSummaryReportModelBuilder = null,
            IExcelFileService excelService = null,
            IRenderService<IFundingSummaryReport> fundingSummaryReportRenderService = null)
        {
            return new DesktopFundingSummaryReport(fileNameService, fundingSummaryReportModelBuilder, excelService, fundingSummaryReportRenderService);
        }
    }
}
