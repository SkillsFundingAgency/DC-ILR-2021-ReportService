using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Configuration;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.Lars;
using ESFA.DC.ILR1819.ReportService.Service.Builders;
using ESFA.DC.ILR1819.ReportService.Service.Mapper;
using ESFA.DC.ILR1819.ReportService.Service.Reports;
using ESFA.DC.ILR1819.ReportService.Service.Service;
using ESFA.DC.ILR1819.ReportService.Tests.AutoFac;
using ESFA.DC.ILR1819.ReportService.Tests.Helpers;
using ESFA.DC.ILR1819.ReportService.Tests.Models;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.JobContext;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using ESFA.DC.Serialization.Xml;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR1819.ReportService.Tests.Reports
{
    public class TestMainOccupancyReport
    {
        [Theory]
        [InlineData("ILR-10033670-1819-20180830-150220-03.xml", "Alb.json", "Fm25.json", "Fm35.json")]
        public async Task TestMainOccupancyReportGeneration(string ilrFilename, string albFilename, string fm25Filename, string fm35Filename)
        {
            string csv = string.Empty;
            DateTime dateTime = DateTime.UtcNow;
            string filename = $"10033670_1_Main Occupancy Report {dateTime:yyyyMMdd-HHmmss}";

            Mock<ILogger> logger = new Mock<ILogger>();
            Mock<IKeyValuePersistenceService> storage = new Mock<IKeyValuePersistenceService>();
            Mock<IKeyValuePersistenceService> redis = new Mock<IKeyValuePersistenceService>();
            IJsonSerializationService jsonSerializationService = new JsonSerializationService();
            IXmlSerializationService xmlSerializationService = new XmlSerializationService();
            IFM35ProviderService fm35ProviderService = new FM35ProviderService(logger.Object, redis.Object, storage.Object, jsonSerializationService);
            IFM25ProviderService fm25ProviderService = new FM25ProviderService(logger.Object, redis.Object, storage.Object, jsonSerializationService);
            IIlrProviderService ilrProviderService = new IlrProviderService(logger.Object, storage.Object, xmlSerializationService);
            IValidLearnersService validLearnersService = new ValidLearnersService(logger.Object, redis.Object, storage.Object, jsonSerializationService);
            Mock<ILarsProviderService> larsProviderService = new Mock<ILarsProviderService>();
            IStringUtilitiesService stringUtilitiesService = new StringUtilitiesService();
            Mock<IDateTimeProvider> dateTimeProviderMock = new Mock<IDateTimeProvider>();
            ITopicAndTaskSectionOptions topicsAndTasks = TestConfigurationHelper.GetTopicsAndTasks();
            IMainOccupancyReportModelBuilder reportModelBuilder = new MainOccupancyReportModelBuilder();

            storage.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(File.ReadAllText(ilrFilename));
            storage.Setup(x => x.SaveAsync($"{filename}.csv", It.IsAny<string>(), It.IsAny<CancellationToken>())).Callback<string, string, CancellationToken>((key, value, ct) => csv = value).Returns(Task.CompletedTask);
            redis.Setup(x => x.GetAsync("FundingFm35Output", It.IsAny<CancellationToken>())).ReturnsAsync(File.ReadAllText(fm35Filename));
            redis.Setup(x => x.GetAsync("FundingFm25Output", It.IsAny<CancellationToken>())).ReturnsAsync(File.ReadAllText(fm25Filename));
            redis.Setup(x => x.GetAsync("ValidLearners", It.IsAny<CancellationToken>())).ReturnsAsync(jsonSerializationService.Serialize(
                new List<string>
                {
                    "8fm3501",
                    "0fm3501"
                }));
            larsProviderService.Setup(x => x.GetLearningDeliveries(It.IsAny<List<string>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Dictionary<string, LarsLearningDelivery>()
                {
                    { "60133533", new LarsLearningDelivery { LearningAimTitle = "A", NotionalNvqLevel = "B", Tier2SectorSubjectArea = 3 } }
                });
            larsProviderService.Setup(x => x.GetFrameworkAims(It.IsAny<List<string>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Dictionary<string, LarsFrameworkAim>
                {
                    { "60133533", new LarsFrameworkAim { FrameworkComponentType = 0 } }
                });

            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(dateTime);
            dateTimeProviderMock.Setup(x => x.ConvertUtcToUk(It.IsAny<DateTime>())).Returns(dateTime);

            var mainOccupancyReport = new MainOccupancyReport(
                logger.Object,
                storage.Object,
                ilrProviderService,
                stringUtilitiesService,
                validLearnersService,
                fm25ProviderService,
                fm35ProviderService,
                larsProviderService.Object,
                dateTimeProviderMock.Object,
                topicsAndTasks,
                reportModelBuilder);

            IJobContextMessage jobContextMessage = new JobContextMessage(1, new ITopicItem[0], 0, DateTime.UtcNow);
            jobContextMessage.KeyValuePairs[JobContextMessageKey.UkPrn] = "10033670";
            jobContextMessage.KeyValuePairs[JobContextMessageKey.Filename] = ilrFilename;
            jobContextMessage.KeyValuePairs[JobContextMessageKey.ValidLearnRefNumbers] = "ValidLearners";
            jobContextMessage.KeyValuePairs[JobContextMessageKey.FundingFm35Output] = "FundingFm35Output";
            jobContextMessage.KeyValuePairs[JobContextMessageKey.FundingFm25Output] = "FundingFm25Output";

            await mainOccupancyReport.GenerateReport(jobContextMessage, null, CancellationToken.None);

            csv.Should().NotBeNullOrEmpty();

            TestCsvHelper.CheckCsv(csv, new CsvEntry(new MainOccupancyFM25Mapper(), 1), new CsvEntry(new MainOccupancyFM35Mapper(), 1));
        }
    }
}
