using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Main.AEBSTFInitiativesOccupancy;
using ESFA.DC.ILR.ReportService.Reports.Model.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding.AEBSTFInitiativesOccupancy
{
    public class AEBSTFInitiativesOccupancyReportModelBuilderTests
    {
        [Fact]
        public void LearningDeliveryFilter_FalseNullFAMS()
        {
            var learningDeliveryMock = new Mock<ILearningDelivery>();

            learningDeliveryMock.SetupGet(ld => ld.LearningDeliveryFAMs).Returns(null as IReadOnlyCollection<ILearningDeliveryFAM>);

            NewBuilder().LearningDeliveryFilter(learningDeliveryMock.Object).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFilter_FalseNull()
        {
            NewBuilder().LearningDeliveryFilter(null).Should().BeFalse();
        }


        [Fact]
        public void LearningDeliveryFilter_False()
        {
            var learningDeliveryMock = new Mock<ILearningDelivery>();

            var learningDeliveryFamMockLdm = new Mock<ILearningDeliveryFAM>();

            learningDeliveryFamMockLdm.SetupGet(f => f.LearnDelFAMType).Returns("LDM");
            learningDeliveryFamMockLdm.SetupGet(f => f.LearnDelFAMCode).Returns("100");

            var learningDeliveryFams = new List<ILearningDeliveryFAM>()
            {
                learningDeliveryFamMockLdm.Object
            };

            learningDeliveryMock.SetupGet(ld => ld.LearningDeliveryFAMs).Returns(learningDeliveryFams);

            NewBuilder().LearningDeliveryFilter(learningDeliveryMock.Object).Should().BeFalse();
        }

        [Theory]
        [InlineData("370")]
        [InlineData("371")]
        [InlineData("372")]
        [InlineData("373")]
        public void LearningDeliveryFilter_True(string famCode)
        {
            var learningDeliveryMock = new Mock<ILearningDelivery>();

            var learningDeliveryFamMockLdm = new Mock<ILearningDeliveryFAM>();

            learningDeliveryFamMockLdm.SetupGet(f => f.LearnDelFAMType).Returns("LDM");
            learningDeliveryFamMockLdm.SetupGet(f => f.LearnDelFAMCode).Returns(famCode);

            var learningDeliveryFams = new List<ILearningDeliveryFAM>()
            {
                learningDeliveryFamMockLdm.Object
            };

            learningDeliveryMock.SetupGet(ld => ld.LearningDeliveryFAMs).Returns(learningDeliveryFams);

            NewBuilder().LearningDeliveryFilter(learningDeliveryMock.Object).Should().BeTrue();
        }


        private AEBSTFInitiativesOccupancyReportModelBuilder NewBuilder(IIlrModelMapper ilrModelMapper = null)
        {
            return new AEBSTFInitiativesOccupancyReportModelBuilder(ilrModelMapper);
        }
    }
}
