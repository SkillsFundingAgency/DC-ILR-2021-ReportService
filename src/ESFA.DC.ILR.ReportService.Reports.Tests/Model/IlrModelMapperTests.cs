﻿using ESFA.DC.ILR.Model.Interface;
using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Reports.Model;
using FluentAssertions;
using Xunit;
using Moq;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Model
{
    public class IlrModelMapperTests
    {
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

            var result = NewMapper().MapProviderSpecLearnerMonitorings(monitorings);

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

            var result = NewMapper().MapProviderSpecLearnerMonitorings(monitorings);

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

            var result = NewMapper().MapProviderSpecLearnerMonitorings(monitorings);

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

            var result = NewMapper().MapProviderSpecDeliveryMonitorings(monitorings);

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

            var result = NewMapper().MapProviderSpecDeliveryMonitorings(monitorings);

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

            var result = NewMapper().MapProviderSpecDeliveryMonitorings(monitorings);

            result.A.Should().Be(aMon);
            result.B.Should().Be(bMon);
            result.C.Should().Be(cMon);
            result.D.Should().Be(dMon);
        }

        private IlrModelMapper NewMapper()
        {
            return new IlrModelMapper();
        }

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
