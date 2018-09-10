using System.Linq;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Attribute;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;

namespace ESFA.DC.ILR1819.ReportService.Service.Commands.AppsIndicativeEarnings
{
    public class AppsIndicativeAugustCommand : IAppsIndicativeCommand
    {
        public void Execute(AppsIndicativeEarningsModel model, LearningDeliveryAttribute learningDeliveryAttribute)
        {
            model.AugustOnProgrammeEarnings =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeOnProgPaymentAttributeName)?.Period1 ?? 0;

            model.AugustBalancingPaymentEarnings =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm3PriceEpisodeBalancePaymentAttributeName)?.Period1 ?? 0;

            model.AugustAimCompletionEarnings =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeCompletionPaymentAttributeName)?.Period1 ?? 0;

            var periodAttribute = learningDeliveryAttribute.LearningDeliveryAttributeDatas.LearnDelMathEng ?? false
                ? learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36LearnSuppFundCashAttributeName)
                : learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeLSFCashAttributeName);
            model.AugustLearningSupportEarnings = periodAttribute?.Period1 ?? 0;

            model.AugustEnglishMathsOnProgrammeEarnings =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36MathEngOnProgPaymentAttributeName)?.Period1 ?? 0;

            model.AugustEnglishMathsBalancingPaymentEarnings =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36MathEngBalPaymentAttributeName)?.Period1 ?? 0;

            model.AugustDisadvantageEarnings =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeFirstDisadvantagePaymentAttributeName)?.Period1 ?? 0
                + learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeSecondDisadvantagePaymentAttributeName)?.Period1 ?? 0;

            model.August1618AdditionalPaymentForEmployers =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeFirstEmp1618PayAttributeName)?.Period1 ?? 0
                + learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeSecondEmp1618PayAttributeName)?.Period1 ?? 0;

            model.August1618AdditionalPaymentForProviders =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeFirstProv1618PayAttributeName)?.Period1 ?? 0
                + learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeSecondProv1618PayAttributeName)?.Period1 ?? 0;

            model.AugustAdditionalPaymentsForApprentices =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeLearnerAdditionalPaymentAttributeName)?.Period1 ?? 0;

            model.August1618FrameworkUpliftOnProgrammePayment =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att =>
                        att.AttributeName ==
                        Constants.Fm36PriceEpisodeApplic1618FrameworkUpliftOnProgPaymentAttributeName)?.Period1 ?? 0;

            model.August1618FrameworkUpliftBalancingPayment =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att =>
                        att.AttributeName ==
                        Constants.Fm36PriceEpisodeApplic1618FrameworkUpliftBalancingAttributeName)?.Period1 ?? 0;

            model.August1618FrameworkUpliftCompletionPayment =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att =>
                        att.AttributeName ==
                        Constants.Fm36PriceEpisodeApplic1618FrameworkUpliftCompletionPaymentAttributeName)?.Period1 ?? 0;
        }
    }
}
