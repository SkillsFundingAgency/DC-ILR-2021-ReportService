using System.Collections.Generic;

namespace ESFA.DC.ILR.ReportService.Model.NonContractedAppsActivity
{
    public class NonContractedAppsActivityILRInfo
    {
        public int UkPrn { get; set; }

        public List<NonContractedAppsActivityLearnerInfo> Learners { get; set; }
    }
}
