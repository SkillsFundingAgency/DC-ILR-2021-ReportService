using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.FundingService.FM25.Model.Output;
using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReferenceDataService.Model.LARS;
using ESFA.DC.ILR.ReferenceDataService.Model.MetaData;
using ESFA.DC.ILR.ReferenceDataService.Model.MetaData.ReferenceDataVersions;
using ESFA.DC.ILR.ReferenceDataService.Model.Organisations;
using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.HighNeedsStudentSummary;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.Tests.Model;
using Moq;
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
                albLearningDeliveryFam.Object,

            };

            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = 25,
                LearnAimRef = "learnAimRef",
                AimSeqNumber = 1,
                LearningDeliveryFAMs = learningDeliveryFams,
                StdCodeNullable = 1
            };

            var learner1 = new TestLearner()
            {
                LearnRefNumber = "LearnRefNumber",
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

            var learner2 = new TestLearner()
            {
                LearnRefNumber = "LearnRefNumber2",
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

            var message = new TestMessage()
            {
                Learners = new List<ILearner>()
                {
                    learner1, learner2
                },
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
                        OrganisationsVersion =new OrganisationsVersion("1.1.1.1"),
                        Employers = new EmployersVersion("2.2.2.2"),
                        LarsVersion = new LarsVersion("3.3.3.3"),
                        PostcodesVersion = new PostcodesVersion("4.4.4.4")
                    }
                }
                //LARSLearningDeliveries = new List<LARSLearningDelivery>()
                //{
                //    larsLearningDelivery
                //},
                //LARSStandards = new List<LARSStandard>()
                //{
                //    larsStandard
                //}
            };

            var fm25Global = new FM25Global()
            {
                Learners = new List<FM25Learner>()
                {
                    new FM25Learner()
                    {
                        LearnRefNumber = "LearnRefNumber",
                        StartFund = true,
                        FundLine = "14-16 Direct Funded Students"
                    },
                    new FM25Learner()
                    {
                        LearnRefNumber = "LearnRefNumber2",
                        StartFund = true,
                        FundLine = "14-16 Direct Funded Students"
                    }
                }
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

        }

        private HighNeedsStudentSummaryReportModelBuilder NewBuilder(IDateTimeProvider dateTimeProvider= null)
        {
            return new HighNeedsStudentSummaryReportModelBuilder(dateTimeProvider);
        }
    }
}
