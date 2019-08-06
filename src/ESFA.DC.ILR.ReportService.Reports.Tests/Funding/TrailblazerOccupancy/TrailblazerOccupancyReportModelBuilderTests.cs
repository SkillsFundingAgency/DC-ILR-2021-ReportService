using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.FundingService.FM81.FundingOutput.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Trailblazer;
using ESFA.DC.ILR.ReportService.Reports.Model.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding.TrailblazerOccupancy
{
    public class TrailblazerOccupancyReportModelBuilderTests
    {
        [Fact]
        public void LearningDeliveryFilter_False_NullLearningDelivery()
        {
            ILearningDelivery learningDelivery = null;

            NewBuilder().Filter(learningDelivery).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFilter_False_Empty()
        {
            var learningDeliveryMock = new Mock<ILearningDelivery>();

            learningDeliveryMock.SetupGet(ld => ld.LearningDeliveryFAMs).Returns(new List<ILearningDeliveryFAM>());

            NewBuilder().Filter(learningDeliveryMock.Object).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFilter_False_FundModel()
        {
            var learningDeliveryMock = new Mock<ILearningDelivery>();

            learningDeliveryMock.SetupGet(ld => ld.FundModel).Returns(35);
            learningDeliveryMock.SetupGet(ld => ld.ProgTypeNullable).Returns(25);

            NewBuilder().Filter(learningDeliveryMock.Object).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFilter_False_ProgType()
        {
            var learningDeliveryMock = new Mock<ILearningDelivery>();

            learningDeliveryMock.SetupGet(ld => ld.FundModel).Returns(81);
            learningDeliveryMock.SetupGet(ld => ld.ProgTypeNullable).Returns(1);

            NewBuilder().Filter(learningDeliveryMock.Object).Should().BeFalse();
        }

        private TrailblazerOccupancyReportModelBuilder NewBuilder(IIlrModelMapper ilrModelMapper = null)
        {
            return new TrailblazerOccupancyReportModelBuilder(ilrModelMapper);
        }

        private LearningDeliveryPeriodisedValue BuildPeriodisedValuesForAttribute(string attribute)
        {
            return new LearningDeliveryPeriodisedValue()
            {
                AttributeName = attribute,
                Period1 = 1.111m,
                Period2 = 2.222m,
                Period3 = 3.333m,
                Period4 = 4.444m,
                Period5 = 5.555m,
                Period6 = 6.666m,
                Period7 = 7.777m,
                Period8 = 8.888m,
                Period9 = 9.999m,
                Period10 = 10.1010m,
                Period11 = 11.1111m,
                Period12 = 12.1212m
            };
        }
    }
}
