using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
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
    public sealed class TestMathsAndEnglishReport
    {
        [Fact]
        public async Task TestMathsAndEnglishReportGeneration()
        {
            string csv = string.Empty;
            Mock<ILogger> logger = new Mock<ILogger>();
            Mock<IKeyValuePersistenceService> storage = new Mock<IKeyValuePersistenceService>();
            Mock<IKeyValuePersistenceService> redis = new Mock<IKeyValuePersistenceService>();
            IJsonSerializationService jsonSerializationService = new JsonSerializationService();
            IXmlSerializationService xmlSerializationService = new XmlSerializationService();
            IIlrProviderService ilrProviderService = new IlrProviderService(logger.Object, storage.Object, xmlSerializationService);
            IValidLearnersService validLearnersService = new ValidLearnersService(logger.Object, redis.Object, jsonSerializationService);
            IStringUtilitiesService stringUtilitiesService = new StringUtilitiesService();

            storage.Setup(x => x.GetAsync(It.IsAny<string>())).ReturnsAsync(File.ReadAllText("ILR-10033670-1819-20180712-144437-03.xml"));
            storage.Setup(x => x.SaveAsync("Maths and English Report.csv", It.IsAny<string>())).Callback<string, string>((key, value) => csv = value).Returns(Task.CompletedTask);
            redis.Setup(x => x.GetAsync("ValidLearners")).ReturnsAsync(jsonSerializationService.Serialize(
                new List<string>
                {
                    "3fm9901",
                    "5fm9901"
                }));

            MathsAndEnglishReport summaryOfFunding1619Report = new MathsAndEnglishReport(
                logger.Object,
                storage.Object,
                redis.Object,
                xmlSerializationService,
                jsonSerializationService,
                ilrProviderService,
                validLearnersService,
                stringUtilitiesService);

            IJobContextMessage jobContextMessage = new JobContextMessage(1, new ITopicItem[0], 0, System.DateTime.UtcNow);
            jobContextMessage.KeyValuePairs[JobContextMessageKey.Filename] = "ILR-10033670-1819-20180712-144437-03";
            jobContextMessage.KeyValuePairs[JobContextMessageKey.ValidLearnRefNumbers] = "ValidLearners";

            await summaryOfFunding1619Report.GenerateReport(jobContextMessage);

            csv.Should().NotBeNullOrEmpty();
            TestHelper.CheckCsv(csv, new MathsAndEnglishMapper());
        }
    }
}
