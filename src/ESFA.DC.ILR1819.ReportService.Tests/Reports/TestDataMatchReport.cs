using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Data.DAS.Model;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Configuration;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.DasCommitments;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;
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
using ESFA.DC.Serialization.Xml;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR1819.ReportService.Tests.Reports
{
    public sealed class TestDataMatchReport
    {
        [Fact]
        public async Task TestDataMatchReportGeneration()
        {
            string csv = string.Empty;
            DateTime dateTime = DateTime.UtcNow;
            string filename = $"10033670_1_Apprenticeship Data Match Report {dateTime:yyyyMMdd-HHmmss}";
            long ukPrn = 10033670;
            string ilr = "ILR-10033670-1819-20180704-120055-03";

            IJobContextMessage jobContextMessage = new JobContextMessage(1, new ITopicItem[0], 0, DateTime.UtcNow);
            jobContextMessage.KeyValuePairs[JobContextMessageKey.UkPrn] = 10033670;
            jobContextMessage.KeyValuePairs[JobContextMessageKey.Filename] = ilr;
            jobContextMessage.KeyValuePairs[JobContextMessageKey.ValidLearnRefNumbers] = "ValidLearners";
            jobContextMessage.KeyValuePairs[JobContextMessageKey.FundingFm36Output] = "FundingFm36Output";

            Mock<ILogger> logger = new Mock<ILogger>();
            Mock<IStreamableKeyValuePersistenceService> storage = new Mock<IStreamableKeyValuePersistenceService>();
            Mock<IKeyValuePersistenceService> redis = new Mock<IKeyValuePersistenceService>();
            IJsonSerializationService jsonSerializationService = new JsonSerializationService();
            IXmlSerializationService xmlSerializationService = new XmlSerializationService();
            IIlrProviderService ilrProviderService = new IlrProviderService(logger.Object, storage.Object, xmlSerializationService);
            IValidLearnersService validLearnersService = new ValidLearnersService(logger.Object, redis.Object, storage.Object, jsonSerializationService);
            IFM36ProviderService fm36ProviderService = new FM36ProviderService(logger.Object, redis.Object, storage.Object, jsonSerializationService);
            Mock<IDasCommitmentsService> dasCommitmentsService = new Mock<IDasCommitmentsService>();
            Mock<IPeriodProviderService> periodProviderService = new Mock<IPeriodProviderService>();
            IStringUtilitiesService stringUtilitiesService = new StringUtilitiesService();
            Mock<IDateTimeProvider> dateTimeProviderMock = new Mock<IDateTimeProvider>();

            List<DasCommitment> dasCommitments = new List<DasCommitment>
            {
                new DasCommitment(new DasCommitments
                {
                    Uln = 9900001906,
                    Ukprn = ukPrn,
                    //StandardCode = 0,
                    FrameworkCode = 421, // No match - 420
                    PathwayCode = 2, // No match - 1
                    ProgrammeType = 3, // No match - 2
                    AgreedCost = 1.80M, // No match?
                    StartDate = dateTime, // No match
                    PaymentStatus = (int)PaymentStatus.Active
                })
            };

            storage.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>())).Callback<string, Stream, CancellationToken>((st, sr, ct) => File.OpenRead($"{ilr}.xml").CopyTo(sr)).Returns(Task.CompletedTask);
            storage.Setup(x => x.SaveAsync($"{filename}.csv", It.IsAny<string>(), It.IsAny<CancellationToken>())).Callback<string, string, CancellationToken>((key, value, ct) => csv = value).Returns(Task.CompletedTask);
            redis.Setup(x => x.GetAsync("ValidLearners", It.IsAny<CancellationToken>())).ReturnsAsync(jsonSerializationService.Serialize(
                new List<string>
                {
                    "3DOB01"
                }));
            redis.Setup(x => x.GetAsync("FundingFm36Output", It.IsAny<CancellationToken>())).ReturnsAsync(File.ReadAllText("Fm36.json"));
            dasCommitmentsService
                .Setup(x => x.GetCommitments(It.IsAny<long>(), It.IsAny<List<long>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(dasCommitments);
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(dateTime);
            dateTimeProviderMock.Setup(x => x.ConvertUtcToUk(It.IsAny<DateTime>())).Returns(dateTime);

            ITopicAndTaskSectionOptions topicsAndTasks = TestConfigurationHelper.GetTopicsAndTasks();

            var report = new DataMatchReport(
                logger.Object,
                ilrProviderService,
                validLearnersService,
                fm36ProviderService,
                dasCommitmentsService.Object,
                periodProviderService.Object,
                storage.Object,
                dateTimeProviderMock.Object,
                topicsAndTasks);

            await report.GenerateReport(jobContextMessage, null, CancellationToken.None);

            csv.Should().NotBeNullOrEmpty();
            TestCsvHelper.CheckCsv(csv, new CsvEntry(new DataMatchReportMapper(), 1));
        }

        [Fact]
        public async Task TestDataCommitmentsComparer()
        {
            List<DasCommitments> dasCommitments = new List<DasCommitments>
            {
                new DasCommitments
                {
                    CommitmentId = 1,
                    VersionId = "2"
                },
                new DasCommitments
                {
                    CommitmentId = 1,
                    VersionId = "2"
                }
            };

            dasCommitments = dasCommitments.Distinct(new DasCommitmentsComparer()).ToList();
            Assert.Single(dasCommitments);
        }

        [Fact]
        public async Task TestDataMatchModelComparer1()
        {
            List<DataMatchModel> dataMatchModels = new List<DataMatchModel>
            {
                new DataMatchModel
                {
                    LearnRefNumber = "321",
                    AimSeqNumber = 321,
                    RuleName = "Rule_2"
                },
                new DataMatchModel
                {
                    LearnRefNumber = "123",
                    AimSeqNumber = 123,
                    RuleName = "Rule_1"
                }
            };

            dataMatchModels.Sort(new DataMatchModelComparer());
            Assert.Equal("123", dataMatchModels[0].LearnRefNumber);
        }

        [Fact]
        public async Task TestDataMatchModelComparer2()
        {
            List<DataMatchModel> dataMatchModels = new List<DataMatchModel>
            {
                new DataMatchModel
                {
                    LearnRefNumber = "321",
                    AimSeqNumber = 321,
                    RuleName = "Rule_2"
                },
                new DataMatchModel
                {
                    LearnRefNumber = "321",
                    AimSeqNumber = 123,
                    RuleName = "Rule_1"
                }
            };

            dataMatchModels.Sort(new DataMatchModelComparer());
            Assert.Equal(123, dataMatchModels[0].AimSeqNumber);
        }

        [Fact]
        public async Task TestDataMatchModelComparer3()
        {
            List<DataMatchModel> dataMatchModels = new List<DataMatchModel>
            {
                new DataMatchModel
                {
                    LearnRefNumber = "321",
                    AimSeqNumber = 321,
                    RuleName = "Rule_2"
                },
                new DataMatchModel
                {
                    LearnRefNumber = "321",
                    AimSeqNumber = 321,
                    RuleName = "Rule_1"
                }
            };

            dataMatchModels.Sort(new DataMatchModelComparer());
            Assert.Equal("Rule_1", dataMatchModels[0].RuleName);
        }
    }
}
