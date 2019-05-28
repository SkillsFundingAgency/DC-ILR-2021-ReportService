using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.ReportService.Interface.Context;
using ESFA.DC.ILR.ReportService.Interface.Provider;
using ESFA.DC.ILR.ReportService.Interface.Service;
using ESFA.DC.ILR.ReportService.Model.PeriodEnd.ILRDataQuality;
using ESFA.DC.ILR.ReportService.Service.Service;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Tests.Reports.PeriodEnd
{
    public sealed class TestILRDataQualityReport
    {
        [Fact]
        public async Task TestILRDataQualityReportGeneration()
        {
            string csv = string.Empty;
            byte[] xlsx = null;
            DateTime dateTime = DateTime.UtcNow;
            int ukPrn = 10036143;
            string filename = $"10036143_1_ILR Data Quality Report {dateTime:yyyyMMdd-HHmmss}";

            Mock<IReportServiceContext> reportServiceContextMock = new Mock<IReportServiceContext>();
            reportServiceContextMock.SetupGet(x => x.JobId).Returns(1);
            reportServiceContextMock.SetupGet(x => x.SubmissionDateTimeUtc).Returns(DateTime.UtcNow);
            reportServiceContextMock.SetupGet(x => x.Ukprn).Returns(10036143);

            Mock<ILogger> logger = new Mock<ILogger>();
            Mock<IDateTimeProvider> dateTimeProviderMock = new Mock<IDateTimeProvider>();
            Mock<IStreamableKeyValuePersistenceService> storage = new Mock<IStreamableKeyValuePersistenceService>();
            Mock<IIlrPeriodEndProviderService> ilrPeriodEndProviderServiceMock = new Mock<IIlrPeriodEndProviderService>();
            IValueProvider valueProvider = new ValueProvider();
            storage.Setup(x => x.SaveAsync($"{filename}.xlsx", It.IsAny<Stream>(), It.IsAny<CancellationToken>())).Callback<string, Stream, CancellationToken>(
                    (key, value, ct) =>
                    {
                        value.Seek(0, SeekOrigin.Begin);
                        using (MemoryStream ms = new MemoryStream())
                        {
                            value.CopyTo(ms);
                            xlsx = ms.ToArray();
                        }
                    })
                .Returns(Task.CompletedTask);

            var top20Violations = BuildTop20ViolationsModel();

            ilrPeriodEndProviderServiceMock.Setup(x => x.GetTop20RuleViolationsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(top20Violations);
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(dateTime);
            dateTimeProviderMock.Setup(x => x.ConvertUtcToUk(It.IsAny<DateTime>())).Returns(dateTime);
            var ilrDataQualityReport = new ReportService.Service.Reports.PeriodEnd.ILRDataQualityReport(
                logger.Object,
                storage.Object,
                dateTimeProviderMock.Object,
                valueProvider,
                ilrPeriodEndProviderServiceMock.Object);

            MemoryStream memoryStream = new MemoryStream();

            using (memoryStream)
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Update, true))
                {
                    await ilrDataQualityReport.GenerateReport(reportServiceContextMock.Object, archive, false, CancellationToken.None);
                }

                using (var fileStream = new FileStream($"{filename}.zip", FileMode.Create))
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    memoryStream.CopyTo(fileStream);
                }
            }

            xlsx.Should().NotBeNullOrEmpty();
            File.WriteAllBytes($"{filename}.xlsx", xlsx);
        }

        private List<RuleViolationsInfo> BuildTop20ViolationsModel()
        {
            return new List<RuleViolationsInfo>()
            {
                new RuleViolationsInfo()
                {
                    RuleName = "R108",
                    ErrorMessage = "There must be a Learner Destination and Progression record present",
                    Providers = 88,
                    Learners = 1010,
                    NoOfErrors = 1010
                },
                new RuleViolationsInfo()
                {
                    RuleName = "UKPRN_09",
                    ErrorMessage = "There is no apprenticeship funding relationship for this UKPRN",
                    Providers = 8,
                    Learners = 644,
                    NoOfErrors = 644
                }
            };
        }
    }
}