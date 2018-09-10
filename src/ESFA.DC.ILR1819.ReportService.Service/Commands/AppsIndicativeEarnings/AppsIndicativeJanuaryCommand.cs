using System.Linq;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Attribute;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;

namespace ESFA.DC.ILR1819.ReportService.Service.Commands.AppsIndicativeEarnings
{
    public class AppsIndicativeJanuaryCommand : IAppsIndicativeCommand
    {
        public void Execute(AppsIndicativeEarningsModel model, LearningDeliveryAttribute learningDeliveryAttribute)
        {
            model.JanuaryOnProgrammeEarnings =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeOnProgPaymentAttributeName)?.Period6 ?? 0;

            model.JanuaryBalancingPaymentEarnings =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm3PriceEpisodeBalancePaymentAttributeName)?.Period6 ?? 0;

            model.JanuaryAimCompletionEarnings =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeCompletionPaymentAttributeName)?.Period6 ?? 0;

            var periodAttribute = learningDeliveryAttribute.LearningDeliveryAttributeDatas.LearnDelMathEng ?? false
                ? learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36LearnSuppFundCashAttributeName)
                : learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeLSFCashAttributeName);
            model.JanuaryLearningSupportEarnings = periodAttribute?.Period6 ?? 0;

            model.JanuaryEnglishMathsOnProgrammeEarnings =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36MathEngOnProgPaymentAttributeName)?.Period6 ?? 0;

            model.JanuaryEnglishMathsBalancingPaymentEarnings =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36MathEngBalPaymentAttributeName)?.Period6 ?? 0;

            model.JanuaryDisadvantageEarnings =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeFirstDisadvantagePaymentAttributeName)?.Period6 ?? 0
                + learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeSecondDisadvantagePaymentAttributeName)?.Period6 ?? 0;

            model.January1618AdditionalPaymentForEmployers =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeFirstEmp1618PayAttributeName)?.Period6 ?? 0
                + learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeSecondEmp1618PayAttributeName)?.Period6 ?? 0;

            model.January1618AdditionalPaymentForProviders =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeFirstProv1618PayAttributeName)?.Period6 ?? 0
                + learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeSecondProv1618PayAttributeName)?.Period6 ?? 0;

            model.JanuaryAdditionalPaymentsForApprentices =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeLearnerAdditionalPaymentAttributeName)?.Period6 ?? 0;

            model.January1618FrameworkUpliftOnProgrammePayment =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att =>
                        att.AttributeName ==
                        Constants.Fm36PriceEpisodeApplic1618FrameworkUpliftOnProgPaymentAttributeName)?.Period6 ?? 0;

            model.January1618FrameworkUpliftBalancingPayment =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att =>
                        att.AttributeName ==
                        Constants.Fm36PriceEpisodeApplic1618FrameworkUpliftBalancingAttributeName)?.Period6 ?? 0;

            model.January1618FrameworkUpliftCompletionPayment =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att =>
                        att.AttributeName ==
                        Constants.Fm36PriceEpisodeApplic1618FrameworkUpliftCompletionPaymentAttributeName)?.Period6 ?? 0;
        }
    }
}
