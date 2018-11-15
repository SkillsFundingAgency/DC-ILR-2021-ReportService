using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Configuration;
using ESFA.DC.ILR1819.ReportService.Interface.Reports;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
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

namespace ESFA.DC.ILR1819.ReportService.Tests.Reports.Validation
{
    public sealed class TestValidationReportLotsOfErrors
    {
        [Fact]
        public async Task TestValidationReportGeneration()
        {
            string csv = string.Empty;
            string json = string.Empty;
            byte[] xlsx = null;
            DateTime dateTime = DateTime.UtcNow;
            string filename = $"10000020_1_Validation Errors Report {dateTime:yyyyMMdd-HHmmss}";
            string ilrFilename = @"Reports\Validation\ILR-10000020-1819-20181005-120953-03.xml";

            Mock<ILogger> logger = new Mock<ILogger>();
            Mock<IStreamableKeyValuePersistenceService> storage = new Mock<IStreamableKeyValuePersistenceService>();
            Mock<IKeyValuePersistenceService> redis = new Mock<IKeyValuePersistenceService>();
            IXmlSerializationService xmlSerializationService = new XmlSerializationService();
            IJsonSerializationService jsonSerializationService = new JsonSerializationService();
            Mock<IDateTimeProvider> dateTimeProviderMock = new Mock<IDateTimeProvider>();
            IValueProvider valueProvider = new ValueProvider();

            storage.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(File.ReadAllText(ilrFilename));
            storage.Setup(x => x.SaveAsync($"{filename}.csv", It.IsAny<string>(), It.IsAny<CancellationToken>())).Callback<string, string, CancellationToken>((key, value, ct) => csv = value).Returns(Task.CompletedTask);
            storage.Setup(x => x.SaveAsync($"{filename}.json", It.IsAny<string>(), It.IsAny<CancellationToken>())).Callback<string, string, CancellationToken>((key, value, ct) => json = value).Returns(Task.CompletedTask);
            storage.Setup(x => x.SaveAsync($"{filename}.xlsx", It.IsAny<Stream>(), It.IsAny<CancellationToken>())).Callback<string, Stream, CancellationToken>(
                (key, value, ct) =>
                {
                    value.Seek(0, SeekOrigin.Begin);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        value.CopyTo(ms);
                        xlsx = ms.ToArray();
                    }
                })
                .Returns(Task.CompletedTask);
            redis.Setup(x => x.GetAsync("ValidationErrors", It.IsAny<CancellationToken>())).ReturnsAsync(File.ReadAllText(@"Reports\Validation\ValidationErrors.json"));
            redis.Setup(x => x.GetAsync("ValidationErrorsLookup", It.IsAny<CancellationToken>())).ReturnsAsync(File.ReadAllText(@"Reports\Validation\ValidationErrorsLookup.json"));
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(dateTime);
            dateTimeProviderMock.Setup(x => x.ConvertUtcToUk(It.IsAny<DateTime>())).Returns(dateTime);

            IIlrProviderService ilrProviderService = new IlrProviderService(logger.Object, storage.Object, xmlSerializationService, null);

            ITopicAndTaskSectionOptions topicsAndTasks = TestConfigurationHelper.GetTopicsAndTasks();
            IValidationStageOutputCache validationStageOutputCache = new ValidationStageOutputCache();

            IReport validationErrorsReport = new ValidationErrorsReport(
                logger.Object,
                redis.Object,
                storage.Object,
                jsonSerializationService,
                ilrProviderService,
                dateTimeProviderMock.Object,
                valueProvider,
                topicsAndTasks,
                validationStageOutputCache);

            IJobContextMessage jobContextMessage = new JobContextMessage(1, new ITopicItem[0], 0, DateTime.UtcNow);
            jobContextMessage.KeyValuePairs[JobContextMessageKey.UkPrn] = "10000020";
            jobContextMessage.KeyValuePairs[JobContextMessageKey.Filename] = Path.GetFileNameWithoutExtension(ilrFilename);
            jobContextMessage.KeyValuePairs[JobContextMessageKey.ValidationErrors] = "ValidationErrors";
            jobContextMessage.KeyValuePairs[JobContextMessageKey.ValidationErrorLookups] = "ValidationErrorsLookup";
            jobContextMessage.KeyValuePairs[JobContextMessageKey.ValidLearnRefNumbersCount] = 2;
            jobContextMessage.KeyValuePairs[JobContextMessageKey.InvalidLearnRefNumbersCount] = 3;

            await validationErrorsReport.GenerateReport(jobContextMessage, null, false, CancellationToken.None);

            json.Should().NotBeNullOrEmpty();
            csv.Should().NotBeNullOrEmpty();
            xlsx.Should().NotBeNullOrEmpty();

#if DEBUG
            File.WriteAllBytes($"{filename}.xlsx", xlsx);
#endif

            ValidationErrorMapper helper = new ValidationErrorMapper();
            TestCsvHelper.CheckCsv(csv, new CsvEntry(helper, 1));
            TestXlsxHelper.CheckXlsx(xlsx, new XlsxEntry(helper, 5));
        }
    }
}
