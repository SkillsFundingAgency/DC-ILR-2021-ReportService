using System.Linq;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Attribute;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;

namespace ESFA.DC.ILR1819.ReportService.Service.Commands.AppsIndicativeEarnings
{
    public class AppsIndicativeNovemberCommand : IAppsIndicativeCommand
    {
        public void Execute(AppsIndicativeEarningsModel model, LearningDeliveryAttribute learningDeliveryAttribute, PriceEpisodeAttribute episodeAttribute)
        {
            model.NovemberOnProgrammeEarnings =
                episodeAttribute?.PriceEpisodePeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeOnProgPaymentAttributeName)?.Period4 ?? 0;

            model.NovemberBalancingPaymentEarnings =
                episodeAttribute?.PriceEpisodePeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm3PriceEpisodeBalancePaymentAttributeName)?.Period4 ?? 0;

            model.NovemberAimCompletionEarnings =
                episodeAttribute?.PriceEpisodePeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeCompletionPaymentAttributeName)?.Period4 ?? 0;

            model.NovemberLearningSupportEarnings = learningDeliveryAttribute.LearningDeliveryAttributeDatas.LearnDelMathEng ?? false
                    ? learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                          .SingleOrDefault(att => att.AttributeName == Constants.Fm36LearnSuppFundCashAttributeName)?.Period4 ?? 0
                    : episodeAttribute?.PriceEpisodePeriodisedAttributes
                          .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeLSFCashAttributeName)?.Period4 ?? 0;

            model.NovemberEnglishMathsOnProgrammeEarnings =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36MathEngOnProgPaymentAttributeName)?.Period4 ?? 0;

            model.NovemberEnglishMathsBalancingPaymentEarnings =
                learningDeliveryAttribute.LearningDeliveryPeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36MathEngBalPaymentAttributeName)?.Period4 ?? 0;

            model.NovemberDisadvantageEarnings =
                episodeAttribute?.PriceEpisodePeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeFirstDisadvantagePaymentAttributeName)?.Period4 ?? 0
                + episodeAttribute?.PriceEpisodePeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeSecondDisadvantagePaymentAttributeName)?.Period4 ?? 0;

            model.November1618AdditionalPaymentForEmployers =
                episodeAttribute?.PriceEpisodePeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeFirstEmp1618PayAttributeName)?.Period4 ?? 0
                + episodeAttribute?.PriceEpisodePeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeSecondEmp1618PayAttributeName)?.Period4 ?? 0;

            model.November1618AdditionalPaymentForProviders =
                episodeAttribute?.PriceEpisodePeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeFirstProv1618PayAttributeName)?.Period4 ?? 0
                + episodeAttribute?.PriceEpisodePeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeSecondProv1618PayAttributeName)?.Period4 ?? 0;

            model.NovemberAdditionalPaymentsForApprentices =
                episodeAttribute?.PriceEpisodePeriodisedAttributes
                    .SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeLearnerAdditionalPaymentAttributeName)?.Period4 ?? 0;

            model.November1618FrameworkUpliftOnProgrammePayment =
                episodeAttribute?.PriceEpisodePeriodisedAttributes
                    .SingleOrDefault(att =>
                        att.AttributeName ==
                        Constants.Fm36PriceEpisodeApplic1618FrameworkUpliftOnProgPaymentAttributeName)?.Period4 ?? 0;

            model.November1618FrameworkUpliftBalancingPayment =
                episodeAttribute?.PriceEpisodePeriodisedAttributes
                    .SingleOrDefault(att =>
                        att.AttributeName ==
                        Constants.Fm36PriceEpisodeApplic1618FrameworkUpliftBalancingAttributeName)?.Period4 ?? 0;

            model.November1618FrameworkUpliftCompletionPayment =
                episodeAttribute?.PriceEpisodePeriodisedAttributes
                    .SingleOrDefault(att =>
                        att.AttributeName ==
                        Constants.Fm36PriceEpisodeApplic1618FrameworkUpliftCompletionPaymentAttributeName)?.Period4 ?? 0;
        }
    }
}
