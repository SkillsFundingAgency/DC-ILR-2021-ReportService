using System.Linq;
using ESFA.DC.ILR.ReportService.Data.Interface.Mappers;
using ESFA.DC.ILR.ReportService.Models.Fm36;

namespace ESFA.DC.ILR.ReportService.Data.Mappers
{
    public class Fm36Mapper : IMapper<FundingService.FM36.FundingOutput.Model.Output.FM36Global, FM36Global>
    {
        public FM36Global MapData(FundingService.FM36.FundingOutput.Model.Output.FM36Global fm36Global)
        {
            return MapFm36Global(fm36Global);
        }

        private FM36Global MapFm36Global(FundingService.FM36.FundingOutput.Model.Output.FM36Global fm36Global)
        {
            return new FM36Global()
            {
                Learners = fm36Global.Learners?.Select(MapFm36Learner).ToList()
            };
        }

        private FM36Learner MapFm36Learner(FundingService.FM36.FundingOutput.Model.Output.FM36Learner fm36Learner)
        {
            return new FM36Learner()
            {
                LearnRefNumber = fm36Learner.LearnRefNumber,
                PriceEpisodes = fm36Learner.PriceEpisodes?.Select(MapPriceEpisode).ToList(),
                LearningDeliveries = fm36Learner.LearningDeliveries?.Select(MapFm36LearningDelivery).ToList(),
            };
        }

        private PriceEpisode MapPriceEpisode(FundingService.FM36.FundingOutput.Model.Output.PriceEpisode priceEpisode)
        {
            return new PriceEpisode()
            {
                PriceEpisodeValues = MapPriceEpisodeValues(priceEpisode.PriceEpisodeValues),
                PriceEpisodePeriodisedValues = priceEpisode.PriceEpisodePeriodisedValues.Select(MapPriceEpisodePeriodisedValues).ToList()
            };
        }

        private PriceEpisodeValues MapPriceEpisodeValues(FundingService.FM36.FundingOutput.Model.Output.PriceEpisodeValues priceEpisodeValues)
        {
            return new PriceEpisodeValues()
            {
                EpisodeStartDate = priceEpisodeValues.EpisodeStartDate,
                PriceEpisodeUpperBandLimit = priceEpisodeValues.PriceEpisodeUpperBandLimit,
                PriceEpisodeActualEndDate = priceEpisodeValues.PriceEpisodeActualEndDate,
                PriceEpisodeTotalTNPPrice = priceEpisodeValues.PriceEpisodeTotalTNPPrice,
                PriceEpisodeUpperLimitAdjustment = priceEpisodeValues.PriceEpisodeUpperLimitAdjustment,
                PriceEpisodeCompletionElement = priceEpisodeValues.PriceEpisodeCompletionElement,
                PriceEpisodeCappedRemainingTNPAmount = priceEpisodeValues.PriceEpisodeCappedRemainingTNPAmount,
                PriceEpisodeAimSeqNumber = priceEpisodeValues.PriceEpisodeAimSeqNumber,
                PriceEpisodeFundLineType = priceEpisodeValues.PriceEpisodeFundLineType,
                PriceEpisodeAgreeId = priceEpisodeValues.PriceEpisodeAgreeId,
            };
        }

        private PriceEpisodePeriodisedValues MapPriceEpisodePeriodisedValues(FundingService.FM36.FundingOutput.Model.Output.PriceEpisodePeriodisedValues priceEpisodePeriodisedValues)
        {
            return new PriceEpisodePeriodisedValues()
            {
                AttributeName = priceEpisodePeriodisedValues.AttributeName,
                Period1 = priceEpisodePeriodisedValues.Period1,
                Period2 = priceEpisodePeriodisedValues.Period2,
                Period3 = priceEpisodePeriodisedValues.Period3,
                Period4 = priceEpisodePeriodisedValues.Period4,
                Period5 = priceEpisodePeriodisedValues.Period5,
                Period6 = priceEpisodePeriodisedValues.Period6,
                Period7 = priceEpisodePeriodisedValues.Period7,
                Period8 = priceEpisodePeriodisedValues.Period8,
                Period9 = priceEpisodePeriodisedValues.Period9,
                Period10 = priceEpisodePeriodisedValues.Period10,
                Period11 = priceEpisodePeriodisedValues.Period11,
                Period12 = priceEpisodePeriodisedValues.Period12
            };
        }

