using System.Collections.Generic;

namespace ESFA.DC.ILR.ReportService.Models.Fm99
{
    public class ALBGlobal
    {
        public int UKPRN { get; set; }

        public string LARSVersion { get; set; }

        public string PostcodeAreaCostVersion { get; set; }

        public string RulebaseVersion { get; set; }

        public List<ALBLearner> Learners { get; set; }
    }
}
