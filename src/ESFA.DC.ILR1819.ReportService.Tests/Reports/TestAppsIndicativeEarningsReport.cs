using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Builders;
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
    public sealed class TestAppsIndicativeEarningsReport
    {
        [Fact]
        public async Task TestAppsIndicativeEarningsReportGeneration()
        {
            string csv = string.Empty;
            DateTime dateTime = DateTime.UtcNow;
            string filename = $"10033670_1_Apps Indicative Earnings Report {dateTime:yyyyMMdd-HHmmss}";
            string ilr = "ILR-10033670-1819-20180704-120055-03";

            IJobContextMessage jobContextMessage = new JobContextMessage(1, new ITopicItem[0], 0, DateTime.UtcNow);
            jobContextMessage.KeyValuePairs[JobContextMessageKey.UkPrn] = 10033670;
            jobContextMessage.KeyValuePairs[JobContextMessageKey.Filename] = ilr;
            jobContextMessage.KeyValuePairs[JobContextMessageKey.ValidLearnRefNumbers] = "ValidLearners";
            jobContextMessage.KeyValuePairs[JobContextMessageKey.FundingFm36Output] = "FundingFm36Output";

            Mock<ILogger> logger = new Mock<ILogger>();
            Mock<IDateTimeProvider> dateTimeProviderMock = new Mock<IDateTimeProvider>();
            Mock<IStreamableKeyValuePersistenceService> storage = new Mock<IStreamableKeyValuePersistenceService>();
            Mock<IKeyValuePersistenceService> redis = new Mock<IKeyValuePersistenceService>();
            IIntUtilitiesService intUtilitiesService = new IntUtilitiesService();
            IJsonSerializationService jsonSerializationService = new JsonSerializationService();
            IXmlSerializationService xmlSerializationService = new XmlSerializationService();
            IIlrProviderService ilrProviderService = new IlrProviderService(logger.Object, storage.Object, xmlSerializationService, dateTimeProviderMock.Object, intUtilitiesService, null);
            Mock<ILarsProviderService> larsProviderService = new Mock<ILarsProviderService>();
            IValidLearnersService validLearnersService = new ValidLearnersService(logger.Object, redis.Object, storage.Object, jsonSerializationService);
            IFM36ProviderService fm36ProviderService = new FM36ProviderService(logger.Object, redis.Object, storage.Object, jsonSerializationService);
            IStringUtilitiesService stringUtilitiesService = new StringUtilitiesService();
            ITotalBuilder totalBuilder = new TotalBuilder();

            List<IAppsIndicativeCommand> commands = new List<IAppsIndicativeCommand>();

            AppsIndicativeEarningsModelBuilder builder = new AppsIndicativeEarningsModelBuilder(commands, totalBuilder);

            storage.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>())).Callback<string, Stream, CancellationToken>((st, sr, ct) => File.OpenRead($"{ilr}.xml").CopyTo(sr)).Returns(Task.CompletedTask);
            storage.Setup(x => x.SaveAsync($"{filename}.csv", It.IsAny<string>(), It.IsAny<CancellationToken>())).Callback<string, string, CancellationToken>((key, value, ct) => csv = value).Returns(Task.CompletedTask);
            storage.Setup(x => x.ContainsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
            redis.Setup(x => x.GetAsync("ValidLearners", It.IsAny<CancellationToken>())).ReturnsAsync(jsonSerializationService.Serialize(
                new List<string>
                {
                    "3DOB01",
                }));
            redis.Setup(x => x.GetAsync("FundingFm36Output", It.IsAny<CancellationToken>())).ReturnsAsync(File.ReadAllText("Fm36.json"));
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(dateTime);
            dateTimeProviderMock.Setup(x => x.ConvertUtcToUk(It.IsAny<DateTime>())).Returns(dateTime);

            IMessage message = await ilrProviderService.GetIlrFile(jobContextMessage, CancellationToken.None);
            string validLearners = await redis.Object.GetAsync("ValidLearners");
            Dictionary<string, LarsLearningDelivery> learningDeliveriesDict = new Dictionary<string, LarsLearningDelivery>();

            foreach (ILearner messageLearner in message.Learners)
            {
                if (!validLearners.Contains(messageLearner.LearnRefNumber))
                {
                    continue;
                }

                foreach (ILearningDelivery learningDelivery in messageLearner.LearningDeliveries)
                {
                    learningDeliveriesDict[learningDelivery.LearnAimRef] = new LarsLearningDelivery()
                    {
                        LearningAimTitle = "A",
                        NotionalNvqLevel = "B",
                        Tier2SectorSubjectArea = 3
                    };
                }
            }

            larsProviderService.Setup(x => x.GetLearningDeliveriesAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>())).ReturnsAsync(learningDeliveriesDict);
            larsProviderService.Setup(x => x.GetStandardAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync("NotionalEndLevel");

            ITopicAndTaskSectionOptions topicsAndTasks = TestConfigurationHelper.GetTopicsAndTasks();
            IValueProvider valueProvider = new ValueProvider();

            var report = new AppsIndicativeEarningsReport(
                logger.Object,
                storage.Object,
                ilrProviderService,
                validLearnersService,
                fm36ProviderService,
                larsProviderService.Object,
                stringUtilitiesService,
                builder,
                dateTimeProviderMock.Object,
                valueProvider,
                topicsAndTasks);

            await report.GenerateReport(jobContextMessage, null, false, CancellationToken.None);

            csv.Should().NotBeNullOrEmpty();
            TestCsvHelper.CheckCsv(csv, new CsvEntry(new AppsIndicativeEarningsMapper(), 1));
        }
    }
}
