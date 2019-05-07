using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.ReportService.Interface.Builders;
using ESFA.DC.ILR.ReportService.Interface.Configuration;
using ESFA.DC.ILR.ReportService.Interface.Context;
using ESFA.DC.ILR.ReportService.Interface.Provider;
using ESFA.DC.ILR.ReportService.Interface.Service;
using ESFA.DC.ILR.ReportService.Model.Configuration;
using ESFA.DC.ILR.ReportService.Model.ReportModels;
using ESFA.DC.ILR.ReportService.Service.Builders;
using ESFA.DC.ILR.ReportService.Service.Comparer;
using ESFA.DC.ILR.ReportService.Service.Mapper;
using ESFA.DC.ILR.ReportService.Service.Provider;
using ESFA.DC.ILR.ReportService.Service.Reports;
using ESFA.DC.ILR.ReportService.Service.Service;
using ESFA.DC.ILR.ReportService.Tests.AutoFac;
using ESFA.DC.ILR.ReportService.Tests.Helpers;
using ESFA.DC.ILR.ReportService.Tests.Models;
using ESFA.DC.ILR1819.DataStore.EF;
using ESFA.DC.ILR1819.DataStore.EF.Interface;
using ESFA.DC.ILR1819.DataStore.EF.Valid;
using ESFA.DC.ILR1819.DataStore.EF.Valid.Interface;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using ESFA.DC.Serialization.Xml;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using LearningDelivery = ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output.LearningDelivery;
using VersionInfo = ESFA.DC.ILR1819.ReportService.Stateless.Configuration.VersionInfo;

