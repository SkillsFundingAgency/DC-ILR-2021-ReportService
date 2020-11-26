using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Models.FRM;
using ESFA.DC.ILR.ReportService.Models.ReferenceData;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.LARS;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.Organisations;
using ESFA.DC.ILR.ReportService.Reports.Frm;
using ESFA.DC.ILR.ReportService.Reports.Frm.FRM06;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.Tests.Model;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Frm.FRM06
{
    public class Frm06ReportModelBuilderTests
    {
        [Fact]
        public void Build()
        {
            var referenceData = new ReferenceDataRoot
            {
                Organisations = new List<Organisation>
                {
                    new Organisation
                    {
                        UKPRN = 1,
                        Name = "OrgName1",
                    },
                    new Organisation
                    {
                        UKPRN = 2,
                        Name = "OrgName2",
                    }
                },
                LARSLearningDeliveries = new List<LARSLearningDelivery>
                {
                    new LARSLearningDelivery
                    {
                        LearnAimRef = "LearnAimRef1",
                        LearnAimRefTypeDesc = "LeanAimRef1Desc"
                    }
                }
            };

            var frmData = new FrmReferenceData
            {
                Frm06Learners = new List<FrmLearner>
                {
                    new FrmLearner
                    {
                        LearnRefNumber = "LearnRefNumber1",
                        LearnAimRef = "LearnAimRef1",
                        FundModel = 35,
                    },
                    new FrmLearner
                    {
                        LearnRefNumber = "LearnRefNumber2",
                        LearnAimRef = "LearnAimRef1",
                        FundModel = 99,
                        LearningDeliveryFAMs = new List<LearningDeliveryFAM>
                        {
                            new LearningDeliveryFAM
                            {
                                LearnDelFAMCode = "1",
                                LearnDelFAMType = "ADL"
                            }
                        }
                    },
                    new FrmLearner
                    {
                        LearnRefNumber = "LearnRefNumber3",
                        LearnAimRef = "LearnAimRef1",
                        FundModel = 99,
                    },
                }
            };
            var message = new TestMessage();

            var context = new Mock<IReportServiceContext>();
            context.Setup(x => x.ReturnPeriodName).Returns("R01");

            var refData = new Mock<IReportServiceDependentData>();
            refData.Setup(x => x.Get<FrmReferenceData>()).Returns(frmData);
            refData.Setup(x => x.Get<IMessage>()).Returns(message);
            refData.Setup(x => x.Get<ReferenceDataRoot>()).Returns(referenceData);

            var expectedModels = new List<Frm06ReportModel>
            {
                new Frm06ReportModel
                {
                    LearnAimRef = "LearnAimRef1",
                    FundingModel = 35,
                    AdvancedLoansIndicator = "",
                    ResIndicator = "",
                    Return = "R01",
                    LearnRefNumber = "LearnRefNumber1",
                    SOFCode = "",
                    LearningAimType = "LeanAimRef1Desc"
                },
                new Frm06ReportModel
                {
                    LearnAimRef = "LearnAimRef1",
                    FundingModel = 99,
                    AdvancedLoansIndicator = "1",
                    ResIndicator = "",
                    Return = "R01",
                    LearnRefNumber = "LearnRefNumber2",
                    SOFCode = "",
                    LearningAimType = "LeanAimRef1Desc"
                }
            };

            NewBuilder().Build(context.Object, refData.Object).Should().BeEquivalentTo(expectedModels);
        }

        [Fact]
        public void Build_NoData()
        {
            var referenceData = new ReferenceDataRoot
            {
                Organisations = new List<Organisation>
                {
                    new Organisation
                    {
                        UKPRN = 1,
                        Name = "OrgName1",
                    },
                    new Organisation
                    {
                        UKPRN = 2,
                        Name = "OrgName2",
                    }
                },
                LARSLearningDeliveries = new List<LARSLearningDelivery>
                {
                    new LARSLearningDelivery
                    {
                        LearnAimRef = "LearnAImRef1",
                        LearnAimRefTypeDesc = "LeanAimRef1Desc"
                    }
                }
            };
            var frmData = new FrmReferenceData();
            var message = new TestMessage();

            var context = new Mock<IReportServiceContext>();
            context.Setup(x => x.ReturnPeriodName).Returns("R01");

            var refData = new Mock<IReportServiceDependentData>();
            refData.Setup(x => x.Get<FrmReferenceData>()).Returns(frmData);
            refData.Setup(x => x.Get<IMessage>()).Returns(message);
            refData.Setup(x => x.Get<ReferenceDataRoot>()).Returns(referenceData);

            NewBuilder().Build(context.Object, refData.Object).Should().BeNullOrEmpty();
        }

        [Theory]
        [InlineData(35, "1")]
        [InlineData(99, "1")]
        public void LearningDeliveryFilter_True(int fundModel, string adl)
        {
            var model = new Frm06ReportModel
            {
                FundingModel = fundModel,
                AdvancedLoansIndicator = adl
            };
            
            NewBuilder().LearningDeliveryFilter(model).Should().BeTrue();
        }

        [Theory]
        [InlineData(99, "0")]
        [InlineData(99, null)]
        [InlineData(99, "5")]
        public void LearningDeliveryFilter_False(int fundModel, string adl)
        {
            var model = new Frm06ReportModel
            {
                FundingModel = fundModel,
                AdvancedLoansIndicator = adl
            };

            NewBuilder().LearningDeliveryFilter(model).Should().BeFalse();
        }


        private Frm06ReportModelBuilder NewBuilder()
        {
            return  new Frm06ReportModelBuilder(new FrmLearnerComparer());

        }
    }
}
