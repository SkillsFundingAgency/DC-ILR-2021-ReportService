using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.FundingService.FM25.Model.Output;
using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReferenceDataService.Model.MetaData;
using ESFA.DC.ILR.ReferenceDataService.Model.MetaData.ReferenceDataVersions;
using ESFA.DC.ILR.ReferenceDataService.Model.Organisations;
using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.FundingClaim;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.Tests.Model;
using Moq;
using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding.FundingClaim1619
{
    public class FundingClaim1619ReportModelBuilderTests
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
                StdCodeNullable = 1,
                
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
                        Name = "Provider XYZ",
                        OrganisationCoFRemovals = new List<OrganisationCoFRemoval>()
                        {
                            new OrganisationCoFRemoval()
                            {
                                EffectiveFrom = new DateTime(2019, 01, 01),
                                CoFRemoval = (decimal)4500.12
                            }
                        },
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

            result.Ukprn.Should().Be(987654321);
            result.Year.Should().Be("2019/20");
            result.CofRemoval.Should().Be((decimal)-4500.12);

            result.FundingFactor.PrvRetentFactHist.Should().Be("0.79600");
            result.FundingFactor.ProgWeightHist.Should().Be("1.06100");
            result.FundingFactor.AreaCostFact1618Hist.Should().Be("1.00000");
            result.FundingFactor.PrvDisadvPropnHist.Should().Be("0.34500");
            result.FundingFactor.PrvHistLrgProgPropn.Should().Be("0.21300");

            result.DirectFundingStudents.Band1StudentNumbers.Should().Be(1);
            result.DirectFundingStudents.Band1TotalFunding.Should().Be(1);
            result.DirectFundingStudents.Band2StudentNumbers.Should().Be(1);
            result.DirectFundingStudents.Band2TotalFunding.Should().Be(1);
            result.DirectFundingStudents.Band3StudentNumbers.Should().Be(1);
            result.DirectFundingStudents.Band3TotalFunding.Should().Be(1);
            result.DirectFundingStudents.Band4aStudentNumbers.Should().Be(1);
            result.DirectFundingStudents.Band4aTotalFunding.Should().Be(1);
            result.DirectFundingStudents.Band4bStudentNumbers.Should().Be(1);
            result.DirectFundingStudents.Band4bTotalFunding.Should().Be(1);
            result.DirectFundingStudents.Band5StudentNumbers.Should().Be(1);
            result.DirectFundingStudents.Band5TotalFunding.Should().Be(1);


            result.StudentsIncludingHNS.Band1StudentNumbers.Should().Be(1);
            result.StudentsIncludingHNS.Band1TotalFunding.Should().Be(1);
            result.StudentsIncludingHNS.Band2StudentNumbers.Should().Be(1);
            result.StudentsIncludingHNS.Band2TotalFunding.Should().Be(1);
            result.StudentsIncludingHNS.Band3StudentNumbers.Should().Be(1);
            result.StudentsIncludingHNS.Band3TotalFunding.Should().Be(1);
            result.StudentsIncludingHNS.Band4aStudentNumbers.Should().Be(1);
            result.StudentsIncludingHNS.Band4aTotalFunding.Should().Be(1);
            result.StudentsIncludingHNS.Band4bStudentNumbers.Should().Be(1);
            result.StudentsIncludingHNS.Band4bTotalFunding.Should().Be(1);
            result.StudentsIncludingHNS.Band5StudentNumbers.Should().Be(1);
            result.StudentsIncludingHNS.Band5TotalFunding.Should().Be(1);

            result.StudentsWithEHCPlan.Band1StudentNumbers.Should().Be(1);
            result.StudentsWithEHCPlan.Band1TotalFunding.Should().Be(1);
            result.StudentsWithEHCPlan.Band2StudentNumbers.Should().Be(1);
            result.StudentsWithEHCPlan.Band2TotalFunding.Should().Be(1);
            result.StudentsWithEHCPlan.Band3StudentNumbers.Should().Be(1);
            result.StudentsWithEHCPlan.Band3TotalFunding.Should().Be(1);
            result.StudentsWithEHCPlan.Band4aStudentNumbers.Should().Be(1);
            result.StudentsWithEHCPlan.Band4aTotalFunding.Should().Be(1);
            result.StudentsWithEHCPlan.Band4bStudentNumbers.Should().Be(1);
            result.StudentsWithEHCPlan.Band4bTotalFunding.Should().Be(1);
            result.StudentsWithEHCPlan.Band5StudentNumbers.Should().Be(1);
            result.StudentsWithEHCPlan.Band5TotalFunding.Should().Be(1);

            result.ContinuingStudentsExcludingEHCPlan.Band1StudentNumbers.Should().Be(1);
            result.ContinuingStudentsExcludingEHCPlan.Band1TotalFunding.Should().Be(1);
            result.ContinuingStudentsExcludingEHCPlan.Band2StudentNumbers.Should().Be(1);
            result.ContinuingStudentsExcludingEHCPlan.Band2TotalFunding.Should().Be(1);
            result.ContinuingStudentsExcludingEHCPlan.Band3StudentNumbers.Should().Be(1);
            result.ContinuingStudentsExcludingEHCPlan.Band3TotalFunding.Should().Be(1);
            result.ContinuingStudentsExcludingEHCPlan.Band4aStudentNumbers.Should().Be(1);
            result.ContinuingStudentsExcludingEHCPlan.Band4aTotalFunding.Should().Be(1);
            result.ContinuingStudentsExcludingEHCPlan.Band4bStudentNumbers.Should().Be(1);
            result.ContinuingStudentsExcludingEHCPlan.Band4bTotalFunding.Should().Be(1);
            result.ContinuingStudentsExcludingEHCPlan.Band5StudentNumbers.Should().Be(1);
            result.ContinuingStudentsExcludingEHCPlan.Band5TotalFunding.Should().Be(1);


           
        }

        private List<ILearner> BuildLearners(TestLearningDelivery learningDelivery)
        {
            var directFundedLearnRef1 = new TestLearner()
            {
                LearnRefNumber = "DirectFundedLearnRef1",
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    learningDelivery
                }
            };

            var directFundedLearnRef2 = new TestLearner()
            {
                LearnRefNumber = "DirectFundedLearnRef2",
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    learningDelivery
                }
            };

            var fundingLineBLearnRef1 = new TestLearner()
            {
                LearnRefNumber = "FundingLineBLearnRef1",
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    learningDelivery
                }
            };

            var fundingLineBLearnRef2 = new TestLearner()
            {
                LearnRefNumber = "FundingLineBLearnRef2",
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    learningDelivery
                }
            };

            var fundingLineCLearnRef1 = new TestLearner()
            {
                LearnRefNumber = "FundingLineCLearnRef1",
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    learningDelivery
                }
            };

            var fundingLineCLearnRef2 = new TestLearner()
            {
                LearnRefNumber = "FundingLineCLearnRef2",
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    learningDelivery
                }
            };

            var fundingLineDLearnRef1 = new TestLearner()
            {
                LearnRefNumber = "FundingLineDLearnRef1",
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    learningDelivery
                }
            };

            var fundingLineDLearnRef2 = new TestLearner()
            {
                LearnRefNumber = "FundingLineDLearnRef2",
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    learningDelivery
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
                    FundLine = "14-16 Direct Funded Students",
                    RateBand = "540+ hours (Band 5)",
                    OnProgPayment = (decimal)95,
                    PrvRetentFactHist = (decimal)0.796,
                    ProgWeightHist = (decimal)1.061,
                    AreaCostFact1618Hist = (decimal)1,
                    PrvDisadvPropnHist = (decimal)0.345,
                    PrvHistLrgProgPropn = (decimal)0.213
                },
                new FM25Learner()
                {
                    LearnRefNumber = "DirectFundedLearnRef2",
                    StartFund = true,
                    FundLine = "14-16 Direct Funded Students",
                    RateBand = "450+ hours (Band 4a)",
                    OnProgPayment = (decimal)12,
                    PrvRetentFactHist = (decimal)0.796,
                    ProgWeightHist = (decimal)1.061,
                    AreaCostFact1618Hist = (decimal)1,
                    PrvDisadvPropnHist = (decimal)0.345,
                    PrvHistLrgProgPropn = (decimal)0.213
                },
                new FM25Learner()
                {
                    LearnRefNumber = "FundingLineBLearnRef1",
                    StartFund = true,
                    FundLine = "16-19 Students (including High Needs Students)",
                    RateBand = "450 to 539 hours (Band 4b)",
                    OnProgPayment = (decimal)2589915.43,
                    PrvRetentFactHist = (decimal)0.796,
                    ProgWeightHist = (decimal)1.061,
                    AreaCostFact1618Hist = (decimal)1,
                    PrvDisadvPropnHist = (decimal)0.345,
                    PrvHistLrgProgPropn = (decimal)0.213
                },
                new FM25Learner()
                {
                    LearnRefNumber = "FundingLineBLearnRef2",
                    StartFund = true,
                    FundLine = "16-19 Students (including High Needs Students)",
                    RateBand = "360 to 449 hours (Band 3)",
                    OnProgPayment = (decimal)25815.43,
                },
                new FM25Learner()
                {
                    LearnRefNumber = "FundingLineCLearnRef1",
                    StartFund = true,
                    FundLine = "19-24 Students with an EHCP",
                    RateBand = "280 to 359 hours (Band 2)",
                    OnProgPayment = (decimal)555.12,
                },
                new FM25Learner()
                {
                    LearnRefNumber = "FundingLineCLearnRef2",
                    StartFund = true,
                    FundLine = "19-24 Students with an EHCP",
                    RateBand = "Up to 279 hours (Band 1)",
                    OnProgPayment = (decimal)125.67,
                },
                new FM25Learner()
                {
                    LearnRefNumber = "FundingLineDLearnRef1",
                    StartFund = true,
                    FundLine = "19+ Continuing Students (excluding EHCP)",
                    RateBand = "540+ hours (Band 5)",
                    OnProgPayment = (decimal)56425.99,
                },
                new FM25Learner()
                {
                    LearnRefNumber = "FundingLineDLearnRef2",
                    StartFund = true,
                    FundLine = "19+ Continuing Students (excluding EHCP)",
                    RateBand = "450+ hours (Band 4a)",
                    OnProgPayment = (decimal)855.55,
                }
            };
        }

        private FundingClaimReportModelBuilder NewBuilder(IDateTimeProvider dateTimeProvider = null)
        {
            return new FundingClaimReportModelBuilder(dateTimeProvider);
        }
    }
}
