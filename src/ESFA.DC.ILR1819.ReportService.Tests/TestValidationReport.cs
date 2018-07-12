using System;
using System.IO;
using ESFA.DC.ILR1819.ReportService.Interface.Reports;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Service.Reports;
using ESFA.DC.ILR1819.ReportService.Service.Service;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.JobContext;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using ESFA.DC.Serialization.Xml;
using Moq;

namespace ESFA.DC.ILR1819.ReportService.Tests
{
    public sealed class TestValidationReport
    {
        public void TestValidationReportGeneration()
        {
            Mock<ILogger> logger = new Mock<ILogger>();
            Mock<IKeyValuePersistenceService> storage = new Mock<IKeyValuePersistenceService>();
            Mock<IKeyValuePersistenceService> redis = new Mock<IKeyValuePersistenceService>();
            IXmlSerializationService xmlSerializationService = new XmlSerializationService();
            IJsonSerializationService jsonSerializationService = new JsonSerializationService();

            storage.Setup(x => x.GetAsync(It.IsAny<string>())).ReturnsAsync(File.ReadAllText("ILR-10033670-1819-20180712-144437-03.xml"));
            redis.Setup(x => x.GetAsync("ValidationErrors.json")).ReturnsAsync(File.ReadAllText("ValidationErrors.json"));

            IIlrProviderService ilrProviderService = new IlrProviderService(logger.Object, storage.Object, xmlSerializationService);

            IValidationErrorsReport validationErrorsReport = new ValidationErrorsReport(logger.Object, storage.Object, redis.Object, xmlSerializationService, jsonSerializationService, ilrProviderService);

            IJobContextMessage jobContextMessage = new JobContextMessage(1, new ITopicItem[0], 0, DateTime.UtcNow);
            jobContextMessage.KeyValuePairs[JobContextMessageKey.Filename] = "ILR-10033670-1819-20180712-144437-03.xml";
            jobContextMessage.KeyValuePairs[JobContextMessageKey.ValidationErrors] = "ValidationErrors.json";
            jobContextMessage.KeyValuePairs[JobContextMessageKey.Filename] = "ILR-10033670-1819-20180712-144437-03.xml";
        }
    }
}
