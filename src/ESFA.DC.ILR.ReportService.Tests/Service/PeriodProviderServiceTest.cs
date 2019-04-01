﻿using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.CollectionsManagement.Services.Interface;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Context;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Service.Service;
using ESFA.DC.Logging.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR1819.ReportService.Tests.Service
{
    public sealed class PeriodProviderServiceTest
    {
        [Theory]
        [InlineData("2018-07-04 18:05:00", "2018-07-01 12:01:00", 11)]
        [InlineData("2018-07-04 18:05:00", "2018-07-04 18:06:00", 12)]
        public async Task TestPeriodProviderService(string collectionEndDateTimeUtcStr, string nowDateTimeUtcStr, int expectedPeriod)
        {
            System.DateTime collectionEndDateTimeUtc = System.DateTime.ParseExact(collectionEndDateTimeUtcStr, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            System.DateTime nowDateTimeUtc = System.DateTime.ParseExact(nowDateTimeUtcStr, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

            IDateTimeProvider dateTimeProvider = new DateTimeProvider.DateTimeProvider();
            Mock<IReturnCalendarService> returnCalendarService = new Mock<IReturnCalendarService>();
            Mock<ILogger> logger = new Mock<ILogger>();

            returnCalendarService.Setup(x => x.GetCurrentPeriodAsync("ILR1819")).ReturnsAsync(new ReturnPeriod()
            {
                EndDateTimeUtc = collectionEndDateTimeUtc,
            });

            IPeriodProviderService periodProviderService = new PeriodProviderService(dateTimeProvider, returnCalendarService.Object, logger.Object);

            Mock<IReportServiceContext> reportServiceContextMock = new Mock<IReportServiceContext>();
            reportServiceContextMock.SetupGet(x => x.JobId).Returns(1);
            reportServiceContextMock.SetupGet(x => x.SubmissionDateTimeUtc).Returns(nowDateTimeUtc);

            int period = await periodProviderService.GetPeriod(reportServiceContextMock.Object, CancellationToken.None);

            period.Should().Be(expectedPeriod);
        }
    }
}
