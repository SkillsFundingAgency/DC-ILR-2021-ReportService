using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.ILR1819.ReportService.Model.ReportModels
{
    public class SummaryOfFm35FundingModel
    {
        public string FundingLineType { get; set; }
        public string Period { get; set; }
        public string OnProgramme { get; set; }
        public string Balancing { get; set; }
        public string JobOutcomeAchievement { get; set; }
        public string AimAchievement { get; set; }
        public string TotalAchievement { get; set; }
        public string LearningSupport { get; set; }
        public string Total { get; set; }
    }
}
