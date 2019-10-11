using System;

namespace ESFA.DC.ILR.ReportService.Models.Fm25
{
    public class LearnerPeriod
    {
        public string LearnRefNumber { get; set; }

        public int? Period { get; set; }

        public Decimal? LnrOnProgPay { get; set; }
    }
}
