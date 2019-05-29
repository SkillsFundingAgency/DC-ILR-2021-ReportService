using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.ILR.ReportService.Model.NonContractedAppsActivity
{
    public class AECLearningDeliveryValuesInfo
    {
        public bool LearnDelMathEng { get; set; }

        public string LearnDelInitialFundLineType { get; set; }

        public string LearnAimRef { get; set; }

        public DateTime AppAdjLearnStartDate { get; set; }

        public int AgeAtProgStart { get; set; }
    }
}
