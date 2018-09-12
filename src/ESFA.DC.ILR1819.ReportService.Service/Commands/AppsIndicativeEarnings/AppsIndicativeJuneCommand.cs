using System.Linq;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Attribute;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;

namespace ESFA.DC.ILR1819.ReportService.Service.Commands.AppsIndicativeEarnings
{
    public class AppsIndicativeJuneCommand : IAppsIndicativeCommand
    {
        public void Execute(AppsIndicativeEarningsModel model, LearningDeliveryAttribute learningDeliveryAttribute, PriceEpisodeAttribute episodeAttribute)
        {
            model.JuneOnProgrammeEarnings =
                episodeAttribute?.PriceEpisodePeriodisedAttributes
                    ?.SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeOnProgPaymentAttributeName)?.Period10 ?? 0;

            model.JuneBalancingPaymentEarnings =
                episodeAttribute?.PriceEpisodePeriodisedAttributes
                    ?.SingleOrDefault(att => att.AttributeName == Constants.Fm3PriceEpisodeBalancePaymentAttributeName)?.Period10 ?? 0;

            model.JuneAimCompletionEarnings =
                episodeAttribute?.PriceEpisodePeriodisedAttributes
                    ?.SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeCompletionPaymentAttributeName)?.Period10 ?? 0;

            model.JuneLearningSupportEarnings = learningDeliveryAttribute?.LearningDeliveryAttributeDatas?.LearnDelMathEng ?? false
                    ? learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                          ?.SingleOrDefault(att => att.AttributeName == Constants.Fm36LearnSuppFundCashAttributeName)?.Period10 ?? 0
                    : episodeAttribute?.PriceEpisodePeriodisedAttributes
                          ?.SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeLSFCashAttributeName)?.Period10 ?? 0;

            model.JuneEnglishMathsOnProgrammeEarnings =
                learningDeliveryAttribute?.LearningDeliveryPeriodisedAttributes
                    ?.SingleOrDefault(att => att.AttributeName == Constants.Fm36MathEngOnProgPaymentAttributeName)?.Period10 ?? 0;

            model.JuneEnglishMathsBalancingPaymentEarnings =
                learningDeliveryAttribute?.LearningDeliveryPeriodisedAttributes
                    ?.SingleOrDefault(att => att.AttributeName == Constants.Fm36MathEngBalPaymentAttributeName)?.Period10 ?? 0;

            model.JuneDisadvantageEarnings =
                episodeAttribute?.PriceEpisodePeriodisedAttributes
                    ?.SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeFirstDisadvantagePaymentAttributeName)?.Period10 ?? 0
                + episodeAttribute?.PriceEpisodePeriodisedAttributes
                    ?.SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeSecondDisadvantagePaymentAttributeName)?.Period10 ?? 0;

            model.June1618AdditionalPaymentForEmployers =
                episodeAttribute?.PriceEpisodePeriodisedAttributes
                    ?.SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeFirstEmp1618PayAttributeName)?.Period10 ?? 0
                + episodeAttribute?.PriceEpisodePeriodisedAttributes
                    ?.SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeSecondEmp1618PayAttributeName)?.Period10 ?? 0;

            model.June1618AdditionalPaymentForProviders =
                episodeAttribute?.PriceEpisodePeriodisedAttributes
                    ?.SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeFirstProv1618PayAttributeName)?.Period10 ?? 0
                + episodeAttribute?.PriceEpisodePeriodisedAttributes
                    ?.SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeSecondProv1618PayAttributeName)?.Period10 ?? 0;

            model.JuneAdditionalPaymentsForApprentices =
                episodeAttribute?.PriceEpisodePeriodisedAttributes
                    ?.SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeLearnerAdditionalPaymentAttributeName)?.Period10 ?? 0;

            model.June1618FrameworkUpliftOnProgrammePayment =
                episodeAttribute?.PriceEpisodePeriodisedAttributes
                    ?.SingleOrDefault(att =>
                        att.AttributeName ==
                        Constants.Fm36PriceEpisodeApplic1618FrameworkUpliftOnProgPaymentAttributeName)?.Period10 ?? 0;

            model.June1618FrameworkUpliftBalancingPayment =
                episodeAttribute?.PriceEpisodePeriodisedAttributes
                    ?.SingleOrDefault(att =>
                        att.AttributeName ==
                        Constants.Fm36PriceEpisodeApplic1618FrameworkUpliftBalancingAttributeName)?.Period10 ?? 0;

            model.June1618FrameworkUpliftCompletionPayment =
                episodeAttribute?.PriceEpisodePeriodisedAttributes
                    ?.SingleOrDefault(att =>
                        att.AttributeName ==
                        Constants.Fm36PriceEpisodeApplic1618FrameworkUpliftCompletionPaymentAttributeName)?.Period10 ?? 0;
        }
    }
}
