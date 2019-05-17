using System.Collections.Generic;

namespace ESFA.DC.ILR.ReportService.Model.PeriodEnd.NonContractedAppsActivity
{
    public class NonContractedAppsActivityILRInfo
    {
        public int UkPrn { get; set; }

        public List<NonContractedAppsActivityLearnerInfo> Learners { get; set; }
    }
}