        private LearningDelivery MapFm36LearningDelivery(FundingService.FM36.FundingOutput.Model.Output.LearningDelivery fm36LearningDelivery)
        {
            return new LearningDelivery()
            {
                AimSeqNumber = fm36LearningDelivery.AimSeqNumber,
                LearningDeliveryValues = MapFm36LearningDeliveryValues(fm36LearningDelivery.LearningDeliveryValues),
                LearningDeliveryPeriodisedValues = fm36LearningDelivery.LearningDeliveryPeriodisedValues?.Select(MapFm36LearningDeliveryPeriodisedValues).ToList(),
                LearningDeliveryPeriodisedTextValues = fm36LearningDelivery.LearningDeliveryPeriodisedTextValues?.Select(MapFm36LearningDeliveryPeriodisedTextValues).ToList()
            };
        }

        private LearningDeliveryValues MapFm36LearningDeliveryValues(FundingService.FM36.FundingOutput.Model.Output.LearningDeliveryValues fm36LearningDeliveryValues)
        {
            return new LearningDeliveryValues()
            {
                AgeAtProgStart = fm36LearningDeliveryValues.AgeAtProgStart,
                AppAdjLearnStartDate = fm36LearningDeliveryValues.AppAdjLearnStartDate,
                LearnDelApplicProv1618FrameworkUplift = fm36LearningDeliveryValues.LearnDelApplicProv1618FrameworkUplift,
                LearnDelInitialFundLineType = fm36LearningDeliveryValues.LearnDelInitialFundLineType,
                LearnDelMathEng = fm36LearningDeliveryValues.LearnDelMathEng,
                PlannedNumOnProgInstalm = fm36LearningDeliveryValues.PlannedNumOnProgInstalm,
            };
        }

        private LearningDeliveryPeriodisedValues MapFm36LearningDeliveryPeriodisedValues(FundingService.FM36.FundingOutput.Model.Output.LearningDeliveryPeriodisedValues fm36LearningDeliveryPeriodisedValue)
        {
            return new LearningDeliveryPeriodisedValues()
            {
                AttributeName = fm36LearningDeliveryPeriodisedValue.AttributeName,
                Period1 = fm36LearningDeliveryPeriodisedValue.Period1,
                Period2 = fm36LearningDeliveryPeriodisedValue.Period2,
                Period3 = fm36LearningDeliveryPeriodisedValue.Period3,
                Period4 = fm36LearningDeliveryPeriodisedValue.Period4,
                Period5 = fm36LearningDeliveryPeriodisedValue.Period5,
                Period6 = fm36LearningDeliveryPeriodisedValue.Period6,
                Period7 = fm36LearningDeliveryPeriodisedValue.Period7,
                Period8 = fm36LearningDeliveryPeriodisedValue.Period8,
                Period9 = fm36LearningDeliveryPeriodisedValue.Period9,
                Period10 = fm36LearningDeliveryPeriodisedValue.Period10,
                Period11 = fm36LearningDeliveryPeriodisedValue.Period11,
                Period12 = fm36LearningDeliveryPeriodisedValue.Period12
            };
        }

        private LearningDeliveryPeriodisedTextValues MapFm36LearningDeliveryPeriodisedTextValues(FundingService.FM36.FundingOutput.Model.Output.LearningDeliveryPeriodisedTextValues fm36LearningDeliveryPeriodisedTextValue)
        {
            return new LearningDeliveryPeriodisedTextValues()
            {
                AttributeName = fm36LearningDeliveryPeriodisedTextValue.AttributeName,
                Period1 = fm36LearningDeliveryPeriodisedTextValue.Period1,
                Period2 = fm36LearningDeliveryPeriodisedTextValue.Period2,
                Period3 = fm36LearningDeliveryPeriodisedTextValue.Period3,
                Period4 = fm36LearningDeliveryPeriodisedTextValue.Period4,
                Period5 = fm36LearningDeliveryPeriodisedTextValue.Period5,
                Period6 = fm36LearningDeliveryPeriodisedTextValue.Period6,
                Period7 = fm36LearningDeliveryPeriodisedTextValue.Period7,
                Period8 = fm36LearningDeliveryPeriodisedTextValue.Period8,
                Period9 = fm36LearningDeliveryPeriodisedTextValue.Period9,
                Period10 = fm36LearningDeliveryPeriodisedTextValue.Period10,
                Period11 = fm36LearningDeliveryPeriodisedTextValue.Period11,
                Period12 = fm36LearningDeliveryPeriodisedTextValue.Period12
            };
        }
    }
}
