using System.Linq;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Attribute;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;

namespace ESFA.DC.ILR1819.ReportService.Service.Commands.AppsIndicativeEarnings
{
    public class AppsIndicativeSeptemberCommand : IAppsIndicativeCommand
    {
        public void Execute(AppsIndicativeEarningsModel model, LearningDeliveryAttribute learningDeliveryAttribute)
        {
            model.SeptemberOnProgrammeEarnings =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeOnProgPaymentAttributeName)?.Period2 ?? 0;

            model.SeptemberBalancingPaymentEarnings =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm3PriceEpisodeBalancePaymentAttributeName)?.Period2 ?? 0;

            model.SeptemberAimCompletionEarnings =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeCompletionPaymentAttributeName)?.Period2 ?? 0;

            var periodAttribute = learningDeliveryAttribute.LearningDeliveryAttributeDatas.LearnDelMathEng ?? false
                ? learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36LearnSuppFundCashAttributeName)
                : learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeLSFCashAttributeName);
            model.SeptemberLearningSupportEarnings = periodAttribute?.Period2 ?? 0;

            model.SeptemberEnglishMathsOnProgrammeEarnings =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36MathEngOnProgPaymentAttributeName)?.Period2 ?? 0;

            model.SeptemberEnglishMathsBalancingPaymentEarnings =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36MathEngBalPaymentAttributeName)?.Period2 ?? 0;

            model.SeptemberDisadvantageEarnings =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeFirstDisadvantagePaymentAttributeName)?.Period2 ?? 0
                + learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeSecondDisadvantagePaymentAttributeName)?.Period2 ?? 0;

            model.September1618AdditionalPaymentForEmployers =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeFirstEmp1618PayAttributeName)?.Period2 ?? 0
                + learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeSecondEmp1618PayAttributeName)?.Period2 ?? 0;

            model.September1618AdditionalPaymentForProviders =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeFirstProv1618PayAttributeName)?.Period2 ?? 0
                + learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeSecondProv1618PayAttributeName)?.Period2 ?? 0;

            model.SeptemberAdditionalPaymentsForApprentices =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeLearnerAdditionalPaymentAttributeName)?.Period2 ?? 0;

            model.September1618FrameworkUpliftOnProgrammePayment =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att =>
                        att.AttributeName ==
                        Constants.Fm36PriceEpisodeApplic1618FrameworkUpliftOnProgPaymentAttributeName)?.Period2 ?? 0;

            model.September1618FrameworkUpliftBalancingPayment =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att =>
                        att.AttributeName ==
                        Constants.Fm36PriceEpisodeApplic1618FrameworkUpliftBalancingAttributeName)?.Period2 ?? 0;

            model.September1618FrameworkUpliftCompletionPayment =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att =>
                        att.AttributeName ==
                        Constants.Fm36PriceEpisodeApplic1618FrameworkUpliftCompletionPaymentAttributeName)?.Period2 ?? 0;
        }
    }
}
