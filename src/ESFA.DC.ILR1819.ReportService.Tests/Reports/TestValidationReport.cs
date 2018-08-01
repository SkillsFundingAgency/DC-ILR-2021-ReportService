using System.IO;
using System.Threading.Tasks;
using ESFA.DC.ILR1819.ReportService.Interface.Reports;
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
    public sealed class TestValidationReport
    {
        [Fact]
        public async Task TestValidationReportGeneration()
        {
            string csv = string.Empty;
            string json = string.Empty;
            Mock<ILogger> logger = new Mock<ILogger>();
            Mock<IKeyValuePersistenceService> storage = new Mock<IKeyValuePersistenceService>();
            Mock<IKeyValuePersistenceService> redis = new Mock<IKeyValuePersistenceService>();
            IXmlSerializationService xmlSerializationService = new XmlSerializationService();
            IJsonSerializationService jsonSerializationService = new JsonSerializationService();

            storage.Setup(x => x.GetAsync(It.IsAny<string>())).ReturnsAsync(File.ReadAllText("ILR-10033670-1819-20180712-144437-03.xml"));
            storage.Setup(x => x.SaveAsync("ValidationErrors.csv", It.IsAny<string>())).Callback<string, string>((key, value) => csv = value).Returns(Task.CompletedTask);
            storage.Setup(x => x.SaveAsync("ValidationErrors.json", It.IsAny<string>())).Callback<string, string>((key, value) => json = value).Returns(Task.CompletedTask);
            redis.Setup(x => x.GetAsync("ValidationErrors")).ReturnsAsync(File.ReadAllText("ValidationErrors.json"));
            redis.Setup(x => x.GetAsync("ValidationErrorsLookup")).ReturnsAsync(File.ReadAllText("ValidationErrorsLookup.json"));

            IIlrProviderService ilrProviderService = new IlrProviderService(logger.Object, storage.Object, xmlSerializationService);

            IInvalidLearnersService validLearnersService = new InvalidLearnersService(logger.Object, redis.Object, jsonSerializationService);

            IReport validationErrorsReport = new ValidationErrorsReport(logger.Object, storage.Object, redis.Object, xmlSerializationService, jsonSerializationService, ilrProviderService, validLearnersService);

            IJobContextMessage jobContextMessage = new JobContextMessage(1, new ITopicItem[0], 0, System.DateTime.UtcNow);
            jobContextMessage.KeyValuePairs[JobContextMessageKey.Filename] = "ILR-10033670-1819-20180712-144437-03";
            jobContextMessage.KeyValuePairs[JobContextMessageKey.ValidationErrors] = "ValidationErrors";
            jobContextMessage.KeyValuePairs[JobContextMessageKey.ValidationErrorLookups] = "ValidationErrorsLookup";

            await validationErrorsReport.GenerateReport(jobContextMessage);

            json.Should().NotBeNullOrEmpty();
            csv.Should().NotBeNullOrEmpty();
            TestCsvHelper.CheckCsv(csv, new ValidationErrorMapper());
        }
    }
}
