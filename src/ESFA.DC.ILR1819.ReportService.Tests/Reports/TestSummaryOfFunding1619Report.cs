using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Configuration;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Service.Mapper;
using ESFA.DC.ILR1819.ReportService.Service.Reports;
using ESFA.DC.ILR1819.ReportService.Service.Service;
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
    public sealed class TestSummaryOfFunding1619Report
    {
        [Fact]
        public async Task TestSummaryOfFunding1619ReportGeneration()
        {
            string csv = string.Empty;
            System.DateTime dateTime = System.DateTime.UtcNow;
            string filename = $"10033670_1_16-19 Summary of Funding by Student Report {dateTime:yyyyMMdd-HHmmss}";

            Mock<ILogger> logger = new Mock<ILogger>();
            Mock<IDateTimeProvider> dateTimeProviderMock = new Mock<IDateTimeProvider>();
            Mock<IStreamableKeyValuePersistenceService> storage = new Mock<IStreamableKeyValuePersistenceService>();
            Mock<IKeyValuePersistenceService> redis = new Mock<IKeyValuePersistenceService>();
            IJsonSerializationService jsonSerializationService = new JsonSerializationService();
            IXmlSerializationService xmlSerializationService = new XmlSerializationService();
            IIlrProviderService ilrProviderService = new IlrProviderService(logger.Object, storage.Object, xmlSerializationService, dateTimeProviderMock.Object, null);
            IValidLearnersService validLearnersService = new ValidLearnersService(logger.Object, redis.Object, storage.Object, jsonSerializationService);
            IFM25ProviderService fm25ProviderService = new FM25ProviderService(logger.Object, redis.Object, storage.Object, jsonSerializationService, null);
            IStringUtilitiesService stringUtilitiesService = new StringUtilitiesService();
            IValueProvider valueProvider = new ValueProvider();

            storage.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>())).Callback<string, Stream, CancellationToken>((st, sr, ct) => File.OpenRead("ILR-10033670-1819-20180712-144437-03.xml").CopyTo(sr)).Returns(Task.CompletedTask);
            storage.Setup(x => x.SaveAsync($"{filename}.csv", It.IsAny<string>(), It.IsAny<CancellationToken>())).Callback<string, string, CancellationToken>((key, value, ct) => csv = value).Returns(Task.CompletedTask);
            redis.Setup(x => x.GetAsync("ValidLearners", It.IsAny<CancellationToken>())).ReturnsAsync(jsonSerializationService.Serialize(
                new List<string>
                {
                    "0fm2501",
                    "3fm9901",
                    "5fm9901"
                }));
            redis.Setup(x => x.GetAsync("FundingFm25Output", It.IsAny<CancellationToken>())).ReturnsAsync(File.ReadAllText("Fm25.json"));

            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(dateTime);
            dateTimeProviderMock.Setup(x => x.ConvertUtcToUk(It.IsAny<System.DateTime>())).Returns(dateTime);

            ITopicAndTaskSectionOptions topicsAndTasks = TestConfigurationHelper.GetTopicsAndTasks();

            SummaryOfFunding1619Report summaryOfFunding1619Report = new SummaryOfFunding1619Report(
                logger.Object,
                storage.Object,
                ilrProviderService,
                validLearnersService,
                fm25ProviderService,
                stringUtilitiesService,
                dateTimeProviderMock.Object,
                valueProvider,
                topicsAndTasks);

            IJobContextMessage jobContextMessage = new JobContextMessage(1, new ITopicItem[0], 0, System.DateTime.UtcNow);
            jobContextMessage.KeyValuePairs[JobContextMessageKey.UkPrn] = 10033670;
            jobContextMessage.KeyValuePairs[JobContextMessageKey.Filename] = "ILR-10033670-1819-20180712-144437-03";
            jobContextMessage.KeyValuePairs[JobContextMessageKey.ValidLearnRefNumbers] = "ValidLearners";
            jobContextMessage.KeyValuePairs[JobContextMessageKey.FundingFm25Output] = "FundingFm25Output";

            await summaryOfFunding1619Report.GenerateReport(jobContextMessage, null, false, CancellationToken.None);

            csv.Should().NotBeNullOrEmpty();
            TestCsvHelper.CheckCsv(csv, new CsvEntry(new SummaryOfFunding1619Mapper(), 1));
        }
    }
}
