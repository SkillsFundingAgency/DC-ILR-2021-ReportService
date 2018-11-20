using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Configuration;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.Configuration;
using ESFA.DC.ILR1819.ReportService.Service.Builders;
using ESFA.DC.ILR1819.ReportService.Service.Reports;
using ESFA.DC.ILR1819.ReportService.Service.Service;
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

namespace ESFA.DC.ILR1819.ReportService.Tests.Reports.Main_Occupancy
{
    public sealed class TestMainOccupancyReportWithTestData
    {
#if DEBUG
        [Fact]
#endif
        public async Task TestMainOccupancyReportGeneration()
        {
            string csv = string.Empty;
            DateTime dateTime = DateTime.UtcNow;
            string filename = $"10033670_1_Main Occupancy Report {dateTime:yyyyMMdd-HHmmss}";
            List<string> validLearners = new List<string>() { "fm35 TNP 13" };
            string ilrFilename = @"Reports\Main Occupancy\ILR-10033670-1819-20181030-101919-07.xml";
            string fm35Filename = @"Reports\Main Occupancy\FundingFm35Output_FM35TNP13.json";

            IJobContextMessage jobContextMessage = new JobContextMessage(1, new ITopicItem[0], 0, DateTime.UtcNow);
            jobContextMessage.KeyValuePairs[JobContextMessageKey.UkPrn] = 10033670;
            jobContextMessage.KeyValuePairs[JobContextMessageKey.Filename] = ilrFilename;
            jobContextMessage.KeyValuePairs[JobContextMessageKey.ValidLearnRefNumbers] = "ValidLearners";
            jobContextMessage.KeyValuePairs[JobContextMessageKey.FundingFm35Output] = "FundingFm35Output";

            Mock<ILogger> logger = new Mock<ILogger>();
            Mock<IDateTimeProvider> dateTimeProviderMock = new Mock<IDateTimeProvider>();
            Mock<IStreamableKeyValuePersistenceService> storage = new Mock<IStreamableKeyValuePersistenceService>();
            Mock<IKeyValuePersistenceService> redis = new Mock<IKeyValuePersistenceService>();
            IJsonSerializationService jsonSerializationService = new JsonSerializationService();
            IXmlSerializationService xmlSerializationService = new XmlSerializationService();
            IFM35ProviderService fm35ProviderService = new FM35ProviderService(logger.Object, redis.Object, storage.Object, jsonSerializationService, null);
            IFM25ProviderService fm25ProviderService = new FM25ProviderService(logger.Object, redis.Object, storage.Object, jsonSerializationService, null);
            IIlrProviderService ilrProviderService = new IlrProviderService(logger.Object, storage.Object, xmlSerializationService, dateTimeProviderMock.Object, null);
            IValidLearnersService validLearnersService = new ValidLearnersService(logger.Object, redis.Object, storage.Object, jsonSerializationService);
            LarsConfiguration larsConfiguration = new LarsConfiguration
            {
                LarsConnectionString = ConfigurationManager.AppSettings["LarsConnectionString"]
            };
            ILarsProviderService larsProviderService = new LarsProviderService(logger.Object, larsConfiguration);
            IStringUtilitiesService stringUtilitiesService = new StringUtilitiesService();
            ITopicAndTaskSectionOptions topicsAndTasks = TestConfigurationHelper.GetTopicsAndTasks();
            IMainOccupancyReportModelBuilder reportModelBuilder = new MainOccupancyReportModelBuilder();
            IValueProvider valueProvider = new ValueProvider();

            storage.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>())).Callback<string, Stream, CancellationToken>((st, sr, ct) => File.OpenRead(ilrFilename).CopyTo(sr)).Returns(Task.CompletedTask);
            storage.Setup(x => x.SaveAsync($"{filename}.csv", It.IsAny<string>(), It.IsAny<CancellationToken>())).Callback<string, string, CancellationToken>((key, value, ct) => csv = value).Returns(Task.CompletedTask);
            storage.Setup(x => x.ContainsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
            redis.Setup(x => x.GetAsync("FundingFm35Output", It.IsAny<CancellationToken>())).ReturnsAsync(File.ReadAllText(fm35Filename));
            redis.Setup(x => x.GetAsync("ValidLearners", It.IsAny<CancellationToken>())).ReturnsAsync(jsonSerializationService.Serialize(validLearners));

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
                larsProviderService,
                dateTimeProviderMock.Object,
                valueProvider,
                topicsAndTasks,
                reportModelBuilder);

            await mainOccupancyReport.GenerateReport(jobContextMessage, null, false, CancellationToken.None);

            csv.Should().NotBeNullOrEmpty();

            File.WriteAllText($"{filename}.csv", csv);
        }
    }
}
