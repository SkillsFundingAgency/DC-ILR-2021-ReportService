using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.ILR.ReportService.Reports.Model
{
    public class AppFinRecordModel
    {
        public decimal? LatestTotalNegotiatedPrice1 { get; set; }

        public decimal? LatestTotalNegotiatedPrice2 { get; set; }

        public decimal? SumOfPmrsBeforeFundingYear { get; set; }

        public decimal? SumOfAugustPmrs{ get; set; }

        public decimal? SumOfSeptemberPmrs { get; set; }

        public decimal? SumOfOctoberPmrs { get; set; }

        public decimal? SumOfNovemberPmrs { get; set; }

        public decimal? SumOfDecemberPmrs { get; set; }

        public decimal? SumOfJanuaryPmrs { get; set; }

        public decimal? SumOfFebruaryPmrs { get; set; }

        public decimal? SumOfMarchPmrs { get; set; }

        public decimal? SumOfAprilPmrs { get; set; }

        public decimal? SumOfMayPmrs { get; set; }

        public decimal? SumOfJunePmrs { get; set; }

        public decimal? SumOfJulyPmrs { get; set; }

        public decimal? PmrsTotal { get; set; }
    }
}
