﻿using ESFA.DC.Data.LARS.Model;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR1819.ReportService.Model.Lars;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;
using LearningDelivery = ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output.LearningDelivery;

namespace ESFA.DC.ILR1819.ReportService.Interface.Service
{
    public interface IAppsIndicativeEarningsModelBuilder
    {
        AppsIndicativeEarningsModel BuildModel(
            ILearner learner,
            ILearningDelivery learningDelivery,
            LearningDelivery fm36DeliveryAttribute,
            PriceEpisode fm36EpisodeAttribute,
            LarsLearningDelivery larsLearningDelivery,
            LARS_Standard larsStandard,
            bool earliestEpisode,
            bool hasPriceEpisodes);
    }
}