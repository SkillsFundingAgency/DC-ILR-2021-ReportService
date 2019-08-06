using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Apprenticeship
{
    public sealed class AppsIndicativeEarningsModelComparer : IComparer<AppsIndicativeEarningsReportModel>
    {
        public int Compare(AppsIndicativeEarningsReportModel x, AppsIndicativeEarningsReportModel y)
        {
            if (x == null && y == null)
            {
                return 0;
            }

            if (x == null)
            {
                return -1;
            }

            if (y == null)
            {
                return 1;
            }

            if (x == y)
            {
                return 0;
            }

            int cmp = string.CompareOrdinal(x.LearnRefNumber, y.LearnRefNumber);
            if (cmp != 0)
            {
                return cmp;
            }

            if (x.AimSeqNumber > y.AimSeqNumber)
            {
                return 1;
            }

            if (x.AimSeqNumber < y.AimSeqNumber)
            {
                return -1;
            }

            if (x.PriceEpisodeStartDate.HasValue && y.PriceEpisodeStartDate.HasValue)
            {
                int cmpPriceEpisodeStartDates = DateTime.Compare(x.PriceEpisodeStartDate.Value, y.PriceEpisodeStartDate.Value);
                if (cmpPriceEpisodeStartDates != 0)
                {
                    return cmpPriceEpisodeStartDates;
                }

                if (cmpPriceEpisodeStartDates > 0)
                {
                    return 1;
                }

                if (cmpPriceEpisodeStartDates < 0)
                {
                    return -1;
                }
            }

            return 0;
        }
    }
}
