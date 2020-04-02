using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Data.Eas.Providers;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR1920.DataStore.EF.Valid;
using ESFA.DC.ILR1920.DataStore.EF.Valid.Interface;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace ESFA.DC.ILR.ReportService.Data.Eas.Tests
{
    public class ValidIlrProviderTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ValidIlrProviderTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact(Skip = "Local performance test")]
        public async Task ProvideAsync()
        {
            var connectionString = ConfigurationManager.AppSettings["IlrDbConnectionString"];
            var ukprn = 10003909;

            DbContextOptions<ILR1920_DataStoreEntitiesValid> options = new DbContextOptionsBuilder<ILR1920_DataStoreEntitiesValid>()
                .UseSqlServer(connectionString)
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).Options;

            IILR1920_DataStoreEntitiesValid context = new ILR1920_DataStoreEntitiesValid(options);

            Func<IILR1920_DataStoreEntitiesValid> ilrContext = () => context;

            var reportContextMock = new Mock<IReportServiceContext>();
            reportContextMock.Setup(rcm => rcm.Ukprn).Returns(ukprn);

            var stopWatch = new Stopwatch();
            var service = NewService(ilrContext);

            stopWatch.Start();
            var data = await service.ProvideAsync(reportContextMock.Object, CancellationToken.None);
            stopWatch.Stop();

            _testOutputHelper.WriteLine($"Elapsed Time: {stopWatch.Elapsed}");
        }

        private ValidIlrProvider NewService(Func<IILR1920_DataStoreEntitiesValid> context)
        {
            return new ValidIlrProvider(context);
        }
    }
}
