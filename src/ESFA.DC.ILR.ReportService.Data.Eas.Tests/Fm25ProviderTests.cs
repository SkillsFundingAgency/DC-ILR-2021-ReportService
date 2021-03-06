﻿using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Data.Eas.Providers;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR2021.DataStore.EF;
using ESFA.DC.ILR2021.DataStore.EF.Interface;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace ESFA.DC.ILR.ReportService.Data.Eas.Tests
{
    public class Fm25ProviderTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Fm25ProviderTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact(Skip = "Local performance test")]
        public async Task ProvideAsync()
        {
            var connectionString = ConfigurationManager.AppSettings["IlrDbConnectionString"];
            var ukprn = 10003909;

            DbContextOptions<ILR2021_DataStoreEntities> options = new DbContextOptionsBuilder<ILR2021_DataStoreEntities>()
                    .UseSqlServer(connectionString)
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).Options;

            IILR2021_DataStoreEntities context = new ILR2021_DataStoreEntities(options);

            Func<IILR2021_DataStoreEntities> ilrContext = () => context;

            var reportContextMock = new Mock<IReportServiceContext>();
            reportContextMock.Setup(rcm => rcm.Ukprn).Returns(ukprn);

            var stopWatch = new Stopwatch();
            var service = NewService(ilrContext);

            stopWatch.Start();
            await service.ProvideAsync(reportContextMock.Object, CancellationToken.None);
            stopWatch.Stop();

            _testOutputHelper.WriteLine($"Elapsed Time: {stopWatch.Elapsed}");
        }

        private Fm25Provider NewService(Func<IILR2021_DataStoreEntities> context)
        {
            return new Fm25Provider(context);
        }

    }
}
