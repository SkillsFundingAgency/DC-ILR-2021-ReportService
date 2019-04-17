using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.ReportService.Interface.Configuration;
using ESFA.DC.ILR.ReportService.Interface.Context;
using ESFA.DC.ILR.ReportService.Interface.Service;
using ESFA.DC.ILR.ReportService.Model.PeriodEnd.AppsMonthlyPayment;
using ESFA.DC.ILR.ReportService.Service.Builders.PeriodEnd;
using ESFA.DC.ILR.ReportService.Service.Mapper.PeriodEnd;
using ESFA.DC.ILR.ReportService.Service.Service;
using ESFA.DC.ILR.ReportService.Tests.AutoFac;
using ESFA.DC.ILR.ReportService.Tests.Helpers;
using ESFA.DC.ILR.ReportService.Tests.Models;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Tests.Reports.PeriodEnd.AppsMonthlyPayment
{
    public sealed class TestAppsMonthlyPaymentReport
    {
        [Fact]
        public async Task TestAppsMonthlyPaymentReportGeneration()
        {
            string csv = string.Empty;
            DateTime dateTime = DateTime.UtcNow;
            int ukPrn = 10036143;
            string filename = $"10036143_1_Apps Monthly Payment Report {dateTime:yyyyMMdd-HHmmss}";

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
            IStringUtilitiesService stringUtilitiesService = new StringUtilitiesService();

            var appsMonthlyPaymentIlrInfo = BuildILRModel(ukPrn);
            var appsMonthlyPaymentRulebaseInfo = BuildFm36Model(ukPrn);
            var appsMonthlyPaymentDasInfo = BuildDasPaymentsModel(ukPrn);

            ilrProviderServiceMock.Setup(x => x.GetILRInfoForAppsMonthlyPaymentReportAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(appsMonthlyPaymentIlrInfo);
            fm36ProviderServiceMock.Setup(x => x.GetFM36DataForAppsMonthlyPaymentReportAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(appsMonthlyPaymentRulebaseInfo);
            dasPaymentProviderMock.Setup(x => x.GetPaymentsInfoForAppsMonthlyPaymentReportAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(appsMonthlyPaymentDasInfo);

            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(dateTime);
            dateTimeProviderMock.Setup(x => x.ConvertUtcToUk(It.IsAny<DateTime>())).Returns(dateTime);
            var appsMonthlyPaymentModelBuilder = new AppsMonthlyPaymentModelBuilder();

            var report = new ReportService.Service.Reports.PeriodEnd.AppsMonthlyPaymentReport(
                logger.Object,
                storage.Object,
                ilrProviderServiceMock.Object,
                fm36ProviderServiceMock.Object,
                dasPaymentProviderMock.Object,
                stringUtilitiesService,
                dateTimeProviderMock.Object,
                valueProvider,
                topicsAndTasks,
                appsMonthlyPaymentModelBuilder);

            await report.GenerateReport(reportServiceContextMock.Object, null, false, CancellationToken.None);

            csv.Should().NotBeNullOrEmpty();
            File.WriteAllText($"{filename}.csv", csv);
            TestCsvHelper.CheckCsv(csv, new CsvEntry(new AppsMonthlyPaymentMapper(), 1));
        }

        private AppsMonthlyPaymentILRInfo BuildILRModel(int ukPrn)
        {
            return new AppsMonthlyPaymentILRInfo()
            {
                UkPrn = ukPrn,
                Learners = new List<AppsMonthlyPaymentLearnerInfo>()
                {
                    new AppsMonthlyPaymentLearnerInfo()
                    {
                        LearnRefNumber = "A12345",
                        CampId = "camp101",
                        LearningDeliveries = new List<AppsMonthlyPaymentLearningDeliveryInfo>
                        {
                            new AppsMonthlyPaymentLearningDeliveryInfo()
                            {
                                UKPRN = ukPrn,
                                LearnRefNumber = "A12345",
                                LearnAimRef = "50117889",
                                AimType = 3,
                                AimSeqNumber = 1,
                                LearnStartDate = new DateTime(2017, 06, 28),
                                ProgType = 1,
                                StdCode = 1,
                                FworkCode = 1,
                                PwayCode = 1
                            }
                        },
                        ProviderSpecLearnerMonitorings =
                            new List<AppsMonthlyPaymentProviderSpecLearnerMonitoringInfo>()
                            {
                                new AppsMonthlyPaymentProviderSpecLearnerMonitoringInfo()
                                {
                                    UKPRN = ukPrn,
                                    LearnRefNumber = "1",
                                    ProvSpecLearnMon = "A",
                                    ProvSpecLearnMonOccur = "T180400007"
                                },
                                new AppsMonthlyPaymentProviderSpecLearnerMonitoringInfo()
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

        private AppsMonthlyPaymentRulebaseInfo BuildFm36Model(int ukPrn)
        {
            return new AppsMonthlyPaymentRulebaseInfo()
            {
                UkPrn = ukPrn,
                LearnRefNumber = "A12345",
                AECApprenticeshipPriceEpisodes = new List<AECApprenticeshipPriceEpisodeInfo>()
                {
                    new AECApprenticeshipPriceEpisodeInfo()
                    {
                        LearnRefNumber = "A12345",
                        PriceEpisodeAgreeId = "PA101",
                        PriceEpisodeActualEndDate = new DateTime(2019, 06, 28),
                    }
                }
            };
        }

        private AppsMonthlyPaymentDASInfo BuildDasPaymentsModel(int ukPrn)
        {
            var appsMonthlyPaymentDasInfo = new AppsMonthlyPaymentDASInfo()
            {
                UkPrn = ukPrn
            };
            appsMonthlyPaymentDasInfo.Payments = new List<AppsMonthlyPaymentDASPaymentInfo>();
            for (byte i = 1; i < 14; i++)
            {
                var levyPayments = new AppsMonthlyPaymentDASPaymentInfo()
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
                    TransactionType = 2,
                    AcademicYear = 1819,
                    Amount = 11,
                    ContractType = 2,
                    CollectionPeriod = i,
                    DeliveryPeriod = 1,
                    LearningAimFundingLineType = "16-18 Apprenticeship Non-Levy"
                };

                var coInvestmentPayments = new AppsMonthlyPaymentDASPaymentInfo()
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
                    FundingSource = 2,
                    TransactionType = 2,
                    AcademicYear = 1819,
                    Amount = 12,
                    ContractType = 2,
                    CollectionPeriod = i,
                    DeliveryPeriod = 1,
                    LearningAimFundingLineType = "16-18 Apprenticeship Non-Levy"
                };

                var coInvestmentDueFromEmployerPayments = new AppsMonthlyPaymentDASPaymentInfo()
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
                    FundingSource = 3,
                    TransactionType = 2,
                    AcademicYear = 1819,
                    Amount = 13,
                    ContractType = 2,
                    CollectionPeriod = i,
                    DeliveryPeriod = 1,
                    LearningAimFundingLineType = "16-18 Apprenticeship Non-Levy"
                };

                var employerAdditionalPayments = new AppsMonthlyPaymentDASPaymentInfo()
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
                    FundingSource = 3,
                    TransactionType = 4,
                    AcademicYear = 1819,
                    Amount = 14,
                    ContractType = 2,
                    CollectionPeriod = i,
                    DeliveryPeriod = 1,
                    LearningAimFundingLineType = "16-18 Apprenticeship Non-Levy"
                };

                var providerAdditionalPayments = new AppsMonthlyPaymentDASPaymentInfo()
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
                    FundingSource = 3,
                    TransactionType = 5,
                    AcademicYear = 1819,
                    Amount = 15,
                    ContractType = 2,
                    CollectionPeriod = i,
                    DeliveryPeriod = 1,
                    LearningAimFundingLineType = "16-18 Apprenticeship Non-Levy"
                };

                var apprenticeAdditionalPayments = new AppsMonthlyPaymentDASPaymentInfo()
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
                    FundingSource = 3,
                    TransactionType = 16,
                    AcademicYear = 1819,
                    Amount = 16,
                    ContractType = 2,
                    CollectionPeriod = i,
                    DeliveryPeriod = 1,
                    LearningAimFundingLineType = "16-18 Apprenticeship Non-Levy"
                };

                var englishAndMathsPayments = new AppsMonthlyPaymentDASPaymentInfo()
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
                    FundingSource = 3,
                    TransactionType = 13,
                    AcademicYear = 1819,
                    Amount = 17,
                    ContractType = 2,
                    CollectionPeriod = i,
                    DeliveryPeriod = 1,
                    LearningAimFundingLineType = "16-18 Apprenticeship Non-Levy"
                };

                var paymentsForLearningSupport = new AppsMonthlyPaymentDASPaymentInfo()
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
                    FundingSource = 3,
                    TransactionType = 8,
                    AcademicYear = 1819,
                    Amount = 18,
                    ContractType = 2,
                    CollectionPeriod = i,
                    DeliveryPeriod = 1,
                    LearningAimFundingLineType = "16-18 Apprenticeship Non-Levy"
                };

                appsMonthlyPaymentDasInfo.Payments.Add(levyPayments);
                appsMonthlyPaymentDasInfo.Payments.Add(coInvestmentPayments);
                appsMonthlyPaymentDasInfo.Payments.Add(coInvestmentDueFromEmployerPayments);
                appsMonthlyPaymentDasInfo.Payments.Add(employerAdditionalPayments);
                appsMonthlyPaymentDasInfo.Payments.Add(providerAdditionalPayments);
                appsMonthlyPaymentDasInfo.Payments.Add(apprenticeAdditionalPayments);
                appsMonthlyPaymentDasInfo.Payments.Add(englishAndMathsPayments);
                appsMonthlyPaymentDasInfo.Payments.Add(paymentsForLearningSupport);
            }

            return appsMonthlyPaymentDasInfo;
        }
    }
}