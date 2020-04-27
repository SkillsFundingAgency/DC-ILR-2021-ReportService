using System;
using System.Collections.Generic;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Models.ReferenceData;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.MetaData;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.Organisations;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Funding.CommunityLearning;
using ESFA.DC.ILR.ReportService.Reports.Funding.CommunityLearning.Model;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.Tests.Model;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding.CommunityLearning
{
    public class CommunityLearningReportModelBuilderTests
    {
        [Fact]
        public void BuildHeaderData()
        {
            var reportServiceContextMock = new Mock<IReportServiceContext>();

            reportServiceContextMock.Setup(r => r.IlrReportingFilename).Returns("10000000/ILR-2021-20190801-090000.xml");
            reportServiceContextMock.Setup(r => r.Ukprn).Returns(1);
            reportServiceContextMock.Setup(r => r.CollectionYear).Returns("2021");

            var referenceDataRoot = new ReferenceDataRoot
            {
                Organisations = new List<Organisation>
                {
                    new Organisation
                    {
                        UKPRN = 1,
                        Name = "OrgName"
                    }
                }
            };

            var expectedDictionary = new Dictionary<string, string>
            {
                {SummaryPageConstants.ProviderName, "OrgName"},
                {SummaryPageConstants.UKPRN, "1"},
                {SummaryPageConstants.ILRFile, "ILR-2021-20190801-090000.xml"},
                {SummaryPageConstants.Year, "2021"},
                {SummaryPageConstants.SecurityClassification, ReportingConstants.OfficialSensitive}
            };

            NewBuilder().BuildHeaderData(reportServiceContextMock.Object, referenceDataRoot).Should().BeEquivalentTo(expectedDictionary);
        }

        [Fact]
        public void BuildFooterData()
        {
            var date = new DateTime(2019, 8, 1);

            var dateTimeProviderMock = new Mock<IDateTimeProvider>();

            dateTimeProviderMock.Setup(d => d.GetNowUtc()).Returns(date);
            dateTimeProviderMock.Setup(d => d.ConvertUtcToUk(It.IsAny<DateTime>())).Returns(date);

            var reportServiceContextMock = new Mock<IReportServiceContext>();

            IMessage message = new TestMessage
            {
                HeaderEntity = new TestHeader
                {
                    CollectionDetailsEntity = new TestCollectionDetails
                    {
                        FilePreparationDate = new DateTime(2019, 8, 1)
                    }
                }
            };

            var referenceDataRoot = new ReferenceDataRoot
            {
                MetaDatas = new MetaData
                {
                    ReferenceDataVersions = new ReferenceDataVersion
                    {
                        OrganisationsVersion = new OrganisationsVersion { Version = "1" },
                        PostcodesVersion = new PostcodesVersion { Version = "1" },
                        Employers = new EmployersVersion { Version = "1" },
                        LarsVersion = new LarsVersion { Version = "1" }
                    }
                }
            };

            var expectedDictionary = new Dictionary<string, string>
            {
                { SummaryPageConstants.FilePreparationDate, "01/08/2019" },
                { SummaryPageConstants.LARSVersion, "1" },
                { SummaryPageConstants.PostcodeVersion, "1" },
                { SummaryPageConstants.OrganisationVersion, "1" },
                { SummaryPageConstants.LargeEmployersVersion, "1" },
                { SummaryPageConstants.ReportGeneratedAt, "00:00:00 on 01/08/2019" },
            };

            NewBuilder(dateTimeProviderMock.Object).BuildFooterData(reportServiceContextMock.Object, message, referenceDataRoot).Should().BeEquivalentTo(expectedDictionary);
        }

        [Theory]
        [InlineData("1")]
        [InlineData("2")]
        [InlineData("3")]
        [InlineData("4")]
        public void HasAnyASLFamTypeForFamCode_True(string famCode)
        {
            var learningDeliveryFams = new List<ILearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ASL",
                    LearnDelFAMCode = famCode
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "034"
                }
            };

            NewBuilder().HasAnyASLFamTypeForFamCode(learningDeliveryFams, famCode).Should().BeTrue();
        }

        [Theory]
        [InlineData("LDM", "5")]
        [InlineData("LDM", "4")]
        [InlineData("ASL", "5")]
        public void HasAnyASLFamTypeForFamCode_False(string famType, string famCode)
        {
            var learningDeliveryFams = new List<ILearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = famType,
                    LearnDelFAMCode = "4"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = famCode
                }
            };

            NewBuilder().HasAnyASLFamTypeForFamCode(learningDeliveryFams, famCode).Should().BeFalse();
        }

        [Theory]
        [InlineData(2020, 8, 1)]
        [InlineData(2020, 9, 1)]
        [InlineData(2021, 2, 1)]
        [InlineData(2021, 7, 31)]
        public void LearnStartDateIsWithinYear_True(int year, int month, int day)
        {
            NewBuilder().LearnStartDateIsWithinYear(new DateTime(year, month, day)).Should().BeTrue();
        }

        [Theory]
        [InlineData(2020, 7, 1)]
        [InlineData(2020, 7, 31)]
        [InlineData(2021, 8, 1)]
        [InlineData(2021, 12, 31)]
        public void LearnStartDateIsWithinYear_False(int year, int month, int day)
        {
            NewBuilder().LearnStartDateIsWithinYear(new DateTime(year, month, day)).Should().BeFalse();
        }

        [Fact]
        public void IsAdult_True()
        {
            NewBuilder().IsAdult(new DateTime(2000, 8, 1), new DateTime(2019, 8, 1)).Should().BeTrue();
        }

        [Fact]
        public void IsAdult_True_NullDoB()
        {
            NewBuilder().IsAdult(null, new DateTime(2019, 8, 1)).Should().BeTrue();
        }

        [Fact]
        public void IsAdult_False()
        {
            NewBuilder().IsAdult(new DateTime(2002, 8, 1), new DateTime(2019, 8, 1)).Should().BeFalse();
        }

        [Fact]
        public void IsSixteenToEighteen_True()
        {
            NewBuilder().IsSixteenToEighteen(new DateTime(2002, 8, 1), new DateTime(2019, 8, 1)).Should().BeTrue();
        }

        [Fact]
        public void IsSixteenToEighteen_False_NullDoB()
        {
            NewBuilder().IsSixteenToEighteen(null, new DateTime(2019, 8, 1)).Should().BeFalse();
        }

        [Fact]
        public void IsSixteenToEighteen_False()
        {
            NewBuilder().IsSixteenToEighteen(new DateTime(2000, 8, 1), new DateTime(2019, 8, 1)).Should().BeFalse();
        }

        [Fact]
        public void BuildCategoryData()
        {
            var message = TestMessage();

            var expectedData = new List<CommunityLearningData>
            {
                new CommunityLearningData
                {
                    LearnerRefNumber = "Learner1",
                    AimSeqNumber = 1,
                    LearnStartDate = new DateTime(2020, 8, 1),
                    EarliestStartDate = true,
                    EarliestStartDatePersonalAndCommunityDevelopmentLearning = false,
                    EarliestStartDateNeighbourhoodLearningInDeprivedCommunities = false,
                    EarliestStartDateFamilyEnglishMathsAndLanguage = false,
                    EarliestStartDateWiderFamilyLearning = true,
                    SixteenToEighteen = true,
                    Adult = false,
                    LearnStartDateIsInYear = true,
                    PersonalAndCommunityDevelopmentLearning = false,
                    NeighbourhoodLearningInDeprivedCommunities = false,
                    FamilyEnglishMathsAndLanguage = false,
                    WiderFamilyLearning = true,
                },
                new CommunityLearningData
                {
                    LearnerRefNumber = "Learner1",
                    AimSeqNumber = 2,
                    LearnStartDate = new DateTime(2020, 10, 1),
                    EarliestStartDate = false,
                    EarliestStartDatePersonalAndCommunityDevelopmentLearning = false,
                    EarliestStartDateNeighbourhoodLearningInDeprivedCommunities = true,
                    EarliestStartDateFamilyEnglishMathsAndLanguage = false,
                    EarliestStartDateWiderFamilyLearning = false,
                    SixteenToEighteen = false,
                    Adult = true,
                    LearnStartDateIsInYear = true,
                    PersonalAndCommunityDevelopmentLearning = false,
                    NeighbourhoodLearningInDeprivedCommunities = true,
                    FamilyEnglishMathsAndLanguage = false,
                    WiderFamilyLearning = false,
                },
                new CommunityLearningData
                {
                    LearnerRefNumber = "Learner2",
                    AimSeqNumber = 1,
                    LearnStartDate = new DateTime(2020, 8, 1),
                    EarliestStartDate = true,
                    EarliestStartDatePersonalAndCommunityDevelopmentLearning = true,
                    EarliestStartDateNeighbourhoodLearningInDeprivedCommunities = false,
                    EarliestStartDateFamilyEnglishMathsAndLanguage = false,
                    EarliestStartDateWiderFamilyLearning = false,
                    SixteenToEighteen = true,
                    Adult = false,
                    LearnStartDateIsInYear = true,
                    PersonalAndCommunityDevelopmentLearning = true,
                    NeighbourhoodLearningInDeprivedCommunities = false,
                    FamilyEnglishMathsAndLanguage = false,
                    WiderFamilyLearning = false,
                },
                new CommunityLearningData
                {
                    LearnerRefNumber = "Learner3",
                    AimSeqNumber = 1,
                    LearnStartDate = new DateTime(2020, 6, 1),
                    EarliestStartDate = true,
                    EarliestStartDatePersonalAndCommunityDevelopmentLearning = true,
                    EarliestStartDateNeighbourhoodLearningInDeprivedCommunities = false,
                    EarliestStartDateFamilyEnglishMathsAndLanguage = false,
                    EarliestStartDateWiderFamilyLearning = false,
                    SixteenToEighteen = false,
                    Adult = true,
                    LearnStartDateIsInYear = false,
                    PersonalAndCommunityDevelopmentLearning = true,
                    NeighbourhoodLearningInDeprivedCommunities = false,
                    FamilyEnglishMathsAndLanguage = false,
                    WiderFamilyLearning = false,
                },
            };

            NewBuilder().BuildCategoryData(message).Should().BeEquivalentTo(expectedData);
        }

        [Fact]
        public void BuildCategoryData_NoLearners()
        {
            var message = new TestMessage
            {
                Learners = new TestLearner[]
                {
                    new TestLearner
                    {
                        LearnRefNumber = "Learner1",
                        DateOfBirthNullable = new DateTime(2000, 9, 1),
                        LearningDeliveries = new TestLearningDelivery[]
                        {
                            new TestLearningDelivery
                            {
                                AimSeqNumber = 1,
                                FundModel = 35,
                                LearnStartDate = new DateTime(2020, 8, 1),
                                LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                                {
                                    new TestLearningDeliveryFAM
                                    {
                                        LearnDelFAMType = "ASL",
                                        LearnDelFAMCode = "4"
                                    },
                                    new TestLearningDeliveryFAM
                                    {
                                        LearnDelFAMType = "SOF",
                                        LearnDelFAMCode = "105"
                                    }
                                }
                            },
                        }
                    }
                }
            };

            var expectedData = new List<CommunityLearningData>();

            NewBuilder().BuildCategoryData(message).Should().BeEquivalentTo(expectedData);
        }

        [Fact]
        public void BuildModel()
        {
            var headerDictionary = new Dictionary<string, string>
            {
                {SummaryPageConstants.ProviderName, "OrgName"},
                {SummaryPageConstants.UKPRN, "1"},
                {SummaryPageConstants.ILRFile, "Filename"},
                {SummaryPageConstants.Year, "2021"},
                {SummaryPageConstants.SecurityClassification, ReportingConstants.OfficialSensitive}
            };

            var footerDictionary = new Dictionary<string, string>
            {
                { SummaryPageConstants.ApplicationVersion, "ReleaseVersion" },
                { SummaryPageConstants.FilePreparationDate, "01/08/2020 00:00:00" },
                { SummaryPageConstants.LARSVersion, "1" },
                { SummaryPageConstants.PostcodeVersion, "1" },
                { SummaryPageConstants.OrganisationVersion, "1" },
                { SummaryPageConstants.LargeEmployersVersion, "1" },
                { SummaryPageConstants.ReportGeneratedAt, "00:00:00 on 01/08/2020" }
            };

            var categoryData = new List<CommunityLearningData>
            {
                new CommunityLearningData
                {
                    LearnerRefNumber = "Learner1",
                    AimSeqNumber = 1,
                    LearnStartDate = new DateTime(2020, 8, 1),
                    EarliestStartDate = true,
                    EarliestStartDatePersonalAndCommunityDevelopmentLearning = true,
                    EarliestStartDateNeighbourhoodLearningInDeprivedCommunities = true,
                    EarliestStartDateFamilyEnglishMathsAndLanguage = true,
                    EarliestStartDateWiderFamilyLearning = true,
                    SixteenToEighteen = true,
                    Adult = false,
                    LearnStartDateIsInYear = true,
                    PersonalAndCommunityDevelopmentLearning = false,
                    NeighbourhoodLearningInDeprivedCommunities = false,
                    FamilyEnglishMathsAndLanguage = false,
                    WiderFamilyLearning = true,
                },
                new CommunityLearningData
                {
                    LearnerRefNumber = "Learner1",
                    AimSeqNumber = 2,
                    LearnStartDate = new DateTime(2020, 10, 1),
                    EarliestStartDate = false,
                    EarliestStartDatePersonalAndCommunityDevelopmentLearning = false,
                    EarliestStartDateNeighbourhoodLearningInDeprivedCommunities = false,
                    EarliestStartDateFamilyEnglishMathsAndLanguage = false,
                    EarliestStartDateWiderFamilyLearning = false,
                    SixteenToEighteen = false,
                    Adult = true,
                    LearnStartDateIsInYear = true,
                    PersonalAndCommunityDevelopmentLearning = false,
                    NeighbourhoodLearningInDeprivedCommunities = true,
                    FamilyEnglishMathsAndLanguage = false,
                    WiderFamilyLearning = false,
                },
                new CommunityLearningData
                {
                    LearnerRefNumber = "Learner2",
                    AimSeqNumber = 1,
                    LearnStartDate = new DateTime(2020, 8, 1),
                    EarliestStartDate = true,
                    EarliestStartDatePersonalAndCommunityDevelopmentLearning = true,
                    EarliestStartDateNeighbourhoodLearningInDeprivedCommunities = true,
                    EarliestStartDateFamilyEnglishMathsAndLanguage = true,
                    EarliestStartDateWiderFamilyLearning = true,
                    SixteenToEighteen = true,
                    Adult = false,
                    LearnStartDateIsInYear = true,
                    PersonalAndCommunityDevelopmentLearning = true,
                    NeighbourhoodLearningInDeprivedCommunities = false,
                    FamilyEnglishMathsAndLanguage = false,
                    WiderFamilyLearning = false,
                },
                new CommunityLearningData
                {
                    LearnerRefNumber = "Learner3",
                    AimSeqNumber = 1,
                    LearnStartDate = new DateTime(2020, 6, 1),
                    EarliestStartDate = true,
                    EarliestStartDatePersonalAndCommunityDevelopmentLearning = true,
                    EarliestStartDateNeighbourhoodLearningInDeprivedCommunities = true,
                    EarliestStartDateFamilyEnglishMathsAndLanguage = true,
                    EarliestStartDateWiderFamilyLearning = true,
                    SixteenToEighteen = false,
                    Adult = true,
                    LearnStartDateIsInYear = false,
                    PersonalAndCommunityDevelopmentLearning = true,
                    NeighbourhoodLearningInDeprivedCommunities = false,
                    FamilyEnglishMathsAndLanguage = false,
                    WiderFamilyLearning = false,
                },
            };

            var result = NewBuilder().BuildModel(categoryData, headerDictionary, footerDictionary);

            result.TotalCommunityLearning.TotalLearners.Should().Be(4);
            result.TotalCommunityLearning.TotalStartedInFundingYear.Should().Be(2);
            result.TotalCommunityLearning.TotalEnrolmentsInFundingYear.Should().Be(3);

            result.PersonalAndCommunityDevelopment.TotalLearners.Should().Be(2);
            result.PersonalAndCommunityDevelopment.TotalStartedInFundingYear.Should().Be(1);
            result.PersonalAndCommunityDevelopment.TotalEnrolmentsInFundingYear.Should().Be(1);

            result.NeigbourhoodLearning.TotalLearners.Should().Be(1);
            result.NeigbourhoodLearning.TotalStartedInFundingYear.Should().Be(0);
            result.NeigbourhoodLearning.TotalEnrolmentsInFundingYear.Should().Be(1);

            result.FamilyEnglishMaths.TotalLearners.Should().Be(0);
            result.FamilyEnglishMaths.TotalStartedInFundingYear.Should().Be(0);
            result.FamilyEnglishMaths.TotalEnrolmentsInFundingYear.Should().Be(0);

            result.WiderFamilyLearning.TotalLearners.Should().Be(1);
            result.WiderFamilyLearning.TotalStartedInFundingYear.Should().Be(1);
            result.WiderFamilyLearning.TotalEnrolmentsInFundingYear.Should().Be(1);
        }

        [Fact]
        public void BuildModel_NoLearners()
        {
            var headerDictionary = new Dictionary<string, string>
            {
                {SummaryPageConstants.ProviderName, "OrgName"},
                {SummaryPageConstants.UKPRN, "1"},
                {SummaryPageConstants.ILRFile, "Filename"},
                {SummaryPageConstants.Year, "2021"},
                {SummaryPageConstants.SecurityClassification, ReportingConstants.OfficialSensitive}
            };

            var footerDictionary = new Dictionary<string, string>
            {
                { SummaryPageConstants.ApplicationVersion, "ReleaseVersion" },
                { SummaryPageConstants.FilePreparationDate, "01/08/2020 00:00:00" },
                { SummaryPageConstants.LARSVersion, "1" },
                { SummaryPageConstants.PostcodeVersion, "1" },
                { SummaryPageConstants.OrganisationVersion, "1" },
                { SummaryPageConstants.LargeEmployersVersion, "1" },
                { SummaryPageConstants.ReportGeneratedAt, "00:00:00 on 01/08/2020" }
            };

            var categoryData = new List<CommunityLearningData>();

            var result = NewBuilder().BuildModel(categoryData, headerDictionary, footerDictionary);

            result.TotalCommunityLearning.TotalLearners.Should().Be(0);
            result.TotalCommunityLearning.TotalStartedInFundingYear.Should().Be(0);
            result.TotalCommunityLearning.TotalEnrolmentsInFundingYear.Should().Be(0);

            result.PersonalAndCommunityDevelopment.TotalLearners.Should().Be(0);
            result.PersonalAndCommunityDevelopment.TotalStartedInFundingYear.Should().Be(0);
            result.PersonalAndCommunityDevelopment.TotalEnrolmentsInFundingYear.Should().Be(0);

            result.NeigbourhoodLearning.TotalLearners.Should().Be(0);
            result.NeigbourhoodLearning.TotalStartedInFundingYear.Should().Be(0);
            result.NeigbourhoodLearning.TotalEnrolmentsInFundingYear.Should().Be(0);

            result.FamilyEnglishMaths.TotalLearners.Should().Be(0);
            result.FamilyEnglishMaths.TotalStartedInFundingYear.Should().Be(0);
            result.FamilyEnglishMaths.TotalEnrolmentsInFundingYear.Should().Be(0);

            result.WiderFamilyLearning.TotalLearners.Should().Be(0);
            result.WiderFamilyLearning.TotalStartedInFundingYear.Should().Be(0);
            result.WiderFamilyLearning.TotalEnrolmentsInFundingYear.Should().Be(0);
        }

        private IMessage TestMessage() => new TestMessage
        {
            Learners = new TestLearner[]
            {
                new TestLearner
                {
                    LearnRefNumber = "Learner1",
                    DateOfBirthNullable = new DateTime(2001, 9, 1),
                    LearningDeliveries = new TestLearningDelivery[]
                    {
                        new TestLearningDelivery
                        {
                            AimSeqNumber = 1,
                            FundModel = 10,
                            LearnStartDate = new DateTime(2020, 8, 1),
                            LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                            {
                                new TestLearningDeliveryFAM
                                {
                                    LearnDelFAMType = "ASL",
                                    LearnDelFAMCode = "4"
                                },
                                new TestLearningDeliveryFAM
                                {
                                    LearnDelFAMType = "SOF",
                                    LearnDelFAMCode = "105"
                                }
                            }
                        },
                        new TestLearningDelivery
                        {
                            AimSeqNumber = 2,
                            FundModel = 10,
                            LearnStartDate = new DateTime(2020, 10, 1),
                            LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                            {
                                new TestLearningDeliveryFAM
                                {
                                    LearnDelFAMType = "ASL",
                                    LearnDelFAMCode = "2"
                                },
                                new TestLearningDeliveryFAM
                                {
                                    LearnDelFAMType = "SOF",
                                    LearnDelFAMCode = "105"
                                }
                            }
                        }
                    }
                },
                new TestLearner
                {
                    LearnRefNumber = "Learner2",
                    DateOfBirthNullable = new DateTime(2001, 9, 1),
                    LearningDeliveries = new TestLearningDelivery[]
                    {
                        new TestLearningDelivery
                        {
                            AimSeqNumber = 1,
                            FundModel = 10,
                            LearnStartDate = new DateTime(2020, 8, 1),
                            LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                            {
                                new TestLearningDeliveryFAM
                                {
                                    LearnDelFAMType = "ASL",
                                    LearnDelFAMCode = "1"
                                },
                                new TestLearningDeliveryFAM
                                {
                                    LearnDelFAMType = "SOF",
                                    LearnDelFAMCode = "105"
                                }
                            }
                        },
                    }
                },
                new TestLearner
                {
                    LearnRefNumber = "Learner3",
                    DateOfBirthNullable = new DateTime(1991, 9, 1),
                    LearningDeliveries = new TestLearningDelivery[]
                    {
                        new TestLearningDelivery
                        {
                            AimSeqNumber = 1,
                            FundModel = 10,
                            LearnStartDate = new DateTime(2020, 6, 1),
                            LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                            {
                                new TestLearningDeliveryFAM
                                {
                                    LearnDelFAMType = "ASL",
                                    LearnDelFAMCode = "1"
                                },
                                new TestLearningDeliveryFAM
                                {
                                    LearnDelFAMType = "SOF",
                                    LearnDelFAMCode = "105"
                                }
                            }
                        },
                    }
                },
                new TestLearner
                {
                    LearnRefNumber = "Learner4",
                    DateOfBirthNullable = new DateTime(1991, 9, 1),
                    LearningDeliveries = new TestLearningDelivery[]
                    {
                        new TestLearningDelivery
                        {
                            AimSeqNumber = 1,
                            FundModel = 10,
                            LearnStartDate = new DateTime(2020, 6, 1),
                            LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                            {
                                new TestLearningDeliveryFAM
                                {
                                    LearnDelFAMType = "ASL",
                                    LearnDelFAMCode = "1"
                                },
                                new TestLearningDeliveryFAM
                                {
                                    LearnDelFAMType = "LDM",
                                    LearnDelFAMCode = "034"
                                }
                            }
                        },
                    }
                },
                 new TestLearner
                {
                    LearnRefNumber = "Learner5",
                    DateOfBirthNullable = new DateTime(1991, 9, 1),
                    LearningDeliveries = new TestLearningDelivery[]
                    {
                        new TestLearningDelivery
                        {
                            AimSeqNumber = 1,
                            FundModel = 70,
                            LearnStartDate = new DateTime(2020, 6, 1),
                            LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                            {
                                new TestLearningDeliveryFAM
                                {
                                    LearnDelFAMType = "ASL",
                                    LearnDelFAMCode = "1"
                                },
                                new TestLearningDeliveryFAM
                                {
                                    LearnDelFAMType = "SOF",
                                    LearnDelFAMCode = "105"
                                }
                            }
                        },
                    }
                },
            }
        };

        private CommunityLearningReportModelBuilder NewBuilder(IDateTimeProvider dateTimeProvider = null)
        {
            return new CommunityLearningReportModelBuilder(dateTimeProvider);
        }
    }
}
