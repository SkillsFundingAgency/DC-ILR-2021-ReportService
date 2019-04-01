using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.CollectionsManagement.Services.Interface;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.ReportService.Interface.Context;
using ESFA.DC.ILR.ReportService.Interface.Service;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.ILR1819.ReportService.Service.Service
{
    public sealed class PeriodProviderService : IPeriodProviderService
    {
        private const string CurrentCollection = "ILR1819";

        private readonly Dictionary<int, int> monthToCollection = new Dictionary<int, int>
        {
            { 1, 6 },
            { 2, 7 },
            { 3, 8 },
            { 4, 9 },
            { 5, 10 },
            { 6, 11 },
            { 7, 12 },
            { 8, 1 },
            { 9, 2 },
            { 10, 3 },
            { 11, 4 },
            { 12, 5 }
        };

        private readonly IDateTimeProvider _dateTimeProvider;

        private readonly IReturnCalendarService _returnCalendarService;

        private readonly ILogger _logger;

        private readonly SemaphoreSlim _getDataLock;

        private int _cachedData = -1;

        public PeriodProviderService(IDateTimeProvider dateTimeProvider, IReturnCalendarService returnCalendarService, ILogger logger)
        {
            _dateTimeProvider = dateTimeProvider;
            _returnCalendarService = returnCalendarService;
            _logger = logger;

            _getDataLock = new SemaphoreSlim(1, 1);
        }

        public async Task<int> GetPeriod(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            await _getDataLock.WaitAsync(cancellationToken);

            try
            {
                if (_cachedData > -1)
                {
                    return _cachedData;
                }

                ReturnPeriod returnPeriod = await _returnCalendarService.GetCurrentPeriodAsync(CurrentCollection);

                DateTime dateTimeNowUtc = reportServiceContext.SubmissionDateTimeUtc;
                DateTime returnPeriodEndDateTimeUk = _dateTimeProvider.ConvertUtcToUk(returnPeriod.EndDateTimeUtc);
                DateTime dateTimeNowUk = _dateTimeProvider.ConvertUtcToUk(dateTimeNowUtc);
                int period = 12;

                if (dateTimeNowUk <= returnPeriodEndDateTimeUk)
                {
                    if (dateTimeNowUk.Month == returnPeriodEndDateTimeUk.Month &&
                        dateTimeNowUk.Year == returnPeriodEndDateTimeUk.Year)
                    {
                        if (dateTimeNowUk.Day < returnPeriodEndDateTimeUk.Day ||
                            (dateTimeNowUk.Day == returnPeriodEndDateTimeUk.Day &&
                             dateTimeNowUk.Hour < returnPeriodEndDateTimeUk.Hour &&
                             dateTimeNowUk.Minute < returnPeriodEndDateTimeUk.Minute))
                        {
                            int month = dateTimeNowUk.Month - 1;
                            if (month == 0)
                            {
                                month = 12;
                            }

                            period = monthToCollection[month];
                        }
                        else
                        {
                            period = monthToCollection[dateTimeNowUk.Month];
                        }
                    }
                    else
                    {
                        period = monthToCollection[dateTimeNowUk.Month];
                    }
                }

                _cachedData = period;
            }
            catch (Exception ex)
            {
                // Todo: Check behaviour
                _logger.LogError("Failed to get period data", ex);
            }
            finally
            {
                _getDataLock.Release();
            }

            return _cachedData;
        }

        public int MonthFromPeriod(int period)
        {
            return monthToCollection.Single(x => x.Value == period).Key;
        }
    }
}
