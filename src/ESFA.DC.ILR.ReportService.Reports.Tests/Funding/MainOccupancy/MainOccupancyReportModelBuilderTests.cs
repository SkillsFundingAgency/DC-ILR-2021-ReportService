using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Main;
using ESFA.DC.ILR.ReportService.Reports.Model.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding.MainOccupancy
{
    public class MainOccupancyReportModelBuilderTests
    {
        [Fact]
        public void FundModelLearningDeliveryFilter_FalseNullFAMS()
        {
            var learningDeliveryMock = new Mock<ILearningDelivery>();

            learningDeliveryMock.SetupGet(ld => ld.LearningDeliveryFAMs).Returns(null as IReadOnlyCollection<ILearningDeliveryFAM>);

            NewBuilder().FundModelLearningDeliveryFilter(learningDeliveryMock.Object, 1).Should().BeFalse();
        }

        [Fact]
        public void FundModelLearningDeliveryFilter_FalseNull()
        {
            NewBuilder().FundModelLearningDeliveryFilter(null, 1).Should().BeFalse();
        }

        [Fact]
        public void FundModelLearningDeliveryFilter_FalseFM()
        {
            var learningDeliveryMock = new Mock<ILearningDelivery>();
            
            learningDeliveryMock.SetupGet(ld => ld.FundModel).Returns(2);

            NewBuilder().FundModelLearningDeliveryFilter(learningDeliveryMock.Object, 1).Should().BeFalse();
        }

        [Fact]
        public void FundModelLearningDeliveryFilter_FalseType()
        {
            var learningDeliveryMock = new Mock<ILearningDelivery>();

            var fundModel = 25;

            learningDeliveryMock.SetupGet(ld => ld.FundModel).Returns(fundModel);

            var learningDeliveryFamMock = new Mock<ILearningDeliveryFAM>();

            learningDeliveryFamMock.SetupGet(f => f.LearnDelFAMType).Returns("ABC");

            var learningDeliveryFams = new List<ILearningDeliveryFAM>()
            {
                learningDeliveryFamMock.Object
            };

            learningDeliveryMock.SetupGet(ld => ld.LearningDeliveryFAMs).Returns(learningDeliveryFams);

            NewBuilder().FundModelLearningDeliveryFilter(learningDeliveryMock.Object, fundModel).Should().BeFalse();
        }

        [Fact]
        public void FundModelLearningDeliveryFilter_FalseCode()
        {
            var learningDeliveryMock = new Mock<ILearningDelivery>();

            var fundModel = 25;

            learningDeliveryMock.SetupGet(ld => ld.FundModel).Returns(fundModel);

            var learningDeliveryFamMock = new Mock<ILearningDeliveryFAM>();

            learningDeliveryFamMock.SetupGet(f => f.LearnDelFAMType).Returns("SOF");
            learningDeliveryFamMock.SetupGet(f => f.LearnDelFAMCode).Returns("110");

            var learningDeliveryFams = new List<ILearningDeliveryFAM>()
            {
                learningDeliveryFamMock.Object
            };

            learningDeliveryMock.SetupGet(ld => ld.LearningDeliveryFAMs).Returns(learningDeliveryFams);

            NewBuilder().FundModelLearningDeliveryFilter(learningDeliveryMock.Object, fundModel).Should().BeFalse();
        }

        [Fact]
        public void FundModelLearningDeliveryFilter_True()
        {
            var learningDeliveryMock = new Mock<ILearningDelivery>();

            var fundModel = 25;

            learningDeliveryMock.SetupGet(ld => ld.FundModel).Returns(fundModel);

            var learningDeliveryFamMock = new Mock<ILearningDeliveryFAM>();

            learningDeliveryFamMock.SetupGet(f => f.LearnDelFAMType).Returns("SOF");
            learningDeliveryFamMock.SetupGet(f => f.LearnDelFAMCode).Returns("105");

            var learningDeliveryFams = new List<ILearningDeliveryFAM>()
            {
                learningDeliveryFamMock.Object
            };

            learningDeliveryMock.SetupGet(ld => ld.LearningDeliveryFAMs).Returns(learningDeliveryFams);

            NewBuilder().FundModelLearningDeliveryFilter(learningDeliveryMock.Object, fundModel).Should().BeTrue();
        }

        [Fact]
        public void LdmLearningDeliveryFilter_False()
        {
            var learningDeliveryMock = new Mock<ILearningDelivery>();

            var fundModel = 25;

            learningDeliveryMock.SetupGet(ld => ld.FundModel).Returns(fundModel);

            var learningDeliveryFamMockSof105 = new Mock<ILearningDeliveryFAM>();

            learningDeliveryFamMockSof105.SetupGet(f => f.LearnDelFAMType).Returns("SOF");
            learningDeliveryFamMockSof105.SetupGet(f => f.LearnDelFAMCode).Returns("105");

            var learningDeliveryFamMockLdm = new Mock<ILearningDeliveryFAM>();

            learningDeliveryFamMockLdm.SetupGet(f => f.LearnDelFAMType).Returns("LDM");
            learningDeliveryFamMockLdm.SetupGet(f => f.LearnDelFAMCode).Returns("100");

            var learningDeliveryFams = new List<ILearningDeliveryFAM>()
            {
                learningDeliveryFamMockSof105.Object,
                learningDeliveryFamMockLdm.Object
            };

            learningDeliveryMock.SetupGet(ld => ld.LearningDeliveryFAMs).Returns(learningDeliveryFams);

            NewBuilder().FundModelLearningDeliveryFilter(learningDeliveryMock.Object, fundModel).Should().BeTrue();
        }

        [Theory]
        [InlineData("370")]
        [InlineData("371")]
        [InlineData("372")]
        [InlineData("373")]
        public void LdmLearningDeliveryFilter_True(string famCode)
        {
            var learningDeliveryMock = new Mock<ILearningDelivery>();

            var fundModel = 25;

            learningDeliveryMock.SetupGet(ld => ld.FundModel).Returns(fundModel);

            var learningDeliveryFamMockSof105 = new Mock<ILearningDeliveryFAM>();

            learningDeliveryFamMockSof105.SetupGet(f => f.LearnDelFAMType).Returns("SOF");
            learningDeliveryFamMockSof105.SetupGet(f => f.LearnDelFAMCode).Returns("105");

            var learningDeliveryFamMockLdm = new Mock<ILearningDeliveryFAM>();

            learningDeliveryFamMockLdm.SetupGet(f => f.LearnDelFAMType).Returns("LDM");
            learningDeliveryFamMockLdm.SetupGet(f => f.LearnDelFAMCode).Returns(famCode);

            var learningDeliveryFams = new List<ILearningDeliveryFAM>()
            {
                learningDeliveryFamMockSof105.Object,
                learningDeliveryFamMockLdm.Object
            };

            learningDeliveryMock.SetupGet(ld => ld.LearningDeliveryFAMs).Returns(learningDeliveryFams);

            NewBuilder().FundModelLearningDeliveryFilter(learningDeliveryMock.Object, fundModel).Should().BeFalse();
        }


        private MainOccupancyReportModelBuilder NewBuilder(IIlrModelMapper ilrModelMapper = null)
        {
            return new MainOccupancyReportModelBuilder(ilrModelMapper);
        }
    }
}
