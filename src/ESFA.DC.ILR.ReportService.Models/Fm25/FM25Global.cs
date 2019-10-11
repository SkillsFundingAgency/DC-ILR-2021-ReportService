﻿using System.Collections.Generic;

namespace ESFA.DC.ILR.ReportService.Models.Fm25
{
    public class FM25Global
    {
        public int? UKPRN { get; set; }

        public string LARSVersion { get; set; }

        public string OrgVersion { get; set; }

        public string PostcodeDisadvantageVersion { get; set; }

        public string RulebaseVersion { get; set; }

        public List<FM25Learner> Learners { get; set; }
    }
}
