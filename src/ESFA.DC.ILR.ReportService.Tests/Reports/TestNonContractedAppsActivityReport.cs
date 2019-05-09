using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.ReportService.Interface.Context;
using ESFA.DC.ILR.ReportService.Interface.Provider;
using ESFA.DC.ILR.ReportService.Interface.Service;
using ESFA.DC.ILR.ReportService.Service.Builders;
using ESFA.DC.ILR.ReportService.Service.Mapper;
using ESFA.DC.ILR.ReportService.Service.Service;
using ESFA.DC.ILR.ReportService.Tests.Helpers;
using ESFA.DC.ILR.ReportService.Tests.Models;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Tests.Reports
{
    public sealed class TestNonContractedAppsActivityReport
    {
        [Fact]
        public async Task TestNonContractedAppsActivityReportGeneration()
        {
            string csv = string.Empty;
            DateTime dateTime = DateTime.UtcNow;
            string filename = $"10036143_1_Non-Contracted Apprenticeships Activity Report {dateTime:yyyyMMdd-HHmmss}";
            int ukPrn = 10036143;
            Mock<IReportServiceContext> reportServiceContextMock = new Mock<IReportServiceContext>();
            reportServiceContextMock.SetupGet(x => x.JobId).Returns(1);
            reportServiceContextMock.SetupGet(x => x.SubmissionDateTimeUtc).Returns(DateTime.UtcNow);
            reportServiceContextMock.SetupGet(x => x.Ukprn).Returns(ukPrn);

            Mock<ILogger> logger = new Mock<ILogger>();
            Mock<IDateTimeProvider> dateTimeProviderMock = new Mock<IDateTimeProvider>();
            Mock<IStreamableKeyValuePersistenceService> storage = new Mock<IStreamableKeyValuePersistenceService>();
            Mock<IIlrProviderService> ilrProviderServiceMock = new Mock<IIlrProviderService>();
            Mock<IFM36ProviderService> fm36ProviderServiceMock = new Mock<IFM36ProviderService>();
            IValueProvider valueProvider = new ValueProvider();
            storage.Setup(x => x.SaveAsync($"{filename}.csv", It.IsAny<string>(), It.IsAny<CancellationToken>())).Callback<string, string, CancellationToken>((key, value, ct) => csv = value).Returns(Task.CompletedTask);

            ilrProviderServiceMock.Setup(x => x.GetIlrFile(reportServiceContextMock.Object, It.IsAny<CancellationToken>())).ReturnsAsync(new Message());
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(dateTime);
            dateTimeProviderMock.Setup(x => x.ConvertUtcToUk(It.IsAny<DateTime>())).Returns(dateTime);
            var nonContractedAppsActivityModel = new NonContractedAppsActivityModelBuilder();

            var report = new ReportService.Service.Reports.NonContractedAppsActivityReport(
                logger.Object,
                storage.Object,
                ilrProviderServiceMock.Object,
                fm36ProviderServiceMock.Object,
                dateTimeProviderMock.Object,
                valueProvider,
                nonContractedAppsActivityModel);

            await report.GenerateReport(reportServiceContextMock.Object, null, false, CancellationToken.None);

            csv.Should().NotBeNullOrEmpty();
            File.WriteAllText($"{filename}.csv", csv);
            TestCsvHelper.CheckCsv(csv, new CsvEntry(new NonContractedAppsActivityMapper(), 1));
        }
    }
}