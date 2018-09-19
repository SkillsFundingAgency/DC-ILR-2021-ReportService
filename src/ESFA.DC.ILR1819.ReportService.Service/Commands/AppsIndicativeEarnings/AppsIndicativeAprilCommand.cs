﻿using System.Linq;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Attribute;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;

namespace ESFA.DC.ILR1819.ReportService.Service.Commands.AppsIndicativeEarnings
{
    public class AppsIndicativeAprilCommand : IAppsIndicativeCommand
    {
        public void Execute(AppsIndicativeEarningsModel model, LearningDeliveryAttribute learningDeliveryAttribute, PriceEpisodeAttribute episodeAttribute)
        {
            model.AprilOnProgrammeEarnings =
                episodeAttribute?.PriceEpisodePeriodisedAttributes
                    ?.SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeOnProgPaymentAttributeName)?.Period9 ?? 0;

            model.AprilBalancingPaymentEarnings =
                episodeAttribute?.PriceEpisodePeriodisedAttributes
                    ?.SingleOrDefault(att => att.AttributeName == Constants.Fm3PriceEpisodeBalancePaymentAttributeName)?.Period9 ?? 0;

            model.AprilAimCompletionEarnings =
                episodeAttribute?.PriceEpisodePeriodisedAttributes
                    ?.SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeCompletionPaymentAttributeName)?.Period9 ?? 0;

            model.AprilLearningSupportEarnings = learningDeliveryAttribute?.LearningDeliveryAttributeDatas?.LearnDelMathEng ?? false
                    ? learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                          ?.SingleOrDefault(att => att.AttributeName == Constants.Fm36LearnSuppFundCashAttributeName)?.Period9 ?? 0
                    : episodeAttribute?.PriceEpisodePeriodisedAttributes
                          ?.SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeLSFCashAttributeName)?.Period9 ?? 0;

            model.AprilEnglishMathsOnProgrammeEarnings =
                learningDeliveryAttribute?.LearningDeliveryPeriodisedAttributes
                    ?.SingleOrDefault(att => att.AttributeName == Constants.Fm36MathEngOnProgPaymentAttributeName)?.Period9 ?? 0;

            model.AprilEnglishMathsBalancingPaymentEarnings =
                learningDeliveryAttribute?.LearningDeliveryPeriodisedAttributes
                    ?.SingleOrDefault(att => att.AttributeName == Constants.Fm36MathEngBalPaymentAttributeName)?.Period9 ?? 0;

            model.AprilDisadvantageEarnings =
                episodeAttribute?.PriceEpisodePeriodisedAttributes
                    ?.SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeFirstDisadvantagePaymentAttributeName)?.Period9 ?? 0
                + episodeAttribute?.PriceEpisodePeriodisedAttributes
                    ?.SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeSecondDisadvantagePaymentAttributeName)?.Period9 ?? 0;

            model.April1618AdditionalPaymentForEmployers =
                episodeAttribute?.PriceEpisodePeriodisedAttributes
                    ?.SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeFirstEmp1618PayAttributeName)?.Period9 ?? 0
                + episodeAttribute?.PriceEpisodePeriodisedAttributes
                    ?.SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeSecondEmp1618PayAttributeName)?.Period9 ?? 0;

            model.April1618AdditionalPaymentForProviders =
                episodeAttribute?.PriceEpisodePeriodisedAttributes
                    ?.SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeFirstProv1618PayAttributeName)?.Period9 ?? 0
                + episodeAttribute?.PriceEpisodePeriodisedAttributes
                    ?.SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeSecondProv1618PayAttributeName)?.Period9 ?? 0;

            model.AprilAdditionalPaymentsForApprentices =
                episodeAttribute?.PriceEpisodePeriodisedAttributes
                    ?.SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeLearnerAdditionalPaymentAttributeName)?.Period9 ?? 0;

            model.April1618FrameworkUpliftOnProgrammePayment =
                episodeAttribute?.PriceEpisodePeriodisedAttributes
                    ?.SingleOrDefault(att =>
                        att.AttributeName ==
                        Constants.Fm36PriceEpisodeApplic1618FrameworkUpliftOnProgPaymentAttributeName)?.Period9 ?? 0;

            model.April1618FrameworkUpliftBalancingPayment =
                episodeAttribute?.PriceEpisodePeriodisedAttributes
                    ?.SingleOrDefault(att =>
                        att.AttributeName ==
                        Constants.Fm36PriceEpisodeApplic1618FrameworkUpliftBalancingAttributeName)?.Period9 ?? 0;

            model.April1618FrameworkUpliftCompletionPayment =
                episodeAttribute?.PriceEpisodePeriodisedAttributes
                    ?.SingleOrDefault(att =>
                        att.AttributeName ==
                        Constants.Fm36PriceEpisodeApplic1618FrameworkUpliftCompletionPaymentAttributeName)?.Period9 ?? 0;
        }
    }
}