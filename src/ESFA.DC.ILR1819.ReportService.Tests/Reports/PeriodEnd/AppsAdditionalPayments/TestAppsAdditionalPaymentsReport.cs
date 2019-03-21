using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR1819.DataStore.EF;
using ESFA.DC.ILR1819.DataStore.EF.Interface;
using ESFA.DC.ILR1819.DataStore.EF.Valid;
using ESFA.DC.ILR1819.DataStore.EF.Valid.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Configuration;
using ESFA.DC.ILR1819.ReportService.Interface.Context;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.Configuration;
using ESFA.DC.ILR1819.ReportService.Model.PeriodEnd.AppsAdditionalPayment;
using ESFA.DC.ILR1819.ReportService.Service.Builders.PeriodEnd;
using ESFA.DC.ILR1819.ReportService.Service.Mapper.PeriodEnd;
using ESFA.DC.ILR1819.ReportService.Service.Service;
using ESFA.DC.ILR1819.ReportService.Tests.AutoFac;
using ESFA.DC.ILR1819.ReportService.Tests.Helpers;
using ESFA.DC.ILR1819.ReportService.Tests.Models;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using ESFA.DC.Serialization.Xml;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace ESFA.DC.ILR1819.ReportService.Tests.Reports.PeriodEnd.AppsAdditionalPayments
{
    public sealed class TestAppsAdditionalPaymentsReport
    {
        [Fact]
        public async Task TestAppsAdditionalPaymentsReportGeneration()
        {
            string csv = string.Empty;
            DateTime dateTime = DateTime.UtcNow;
            string filename = $"10036143_1_Apps Additional Payments Report {dateTime:yyyyMMdd-HHmmss}";

            Mock<IReportServiceContext> reportServiceContextMock = new Mock<IReportServiceContext>();
            reportServiceContextMock.SetupGet(x => x.JobId).Returns(1);
            reportServiceContextMock.SetupGet(x => x.SubmissionDateTimeUtc).Returns(DateTime.UtcNow);
            reportServiceContextMock.SetupGet(x => x.Ukprn).Returns(10036143);

            Mock<ILogger> logger = new Mock<ILogger>();
            Mock<IDateTimeProvider> dateTimeProviderMock = new Mock<IDateTimeProvider>();
            Mock<IStreamableKeyValuePersistenceService> storage = new Mock<IStreamableKeyValuePersistenceService>();
            Mock<IIlrProviderService> ilrProviderServiceMock = new Mock<IIlrProviderService>();
            Mock<IDASPaymentsProviderService> dasPaymentProviderMock = new Mock<IDASPaymentsProviderService>();
            Mock<IFM36ProviderService> fm36ProviderServiceMock = new Mock<IFM36ProviderService>();
            IValueProvider valueProvider = new ValueProvider();
            ITopicAndTaskSectionOptions topicsAndTasks = TestConfigurationHelper.GetTopicsAndTasks();
            storage.Setup(x => x.SaveAsync($"{filename}.csv", It.IsAny<string>(), It.IsAny<CancellationToken>())).Callback<string, string, CancellationToken>((key, value, ct) => csv = value).Returns(Task.CompletedTask);

            IIntUtilitiesService intUtilitiesService = new IntUtilitiesService();
            IJsonSerializationService jsonSerializationService = new JsonSerializationService();
            IXmlSerializationService xmlSerializationService = new XmlSerializationService();

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

            var appsAdditionalPaymentIlrInfo = BuildILRModel(10036143);
            var appsAdditionalPaymentRulebaseInfo = BuildFm36Model(10036143);
            var appsAdditionalPaymentDasPaymentsInfo = BuildDasPaymentsModel(10036143);

            ilrProviderServiceMock.Setup(x => x.GetILRInfoForAppsAdditionalPaymentsReportAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(appsAdditionalPaymentIlrInfo);
            fm36ProviderServiceMock.Setup(x => x.GetFM36DataForAppsAdditionalPaymentReportAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(appsAdditionalPaymentRulebaseInfo);
            //IIlrProviderService ilrProviderService = new IlrProviderService(logger.Object, storage.Object, xmlSerializationService, dateTimeProviderMock.Object, intUtilitiesService, IlrValidContextFactory, IlrRulebaseContextFactory);
            //IFM36ProviderService fm36ProviderService = new FM36ProviderService(logger.Object, storage.Object, jsonSerializationService, intUtilitiesService, IlrRulebaseContextFactory);
            IStringUtilitiesService stringUtilitiesService = new StringUtilitiesService();

            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(dateTime);
            dateTimeProviderMock.Setup(x => x.ConvertUtcToUk(It.IsAny<DateTime>())).Returns(dateTime);
            var appsAdditionalPaymentsModelBuilder = new AppsAdditionalPaymentsModelBuilder();

            var report = new ReportService.Service.Reports.PeriodEnd.AppsAdditionalPaymentsReport(logger.Object, storage.Object, ilrProviderServiceMock.Object, fm36ProviderServiceMock.Object, stringUtilitiesService, dateTimeProviderMock.Object, valueProvider, topicsAndTasks, dasPaymentProviderMock.Object, appsAdditionalPaymentsModelBuilder);

            await report.GenerateReport(reportServiceContextMock.Object, null, false, CancellationToken.None);

            csv.Should().NotBeNullOrEmpty();
            File.WriteAllText($"{filename}.csv", csv);
            TestCsvHelper.CheckCsv(csv, new CsvEntry(new AppsAdditionalPaymentsMapper(), 1));
        }

        private AppsAdditionalPaymentILRInfo BuildILRModel(int ukPrn)
        {
            //ILR-10036143-1819-20190318-154812-01.XML
            return new AppsAdditionalPaymentILRInfo()
            {
                UkPrn = ukPrn,
                Learners = new List<AppsAdditionalPaymentLearnerInfo>()
                {
                    new AppsAdditionalPaymentLearnerInfo()
                    {
                        LearnRefNumber = "A12345",
                        ULN = 12345,
                        LearningDeliveries = new List<AppsAdditionalPaymentLearningDeliveryInfo>()
                        {
                            new AppsAdditionalPaymentLearningDeliveryInfo()
                            {
                                UKPRN = ukPrn,
                                LearnRefNumber = "A12345",
                                LearnAimRef = "50117889",
                                AimType = 3,
                                AimSeqNumber = 1,
                                LearnStartDate = new DateTime(2017, 06, 28),
                                FundModel = 36,
                                ProgType = 1,
                                StdCode = 1,
                                FworkCode = 1,
                                PwayCode = 1
                            }
                        },
                        ProviderSpecLearnerMonitorings =
                            new List<AppsAdditionalPaymentProviderSpecLearnerMonitoringInfo>()
                            {
                                new AppsAdditionalPaymentProviderSpecLearnerMonitoringInfo()
                                {
                                    UKPRN = ukPrn,
                                    LearnRefNumber = "1",
                                    ProvSpecLearnMon = "A",
                                    ProvSpecLearnMonOccur = "T180400007"
                                },
                                new AppsAdditionalPaymentProviderSpecLearnerMonitoringInfo()
                                {
                                    UKPRN = ukPrn,
                                    LearnRefNumber = "1",
                                    ProvSpecLearnMon = "B",
                                    ProvSpecLearnMonOccur = "150563"
                                }
                            }
                    }
                }
            };
        }

        private AppsAdditionalPaymentRulebaseInfo BuildFm36Model(int ukPrn)
        {
            return new AppsAdditionalPaymentRulebaseInfo()
            {
                UkPrn = ukPrn,
                AECApprenticeshipPriceEpisodePeriodisedValues =
                    new List<AECApprenticeshipPriceEpisodePeriodisedValuesInfo>()
                    {
                        new AECApprenticeshipPriceEpisodePeriodisedValuesInfo()
                        {
                            UKPRN = ukPrn,
                            LearnRefNumber = "A12345",
                            AimSeqNumber = 1,
                            PriceEpisodeIdentifier = "1",
                            Period_1 = 10,
                            Period_2 = 20,
                            Period_3 = 30,
                            Period_4 = 40,
                            Period_5 = 50,
                            Period_6 = 60,
                            Period_7 = 70,
                            Period_8 = 80,
                            Period_9 = 90,
                            Period_10 = 100,
                            Period_11 = 110,
                            Period_12 = 120,
                            AttributeName = "PriceEpisodeFirstEmp1618Pay"
                        }
                    }
            };
        }

        private AppsAdditionalPaymentDasPaymentsInfo BuildDasPaymentsModel(int ukPrn)
        {
            return new AppsAdditionalPaymentDasPaymentsInfo()
            {
                UkPrn = ukPrn,
                Payments = new List<DASPaymentInfo>()
                {
                    new DASPaymentInfo()
                    {
                        UkPrn = ukPrn,
                        LearnerReferenceNumber = "A12345",
                        LearningAimReference = "50117889",
                        LearnerUln = 12345,
                        LearningStartDate = new DateTime(2017, 06, 28),
                        LearningAimProgrammeType = 1,
                        LearningAimStandardCode = 1,
                        LearningAimFrameworkCode = 1,
                        LearningAimPathwayCode = 1,
                        FundingSource = 1,
                        TransactionType = 4,
                        AcademicYear = 2018,
                        Amount = 10,
                        ContractType = 2,
                        CollectionPeriod = 1,
                        DeliveryPeriod = 1,
                        LearningAimFundingLineType = "16-18 Apprenticeship Non-Levy"
                    }
                }
            };
        }
    }
}