using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Builders;
using ESFA.DC.ILR1819.ReportService.Interface.Configuration;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Service.Builders;
using ESFA.DC.ILR1819.ReportService.Service.Mapper;
using ESFA.DC.ILR1819.ReportService.Service.Reports;
using ESFA.DC.ILR1819.ReportService.Service.Service;
using ESFA.DC.ILR1819.ReportService.Stateless.Configuration;
using ESFA.DC.ILR1819.ReportService.Tests.AutoFac;
using ESFA.DC.ILR1819.ReportService.Tests.Helpers;
using ESFA.DC.ILR1819.ReportService.Tests.Models;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.JobContextManager.Model;
using ESFA.DC.JobContextManager.Model.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using ESFA.DC.Serialization.Xml;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR1819.ReportService.Tests.Reports
{
    public sealed class TestFundingSummaryReport
    {
        [Fact]
        public async Task TestFundingSummaryReportGeneration()
        {
            string csv = string.Empty;
            DateTime dateTime = DateTime.UtcNow;
            string filename = $"10033670_1_Funding Summary Report {dateTime:yyyyMMdd-HHmmss}";

            Mock<ILogger> logger = new Mock<ILogger>();
            Mock<IStreamableKeyValuePersistenceService> storage = new Mock<IStreamableKeyValuePersistenceService>();
            Mock<IKeyValuePersistenceService> redis = new Mock<IKeyValuePersistenceService>();
            IJsonSerializationService jsonSerializationService = new JsonSerializationService();
            IXmlSerializationService xmlSerializationService = new XmlSerializationService();
            IIlrProviderService ilrProviderService = new IlrProviderService(logger.Object, storage.Object, xmlSerializationService);
            Mock<IOrgProviderService> orgProviderService = new Mock<IOrgProviderService>();
            Mock<ILarsProviderService> larsProviderService = new Mock<ILarsProviderService>();
            Mock<IEasProviderService> easProviderServiceMock = new Mock<IEasProviderService>();
            Mock<IPostcodeProviderService> postcodeProverServiceMock = new Mock<IPostcodeProviderService>();
            Mock<ILargeEmployerProviderService> largeEmployerProviderService = new Mock<ILargeEmployerProviderService>();
            IAllbProviderService allbProviderService = new AllbProviderService(logger.Object, redis.Object, storage.Object, jsonSerializationService);
            IFM35ProviderService fm35ProviderService = new FM35ProviderService(logger.Object, redis.Object, storage.Object, jsonSerializationService);
            IFM25ProviderService fm25ProviderService = new FM25ProviderService(logger.Object, redis.Object, storage.Object, jsonSerializationService);
            IValidLearnersService validLearnersService = new ValidLearnersService(logger.Object, redis.Object, storage.Object, jsonSerializationService);
            IStringUtilitiesService stringUtilitiesService = new StringUtilitiesService();
            Mock<IPeriodProviderService> periodProviderService = new Mock<IPeriodProviderService>();
            Mock<IDateTimeProvider> dateTimeProviderMock = new Mock<IDateTimeProvider>();
            IVersionInfo versionInfo = new VersionInfo { ServiceReleaseVersion = "1.2.3.4.5" };
            IFm25Builder fm25Builder = new Fm25Builder();
            IAllbBuilder allbBuilder = new AllbBuilder(ilrProviderService, validLearnersService, allbProviderService, periodProviderService.Object, stringUtilitiesService, logger.Object);
            IExcelStyleProvider excelStyleProvider = new ExcelStyleProvider();

            storage.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>())).Callback<string, Stream, CancellationToken>((st, sr, ct) => File.OpenRead("ILR-10033670-1819-20180704-120055-03.xml").CopyTo(sr)).Returns(Task.CompletedTask);
            storage.Setup(x => x.SaveAsync($"{filename}.csv", It.IsAny<string>(), It.IsAny<CancellationToken>())).Callback<string, string, CancellationToken>((key, value, ct) => csv = value).Returns(Task.CompletedTask);
            redis.Setup(x => x.GetAsync("FundingAlbOutput", It.IsAny<CancellationToken>())).ReturnsAsync(File.ReadAllText("ALBOutput1000.json"));
            redis.Setup(x => x.GetAsync("ValidLearners", It.IsAny<CancellationToken>())).ReturnsAsync(jsonSerializationService.Serialize(
                new List<string>
                {
                    "3Addl103",
                    "4Addl103"
                }));
            redis.Setup(x => x.GetAsync("FundingFm35Output", It.IsAny<CancellationToken>())).ReturnsAsync(File.ReadAllText("Fm35.json"));
            redis.Setup(x => x.GetAsync("FundingFm25Output", It.IsAny<CancellationToken>())).ReturnsAsync(File.ReadAllText("Fm25.json"));
            periodProviderService.Setup(x => x.GetPeriod(It.IsAny<IJobContextMessage>(), It.IsAny<CancellationToken>())).ReturnsAsync(12);
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(dateTime);
            dateTimeProviderMock.Setup(x => x.ConvertUtcToUk(It.IsAny<DateTime>())).Returns(dateTime);
            easProviderServiceMock.Setup(x => x.GetLastEasUpdate(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(DateTime.MinValue);
            largeEmployerProviderService.Setup(x => x.GetVersionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync("NA");
            postcodeProverServiceMock.Setup(x => x.GetVersionAsync(It.IsAny<CancellationToken>())).ReturnsAsync("NA");

            ITopicAndTaskSectionOptions topicsAndTasks = TestConfigurationHelper.GetTopicsAndTasks();

            FundingSummaryReport fundingSummaryReport = new FundingSummaryReport(
                logger.Object,
                storage.Object,
                ilrProviderService,
                orgProviderService.Object,
                allbProviderService,
                fm25ProviderService,
                fm35ProviderService,
                validLearnersService,
                stringUtilitiesService,
                periodProviderService.Object,
                dateTimeProviderMock.Object,
                larsProviderService.Object,
                easProviderServiceMock.Object,
                postcodeProverServiceMock.Object,
                largeEmployerProviderService.Object,
                allbBuilder,
                fm25Builder,
                versionInfo,
                excelStyleProvider,
                topicsAndTasks);

            IJobContextMessage jobContextMessage = new JobContextMessage(1, new ITopicItem[0], 0, DateTime.UtcNow);

            jobContextMessage.KeyValuePairs[JobContextMessageKey.UkPrn] = "10033670";
            jobContextMessage.KeyValuePairs[JobContextMessageKey.Filename] = "ILR-10033670-1819-20180704-120055-03";
            jobContextMessage.KeyValuePairs[JobContextMessageKey.FundingAlbOutput] = "FundingAlbOutput";
            jobContextMessage.KeyValuePairs[JobContextMessageKey.ValidLearnRefNumbers] = "ValidLearners";
            jobContextMessage.KeyValuePairs[JobContextMessageKey.FundingFm35Output] = "FundingFm35Output";
            jobContextMessage.KeyValuePairs[JobContextMessageKey.FundingFm25Output] = "FundingFm25Output";

            await fundingSummaryReport.GenerateReport(jobContextMessage, null, CancellationToken.None);

            csv.Should().NotBeNullOrEmpty();

            TestCsvHelper.CheckCsv(
                csv,
                new CsvEntry(new FundingSummaryHeaderMapper(), 1),
                new CsvEntry(new FundingSummaryMapper("16-18 Traineeships"), 1),
                new CsvEntry(new FundingSummaryMapper("Advanced Loans Bursary"), 2, 1),
                new CsvEntry(new FundingSummaryFooterMapper(), 1));
        }
    }
}
