using System.Linq;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Attribute;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;

namespace ESFA.DC.ILR1819.ReportService.Service.Commands.AppsIndicativeEarnings
{
    public class AppsIndicativeMarchCommand : IAppsIndicativeCommand
    {
        public void Execute(AppsIndicativeEarningsModel model, LearningDeliveryAttribute learningDeliveryAttribute, PriceEpisodeAttribute episodeAttribute)
        {
            model.MarchOnProgrammeEarnings =
                episodeAttribute?.PriceEpisodePeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeOnProgPaymentAttributeName)?.Period8 ?? 0;

            model.MarchBalancingPaymentEarnings =
                episodeAttribute?.PriceEpisodePeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm3PriceEpisodeBalancePaymentAttributeName)?.Period8 ?? 0;

            model.MarchAimCompletionEarnings =
                episodeAttribute?.PriceEpisodePeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeCompletionPaymentAttributeName)?.Period8 ?? 0;

            model.MarchLearningSupportEarnings = learningDeliveryAttribute.LearningDeliveryAttributeDatas.LearnDelMathEng ?? false
                    ? learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                          .SingleOrDefault(att => att.AttributeName == Constants.Fm36LearnSuppFundCashAttributeName)?.Period8 ?? 0
                    : episodeAttribute?.PriceEpisodePeriodisedAttributes
                          .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeLSFCashAttributeName)?.Period8 ?? 0;

            model.MarchEnglishMathsOnProgrammeEarnings =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36MathEngOnProgPaymentAttributeName)?.Period8 ?? 0;

            model.MarchEnglishMathsBalancingPaymentEarnings =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36MathEngBalPaymentAttributeName)?.Period8 ?? 0;

            model.MarchDisadvantageEarnings =
                episodeAttribute?.PriceEpisodePeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeFirstDisadvantagePaymentAttributeName)?.Period8 ?? 0
                + episodeAttribute?.PriceEpisodePeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeSecondDisadvantagePaymentAttributeName)?.Period8 ?? 0;

            model.March1618AdditionalPaymentForEmployers =
                episodeAttribute?.PriceEpisodePeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeFirstEmp1618PayAttributeName)?.Period8 ?? 0
                + episodeAttribute?.PriceEpisodePeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeSecondEmp1618PayAttributeName)?.Period8 ?? 0;

            model.March1618AdditionalPaymentForProviders =
                episodeAttribute?.PriceEpisodePeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeFirstProv1618PayAttributeName)?.Period8 ?? 0
                + episodeAttribute?.PriceEpisodePeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeSecondProv1618PayAttributeName)?.Period8 ?? 0;

            model.MarchAdditionalPaymentsForApprentices =
                episodeAttribute?.PriceEpisodePeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeLearnerAdditionalPaymentAttributeName)?.Period8 ?? 0;

            model.March1618FrameworkUpliftOnProgrammePayment =
                episodeAttribute?.PriceEpisodePeriodisedAttributes
                    .SingleOrDefault(att =>
                        att.AttributeName ==
                        Constants.Fm36PriceEpisodeApplic1618FrameworkUpliftOnProgPaymentAttributeName)?.Period8 ?? 0;

            model.March1618FrameworkUpliftBalancingPayment =
                episodeAttribute?.PriceEpisodePeriodisedAttributes
                    .SingleOrDefault(att =>
                        att.AttributeName ==
                        Constants.Fm36PriceEpisodeApplic1618FrameworkUpliftBalancingAttributeName)?.Period8 ?? 0;

            model.March1618FrameworkUpliftCompletionPayment =
                episodeAttribute?.PriceEpisodePeriodisedAttributes
                    .SingleOrDefault(att =>
                        att.AttributeName ==
                        Constants.Fm36PriceEpisodeApplic1618FrameworkUpliftCompletionPaymentAttributeName)?.Period8 ?? 0;
        }
    }
}
