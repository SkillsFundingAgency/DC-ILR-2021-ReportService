using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.FundingService.FM25.Model.Output;
using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReferenceDataService.Model.MetaData;
using ESFA.DC.ILR.ReferenceDataService.Model.MetaData.ReferenceDataVersions;
using ESFA.DC.ILR.ReferenceDataService.Model.Organisations;
using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.HighNeedsStudentSummary;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.Tests.Model;
using Moq;
using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding.HighNeedsStudentSummary
{
    public class HighNeedsStudentSummaryReportModelBuilderTests
    {
        [Fact]
        public void Build()
        {
            var dependentDataMock = new Mock<IReportServiceDependentData>();

            var learningDeliveryFam = new Mock<ILearningDeliveryFAM>();

            learningDeliveryFam.SetupGet(fam => fam.LearnDelFAMType).Returns("SOF");
            learningDeliveryFam.SetupGet(fam => fam.LearnDelFAMCode).Returns("107");

            var albLearningDeliveryFam = new Mock<ILearningDeliveryFAM>();

            albLearningDeliveryFam.SetupGet(fam => fam.LearnDelFAMType).Returns("ALB");

            var learningDeliveryFams = new List<ILearningDeliveryFAM>()
            {
                learningDeliveryFam.Object,
                albLearningDeliveryFam.Object
            };

            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = 25,
                LearnAimRef = "learnAimRef",
                AimSeqNumber = 1,
                LearningDeliveryFAMs = learningDeliveryFams,
                StdCodeNullable = 1
            };

            var message = new TestMessage()
            {
                Learners = BuildLearners(learningDelivery),
                HeaderEntity = new TestHeader()
                {
                    CollectionDetailsEntity = new MessageHeaderCollectionDetails()
                    {
                        FilePreparationDate = new DateTime(2019, 11, 06)
                    }
                }
            };

            var referenceDataRoot = new ReferenceDataRoot()
            {
                Organisations = new List<Organisation>()
                {
                    new Organisation()
                    {
                        UKPRN = 987654321,
                        Name = "Provider XYZ"
                    }
                },
                MetaDatas = new MetaData()
                {
                    ReferenceDataVersions = new ReferenceDataVersion()
                    {
                        OrganisationsVersion = new OrganisationsVersion("1.1.1.1"),
                        Employers = new EmployersVersion("2.2.2.2"),
                        LarsVersion = new LarsVersion("3.3.3.3"),
                        PostcodesVersion = new PostcodesVersion("4.4.4.4")
                    }
                }
            };

            var fm25Global = new FM25Global()
            {
                Learners = BuildFm25Learners()
            };

            dependentDataMock.Setup(d => d.Get<IMessage>()).Returns(message);
            dependentDataMock.Setup(d => d.Get<ReferenceDataRoot>()).Returns(referenceDataRoot);
            dependentDataMock.Setup(d => d.Get<FM25Global>()).Returns(fm25Global);

            var submissionDateTime = new DateTime(2019, 1, 1, 1, 1, 1);
            var ukDateTime = new DateTime(2020, 1, 1, 1, 1, 1);
            var dateTimeProvider = new Mock<IDateTimeProvider>();
            var reportServiceContextMock = new Mock<IReportServiceContext>();

            reportServiceContextMock.SetupGet(c => c.Ukprn).Returns(987654321);
            reportServiceContextMock.SetupGet(c => c.SubmissionDateTimeUtc).Returns(submissionDateTime);
            reportServiceContextMock.SetupGet(c => c.ServiceReleaseVersion).Returns("11.22.3300.4321");
            reportServiceContextMock.SetupGet(c => c.Filename).Returns("ILR-12345678-1920-20191005-151322-01.xml");

            dateTimeProvider.Setup(p => p.ConvertUtcToUk(submissionDateTime)).Returns(ukDateTime);
            dateTimeProvider.Setup(p => p.GetNowUtc()).Returns(submissionDateTime);

            var result = NewBuilder(dateTimeProvider.Object).Build(reportServiceContextMock.Object, dependentDataMock.Object);

            result.ApplicationVersion.Should().Be("11.22.3300.4321");
            result.ComponentSetVersion.Should().Be("NA");
            result.FilePreparationDate.Should().Be("06/11/2019");
            result.IlrFile.Should().Be("ILR-12345678-1920-20191005-151322-01.xml");
            result.LargeEmployerData.Should().Be("2.2.2.2");
            result.LarsData.Should().Be("3.3.3.3");
            result.OrganisationData.Should().Be("1.1.1.1");
            result.PostcodeData.Should().Be("4.4.4.4");
            result.ProviderName.Should().Be("Provider XYZ");
            result.ReportGeneratedAt.Should().Be("Report generated at: 01:01:01 on 01/01/2020");

            result.DirectFunded1416StudentsTotal.WithEHCP.Should().Be(2);
            result.DirectFunded1416StudentsTotal.WithoutEHCP.Should().Be(0);
            result.DirectFunded1416StudentsTotal.HNSWithoutEHCP.Should().Be(0);
            result.DirectFunded1416StudentsTotal.EHCPWithHNS.Should().Be(1);
            result.DirectFunded1416StudentsTotal.EHCPWithoutHNS.Should().Be(1);

            result.IncludingHNS1619StudentsTotal.WithEHCP.Should().Be(2);
            result.IncludingHNS1619StudentsTotal.WithoutEHCP.Should().Be(0);
            result.IncludingHNS1619StudentsTotal.HNSWithoutEHCP.Should().Be(0);
            result.IncludingHNS1619StudentsTotal.EHCPWithHNS.Should().Be(1);
            result.IncludingHNS1619StudentsTotal.EHCPWithoutHNS.Should().Be(1);

            result.EHCP1924StudentsTotal.WithEHCP.Should().Be(2);
            result.EHCP1924StudentsTotal.WithoutEHCP.Should().Be(0);
            result.EHCP1924StudentsTotal.HNSWithoutEHCP.Should().Be(0);
            result.EHCP1924StudentsTotal.EHCPWithHNS.Should().Be(1);
            result.EHCP1924StudentsTotal.EHCPWithoutHNS.Should().Be(1);

            result.Continuing19PlusExcludingEHCPStudentsTotal.WithEHCP.Should().Be(2);
            result.Continuing19PlusExcludingEHCPStudentsTotal.WithoutEHCP.Should().Be(0);
            result.Continuing19PlusExcludingEHCPStudentsTotal.HNSWithoutEHCP.Should().Be(0);
            result.Continuing19PlusExcludingEHCPStudentsTotal.EHCPWithHNS.Should().Be(1);
            result.Continuing19PlusExcludingEHCPStudentsTotal.EHCPWithoutHNS.Should().Be(1);

            result.Ukprn.Should().Be(987654321);
            result.Year.Should().Be("2019/20");
        }

        private List<ILearner> BuildLearners(TestLearningDelivery learningDelivery)
        {
            var directFundedLearnRef1 = new TestLearner()
            {
                LearnRefNumber = "DirectFundedLearnRef1",
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    learningDelivery
                },
                LearnerFAMs = new ILearnerFAM[]
                {
                    new TestLearnerFAM()
                    {
                        LearnFAMType = "EHC",
                        LearnFAMCode = 1
                    },
                    new TestLearnerFAM()
                    {
                        LearnFAMType = "HNS",
                        LearnFAMCode = 1
                    }
                }
            };

            var directFundedLearnRef2 = new TestLearner()
            {
                LearnRefNumber = "DirectFundedLearnRef2",
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    learningDelivery
                },
                LearnerFAMs = new ILearnerFAM[]
                {
                    new TestLearnerFAM()
                    {
                        LearnFAMType = "EHC",
                        LearnFAMCode = 1
                    }
                }
            };

            var fundingLineBLearnRef1 = new TestLearner()
            {
                LearnRefNumber = "FundingLineBLearnRef1",
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    learningDelivery
                },
                LearnerFAMs = new ILearnerFAM[]
                {
                    new TestLearnerFAM()
                    {
                        LearnFAMType = "EHC",
                        LearnFAMCode = 1
                    },
                    new TestLearnerFAM()
                    {
                        LearnFAMType = "HNS",
                        LearnFAMCode = 1
                    }
                }
            };

            var fundingLineBLearnRef2 = new TestLearner()
            {
                LearnRefNumber = "FundingLineBLearnRef2",
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    learningDelivery
                },
                LearnerFAMs = new ILearnerFAM[]
                {
                    new TestLearnerFAM()
                    {
                        LearnFAMType = "EHC",
                        LearnFAMCode = 1
                    }
                }
            };

            var fundingLineCLearnRef1 = new TestLearner()
            {
                LearnRefNumber = "FundingLineCLearnRef1",
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    learningDelivery
                },
                LearnerFAMs = new ILearnerFAM[]
                {
                    new TestLearnerFAM()
                    {
                        LearnFAMType = "EHC",
                        LearnFAMCode = 1
                    },
                    new TestLearnerFAM()
                    {
                        LearnFAMType = "HNS",
                        LearnFAMCode = 1
                    }
                }
            };

            var fundingLineCLearnRef2 = new TestLearner()
            {
                LearnRefNumber = "FundingLineCLearnRef2",
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    learningDelivery
                },
                LearnerFAMs = new ILearnerFAM[]
                {
                    new TestLearnerFAM()
                    {
                        LearnFAMType = "EHC",
                        LearnFAMCode = 1
                    }
                }
            };

            var fundingLineDLearnRef1 = new TestLearner()
            {
                LearnRefNumber = "FundingLineDLearnRef1",
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    learningDelivery
                },
                LearnerFAMs = new ILearnerFAM[]
                {
                    new TestLearnerFAM()
                    {
                        LearnFAMType = "EHC",
                        LearnFAMCode = 1
                    },
                    new TestLearnerFAM()
                    {
                        LearnFAMType = "HNS",
                        LearnFAMCode = 1
                    }
                }
            };

            var fundingLineDLearnRef2 = new TestLearner()
            {
                LearnRefNumber = "FundingLineDLearnRef2",
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    learningDelivery
                },
                LearnerFAMs = new ILearnerFAM[]
                {
                    new TestLearnerFAM()
                    {
                        LearnFAMType = "EHC",
                        LearnFAMCode = 1
                    }
                }
            };
            return new List<ILearner>()
            {
                directFundedLearnRef1,
                directFundedLearnRef2,
                fundingLineBLearnRef1,
                fundingLineBLearnRef2,
                fundingLineCLearnRef1,
                fundingLineCLearnRef2,
                fundingLineDLearnRef1,
                fundingLineDLearnRef2,

            };
        }

        private static List<FM25Learner> BuildFm25Learners()
        {
            return new List<FM25Learner>()
            {
                new FM25Learner()
                {
                    LearnRefNumber = "DirectFundedLearnRef1",
                    StartFund = true,
                    FundLine = "14-16 Direct Funded Students"
                },
                new FM25Learner()
                {
                    LearnRefNumber = "DirectFundedLearnRef2",
                    StartFund = true,
                    FundLine = "14-16 Direct Funded Students"
                },
                new FM25Learner()
                {
                    LearnRefNumber = "FundingLineBLearnRef1",
                    StartFund = true,
                    FundLine = "16-19 Students (including High Needs Students)"
                },
                new FM25Learner()
                {
                    LearnRefNumber = "FundingLineBLearnRef2",
                    StartFund = true,
                    FundLine = "16-19 Students (including High Needs Students)"
                },
                new FM25Learner()
                {
                    LearnRefNumber = "FundingLineCLearnRef1",
                    StartFund = true,
                    FundLine = "19-24 Students with an EHCP"
                },
                new FM25Learner()
                {
                    LearnRefNumber = "FundingLineCLearnRef2",
                    StartFund = true,
                    FundLine = "19-24 Students with an EHCP"
                },
                new FM25Learner()
                {
                    LearnRefNumber = "FundingLineDLearnRef1",
                    StartFund = true,
                    FundLine = "19+ Continuing Students (excluding EHCP)"
                },
                new FM25Learner()
                {
                    LearnRefNumber = "FundingLineDLearnRef2",
                    StartFund = true,
                    FundLine = "19+ Continuing Students (excluding EHCP)"
                }
            };
        }

        private HighNeedsStudentSummaryReportModelBuilder NewBuilder(IDateTimeProvider dateTimeProvider = null)
        {
            return new HighNeedsStudentSummaryReportModelBuilder(dateTimeProvider);
        }
    }
}
