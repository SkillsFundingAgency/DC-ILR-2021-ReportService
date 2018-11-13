using System.Collections.Generic;
using System.Text;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Configuration;
using ESFA.DC.ILR1819.ReportService.Model.ILR;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;
using ESFA.DC.ILR1819.ReportService.Model.Eas;
using ESFA.DC.JobContextManager.Model.Interface;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.ILR1819.ReportService.Interface.Builders
{
    public interface IAdultFundingClaimBuilder
    {
        AdultFundingClaimModel BuildAdultFundingClaimModel(
            ILogger logger, 
            IJobContextMessage jobContextMessage,
            List<FM35LearningDeliveryValues> fm35LearningDeliveryPeriodisedValues,
            List<EasSubmissionValues> easSubmissionValues,
            List<ALBLearningDeliveryValues> albLearningDeliveryPeriodisedValues,
            string providerName,
            ILRFileDetail ilrFileDetail,
            IDateTimeProvider dateTimeProvider,
            IMessage message,
            IVersionInfo versionInfo);
    }
}
