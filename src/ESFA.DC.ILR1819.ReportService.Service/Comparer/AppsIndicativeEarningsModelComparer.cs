using System;
using System.Collections.Generic;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;

namespace ESFA.DC.ILR1819.ReportService.Service.Comparer
{
    public sealed class AppsIndicativeEarningsModelComparer : IComparer<AppsIndicativeEarningsModel>
    {
        public int Compare(AppsIndicativeEarningsModel x, AppsIndicativeEarningsModel y)
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

            if (DateTime.TryParse(x.PriceEpisodeStartDate, out DateTime xPriceEpisodeStartDate) &&
                DateTime.TryParse(y.PriceEpisodeStartDate, out DateTime yPriceEpisodeStartDate))
            {
                int cmpPriceEpisodeStartDates = DateTime.Compare(xPriceEpisodeStartDate, yPriceEpisodeStartDate);
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
