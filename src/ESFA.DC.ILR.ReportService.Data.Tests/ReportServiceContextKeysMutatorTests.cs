using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Data.Tests.Stub;
using ESFA.DC.ILR.ReportService.Models.ReferenceData;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.MetaData;
using ESFA.DC.ILR.ReportService.Service.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Data.Tests
{
    public class ReportServiceContextKeysMutatorTests
    {
        [Fact]
        public async Task MutateAsync()
        {
            var cancellationToken = CancellationToken.None;
            var refDataModel = new ReferenceDataRoot
            {
                MetaDatas = new MetaData
                {
                    ReferenceDataVersions = new ReferenceDataVersion
                    {
                        EasFileDetails = new EasFileDetails
                        {
                            FileName = "EAS-1-2.csv",
                            UploadDateTime = new DateTime(2020, 8, 1)
                        }
                    }

                }
            };

            var reportServiceDependentData = new Mock<IReportServiceDependentData>();
            reportServiceDependentData.Setup(x => x.Get<ReferenceDataRoot>()).Returns(refDataModel);

            IReportServiceContext contextIn = new ReportServiceJobContextMessageContextStub(1, "ILR-1-2.xml", "ILR-1-2.xml", new DateTime(2020, 8, 2));

            var mutator = new ReportServiceContextKeysMutator();
            

            var contextOut = await mutator.MutateAsync(contextIn, reportServiceDependentData.Object, cancellationToken);

            contextOut.IlrReportingFilename.Should().Be("ILR-1-2.xml");
            contextOut.EasReportingFilename.Should().Be("EAS-1-2.csv");
            contextOut.LastIlrFileUpdate.Should().Be("02/08/2020 00:00:00");
            contextOut.LastEasFileUpdate.Should().Be("01/08/2020 00:00:00");
        }
    }
}
