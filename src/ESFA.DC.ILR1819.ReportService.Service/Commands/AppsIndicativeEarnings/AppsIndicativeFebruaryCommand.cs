using System.Linq;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Attribute;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;

namespace ESFA.DC.ILR1819.ReportService.Service.Commands.AppsIndicativeEarnings
{
    public class AppsIndicativeFebruaryCommand : IAppsIndicativeCommand
    {
        public void Execute(AppsIndicativeEarningsModel model, LearningDeliveryAttribute learningDeliveryAttribute)
        {
            model.FebruaryOnProgrammeEarnings =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeOnProgPaymentAttributeName)?.Period7 ?? 0;

            model.FebruaryBalancingPaymentEarnings =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm3PriceEpisodeBalancePaymentAttributeName)?.Period7 ?? 0;

            model.FebruaryAimCompletionEarnings =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeCompletionPaymentAttributeName)?.Period7 ?? 0;

            var periodAttribute = learningDeliveryAttribute.LearningDeliveryAttributeDatas.LearnDelMathEng ?? false
                ? learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36LearnSuppFundCashAttributeName)
                : learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeLSFCashAttributeName);
            model.FebruaryLearningSupportEarnings = periodAttribute?.Period7 ?? 0;

            model.FebruaryEnglishMathsOnProgrammeEarnings =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36MathEngOnProgPaymentAttributeName)?.Period7 ?? 0;

            model.FebruaryEnglishMathsBalancingPaymentEarnings =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36MathEngBalPaymentAttributeName)?.Period7 ?? 0;

            model.FebruaryDisadvantageEarnings =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeFirstDisadvantagePaymentAttributeName)?.Period7 ?? 0
                + learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeSecondDisadvantagePaymentAttributeName)?.Period7 ?? 0;

            model.February1618AdditionalPaymentForEmployers =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeFirstEmp1618PayAttributeName)?.Period7 ?? 0
                + learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeSecondEmp1618PayAttributeName)?.Period7 ?? 0;

            model.February1618AdditionalPaymentForProviders =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeFirstProv1618PayAttributeName)?.Period7 ?? 0
                + learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeSecondProv1618PayAttributeName)?.Period7 ?? 0;

            model.FebruaryAdditionalPaymentsForApprentices =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeLearnerAdditionalPaymentAttributeName)?.Period7 ?? 0;

            model.February1618FrameworkUpliftOnProgrammePayment =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att =>
                        att.AttributeName ==
                        Constants.Fm36PriceEpisodeApplic1618FrameworkUpliftOnProgPaymentAttributeName)?.Period7 ?? 0;

            model.February1618FrameworkUpliftBalancingPayment =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att =>
                        att.AttributeName ==
                        Constants.Fm36PriceEpisodeApplic1618FrameworkUpliftBalancingAttributeName)?.Period7 ?? 0;

            model.February1618FrameworkUpliftCompletionPayment =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att =>
                        att.AttributeName ==
                        Constants.Fm36PriceEpisodeApplic1618FrameworkUpliftCompletionPaymentAttributeName)?.Period7 ?? 0;
        }
    }
}
