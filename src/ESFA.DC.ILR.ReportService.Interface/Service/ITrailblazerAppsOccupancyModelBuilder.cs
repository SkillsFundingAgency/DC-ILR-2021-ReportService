using System;
using System.Collections.Generic;
using System.Text;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR1819.ReportService.Model.Lars;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;
using LearningDelivery = ESFA.DC.ILR.FundingService.FM81.FundingOutput.Model.Output.LearningDelivery;

namespace ESFA.DC.ILR1819.ReportService.Interface.Service
{
    public interface ITrailblazerAppsOccupancyModelBuilder
    {
        TrailblazerAppsOccupancyModel BuildTrailblazerAppsOccupancyModel(
            ILearner learner,
            ILearningDelivery learningDelivery,
            LarsLearningDelivery larsModel,
            LearningDelivery ruleBaseLearningDelivery);
    }
}