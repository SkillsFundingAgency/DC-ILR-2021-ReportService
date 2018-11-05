using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output;
using ESFA.DC.ILR1819.ReportService.Interface.Builders;
using ESFA.DC.ILR1819.ReportService.Interface.Configuration;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;
using ESFA.DC.ILR1819.ReportService.Service.Builders;
using ESFA.DC.ILR1819.ReportService.Service.Comparer;
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
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR1819.ReportService.Tests.Reports
{
    public class TestSummaryOfFM35FundingReport
    {
        [Fact]
        public async Task TestSummaryOfFM35FundingReportGeneration()
        {
            string csv = string.Empty;
            DateTime dateTime = DateTime.UtcNow;
            string filename = $"10033670_1_Summary of Funding Model 35 Funding Report {dateTime:yyyyMMdd-HHmmss}";

            Mock<ILogger> logger = new Mock<ILogger>();
            Mock<IKeyValuePersistenceService> storage = new Mock<IKeyValuePersistenceService>();
            Mock<IKeyValuePersistenceService> redis = new Mock<IKeyValuePersistenceService>();
            IJsonSerializationService jsonSerializationService = new JsonSerializationService();
            IFM35ProviderService fm35ProviderService = new FM35ProviderService(logger.Object, redis.Object, storage.Object, jsonSerializationService);
            IStringUtilitiesService stringUtilitiesService = new StringUtilitiesService();
            Mock<IDateTimeProvider> dateTimeProviderMock = new Mock<IDateTimeProvider>();
            ITotalBuilder totalBuilder = new TotalBuilder();
            IFm35Builder fm35Builder = new Fm35Builder(totalBuilder, new CacheProviderService<LearningDelivery[]>());
            IValueProvider valueProvider = new ValueProvider();

            storage.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(File.ReadAllText("ILR-10033670-1819-20180704-120055-03.xml"));
            storage.Setup(x => x.SaveAsync($"{filename}.csv", It.IsAny<string>(), It.IsAny<CancellationToken>())).Callback<string, string, CancellationToken>((key, value, ct) => csv = value).Returns(Task.CompletedTask);
            redis.Setup(x => x.GetAsync("ValidLearners", It.IsAny<CancellationToken>())).ReturnsAsync(File.ReadAllText("Fm35.json"));

            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(dateTime);
            dateTimeProviderMock.Setup(x => x.ConvertUtcToUk(It.IsAny<DateTime>())).Returns(dateTime);

            ITopicAndTaskSectionOptions topicsAndTasks = TestConfigurationHelper.GetTopicsAndTasks();

            var summaryOfFm35FundingReport = new SummaryOfFm35FundingReport(
                logger.Object,
                storage.Object,
                fm35ProviderService,
                stringUtilitiesService,
                dateTimeProviderMock.Object,
                valueProvider,
                topicsAndTasks,
                fm35Builder);

            IJobContextMessage jobContextMessage = new JobContextMessage(1, new ITopicItem[0], 0, DateTime.UtcNow);
            jobContextMessage.KeyValuePairs[JobContextMessageKey.UkPrn] = "10033670";
            jobContextMessage.KeyValuePairs[JobContextMessageKey.Filename] = "ILR-10033670-1819-20180704-120055-03";
            jobContextMessage.KeyValuePairs[JobContextMessageKey.FundingFm35Output] = "ValidLearners";

            await summaryOfFm35FundingReport.GenerateReport(jobContextMessage, null, false, CancellationToken.None);

            csv.Should().NotBeNullOrEmpty();

            TestCsvHelper.CheckCsv(csv, new CsvEntry(new SummaryOfFM35FundingMapper(), 1));
        }

        [Theory]
        [InlineData("AEB – Other Learning (non-procured)", 5, "AEB – Other Learning (procured from Nov 2017)", 5, "AEB – Other Learning (procured from Nov 2017)", 5, "AEB – Other Learning (non-procured)", 5)]
        [InlineData("AEB – Other Learning (non-procured)", 6, "AEB – Other Learning (non-procured)", 5, "AEB – Other Learning (non-procured)", 5, "AEB – Other Learning (non-procured)", 6)]
        [InlineData("AEB – Other Learning (procured from Nov 2017)", 1, "AEB – Other Learning (non-procured)", 1, "19-24 Traineeship (procured from Nov 2017)", 1, "19-24 Traineeship (procured from Nov 2017)", 1, "AEB – Other Learning (non-procured)", 1, "AEB – Other Learning (procured from Nov 2017)", 1)]
        public async Task TestSummaryOfFm35FundingModelComparer(params object[] inputs)
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
