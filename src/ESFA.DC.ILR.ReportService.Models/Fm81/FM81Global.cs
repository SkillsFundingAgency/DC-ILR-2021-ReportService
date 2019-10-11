﻿using System.Collections.Generic;

namespace ESFA.DC.ILR.ReportService.Models.Fm81
{
    public class FM81Global
    {
        public int UKPRN { get; set; }

        public string CurFundYr { get; set; }

        public string LARSVersion { get; set; }

        public string RulebaseVersion { get; set; }

        public List<FM81Learner> Learners { get; set; }
    }
}
