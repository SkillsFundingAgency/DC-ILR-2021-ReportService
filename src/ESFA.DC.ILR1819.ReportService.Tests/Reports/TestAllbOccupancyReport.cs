using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ESFA.DC.ILR1819.ReportService.Interface.Model;
using ESFA.DC.ILR1819.ReportService.Interface.Reports;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.Lars;
using ESFA.DC.ILR1819.ReportService.Service.Mapper;
using ESFA.DC.ILR1819.ReportService.Service.Reports;
using ESFA.DC.ILR1819.ReportService.Service.Service;
using ESFA.DC.ILR1819.ReportService.Tests.Helpers;
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
    public sealed class TestAllbOccupancyReport
    {
        [Fact]
        public async Task TestAllbOccupancyReportGeneration()
        {
            string csv = string.Empty;
            Mock<ILogger> logger = new Mock<ILogger>();

            Mock<IKeyValuePersistenceService> storage = new Mock<IKeyValuePersistenceService>();
            Mock<IKeyValuePersistenceService> redis = new Mock<IKeyValuePersistenceService>();
            IXmlSerializationService xmlSerializationService = new XmlSerializationService();
            IJsonSerializationService jsonSerializationService = new JsonSerializationService();

            storage.Setup(x => x.GetAsync(It.IsAny<string>())).ReturnsAsync(File.ReadAllText("ILR-10033670-1819-20180712-144437-03.xml"));
            storage.Setup(x => x.SaveAsync("ALLB_Occupancy_Report.csv", It.IsAny<string>())).Callback<string, string>((key, value) => csv = value).Returns(Task.CompletedTask);
            redis.Setup(x => x.GetAsync("FundingAlbOutput")).ReturnsAsync(File.ReadAllText("FundingAlbOutput.json"));
            redis.Setup(x => x.GetAsync("ValidLearners")).ReturnsAsync(jsonSerializationService.Serialize(
                new List<string>
                {
                    "3fm9901",
                    "5fm9901"
                }));

            IIlrProviderService ilrProviderService = new IlrProviderService(logger.Object, storage.Object, xmlSerializationService);
            Mock<ILarsProviderService> larsProviderService = new Mock<ILarsProviderService>();
            larsProviderService.Setup(x => x.GetLearningDeliveries(It.IsAny<List<string>>()))
                .ReturnsAsync(new Dictionary<string, ILarsLearningDelivery>()
                {
                    { "3fm9901", new LarsLearningDelivery { LearningAimTitle = "A", NotionalNvqLevel = "B", Tier2SectorSubjectArea = 3 } },
                    { "5fm9901", new LarsLearningDelivery { LearningAimTitle = "A", NotionalNvqLevel = "B", Tier2SectorSubjectArea = 3 } }
                });

            IAllbProviderService allbProviderService = new AllbProviderService(logger.Object, redis.Object, jsonSerializationService);

            IValidLearnersService validLearnersService = new ValidLearnersService(logger.Object, redis.Object, jsonSerializationService);

            IStringUtilitiesService stringUtilitiesService = new StringUtilitiesService();

            IReport allbOccupancyReport = new AllbOccupancyReport(
                logger.Object,
                storage.Object,
                redis.Object,
                xmlSerializationService,
                jsonSerializationService,
                ilrProviderService,
                larsProviderService.Object,
                allbProviderService,
                validLearnersService,
                stringUtilitiesService);

            IJobContextMessage jobContextMessage = new JobContextMessage(1, new ITopicItem[0], 0, System.DateTime.UtcNow);
            jobContextMessage.KeyValuePairs[JobContextMessageKey.Filename] = "ILR-10033670-1819-20180712-144437-03";
            jobContextMessage.KeyValuePairs[JobContextMessageKey.FundingAlbOutput] = "FundingAlbOutput";
            jobContextMessage.KeyValuePairs[JobContextMessageKey.ValidLearnRefNumbers] = "ValidLearners";

            await allbOccupancyReport.GenerateReport(jobContextMessage);

            csv.Should().NotBeNullOrEmpty();
            TestHelper.CheckCsv(csv, new AllbOccupancyMapper());
        }
    }
}
