using System.Linq;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Attribute;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;

namespace ESFA.DC.ILR1819.ReportService.Service.Commands.AppsIndicativeEarnings
{
    public class AppsIndicativeOctoberCommand : IAppsIndicativeCommand
    {
        public void Execute(AppsIndicativeEarningsModel model, LearningDeliveryAttribute learningDeliveryAttribute)
        {
            model.OctoberOnProgrammeEarnings =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeOnProgPaymentAttributeName)?.Period3 ?? 0;

            model.OctoberBalancingPaymentEarnings =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm3PriceEpisodeBalancePaymentAttributeName)?.Period3 ?? 0;

            model.OctoberAimCompletionEarnings =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeCompletionPaymentAttributeName)?.Period3 ?? 0;

            var periodAttribute = learningDeliveryAttribute.LearningDeliveryAttributeDatas.LearnDelMathEng ?? false
                ? learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36LearnSuppFundCashAttributeName)
                : learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeLSFCashAttributeName);
            model.OctoberLearningSupportEarnings = periodAttribute?.Period3 ?? 0;

            model.OctoberEnglishMathsOnProgrammeEarnings =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36MathEngOnProgPaymentAttributeName)?.Period3 ?? 0;

            model.OctoberEnglishMathsBalancingPaymentEarnings =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36MathEngBalPaymentAttributeName)?.Period3 ?? 0;

            model.OctoberDisadvantageEarnings =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeFirstDisadvantagePaymentAttributeName)?.Period3 ?? 0
                + learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeSecondDisadvantagePaymentAttributeName)?.Period3 ?? 0;

            model.October1618AdditionalPaymentForEmployers =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeFirstEmp1618PayAttributeName)?.Period3 ?? 0
                + learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeSecondEmp1618PayAttributeName)?.Period3 ?? 0;

            model.October1618AdditionalPaymentForProviders =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeFirstProv1618PayAttributeName)?.Period3 ?? 0
                + learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeSecondProv1618PayAttributeName)?.Period3 ?? 0;

            model.OctoberAdditionalPaymentsForApprentices =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeLearnerAdditionalPaymentAttributeName)?.Period3 ?? 0;

            model.October1618FrameworkUpliftOnProgrammePayment =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att =>
                        att.AttributeName ==
                        Constants.Fm36PriceEpisodeApplic1618FrameworkUpliftOnProgPaymentAttributeName)?.Period3 ?? 0;

            model.October1618FrameworkUpliftBalancingPayment =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att =>
                        att.AttributeName ==
                        Constants.Fm36PriceEpisodeApplic1618FrameworkUpliftBalancingAttributeName)?.Period3 ?? 0;

            model.October1618FrameworkUpliftCompletionPayment =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att =>
                        att.AttributeName ==
                        Constants.Fm36PriceEpisodeApplic1618FrameworkUpliftCompletionPaymentAttributeName)?.Period3 ?? 0;
        }
    }
}
