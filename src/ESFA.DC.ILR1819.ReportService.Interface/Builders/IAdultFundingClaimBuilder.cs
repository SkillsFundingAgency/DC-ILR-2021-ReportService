using System.Collections.Generic;
using System.Text;
using ESFA.DC.ILR1819.ReportService.Model.ILR;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;
using ESFA.DC.ILR1819.ReportService.Model.Eas;

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
