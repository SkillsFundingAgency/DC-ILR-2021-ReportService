using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.ReportService.Interface.Context;
using ESFA.DC.ILR.ReportService.Interface.Provider;
using ESFA.DC.ILR.ReportService.Interface.Service;
using ESFA.DC.ILR.ReportService.Model.FCS;
using ESFA.DC.ILR.ReportService.Model.Lars;
using ESFA.DC.ILR.ReportService.Model.NonContractedAppsActivity;
using ESFA.DC.ILR.ReportService.Model.ReportModels;
using ESFA.DC.ILR.ReportService.Service.Builders;
using ESFA.DC.ILR.ReportService.Service.Mapper;
using ESFA.DC.ILR.ReportService.Service.Service;
using ESFA.DC.ILR.ReportService.Tests.Helpers;
using ESFA.DC.ILR.ReportService.Tests.Models;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Tests.Reports
{
    public sealed class TestNonContractedAppsActivityReport
    {
        [Fact]
        public async Task TestNonContractedAppsActivityReportGeneration()
        {
            string csv = string.Empty;
            DateTime dateTime = DateTime.UtcNow;
            string filename = $"10036143_1_Non-Contracted Apprenticeships Activity Report {dateTime:yyyyMMdd-HHmmss}";
            int ukPrn = 10036143;
            Mock<IReportServiceContext> reportServiceContextMock = new Mock<IReportServiceContext>();
            reportServiceContextMock.SetupGet(x => x.JobId).Returns(1);
            reportServiceContextMock.SetupGet(x => x.SubmissionDateTimeUtc).Returns(DateTime.UtcNow);
            reportServiceContextMock.SetupGet(x => x.Ukprn).Returns(ukPrn);

            Mock<ILogger> logger = new Mock<ILogger>();
            Mock<IDateTimeProvider> dateTimeProviderMock = new Mock<IDateTimeProvider>();
            Mock<IStreamableKeyValuePersistenceService> storage = new Mock<IStreamableKeyValuePersistenceService>();
            Mock<IIlrProviderService> ilrProviderServiceMock = new Mock<IIlrProviderService>();
            Mock<IFCSProviderService> fcsProviderMock = new Mock<IFCSProviderService>();
            Mock<IValidLearnersService> validLearnerServiceMock = new Mock<IValidLearnersService>();
            Mock<ILarsProviderService> larsProviderServiceMock = new Mock<ILarsProviderService>();
            Mock<IFM36NonContractedActivityProviderService> fm36ProviderServiceMock = new Mock<IFM36NonContractedActivityProviderService>();
            IValueProvider valueProvider = new ValueProvider();
            storage.Setup(x => x.SaveAsync($"{filename}.csv", It.IsAny<string>(), It.IsAny<CancellationToken>())).Callback<string, string, CancellationToken>((key, value, ct) => csv = value).Returns(Task.CompletedTask);

            var nonContractedAppsActivityIlrInfo = BuildIlrModel(ukPrn);
            var nonContractedActivityRuleBaseInfo = BuildFm36Model(ukPrn);
            var contractAllocationInfo = BuildFcsModel();
            var larsLearningDeliveries = BuildLarsModel();

            ilrProviderServiceMock.Setup(x => x.GetILRInfoForNonContractedAppsActivityReportAsync(It.IsAny<List<string>>(), reportServiceContextMock.Object, It.IsAny<CancellationToken>())).ReturnsAsync(nonContractedAppsActivityIlrInfo);
            fm36ProviderServiceMock.Setup(x => x.GetFM36InfoForNonContractedActivityReportAsync(It.IsAny<List<string>>(), reportServiceContextMock.Object, It.IsAny<CancellationToken>())).ReturnsAsync(nonContractedActivityRuleBaseInfo);
            fcsProviderMock
                .Setup(x => x.GetContractAllocationsForProviderAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(contractAllocationInfo);
            larsProviderServiceMock.Setup(x =>
                x.GetLearningDeliveriesAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>())).ReturnsAsync(larsLearningDeliveries);
            validLearnerServiceMock
                .Setup(x => x.GetLearnersAsync(reportServiceContextMock.Object, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<string>() { "fm36 18 20" });

            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(dateTime);
            dateTimeProviderMock.Setup(x => x.ConvertUtcToUk(It.IsAny<DateTime>())).Returns(dateTime);
            var nonContractedAppsActivityModel = new NonContractedAppsActivityModelBuilder(new PeriodProviderService());

            var report = new ReportService.Service.Reports.NonContractedAppsActivityReport(
                logger.Object,
                storage.Object,
                ilrProviderServiceMock.Object,
                fcsProviderMock.Object,
                validLearnerServiceMock.Object,
                fm36ProviderServiceMock.Object,
                larsProviderServiceMock.Object,
                dateTimeProviderMock.Object,
                valueProvider,
                nonContractedAppsActivityModel);

            await report.GenerateReport(reportServiceContextMock.Object, null, false, CancellationToken.None);

            csv.Should().NotBeNullOrEmpty();
            File.WriteAllText($"{filename}.csv", csv);
            TestCsvHelper.CheckCsv(csv, new CsvEntry(new NonContractedAppsActivityMapper(), 1));
            IEnumerable<NonContractedAppsActivityModel> result;

            using (var reader = new StreamReader($"{filename}.csv"))
            {
                using (var csvReader = new CsvReader(reader))
                {
                    csvReader.Configuration.RegisterClassMap<NonContractedAppsActivityMapper>();
                    result = csvReader.GetRecords<NonContractedAppsActivityModel>().ToList();
                }
            }

            result.Should().NotBeNullOrEmpty();
            result.Count().Should().Be(3);
            result.First().UniqueLearnerNumber.Should().Be(1000000205);
            result.First().AgeAtProgrammeStart.Should().Be(25);
            result.First().AimSeqNumber.Should().Be(1);
            result.First().AimType.Should().Be(1);
            result.First().AppAdjLearnStartDate.Should().Be("01/11/2018");
            result.First().ApprenticeshipPathway.Should().Be(2);
            result.First().CampusIdentifier.Should().Be("ZZ123456");
            result.First().DateOfBirth.Should().Be("15/05/1982");
            result.First().FrameworkCode.Should().Be(563);
            result.First().FundingLineType.Should().Be("19+ Apprenticeship Non-Levy Contract (procured)");
            result.First().LearnerReferenceNumber.Should().Be("fm36 18 20");
            result.First().LearningAimReference.Should().Be("ZPROG001");
            result.First().LearningAimTitle.Should().Be("LearnAimTitle");
            result.First().LearningDeliveryFAMTypeACTDateAppliesFrom.Should().Be("01/01/2019");
            result.First().LearningDeliveryFAMTypeACTDateAppliesTo.Should().Be(string.Empty);
            result.First().LearningDeliveryFAMTypeApprenticeshipContractType.Should().Be("2");
            result.First().LearningPlannedEndDate.Should().Be("31/03/2020");
            result.First().LearningStartDate.Should().Be("01/11/2018");
            result.First().OriginalLearningStartDate.Should().Be("01/11/2018");
            result.First().PriceEpisodeActualEndDate.Should().Be("31/07/2019");
            result.First().ProgrammeType.Should().Be(21);
            result.First().SoftwareSupplierAimIdentifier.Should().Be("83282eb2aa2230439a9964374c163b9c");
            result.First().AugustTotalEarnings.Should().Be(1);
            result.First().SeptemberTotalEarnings.Should().Be(2);
            result.First().OctoberTotalEarnings.Should().Be(3);
            result.First().NovemberTotalEarnings.Should().Be(4);
            result.First().DecemberTotalEarnings.Should().Be(5);
            result.First().JanuaryTotalEarnings.Should().Be(6);
            result.First().FebruaryTotalEarnings.Should().Be(7);
            result.First().MarchTotalEarnings.Should().Be(8);
            result.First().AprilTotalEarnings.Should().Be(9);
            result.First().MayTotalEarnings.Should().Be(10);
            result.First().JuneTotalEarnings.Should().Be(11);
            result.First().JulyTotalEarnings.Should().Be(12);
            result.First().TotalEarnings.Should().Be(78);

            result.ElementAt(1).ProgrammeType.Should().Be(25);
            result.ElementAt(1).StandardCode.Should().Be(23);
            result.ElementAt(1).ApprenticeshipPathway.Should().Be(2);
            result.ElementAt(1).LearningAimReference.Should().Be("50086832");
            result.ElementAt(1).FundingLineType.Should().Be("19+ Apprenticeship (From May 2017) Levy Contract");
            result.ElementAt(1).LearningDeliveryFAMTypeApprenticeshipContractType.Should().Be("1");
            result.ElementAt(1).LearningDeliveryFAMTypeACTDateAppliesFrom.Should().Be("01/11/2018");
            result.ElementAt(1).LearningDeliveryFAMTypeACTDateAppliesTo.Should().Be("31/12/2018");

            result.ElementAt(2).ProgrammeType.Should().Be(25);
            result.ElementAt(2).StandardCode.Should().Be(23);
            result.ElementAt(2).ApprenticeshipPathway.Should().Be(2);
            result.ElementAt(2).LearningAimReference.Should().Be("50086832");
            result.ElementAt(2).FundingLineType.Should().Be("19+ Apprenticeship Non-Levy Contract (procured)");
            result.ElementAt(2).LearningDeliveryFAMTypeApprenticeshipContractType.Should().Be("2");
            result.ElementAt(2).LearningDeliveryFAMTypeACTDateAppliesFrom.Should().Be("01/01/2019");
            result.ElementAt(2).LearningDeliveryFAMTypeACTDateAppliesTo.Should().Be(string.Empty);

            csv.Should().NotBeNullOrEmpty();
            File.WriteAllText($"{filename}.csv", csv);
            TestCsvHelper.CheckCsv(csv, new CsvEntry(new NonContractedAppsActivityMapper(), 1));
        }

        private Dictionary<string, LarsLearningDelivery> BuildLarsModel()
        {
            return new Dictionary<string, LarsLearningDelivery>()
            {
                {
                    "ZPROG001", new LarsLearningDelivery() { LearningAimTitle = "LearnAimTitle" }
                }
            };
        }

        private NonContractedActivityRuleBaseInfo BuildFm36Model(int ukPrn)
        {
            return new NonContractedActivityRuleBaseInfo()
            {
                UkPrn = ukPrn,
                PriceEpisodes = new List<PriceEpisodeInfo>()
                {
                    new PriceEpisodeInfo()
                    {
                        PriceEpisodeValues = new PriceEpisodeValuesInfo()
                        {
                            PriceEpisodeFundLineType = "19+ Apprenticeship Non-Levy Contract (procured)",
                            PriceEpisodeAimSeqNumber = 1,
                            EpisodeStartDate = new DateTime(2019, 02, 01),
                            PriceEpisodeActualEndDate = new DateTime(2019, 07, 31)
                        },
                        AECApprenticeshipPriceEpisodePeriodisedValues = new List<AECApprenticeshipPriceEpisodePeriodisedValuesInfo>()
                        {
                            new AECApprenticeshipPriceEpisodePeriodisedValuesInfo()
                            {
                                LearnRefNumber = "fm36 18 20",
                                AimSeqNumber = 1,
                                PriceEpisodeIdentifier = "25-23-01/11/2018",
                                AttributeName = "PriceEpisodeApplic1618FrameworkUpliftBalancing",
                                Periods = new decimal[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 }
                            }
                        }
                    }
                },
                AECLearningDeliveries = new List<AECLearningDeliveryInfo>()
                {
                    new AECLearningDeliveryInfo()
                    {
                        LearnRefNumber = "fm36 18 20",
                        AimSeqNumber = 1,
                        LearningDeliveryValues = new AECLearningDeliveryValuesInfo()
                        {
                            LearnDelMathEng = false,
                            LearnAimRef = "ZPROG001",
                            AppAdjLearnStartDate = new DateTime(2018, 11, 01),
                            AgeAtProgStart = 25,
                            LearnDelInitialFundLineType = "19+ Apprenticeship (From May 2017) Levy Contract"
                        },
                        LearningDeliveryPeriodisedValues = new List<AECApprenticeshipLearningDeliveryPeriodisedValuesInfo>()
                        {
                            new AECApprenticeshipLearningDeliveryPeriodisedValuesInfo()
                            {
                                AttributeName = "DisadvFirstPayment",
                                Periods = new decimal[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 }
                            }
                        }
                    },
                    new AECLearningDeliveryInfo()
                    {
                        LearnRefNumber = "fm36 18 20",
                        AimSeqNumber = 2,
                        LearningDeliveryValues = new AECLearningDeliveryValuesInfo()
                        {
                            LearnDelMathEng = true,
                            LearnAimRef = "50086832",
                            AppAdjLearnStartDate = new DateTime(2018, 11, 01),
                            AgeAtProgStart = 25,
                        },
                        LearningDeliveryPeriodisedValues = new List<AECApprenticeshipLearningDeliveryPeriodisedValuesInfo>()
                        {
                            new AECApprenticeshipLearningDeliveryPeriodisedValuesInfo()
                            {
                                AttributeName = "MathEngOnProgPayment",
                                Periods = new decimal[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 }
                            },
                            new AECApprenticeshipLearningDeliveryPeriodisedValuesInfo()
                            {
                                AttributeName = "MathEngBalPayment",
                                Periods = new decimal[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 }
                            },
                            new AECApprenticeshipLearningDeliveryPeriodisedValuesInfo()
                            {
                                AttributeName = "LearnSuppFundCash",
                                Periods = new decimal[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 }
                            }
                        },
                        LearningDeliveryPeriodisedTextValues = new AECApprenticeshipLearningDeliveryPeriodisedTextValuesInfo()
                        {
                            AttributeName = "FundingLineType",
                            Periods = new string[]
                            {
                                "None",
                                "None",
                                "None",
                                "19+ Apprenticeship (From May 2017) Levy Contract",
                                "19+ Apprenticeship (From May 2017) Levy Contract",
                                "19+ Apprenticeship Non-Levy Contract (procured)",
                                "19+ Apprenticeship Non-Levy Contract (procured)",
                                "19+ Apprenticeship Non-Levy Contract (procured)",
                                "19+ Apprenticeship Non-Levy Contract (procured)",
                                "19+ Apprenticeship Non-Levy Contract (procured)",
                                "19+ Apprenticeship Non-Levy Contract (procured)",
                                "19+ Apprenticeship Non-Levy Contract (procured)",
                            }
                        }
                    }
                }
            };
        }

        private NonContractedAppsActivityILRInfo BuildIlrModel(int ukPrn)
        {
            var learnerInfo = new NonContractedAppsActivityLearnerInfo
            {
                LearnRefNumber = "fm36 18 20",
                UniqueLearnerNumber = 1000000205,
                DateOfBirth = new DateTime(1982, 05, 15),
                CampId = "ZZ123456",
                LearningDeliveries = new List<NonContractedAppsActivityLearningDeliveryInfo>()
                {
                   new NonContractedAppsActivityLearningDeliveryInfo()
                   {
                       AimSeqNumber = 1,
                       LearnAimRef = "ZPROG001",
                       FundModel = 36,
                       UKPRN = ukPrn,
                       AimType = 1,
                       SWSupAimId = "83282eb2aa2230439a9964374c163b9c",
                       OriginalLearnStartDate = new DateTime(2018, 11, 01),
                       LearnStartDate = new DateTime(2018, 11, 01),
                       LearningPlannedEndDate = new DateTime(2020, 03, 31),
                       ProgType = 21,
                       StdCode = null,
                       FworkCode = 563,
                       PwayCode = 2,
                       EPAOrganisation = null,
                       PartnerUkPrn = null,
                       ProviderSpecDeliveryMonitorings = new List<NonContractedAppsActivityProviderSpecDeliveryMonitoringInfo>()
                           {
                                   new NonContractedAppsActivityProviderSpecDeliveryMonitoringInfo()
                                   {
                                       ProvSpecDelMon = "A",
                                       ProvSpecDelMonOccur = "1"
                                   },
                             },
                       LearningDeliveryFams = new List<NonContractedAppsActivityLearningDeliveryFAMInfo>()
                       {
                           new NonContractedAppsActivityLearningDeliveryFAMInfo()
                           {
                               LearnDelFAMType = "ACT",
                               LearnDelFAMCode = "1",
                               LearnDelFAMAppliesFrom = new DateTime(2018, 11, 01),
                               LearnDelFAMAppliesTo = new DateTime(2018, 12, 31)
                           },
                           new NonContractedAppsActivityLearningDeliveryFAMInfo()
                           {
                               LearnDelFAMType = "ACT",
                               LearnDelFAMCode = "2",
                               LearnDelFAMAppliesFrom = new DateTime(2019, 01, 01)
                           }
                       }
                   },
                     new NonContractedAppsActivityLearningDeliveryInfo()
                   {
                       AimSeqNumber = 2,
                       LearnAimRef = "50086832",
                       FundModel = 36,
                       UKPRN = ukPrn,
                       AimType = 3,
                       SWSupAimId = "83282eb2aa2230439a9964374c163b9c",
                       OriginalLearnStartDate = new DateTime(2018, 11, 01),
                       LearnStartDate = new DateTime(2018, 11, 01),
                       LearningPlannedEndDate = new DateTime(2020, 03, 31),
                       ProgType = 25,
                       StdCode = 23,
                       FworkCode = null,
                       PwayCode = 2,
                       EPAOrganisation = null,
                       PartnerUkPrn = null,
                       ProviderSpecDeliveryMonitorings = new List<NonContractedAppsActivityProviderSpecDeliveryMonitoringInfo>()
                           {
                                   new NonContractedAppsActivityProviderSpecDeliveryMonitoringInfo()
                                   {
                                       ProvSpecDelMon = "A",
                                       ProvSpecDelMonOccur = "1"
                                   },
                             },
                       LearningDeliveryFams = new List<NonContractedAppsActivityLearningDeliveryFAMInfo>()
                       {
                           new NonContractedAppsActivityLearningDeliveryFAMInfo()
                           {
                               LearnDelFAMType = "SOF",
                               LearnDelFAMCode = "105"
                           },
                           new NonContractedAppsActivityLearningDeliveryFAMInfo()
                           {
                               LearnDelFAMType = "ACT",
                               LearnDelFAMCode = "1",
                               LearnDelFAMAppliesFrom = new DateTime(2018, 11, 01),
                               LearnDelFAMAppliesTo = new DateTime(2018, 12, 31)
                           },
                           new NonContractedAppsActivityLearningDeliveryFAMInfo()
                           {
                               LearnDelFAMType = "ACT",
                               LearnDelFAMCode = "2",
                               LearnDelFAMAppliesFrom = new DateTime(2019, 01, 01)
                           }
                       }
                   }
                }
            };

            return new NonContractedAppsActivityILRInfo()
            {
                UkPrn = ukPrn,
                Learners = new List<NonContractedAppsActivityLearnerInfo>()
                {
                    learnerInfo
                }
            };
        }

        private List<ContractAllocationInfo> BuildFcsModel()
        {
            return new List<ContractAllocationInfo>();
        }
    }
}