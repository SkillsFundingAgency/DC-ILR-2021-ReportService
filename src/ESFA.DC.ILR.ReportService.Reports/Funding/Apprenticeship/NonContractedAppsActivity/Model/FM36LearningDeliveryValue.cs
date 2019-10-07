using System.Collections.Generic;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Apprenticeship.NonContractedAppsActivity.Model
{
    public class FM36LearningDeliveryValue
    {
        public LearningDeliveryValues LearningDeliveryValues { get; set; }

        public ICollection<FundLineValue> FundLineValues { get; set; }
    }
}
 