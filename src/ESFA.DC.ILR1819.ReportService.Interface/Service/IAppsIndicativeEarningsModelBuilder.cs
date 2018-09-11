using ESFA.DC.Data.LARS.Model;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Attribute;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR1819.ReportService.Model.Lars;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;

namespace ESFA.DC.ILR1819.ReportService.Interface.Service
{
    public interface IAppsIndicativeEarningsModelBuilder
    {
        AppsIndicativeEarningsModel BuildModel(
            ILearner learner,
            ILearningDelivery learningDelivery,
            LearningDeliveryAttribute fm36DeliveryAttribute,
            PriceEpisodeAttribute fm36EpisodeAttribute,
            LarsLearningDelivery larsLearningDelivery,
            LARS_Standard larsStandard,
            bool earliestEpisode,
            bool hasPriceEpisodes);
    }
}