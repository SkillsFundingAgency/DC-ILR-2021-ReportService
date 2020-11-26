using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Models.ReferenceData;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.Organisations;
using ESFA.DC.ILR.ReportService.Reports.Extensions;
using ESFA.DC.ILR.ReportService.Reports.Frm;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.Tests.Model;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Frm.Summary
{
    public class FrmSummaryReportModelBuilderTests
    {
        [Fact]
        public void Build()
        {
            var referenceDataRoot = new ReferenceDataRoot()
            {
                Organisations = new List<Organisation>()
                {
                    new Organisation()
                    {
                        UKPRN = 987654321,
                        Name = "Provider XYZ",
                    }
                }
            };

            var message = new TestMessage()
            {
                HeaderEntity = new TestHeader()
                {
                    SourceEntity = new TestSource()
                    {
                        ProtectiveMarkingString = "OFFICIAL-SENSITIVE-Personal"
                    }
                }
            };
            var dependentDataMock = new Mock<IReportServiceDependentData>();
            dependentDataMock.Setup(d => d.Get<IMessage>()).Returns(message);
            dependentDataMock.Setup(d => d.Get<ReferenceDataRoot>()).Returns(referenceDataRoot);

            var reportServiceContextMock = new Mock<IReportServiceContext>();

            reportServiceContextMock.SetupGet(c => c.Ukprn).Returns(987654321);
            reportServiceContextMock.SetupGet(c => c.LastIlrFileUpdate).Returns("01/01/2020 12:00");
            reportServiceContextMock.SetupGet(c => c.OriginalFilename).Returns("ILR-12345678-1920-20191005-151322-01.xml");

            var sut = NewBuilder().Build(reportServiceContextMock.Object, dependentDataMock.Object);

            sut.HeaderData.Should().NotBeNull();
            sut.HeaderData.Count.Should().Be(5);
            sut.HeaderData.GetValueOrDefault("Provider Name:").Should().Be("Provider XYZ");
            sut.HeaderData.GetValueOrDefault("UKPRN:").Should().Be("987654321");
            sut.HeaderData.GetValueOrDefault("ILR File:").Should().Be("ILR-12345678-1920-20191005-151322-01.xml");
            sut.HeaderData.GetValueOrDefault("Last ILR File Update:").Should().Be("01/01/2020 12:00");
            sut.HeaderData.GetValueOrDefault("Security Classification").Should().Be("OFFICIAL-SENSITIVE-Personal");
        }

        private FrmReportModelBuilder NewBuilder()
        {
            return new FrmReportModelBuilder();
        }
    }
}
