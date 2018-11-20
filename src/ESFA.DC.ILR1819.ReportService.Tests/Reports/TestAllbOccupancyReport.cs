using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Configuration;
using ESFA.DC.ILR1819.ReportService.Interface.Reports;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.Lars;
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
    public sealed class TestAllbOccupancyReport
    {
        [Fact]
        public async Task TestAllbOccupancyReportGeneration()
        {
            string csv = string.Empty;
            System.DateTime dateTime = System.DateTime.UtcNow;
            string filename = $"10033670_1_ALLB Occupancy Report {dateTime:yyyyMMdd-HHmmss}";

            Mock<ILogger> logger = new Mock<ILogger>();

            Mock<IStreamableKeyValuePersistenceService> storage = new Mock<IStreamableKeyValuePersistenceService>();
            Mock<IKeyValuePersistenceService> redis = new Mock<IKeyValuePersistenceService>();
            Mock<IDateTimeProvider> dateTimeProviderMock = new Mock<IDateTimeProvider>();
            IXmlSerializationService xmlSerializationService = new XmlSerializationService();
            IJsonSerializationService jsonSerializationService = new JsonSerializationService();

            storage.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Callback<string, Stream, CancellationToken>((st, sr, ct) => File.OpenRead("ILR-10033670-1819-20180704-120055-03.xml").CopyTo(sr)).Returns(Task.CompletedTask);
            storage.Setup(x => x.SaveAsync($"{filename}.csv", It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Callback<string, string, CancellationToken>((key, value, ct) => csv = value).Returns(Task.CompletedTask);
            storage.Setup(x => x.ContainsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
            redis.Setup(x => x.GetAsync("FundingAlbOutput", It.IsAny<CancellationToken>())).ReturnsAsync(File.ReadAllText("ALBOutput1000.json"));
            redis.Setup(x => x.GetAsync("ValidLearners", It.IsAny<CancellationToken>())).ReturnsAsync(jsonSerializationService.Serialize(
                new List<string>
                {
                    "3Addl103",
                    "4Addl103"
                }));
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(dateTime);
            dateTimeProviderMock.Setup(x => x.ConvertUtcToUk(It.IsAny<System.DateTime>())).Returns(dateTime);

            IIlrProviderService ilrProviderService = new IlrProviderService(logger.Object, storage.Object, xmlSerializationService, dateTimeProviderMock.Object, null);
            Mock<ILarsProviderService> larsProviderService = new Mock<ILarsProviderService>();
            larsProviderService.Setup(x => x.GetLearningDeliveriesAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Dictionary<string, LarsLearningDelivery>()
                {
                    { "60133533", new LarsLearningDelivery { LearningAimTitle = "A", NotionalNvqLevel = "B", Tier2SectorSubjectArea = 3 } }
                });

            IAllbProviderService allbProviderService = new AllbProviderService(logger.Object, redis.Object, storage.Object, jsonSerializationService, null);

            IValidLearnersService validLearnersService = new ValidLearnersService(logger.Object, redis.Object, storage.Object, jsonSerializationService);

            IStringUtilitiesService stringUtilitiesService = new StringUtilitiesService();

            ITopicAndTaskSectionOptions topicsAndTasks = TestConfigurationHelper.GetTopicsAndTasks();

            IValueProvider valueProvider = new ValueProvider();

            IReport allbOccupancyReport = new AllbOccupancyReport(
                logger.Object,
                storage.Object,
                ilrProviderService,
                larsProviderService.Object,
                allbProviderService,
                validLearnersService,
                stringUtilitiesService,
                dateTimeProviderMock.Object,
                valueProvider,
                topicsAndTasks);

            IJobContextMessage jobContextMessage = new JobContextMessage(1, new ITopicItem[0], 0, System.DateTime.UtcNow);
            jobContextMessage.KeyValuePairs[JobContextMessageKey.UkPrn] = 10033670;
            jobContextMessage.KeyValuePairs[JobContextMessageKey.Filename] = "ILR-10033670-1819-20180704-120055-03";
            jobContextMessage.KeyValuePairs[JobContextMessageKey.FundingAlbOutput] = "FundingAlbOutput";
            jobContextMessage.KeyValuePairs[JobContextMessageKey.ValidLearnRefNumbers] = "ValidLearners";

            await allbOccupancyReport.GenerateReport(jobContextMessage, null, false, CancellationToken.None);

            csv.Should().NotBeNullOrEmpty();
            TestCsvHelper.CheckCsv(csv, new CsvEntry(new AllbOccupancyMapper(), 1));
        }
    }
}
