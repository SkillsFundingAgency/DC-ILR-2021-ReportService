using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Data.Eas.Tests.Stub;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR2021.DataStore.EF;
using ESFA.DC.ILR2021.DataStore.EF.Interface;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Data.Eas.Tests
{
    public class ReportServiceContextKeysMutatorTests
    {
        [Fact]
        public async Task MutateAsync()
        {
            var cancellationToken = CancellationToken.None;
            IEnumerable<FileDetail> submissionsList = new List<FileDetail>
            {
                new FileDetail { UKPRN = 1, SubmittedTime = new DateTime(2020, 8, 1), Filename = "1/ILR-1-1.xml" },
                new FileDetail { UKPRN = 1, SubmittedTime = new DateTime(2020, 8, 2), Filename = "1/ILR-1-2.xml" },
                new FileDetail { UKPRN = 2, SubmittedTime = new DateTime(2020, 9, 1), Filename = "2/ILR-2-1.xml" }
            };

            var ilrDbMock = submissionsList.AsQueryable().BuildMockDbSet();

            var IlrMock = new Mock<IILR2021_DataStoreEntities>();
            IlrMock.Setup(x => x.FileDetails).Returns(ilrDbMock.Object);

            Func<IILR2021_DataStoreEntities> ilrFunc = () => { return IlrMock.Object; };

            var mutator = new ReportServiceContextKeysMutator(ilrFunc);
            IReportServiceContext contextIn = new ReportServiceJobContextMessageContextStub(1, "1/EAS-1-2.csv", "1/EAS-1-2.csv", new DateTime(2020, 8, 1));

            var contextOut = await mutator.MutateAsync(contextIn, null, cancellationToken);

            contextOut.IlrReportingFilename.Should().Be("ILR-1-2.xml");
            contextOut.EasReportingFilename.Should().Be("EAS-1-2.csv");
            contextOut.LastIlrFileUpdate.Should().Be("02/08/2020 00:00:00");
            contextOut.LastEasFileUpdate.Should().Be("01/08/2020 00:00:00");
        }
    }
}
