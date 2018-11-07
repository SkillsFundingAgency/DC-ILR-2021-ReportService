using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.EAS1819.EF;
using ESFA.DC.EAS1819.EF.Interface;
using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output;
using ESFA.DC.ILR1819.ReportService.Interface.Builders;
using ESFA.DC.ILR1819.ReportService.Interface.Configuration;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.Configuration;
using ESFA.DC.ILR1819.ReportService.Service;
using ESFA.DC.ILR1819.ReportService.Service.Builders;
using ESFA.DC.ILR1819.ReportService.Service.Reports;
using ESFA.DC.ILR1819.ReportService.Service.Service;
using ESFA.DC.ILR1819.ReportService.Stateless.Configuration;
using ESFA.DC.ILR1819.ReportService.Tests.AutoFac;
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
    public sealed class TestAdultFundingClaimReport
    {
        [Fact]
        public async Task TestAdultFundingClaimReportGeneration()
        {
            string csv = string.Empty;
            byte[] xlsx = null;
            DateTime dateTime = DateTime.UtcNow;
            string filename = $"10033670_1_Adult Funding Claim Report {dateTime:yyyyMMdd-HHmmss}";

            Mock<ILogger> logger = new Mock<ILogger>();
            Mock<IStreamableKeyValuePersistenceService> storage = new Mock<IStreamableKeyValuePersistenceService>();
            Mock<IKeyValuePersistenceService> redis = new Mock<IKeyValuePersistenceService>();
            IJsonSerializationService jsonSerializationService = new JsonSerializationService();
            IXmlSerializationService xmlSerializationService = new XmlSerializationService();
            IIlrProviderService ilrProviderService = new IlrProviderService(logger.Object, storage.Object, xmlSerializationService);
            Mock<IOrgProviderService> orgProviderService = new Mock<IOrgProviderService>();
            Mock<ILarsProviderService> larsProviderService = new Mock<ILarsProviderService>();

            EasConfiguration easConfiguration = new EasConfiguration() { EasConnectionString = new TestConfigurationHelper().GetSectionValues<EasConfiguration>("EasSection").EasConnectionString };
            IEasProviderService easProviderService = new EasProviderService(logger.Object, easConfiguration);

            Mock <IPostcodeProviderService> postcodeProverServiceMock = new Mock<IPostcodeProviderService>();
            Mock<ILargeEmployerProviderService> largeEmployerProviderService = new Mock<ILargeEmployerProviderService>();
            IAllbProviderService allbProviderService = new AllbProviderService(logger.Object, redis.Object, storage.Object, jsonSerializationService);
            IFM35ProviderService fm35ProviderService = new FM35ProviderService(logger.Object, redis.Object, storage.Object, jsonSerializationService);
            IValidLearnersService validLearnersService = new ValidLearnersService(logger.Object, redis.Object, storage.Object, jsonSerializationService);
            IStringUtilitiesService stringUtilitiesService = new StringUtilitiesService();
            Mock<IPeriodProviderService> periodProviderService = new Mock<IPeriodProviderService>();
            Mock<IDateTimeProvider> dateTimeProviderMock = new Mock<IDateTimeProvider>();
            IVersionInfo versionInfo = new VersionInfo { ServiceReleaseVersion = "1.2.3.4.5" };
            ITotalBuilder totalBuilder = new TotalBuilder();
            IFm35Builder fm35Builder = new Fm35Builder(totalBuilder, new CacheProviderService<LearningDelivery[]>());
            IAllbBuilder allbBuilder = new AllbBuilder(ilrProviderService, validLearnersService, allbProviderService, periodProviderService.Object, totalBuilder, stringUtilitiesService, logger.Object);
            IExcelStyleProvider excelStyleProvider = new ExcelStyleProvider();

            IEasBuilder easBuilder = new EasBuilder(easProviderService);

            storage.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>())).Callback<string, Stream, CancellationToken>((st, sr, ct) => File.OpenRead("AdultFundingClaimReportTemplate.xlsx").CopyTo(sr)).Returns(Task.CompletedTask);
            storage.Setup(x => x.SaveAsync($"{filename}.csv", It.IsAny<string>(), It.IsAny<CancellationToken>())).Callback<string, string, CancellationToken>((key, value, ct) => csv = value).Returns(Task.CompletedTask);
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
            largeEmployerProviderService.Setup(x => x.GetVersionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync("NA");
            postcodeProverServiceMock.Setup(x => x.GetVersionAsync(It.IsAny<CancellationToken>())).ReturnsAsync("NA");
            orgProviderService
                .Setup(x => x.GetProviderName(It.IsAny<IJobContextMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("Test Provider");

            ITopicAndTaskSectionOptions topicsAndTasks = TestConfigurationHelper.GetTopicsAndTasks();
            IValueProvider valueProvider = new ValueProvider();

            AdultFundingClaimReport adultFundingClaimReport = new AdultFundingClaimReport(
                logger.Object,
                storage.Object,
                ilrProviderService,
                orgProviderService.Object,
                allbProviderService,
                fm35ProviderService,
                periodProviderService.Object,
                dateTimeProviderMock.Object,
                valueProvider,
                larsProviderService.Object,
                easProviderService,
                postcodeProverServiceMock.Object,
                largeEmployerProviderService.Object,
                allbBuilder,
                fm35Builder,
                totalBuilder,
                versionInfo,
                excelStyleProvider,
                topicsAndTasks,
                easBuilder);
            IJobContextMessage jobContextMessage = new JobContextMessage(1, new ITopicItem[0], 0, DateTime.UtcNow);

            jobContextMessage.KeyValuePairs[JobContextMessageKey.UkPrn] = "10033670";
            jobContextMessage.KeyValuePairs[JobContextMessageKey.Filename] = "ILR-10033670-1819-20180704-120055-03";
            jobContextMessage.KeyValuePairs[JobContextMessageKey.FundingAlbOutput] = "FundingAlbOutput";
            jobContextMessage.KeyValuePairs[JobContextMessageKey.ValidLearnRefNumbers] = "ValidLearners";
            jobContextMessage.KeyValuePairs[JobContextMessageKey.FundingFm35Output] = "FundingFm35Output";
            jobContextMessage.KeyValuePairs[JobContextMessageKey.FundingFm25Output] = "FundingFm25Output";

            await adultFundingClaimReport.GenerateReport(jobContextMessage, null, false, CancellationToken.None);

            xlsx.Should().NotBeNullOrEmpty();

            File.WriteAllBytes($"{filename}.xlsx", xlsx);
        }
    }
}
