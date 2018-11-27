using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.Model.Interface;
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
    public class TestMainOccupancyReport
    {
        // Used to ensure EF is initialised
        private static SqlProviderServices instance = SqlProviderServices.Instance;

        [Theory]
        [InlineData("ILR-10033670-1819-20180831-094549-03.xml", "ValidLearnRefNumbers.json", "Alb.json", "Fm25.json", "Fm35.json")]
        public async Task TestMainOccupancyReportGeneration(string ilrFilename, string validLearnRefNumbersFilename, string albFilename, string fm25Filename, string fm35Filename)
        {
            string csv = string.Empty;
            DateTime dateTime = DateTime.UtcNow;
            string filename = $"10033670_1_Main Occupancy Report {dateTime:yyyyMMdd-HHmmss}";

            IJobContextMessage jobContextMessage = new JobContextMessage(1, new ITopicItem[0], 0, DateTime.UtcNow);
            jobContextMessage.KeyValuePairs[JobContextMessageKey.UkPrn] = 10033670;
            jobContextMessage.KeyValuePairs[JobContextMessageKey.Filename] = ilrFilename;
            jobContextMessage.KeyValuePairs[JobContextMessageKey.ValidLearnRefNumbers] = "ValidLearners";
            jobContextMessage.KeyValuePairs[JobContextMessageKey.FundingFm35Output] = "FundingFm35Output";
            jobContextMessage.KeyValuePairs[JobContextMessageKey.FundingFm25Output] = "FundingFm25Output";

            Mock<ILogger> logger = new Mock<ILogger>();
            Mock<IDateTimeProvider> dateTimeProviderMock = new Mock<IDateTimeProvider>();
            Mock<IStreamableKeyValuePersistenceService> storage = new Mock<IStreamableKeyValuePersistenceService>();
            Mock<IKeyValuePersistenceService> redis = new Mock<IKeyValuePersistenceService>();
            IIntUtilitiesService intUtilitiesService = new IntUtilitiesService();
            IJsonSerializationService jsonSerializationService = new JsonSerializationService();
            IXmlSerializationService xmlSerializationService = new XmlSerializationService();
            IFM35ProviderService fm35ProviderService = new FM35ProviderService(logger.Object, redis.Object, storage.Object, jsonSerializationService, intUtilitiesService, null);
            IFM25ProviderService fm25ProviderService = new FM25ProviderService(logger.Object, redis.Object, storage.Object, jsonSerializationService, intUtilitiesService, null);
            IIlrProviderService ilrProviderService = new IlrProviderService(logger.Object, storage.Object, xmlSerializationService, dateTimeProviderMock.Object, intUtilitiesService, null);
            IValidLearnersService validLearnersService =
                new ValidLearnersService(logger.Object, redis.Object, storage.Object, jsonSerializationService);
            Mock<ILarsProviderService> larsProviderService = new Mock<ILarsProviderService>();
            IStringUtilitiesService stringUtilitiesService = new StringUtilitiesService();
            ITopicAndTaskSectionOptions topicsAndTasks = TestConfigurationHelper.GetTopicsAndTasks();
            IMainOccupancyReportModelBuilder reportModelBuilder = new MainOccupancyReportModelBuilder();
            IValueProvider valueProvider = new ValueProvider();

            var validLearnersStr = File.ReadAllText(validLearnRefNumbersFilename);
            storage.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Callback<string, Stream, CancellationToken>((st, sr, ct) => File.OpenRead(ilrFilename).CopyTo(sr))
                .Returns(Task.CompletedTask);
            storage.Setup(x => x.SaveAsync($"{filename}.csv", It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Callback<string, string, CancellationToken>((key, value, ct) => csv = value)
                .Returns(Task.CompletedTask);
            storage.Setup(x => x.ContainsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
            redis.Setup(x => x.GetAsync("FundingFm35Output", It.IsAny<CancellationToken>()))
                .ReturnsAsync(File.ReadAllText(fm35Filename));
            redis.Setup(x => x.GetAsync("FundingFm25Output", It.IsAny<CancellationToken>()))
                .ReturnsAsync(File.ReadAllText(fm25Filename));
            redis.Setup(x => x.GetAsync("ValidLearners", It.IsAny<CancellationToken>())).ReturnsAsync(validLearnersStr);

            IMessage message = await ilrProviderService.GetIlrFile(jobContextMessage, CancellationToken.None);
            List<string> validLearners = jsonSerializationService.Deserialize<List<string>>(validLearnersStr);
            Dictionary<string, LarsLearningDelivery> learningDeliveriesDict =
                new Dictionary<string, LarsLearningDelivery>();
            List<LearnerAndDeliveries> learnerAndDeliveries = new List<LearnerAndDeliveries>();
            foreach (ILearner messageLearner in message.Learners)
            {
                if (validLearners.Contains(messageLearner.LearnRefNumber))
                {
                    List<LearningDelivery> learningDeliveries = new List<LearningDelivery>();
                    foreach (ILearningDelivery learningDelivery in messageLearner.LearningDeliveries)
                    {
                        var learningDeliveryRes = new LearningDelivery(
                            learningDelivery.LearnAimRef,
                            learningDelivery.AimSeqNumber,
                            learningDelivery.FworkCodeNullable,
                            learningDelivery.ProgTypeNullable,
                            learningDelivery.PwayCodeNullable,
                            learningDelivery.LearnStartDate);
                        learningDeliveryRes.FrameworkComponentType = 1;
                        learningDeliveries.Add(learningDeliveryRes);
                        learningDeliveriesDict[learningDelivery.LearnAimRef] = new LarsLearningDelivery()
                        {
                            LearningAimTitle = "A",
                            NotionalNvqLevel = "B",
                            Tier2SectorSubjectArea = 3
                        };
                    }

                    learnerAndDeliveries.Add(
                        new LearnerAndDeliveries(messageLearner.LearnRefNumber, learningDeliveries));
                }
            }

            larsProviderService
                .Setup(x => x.GetLearningDeliveriesAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(learningDeliveriesDict);
            larsProviderService
                .Setup(x => x.GetFrameworkAimsAsync(It.IsAny<string[]>(), It.IsAny<List<ILearner>>(), It.IsAny<CancellationToken>())).ReturnsAsync(learnerAndDeliveries);

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
                valueProvider,
                topicsAndTasks,
                reportModelBuilder);

            await mainOccupancyReport.GenerateReport(jobContextMessage, null, false, CancellationToken.None);

            csv.Should().NotBeNullOrEmpty();

#if DEBUG
            File.WriteAllText($"{filename}.csv", csv);
#endif

            TestCsvHelper.CheckCsv(csv, new CsvEntry(new MainOccupancyMapper(), 14));
        }
    }
}