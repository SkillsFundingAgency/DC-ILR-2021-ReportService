using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.DevolvedOccupancy;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding.DevolvedOccupancy
{
    public class DevolvedAdultEducationOccupancyReportModelBuilderTests
    {
        [Fact]
        public void LearningDeliveryFilter_False_Null()
        {
            var learningDeliveryMock = new Mock<ILearningDelivery>();

            learningDeliveryMock.SetupGet(ld => ld.LearningDeliveryFAMs).Returns(null as IReadOnlyCollection<ILearningDeliveryFAM>);

            NewBuilder().LearningDeliveryReportFilter(learningDeliveryMock.Object).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFilter_False_Empty()
        {
            var learningDeliveryMock = new Mock<ILearningDelivery>();

            learningDeliveryMock.SetupGet(ld => ld.LearningDeliveryFAMs).Returns(new List<ILearningDeliveryFAM>());

            NewBuilder().LearningDeliveryReportFilter(learningDeliveryMock.Object).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFilter_False_Type()
        {
            var learningDeliveryMock = new Mock<ILearningDelivery>();
            var learningDeliveryFAM = new Mock<ILearningDeliveryFAM>();

            learningDeliveryFAM.SetupGet(fam => fam.LearnDelFAMType).Returns("NotSOF");

            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>()
            {
                learningDeliveryFAM.Object
            };
            
            learningDeliveryMock.SetupGet(ld => ld.LearningDeliveryFAMs).Returns(learningDeliveryFAMs);

            NewBuilder().LearningDeliveryReportFilter(learningDeliveryMock.Object).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFilter_False_Code()
        {
            var learningDeliveryMock = new Mock<ILearningDelivery>();
            var learningDeliveryFAM = new Mock<ILearningDeliveryFAM>();

            learningDeliveryFAM.SetupGet(fam => fam.LearnDelFAMType).Returns("SOF");
            learningDeliveryFAM.SetupGet(fam => fam.LearnDelFAMCode).Returns("999");

            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>()
            {
                learningDeliveryFAM.Object
            };

            learningDeliveryMock.SetupGet(ld => ld.LearningDeliveryFAMs).Returns(learningDeliveryFAMs);

            NewBuilder().LearningDeliveryReportFilter(learningDeliveryMock.Object).Should().BeFalse();
        }

        [InlineData("110")]
        [InlineData("111")]
        [InlineData("112")]
        [InlineData("113")]
        [InlineData("114")]
        [InlineData("115")]
        [InlineData("116")]
        [Theory]
        public void LearningDeliveryFilter_True(string code)
        {
            var learningDeliveryMock = new Mock<ILearningDelivery>();
            var learningDeliveryFAM = new Mock<ILearningDeliveryFAM>();

            learningDeliveryFAM.SetupGet(fam => fam.LearnDelFAMType).Returns("SOF");
            learningDeliveryFAM.SetupGet(fam => fam.LearnDelFAMCode).Returns(code);

            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>()
            {
                learningDeliveryFAM.Object
            };

            learningDeliveryMock.SetupGet(ld => ld.LearningDeliveryFAMs).Returns(learningDeliveryFAMs);

            NewBuilder().LearningDeliveryReportFilter(learningDeliveryMock.Object).Should().BeTrue();
        }

        [Fact]
        public void ProviderSpecLearnMonitoringBuilder_Match()
        {
            var aMon = "AMonitoring";
            var bMon = "BMonitoring";

            var monitorings = new List<IProviderSpecLearnerMonitoring>()
            {
                BuildProviderSpecLearnerMonitoring("A", aMon),
                BuildProviderSpecLearnerMonitoring("B", bMon),
            };

            var result = NewBuilder().BuildProviderSpecLearnerMonitoring(monitorings);

            result.A.Should().Be(aMon);
            result.B.Should().Be(bMon);
        }

        [Fact]
        public void ProviderSpecLearnMonitoringBuilder_NonMatch()
        {
            var aMon = "AMonitoring";
            var bMon = "BMonitoring";

            var monitorings = new List<IProviderSpecLearnerMonitoring>()
            {
                BuildProviderSpecLearnerMonitoring("C", aMon),
                BuildProviderSpecLearnerMonitoring("D", bMon),
            };

            var result = NewBuilder().BuildProviderSpecLearnerMonitoring(monitorings);

            result.A.Should().BeNull();
            result.B.Should().BeNull();
        }

        [Fact]
        public void ProviderSpecLearnMonitoringBuilder_MixedCase()
        {
            var aMon = "AMonitoring";
            var bMon = "BMonitoring";

            var monitorings = new List<IProviderSpecLearnerMonitoring>()
            {
                BuildProviderSpecLearnerMonitoring("a", aMon),
                BuildProviderSpecLearnerMonitoring("b", bMon),
            };

            var result = NewBuilder().BuildProviderSpecLearnerMonitoring(monitorings);

            result.A.Should().Be(aMon);
            result.B.Should().Be(bMon);
        }

        [Fact]
        public void ProviderSpecDeliveryMonitoringBuilder_Match()
        {
            var aMon = "AMonitoring";
            var bMon = "BMonitoring";
            var cMon = "CMonitoring";
            var dMon = "DMonitoring";

            var monitorings = new List<IProviderSpecDeliveryMonitoring>()
            {
                BuildProviderSpecDeliveryMonitoring("A", aMon),
                BuildProviderSpecDeliveryMonitoring("B", bMon),
                BuildProviderSpecDeliveryMonitoring("C", cMon),
                BuildProviderSpecDeliveryMonitoring("D", dMon),
            };

            var result = NewBuilder().BuildProviderSpecDeliveryMonitoring(monitorings);

            result.A.Should().Be(aMon);
            result.B.Should().Be(bMon);
            result.C.Should().Be(cMon);
            result.D.Should().Be(dMon);
        }

        [Fact]
        public void ProviderSpecDeliveryMonitoringBuilder_NonMatch()
        {
            var aMon = "AMonitoring";
            var bMon = "BMonitoring";
            var cMon = "CMonitoring";
            var dMon = "DMonitoring";

            var monitorings = new List<IProviderSpecDeliveryMonitoring>()
            {
                BuildProviderSpecDeliveryMonitoring("E", aMon),
                BuildProviderSpecDeliveryMonitoring("F", bMon),
                BuildProviderSpecDeliveryMonitoring("G", cMon),
                BuildProviderSpecDeliveryMonitoring("H", dMon),
            };

            var result = NewBuilder().BuildProviderSpecDeliveryMonitoring(monitorings);

            result.A.Should().BeNull();
            result.B.Should().BeNull();
            result.C.Should().BeNull();
            result.D.Should().BeNull();
        }

        [Fact]
        public void ProviderSpecDeliveryMonitoringBuilder_MixedCase()
        {
            var aMon = "AMonitoring";
            var bMon = "BMonitoring";
            var cMon = "CMonitoring";
            var dMon = "DMonitoring";

            var monitorings = new List<IProviderSpecDeliveryMonitoring>()
            {
                BuildProviderSpecDeliveryMonitoring("a", aMon),
                BuildProviderSpecDeliveryMonitoring("b", bMon),
                BuildProviderSpecDeliveryMonitoring("c", cMon),
                BuildProviderSpecDeliveryMonitoring("d", dMon),
            };

            var result = NewBuilder().BuildProviderSpecDeliveryMonitoring(monitorings);

            result.A.Should().Be(aMon);
            result.B.Should().Be(bMon);
            result.C.Should().Be(cMon);
            result.D.Should().Be(dMon);
        }

        private DevolvedAdultEducationOccupancyReportModelBuilder NewBuilder() => new DevolvedAdultEducationOccupancyReportModelBuilder();

        private IProviderSpecLearnerMonitoring BuildProviderSpecLearnerMonitoring(string occur, string mon)
        {
            var monitoringMock = new Mock<IProviderSpecLearnerMonitoring>();
            
            monitoringMock.SetupGet(a => a.ProvSpecLearnMonOccur).Returns(occur);
            monitoringMock.SetupGet(a => a.ProvSpecLearnMon).Returns(mon);

            return monitoringMock.Object;
        }

        private IProviderSpecDeliveryMonitoring BuildProviderSpecDeliveryMonitoring(string occur, string mon)
        {
            var monitoringMock = new Mock<IProviderSpecDeliveryMonitoring>();

            monitoringMock.SetupGet(a => a.ProvSpecDelMonOccur).Returns(occur);
            monitoringMock.SetupGet(a => a.ProvSpecDelMon).Returns(mon);

            return monitoringMock.Object;
        }
    }
}
