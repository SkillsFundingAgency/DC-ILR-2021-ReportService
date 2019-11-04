using System;
using ESFA.DC.ILR.ReportService.Reports.Constants;

namespace ESFA.DC.ILR.ReportService.Reports.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ShortDateStringFormat(this DateTime source)
        {
            if (source == null)
            {
                return null;
            }

            return source.Date.ToString(FormattingConstants.ShortDateStringFormat);
        }

        public static string LongDateStringFormat(this DateTime source)
        {
            if (source == null)
            {
                return null;
            }

            return source.ToString(FormattingConstants.LongDateTimeStringFormat);
        }

        public static string TimeOfDayOnDateStringFormat(this DateTime source)
        {
            if (source == null)
            {
                return null;
            }

            return source.ToString(FormattingConstants.TimeofDayOnDateStringFormat);
        }
    }
}