namespace ESFA.DC.ILR.ReportService.Tests.Reports
{
    public class TestSummaryOfFM35FundingReport
    {
        [Fact]
        public async Task TestSummaryOfFM35FundingReportGeneration()
        {
            string csv = string.Empty;
            DateTime dateTime = DateTime.UtcNow;
            string filename = $"10033670_1_Summary of Funding Model 35 Funding Report {dateTime:yyyyMMdd-HHmmss}";
            byte[] xlsx = null;

            Mock<ILogger> logger = new Mock<ILogger>();
            Mock<IStreamableKeyValuePersistenceService> storage = new Mock<IStreamableKeyValuePersistenceService>();
            Mock<IStreamableKeyValuePersistenceService> redis = new Mock<IStreamableKeyValuePersistenceService>();
            IJsonSerializationService jsonSerializationService = new JsonSerializationService();
            IXmlSerializationService xmlSerializationService = new XmlSerializationService();
            Mock<IDateTimeProvider> dateTimeProviderMock = new Mock<IDateTimeProvider>();

            Mock<ILarsProviderService> larsProviderService = new Mock<ILarsProviderService>();
            Mock<IOrgProviderService> orgProviderService = new Mock<IOrgProviderService>();
            Mock<IPostcodeProviderService> postcodeProverServiceMock = new Mock<IPostcodeProviderService>();
            Mock<ILargeEmployerProviderService> largeEmployerProviderService = new Mock<ILargeEmployerProviderService>();

            ITotalBuilder totalBuilder = new TotalBuilder();
            IFm35Builder fm35Builder = new Fm35Builder(totalBuilder, new CacheProviderService<LearningDelivery[]>());
            IValueProvider valueProvider = new ValueProvider();
            IVersionInfo versionInfo = new VersionInfo { ServiceReleaseVersion = "1.2.3.4.5" };

            Mock<IReportServiceContext> reportServiceContextMock = new Mock<IReportServiceContext>();
            reportServiceContextMock.SetupGet(x => x.JobId).Returns(1);
            reportServiceContextMock.SetupGet(x => x.SubmissionDateTimeUtc).Returns(DateTime.UtcNow);
            reportServiceContextMock.SetupGet(x => x.Ukprn).Returns(10033670);
            reportServiceContextMock.SetupGet(x => x.Filename).Returns("ILR-10033670-1819-20180704-120055-03");
            reportServiceContextMock.SetupGet(x => x.ValidLearnRefNumbersKey).Returns("ValidLearnRefNumbers");
            reportServiceContextMock.SetupGet(x => x.FundingFM35OutputKey).Returns("FundingFm35Output");
            reportServiceContextMock.SetupGet(x => x.CollectionName).Returns("ILR1819");

            redis.Setup(x => x.GetAsync("ValidLearnRefNumbers", It.IsAny<CancellationToken>())).ReturnsAsync(File.ReadAllText("ValidLearnRefNumbers.json"));
            redis.Setup(x => x.GetAsync("FundingFm35Output", It.IsAny<Stream>(), It.IsAny<CancellationToken>())).Callback<string, Stream, CancellationToken>((st, sr, ct) => File.OpenRead("Fm35.json").CopyTo(sr)).Returns(Task.CompletedTask);

            storage.Setup(x => x.GetAsync("ILR-10033670-1819-20180704-120055-03", It.IsAny<Stream>(), It.IsAny<CancellationToken>())).Callback<string, Stream, CancellationToken>((st, sr, ct) => File.OpenRead("ILR-10033670-1819-20180704-120055-03.xml").CopyTo(sr)).Returns(Task.CompletedTask);
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
            redis.Setup(x => x.ContainsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(dateTime);
            dateTimeProviderMock.Setup(x => x.ConvertUtcToUk(It.IsAny<DateTime>())).Returns(dateTime);
            largeEmployerProviderService.Setup(x => x.GetVersionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync("NA");
            postcodeProverServiceMock.Setup(x => x.GetVersionAsync(It.IsAny<CancellationToken>())).ReturnsAsync("NA");
            orgProviderService.Setup(x => x.GetProviderName(It.IsAny<IReportServiceContext>(), It.IsAny<CancellationToken>())).ReturnsAsync("Test Provider");
            larsProviderService.Setup(x => x.GetVersionAsync(It.IsAny<CancellationToken>())).ReturnsAsync("1.2.3.4.5");

            DataStoreConfiguration dataStoreConfiguration = new DataStoreConfiguration()
            {
                ILRDataStoreConnectionString = new TestConfigurationHelper().GetSectionValues<DataStoreConfiguration>("DataStoreSection").ILRDataStoreConnectionString,
                ILRDataStoreValidConnectionString = new TestConfigurationHelper().GetSectionValues<DataStoreConfiguration>("DataStoreSection").ILRDataStoreValidConnectionString
            };

            IIlr1819ValidContext IlrValidContextFactory()
            {
                var options = new DbContextOptionsBuilder<ILR1819_DataStoreEntitiesValid>().UseSqlServer(dataStoreConfiguration.ILRDataStoreValidConnectionString).Options;
                return new ILR1819_DataStoreEntitiesValid(options);
            }

            IIlr1819RulebaseContext IlrRulebaseContextFactory()
            {
                var options = new DbContextOptionsBuilder<ILR1819_DataStoreEntities>().UseSqlServer(dataStoreConfiguration.ILRDataStoreConnectionString).Options;
                return new ILR1819_DataStoreEntities(options);
            }

            IIlrProviderService ilrProviderService = new IlrFileServiceProvider(logger.Object, storage.Object, xmlSerializationService);
            IIlrMetadataProviderService ilrMetadataProviderService = new IlrMetadataProviderService(dateTimeProviderMock.Object, IlrValidContextFactory, IlrRulebaseContextFactory);

            IFM35ProviderService fm35ProviderService = new FM35ProviderService(logger.Object, storage.Object, jsonSerializationService, null, null);

            var summaryOfFm35FundingReport = new SummaryOfFm35FundingReport(
                logger.Object,
                storage.Object,
                ilrProviderService,
                ilrMetadataProviderService,
                fm35ProviderService,
                dateTimeProviderMock.Object,
                valueProvider,
                versionInfo,
                larsProviderService.Object,
                postcodeProverServiceMock.Object,
                largeEmployerProviderService.Object,
                orgProviderService.Object,
                fm35Builder);

            await summaryOfFm35FundingReport.GenerateReport(reportServiceContextMock.Object, null, false, CancellationToken.None);

            csv.Should().NotBeNullOrEmpty();
            xlsx.Should().NotBeNullOrEmpty();
#if DEBUG
            File.WriteAllBytes($"{filename}.csv", Encoding.ASCII.GetBytes(csv));
            File.WriteAllBytes($"{filename}.xlsx", xlsx);
#endif

            TestCsvHelper.CheckCsv(csv, new CsvEntry(new SummaryOfFM35FundingMapper(), 1));
        }

        [Theory]
        [InlineData("AEB – Other Learning (non-procured)", 5, "AEB – Other Learning (procured from Nov 2017)", 5, "AEB – Other Learning (procured from Nov 2017)", 5, "AEB – Other Learning (non-procured)", 5)]
        [InlineData("AEB – Other Learning (non-procured)", 6, "AEB – Other Learning (non-procured)", 5, "AEB – Other Learning (non-procured)", 5, "AEB – Other Learning (non-procured)", 6)]
        [InlineData("AEB – Other Learning (procured from Nov 2017)", 1, "AEB – Other Learning (non-procured)", 1, "19-24 Traineeship (procured from Nov 2017)", 1, "19-24 Traineeship (procured from Nov 2017)", 1, "AEB – Other Learning (non-procured)", 1, "AEB – Other Learning (procured from Nov 2017)", 1)]
        public void TestSummaryOfFm35FundingModelComparer(params object[] inputs)
        {
            SummaryOfFm35FundingModelComparer summaryOfFm35FundingModelComparer = new SummaryOfFm35FundingModelComparer();

            List<SummaryOfFm35FundingModel> SummaryOfFm35FundingModels = new List<SummaryOfFm35FundingModel>();
            List<string> expectedFundingLine = new List<string>();
            List<int> expectedPeriod = new List<int>();

            int resultsPointer = inputs.Length - (inputs.Length / 2);

            for (int i = 0; i < (inputs.Length / 2) / 2; i++)
            {
                SummaryOfFm35FundingModels.Add(new SummaryOfFm35FundingModel
                {
                    FundingLineType = inputs[i * 2].ToString(),
                    Period = Convert.ToInt32(inputs[(i * 2) + 1])
                });

                expectedFundingLine.Add(inputs[resultsPointer + (i * 2)].ToString());
                expectedPeriod.Add(Convert.ToInt32(inputs[resultsPointer + (i * 2) + 1]));
            }

            SummaryOfFm35FundingModels.Sort(summaryOfFm35FundingModelComparer);

            SummaryOfFm35FundingModels.Select(x => x.FundingLineType).Should().BeEquivalentTo(expectedFundingLine);
            SummaryOfFm35FundingModels.Select(x => x.Period).Should().BeEquivalentTo(expectedPeriod);
        }
    }
}
