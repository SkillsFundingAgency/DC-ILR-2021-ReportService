using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output;
using ESFA.DC.ILR1819.ReportService.Interface.Builders;
using ESFA.DC.ILR1819.ReportService.Interface.Configuration;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.Configuration;
using ESFA.DC.ILR1819.ReportService.Service.Builders;
using ESFA.DC.ILR1819.ReportService.Service.Mapper;
using ESFA.DC.ILR1819.ReportService.Service.Reports;
using ESFA.DC.ILR1819.ReportService.Service.Service;
using ESFA.DC.ILR1819.ReportService.Stateless.Configuration;
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
    public sealed class TestFundingSummaryReport
    {
        [Fact]
        public async Task TestFundingSummaryReportGeneration()
        {
            string csv = string.Empty;
            byte[] xlsx = null;
            DateTime dateTime = DateTime.UtcNow;
            string filename = $"10033670_1_Funding Summary Report {dateTime:yyyyMMdd-HHmmss}";

            Mock<ILogger> logger = new Mock<ILogger>();
            Mock<IDateTimeProvider> dateTimeProviderMock = new Mock<IDateTimeProvider>();
            Mock<IStreamableKeyValuePersistenceService> storage = new Mock<IStreamableKeyValuePersistenceService>();
            Mock<IKeyValuePersistenceService> redis = new Mock<IKeyValuePersistenceService>();
            IJsonSerializationService jsonSerializationService = new JsonSerializationService();
            IXmlSerializationService xmlSerializationService = new XmlSerializationService();
            IIlrProviderService ilrProviderService = new IlrProviderService(logger.Object, storage.Object, xmlSerializationService, dateTimeProviderMock.Object, null);

            // Mock<IOrgProviderService> orgProviderService = new Mock<IOrgProviderService>();
            IOrgProviderService orgProviderService = new OrgProviderService(logger.Object, new OrgConfiguration() { OrgConnectionString = ConfigurationManager.AppSettings["OrgConnectionString"] });

            // Mock<ILarsProviderService> larsProviderService = new Mock<ILarsProviderService>();
            ILarsProviderService larsProviderService = new LarsProviderService(logger.Object, new LarsConfiguration() { LarsConnectionString = ConfigurationManager.AppSettings["LarsConnectionString"] });

            IEasProviderService easProviderService = new EasProviderService(logger.Object, new EasConfiguration() { EasConnectionString = ConfigurationManager.AppSettings["EasdbConnectionString"] });

            Mock <IPostcodeProviderService> postcodeProverServiceMock = new Mock<IPostcodeProviderService>();
            Mock<ILargeEmployerProviderService> largeEmployerProviderService = new Mock<ILargeEmployerProviderService>();
            IAllbProviderService allbProviderService = new AllbProviderService(logger.Object, redis.Object, storage.Object, jsonSerializationService, null);
            IFM35ProviderService fm35ProviderService = new FM35ProviderService(logger.Object, redis.Object, storage.Object, jsonSerializationService, null);
            IFM25ProviderService fm25ProviderService = new FM25ProviderService(logger.Object, redis.Object, storage.Object, jsonSerializationService, null);
            IFM36ProviderService fm36ProviderService = new FM36ProviderService(logger.Object, redis.Object, storage.Object, jsonSerializationService);
            IFM81TrailBlazerProviderService fm81TrailBlazerProviderService = new FM81TrailBlazerProviderService(logger.Object, redis.Object, storage.Object, jsonSerializationService);
            IValidLearnersService validLearnersService = new ValidLearnersService(logger.Object, redis.Object, storage.Object, jsonSerializationService);
            IStringUtilitiesService stringUtilitiesService = new StringUtilitiesService();
            Mock<IPeriodProviderService> periodProviderService = new Mock<IPeriodProviderService>();
            IVersionInfo versionInfo = new VersionInfo { ServiceReleaseVersion = "1.2.3.4.5" };
            ITotalBuilder totalBuilder = new TotalBuilder();
            IFm25Builder fm25Builder = new Fm25Builder();
            IFm35Builder fm35Builder = new Fm35Builder(totalBuilder, new CacheProviderService<LearningDelivery[]>());
            IFm36Builder fm36Builder = new Fm36Builder(totalBuilder, new CacheProviderService<ILR.FundingService.FM36.FundingOutput.Model.Output.LearningDelivery[]>());
            IFm81Builder fm81Builder = new Fm81Builder(totalBuilder, new CacheProviderService<ILR.FundingService.FM81.FundingOutput.Model.Output.LearningDelivery[]>());
            IAllbBuilder allbBuilder = new AllbBuilder(ilrProviderService, validLearnersService, allbProviderService, periodProviderService.Object, totalBuilder, stringUtilitiesService, logger.Object);
            IExcelStyleProvider excelStyleProvider = new ExcelStyleProvider();

            IEasBuilder easBuilder = new EasBuilder(easProviderService);

            storage.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>())).Callback<string, Stream, CancellationToken>((st, sr, ct) => File.OpenRead("ILR-10033670-1819-20180704-120055-03.xml").CopyTo(sr)).Returns(Task.CompletedTask);
            storage.Setup(x => x.SaveAsync($"{filename}.csv", It.IsAny<string>(), It.IsAny<CancellationToken>())).Callback<string, string, CancellationToken>((key, value, ct) => csv = value).Returns(Task.CompletedTask);
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
            storage.Setup(x => x.ContainsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
            redis.Setup(x => x.GetAsync("FundingAlbOutput", It.IsAny<CancellationToken>())).ReturnsAsync(File.ReadAllText("ALBOutput1000.json"));
            redis.Setup(x => x.GetAsync("ValidLearners", It.IsAny<CancellationToken>())).ReturnsAsync(jsonSerializationService.Serialize(
                new List<string>
                {
                    "3Addl103",
                    "4Addl103"
                }));
            redis.Setup(x => x.GetAsync("FundingFm35Output", It.IsAny<CancellationToken>())).ReturnsAsync(File.ReadAllText("Fm35.json"));
            redis.Setup(x => x.GetAsync("FundingFm25Output", It.IsAny<CancellationToken>())).ReturnsAsync(File.ReadAllText("Fm25.json"));
            periodProviderService.Setup(x => x.GetPeriod(It.IsAny<IJobContextMessage>(), It.IsAny<CancellationToken>())).ReturnsAsync(12);
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(dateTime);
            dateTimeProviderMock.Setup(x => x.ConvertUtcToUk(It.IsAny<DateTime>())).Returns(dateTime);
            //easProviderServiceMock.Setup(x => x.GetLastEasUpdate(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            //    .ReturnsAsync(DateTime.MinValue);
            largeEmployerProviderService.Setup(x => x.GetVersionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync("NA");
            postcodeProverServiceMock.Setup(x => x.GetVersionAsync(It.IsAny<CancellationToken>())).ReturnsAsync("NA");
            //orgProviderService
            //    .Setup(x => x.GetProviderName(It.IsAny<IJobContextMessage>(), It.IsAny<CancellationToken>()))
            //    .ReturnsAsync("Test Provider");

            ITopicAndTaskSectionOptions topicsAndTasks = TestConfigurationHelper.GetTopicsAndTasks();
            IValueProvider valueProvider = new ValueProvider();

            FundingSummaryReport fundingSummaryReport = new FundingSummaryReport(
                logger.Object,
                storage.Object,
                ilrProviderService,
                orgProviderService,
                allbProviderService,
                fm25ProviderService,
                fm35ProviderService,
                fm36ProviderService,
                fm81TrailBlazerProviderService,
                validLearnersService,
                stringUtilitiesService,
                periodProviderService.Object,
                dateTimeProviderMock.Object,
                valueProvider,
                larsProviderService,
                easProviderService,
                postcodeProverServiceMock.Object,
                largeEmployerProviderService.Object,
                allbBuilder,
                fm25Builder,
                fm35Builder,
                fm36Builder,
                fm81Builder,
                totalBuilder,
                versionInfo,
                excelStyleProvider,
                topicsAndTasks,
                easBuilder);

            IJobContextMessage jobContextMessage = new JobContextMessage(1, new ITopicItem[0], 0, DateTime.UtcNow);

            jobContextMessage.KeyValuePairs[JobContextMessageKey.UkPrn] = "10033670";
            jobContextMessage.KeyValuePairs[JobContextMessageKey.Filename] = "ILR-10033670-1819-20180704-120055-03";
            jobContextMessage.KeyValuePairs[JobContextMessageKey.FundingAlbOutput] = "FundingAlbOutput";
            jobContextMessage.KeyValuePairs[JobContextMessageKey.ValidLearnRefNumbers] = "ValidLearners";
            jobContextMessage.KeyValuePairs[JobContextMessageKey.FundingFm35Output] = "FundingFm35Output";
            jobContextMessage.KeyValuePairs[JobContextMessageKey.FundingFm25Output] = "FundingFm25Output";

            await fundingSummaryReport.GenerateReport(jobContextMessage, null, false, CancellationToken.None);

            csv.Should().NotBeNullOrEmpty();
            xlsx.Should().NotBeNullOrEmpty();

            File.WriteAllBytes($"{filename}.xlsx", xlsx);

            //var fundingSummaryHeaderMapper = new FundingSummaryHeaderMapper();
            //var fundingSummaryMapper = new FundingSummaryMapper();
            //var fundingSummaryFooterMapper = new FundingSummaryFooterMapper();
            //TestCsvHelper.CheckCsv(
            //    csv,
            //    new CsvEntry(fundingSummaryHeaderMapper, 1),
            //    new CsvEntry(fundingSummaryMapper, 0, "16-18 Traineeships Budget", 1),
            //    new CsvEntry(fundingSummaryMapper, 3, "16-18 Traineeships"),
            //    new CsvEntry(fundingSummaryMapper, 1, "16-18 Trailblazer Apprenticeships for starts before 1 May 2017", 1),
            //    new CsvEntry(fundingSummaryMapper, 0, "Advanced Loans Bursary Budget", 1),
            //    new CsvEntry(fundingSummaryMapper, 3, "Advanced Loans Bursary"),
            //    new CsvEntry(fundingSummaryMapper, 0, Constants.ALBInfoText),
            //    new CsvEntry(fundingSummaryFooterMapper, 1, blankRowsBefore: 1));

            //var fundingSummaryHeaderMapper = new FundingSummaryHeaderMapper();
            //var fundingSummaryMapper = new FundingSummaryMapper();
            //TestXlsxHelper.CheckXlsxWithTitles(
            //    xlsx,
            //    new XlsxEntry(fundingSummaryHeaderMapper, fundingSummaryHeaderMapper.GetMaxIndex(), true),
            //    new XlsxEntry(fundingSummaryMapper, 0, "16-18 Traineeships Budget", 1),
            //    new XlsxEntry(fundingSummaryMapper, 13, "16-18 Traineeships", 0),
            //    new XlsxEntry(fundingSummaryMapper, 0, "Carry-in Apprenticeships Budget (for starts before 1 May 2017 and non-procured delivery)", 1),
            //    new XlsxEntry(fundingSummaryMapper, 9, "16-18 Apprenticeship Frameworks for starts before 1 May 2017", 1),
            //    new XlsxEntry(fundingSummaryMapper, 9, "16-18 Trailblazer Apprenticeships for starts before 1 May 2017", 1),
            //    new XlsxEntry(fundingSummaryMapper, 18, "16-18 Non-Levy Contracted Apprenticeships - Non-procured delivery", 1),
            //    new XlsxEntry(fundingSummaryMapper, 9, "19-23 Apprenticeship Frameworks for starts before 1 May 2017", 1),
            //    new XlsxEntry(fundingSummaryMapper, 9, "19-23 Trailblazer Apprenticeships for starts before 1 May 2017", 0),
            //    new XlsxEntry(fundingSummaryMapper, 9, "24+ Apprenticeship Frameworks for starts before 1 May 2017", 1));
        }
    }
}
