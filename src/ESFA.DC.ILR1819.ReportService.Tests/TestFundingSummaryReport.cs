using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ESFA.DC.DateTime.Provider;
using ESFA.DC.DateTime.Provider.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Configuration;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Service.Reports;
using ESFA.DC.ILR1819.ReportService.Service.Service;
using ESFA.DC.ILR1819.ReportService.Stateless.Configuration;
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

namespace ESFA.DC.ILR1819.ReportService.Tests
{
    public sealed class TestFundingSummaryReport
    {
        [Fact]
        public async Task TestFundingSummaryReportGeneration()
        {
            string csv = string.Empty;
            string json = string.Empty;
            Mock<ILogger> logger = new Mock<ILogger>();
            Mock<IKeyValuePersistenceService> storage = new Mock<IKeyValuePersistenceService>();
            Mock<IKeyValuePersistenceService> redis = new Mock<IKeyValuePersistenceService>();
            IJsonSerializationService jsonSerializationService = new JsonSerializationService();
            IXmlSerializationService xmlSerializationService = new XmlSerializationService();
            IIlrProviderService ilrProviderService = new IlrProviderService(logger.Object, storage.Object, xmlSerializationService);
            Mock<IOrgProviderService> orgProviderService = new Mock<IOrgProviderService>();
            IAllbProviderService allbProviderService = new AllbProviderService(logger.Object, redis.Object, jsonSerializationService);
            IValidLearnersService validLearnersService = new ValidLearnersService(logger.Object, redis.Object, jsonSerializationService);
            IStringUtilitiesService stringUtilitiesService = new StringUtilitiesService();
            Mock<IPeriodProviderService> periodProviderService = new Mock<IPeriodProviderService>();
            IDateTimeProvider dateTimeProvider = new DateTimeProvider();
            IVersionInfo versionInfo = new VersionInfo() { ServiceReleaseVersion = "1.2.3.4.5" };

            storage.Setup(x => x.GetAsync(It.IsAny<string>())).ReturnsAsync(File.ReadAllText("ILR-10033670-1819-20180712-144437-03.xml"));
            storage.Setup(x => x.SaveAsync("Funding_Summary_Report.csv", It.IsAny<string>())).Callback<string, string>((key, value) => csv = value).Returns(Task.CompletedTask);
            storage.Setup(x => x.SaveAsync("Funding_Summary_Report.json", It.IsAny<string>())).Callback<string, string>((key, value) => json = value).Returns(Task.CompletedTask);
            redis.Setup(x => x.GetAsync("FundingAlbOutput")).ReturnsAsync(File.ReadAllText("FundingAlbOutput.json"));
            redis.Setup(x => x.GetAsync("ValidLearners")).ReturnsAsync(jsonSerializationService.Serialize(
                new List<string>
                {
                    "3fm9901",
                    "5fm9901"
                }));
            periodProviderService.Setup(x => x.GetPeriod(It.IsAny<IJobContextMessage>())).Returns(12);

            FundingSummaryReport fundingSummaryReport = new FundingSummaryReport(
                logger.Object,
                storage.Object,
                jsonSerializationService,
                ilrProviderService,
                orgProviderService.Object,
                allbProviderService,
                validLearnersService,
                stringUtilitiesService,
                periodProviderService.Object,
                dateTimeProvider,
                versionInfo);

            IJobContextMessage jobContextMessage = new JobContextMessage(1, new ITopicItem[0], 0, System.DateTime.UtcNow);
            jobContextMessage.KeyValuePairs[JobContextMessageKey.Filename] = "ILR-10033670-1819-20180712-144437-03";
            jobContextMessage.KeyValuePairs[JobContextMessageKey.FundingAlbOutput] = "FundingAlbOutput";
            jobContextMessage.KeyValuePairs[JobContextMessageKey.ValidLearnRefNumbers] = "ValidLearners";

            await fundingSummaryReport.GenerateReport(jobContextMessage);

            csv.Should().NotBeNullOrEmpty();
            json.Should().NotBeNullOrEmpty();
        }
    }
}
