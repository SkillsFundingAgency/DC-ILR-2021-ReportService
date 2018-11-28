using System.Collections.Generic;
using System.Text;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.FundingService.ALB.FundingOutput.Model.Output;
using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Configuration;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
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
            ILRSourceFileInfo ilrSourceFileInfo,
            IDateTimeProvider dateTimeProvider,
            IIntUtilitiesService intUtilitiesService,
            IMessage message,
            IVersionInfo versionInfo,
            string orgData,
            string largeEmployersData,
            string postcodeData,
            string larsData);

        AdultFundingClaimModel BuildAdultFundingClaimModel(
            ILogger logger,
            IJobContextMessage jobContextMessage,
            FM35Global fm35Global,
            List<EasSubmissionValues> easSubmissionValues,
            ALBGlobal albGlobal,
            string providerName,
            ILRSourceFileInfo ilrSourceFileInfo,
            IDateTimeProvider dateTimeProvider,
            IIntUtilitiesService intUtilitiesService,
            IMessage message,
            IVersionInfo versionInfo,
            string orgData,
            string largeEmployersData,
            string postcodeData,
            string larsData);
    }
}