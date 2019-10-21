using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Models.Fm36;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Apprenticeship.NonContractedAppsActivity.Model
{
    public class FM36LearningDeliveryValue
    {
        public LearningDeliveryValues LearningDeliveryValues { get; set; }

        public ICollection<FundLineValue> FundLineValues { get; set; }
    }
}
 