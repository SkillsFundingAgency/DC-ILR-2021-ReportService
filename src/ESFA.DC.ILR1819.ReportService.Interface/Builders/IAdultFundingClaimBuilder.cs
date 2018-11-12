using System.Collections.Generic;
using System.Text;
using ESFA.DC.EAS1819.EF;
using ESFA.DC.ILR1819.DataStore.EF;
using ESFA.DC.ILR1819.ReportService.Model.ILR;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;

namespace ESFA.DC.ILR1819.ReportService.Interface.Builders
{
    public interface IAdultFundingClaimBuilder
    {
        AdultFundingClaimModel BuildAdultFundingClaimModel(
            List<FM35LearningDeliveryValues> fm35LearningDeliveryPeriodisedValues,
            List<EasSubmissionValues> easSubmissionValues,
            List<ALBLearningDeliveryValues> albLearningDeliveryPeriodisedValues);
    }
}
