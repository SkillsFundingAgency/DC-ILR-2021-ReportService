using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS2021.EF;
using ESFA.DC.EAS2021.EF.Interface;
using ESFA.DC.ILR.ReportService.Data.Eas.Providers;
using ESFA.DC.ILR.ReportService.Service.Interface;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace ESFA.DC.ILR.ReportService.Data.Eas.Tests
{
    public class EasProviderTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public EasProviderTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact(Skip = "Local performance test")]
        public async Task ProvideAsync()
        {
            var connectionString = ConfigurationManager.AppSettings["EasDbConnectionString"];
            var ukprn = 10006341;

            DbContextOptions<EasContext> options = new DbContextOptionsBuilder<EasContext>()
                .UseSqlServer(connectionString)
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).Options;

            IEasdbContext context = new EasContext(options);

            Func<IEasdbContext> easContext = () => context;

            var reportContextMock = new Mock<IReportServiceContext>();
            reportContextMock.Setup(rcm => rcm.Ukprn).Returns(ukprn);

            var stopWatch = new Stopwatch();
            var service = NewService(easContext);

            stopWatch.Start();
            await service.ProvideAsync(reportContextMock.Object, CancellationToken.None);
            stopWatch.Stop();

            _testOutputHelper.WriteLine($"Elapsed Time: {stopWatch.Elapsed}");
        }

        private EasProvider NewService(Func<IEasdbContext> context)
        {
            return new EasProvider(context);
        }
    }
}
