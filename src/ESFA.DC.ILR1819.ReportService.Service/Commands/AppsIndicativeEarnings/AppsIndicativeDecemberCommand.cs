using System.Linq;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;

namespace ESFA.DC.ILR1819.ReportService.Service.Commands.AppsIndicativeEarnings
{
    public class AppsIndicativeDecemberCommand : IAppsIndicativeCommand
    {
        public void Execute(AppsIndicativeEarningsModel model, LearningDelivery LearningDelivery, PriceEpisode episodeAttribute)
        {
            model.DecemberOnProgrammeEarnings =
                episodeAttribute?.PriceEpisodePeriodisedValues
                    ?.SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeOnProgPaymentAttributeName)?.Period5 ?? 0;

            model.DecemberBalancingPaymentEarnings =
                episodeAttribute?.PriceEpisodePeriodisedValues
                    ?.SingleOrDefault(att => att.AttributeName == Constants.Fm3PriceEpisodeBalancePaymentAttributeName)?.Period5 ?? 0;

            model.DecemberAimCompletionEarnings =
                episodeAttribute?.PriceEpisodePeriodisedValues
                    ?.SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeCompletionPaymentAttributeName)?.Period5 ?? 0;

            model.DecemberLearningSupportEarnings = LearningDelivery?.LearningDeliveryValues?.LearnDelMathEng ?? false
                    ? LearningDelivery.LearningDeliveryPeriodisedValues
                          ?.SingleOrDefault(att => att.AttributeName == Constants.Fm36LearnSuppFundCashAttributeName)?.Period5 ?? 0
                    : episodeAttribute?.PriceEpisodePeriodisedValues
                          ?.SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeLSFCashAttributeName)?.Period5 ?? 0;

            model.DecemberEnglishMathsOnProgrammeEarnings =
                LearningDelivery?.LearningDeliveryPeriodisedValues
                    ?.SingleOrDefault(att => att.AttributeName == Constants.Fm36MathEngOnProgPaymentAttributeName)?.Period5 ?? 0;

            model.DecemberEnglishMathsBalancingPaymentEarnings =
                LearningDelivery?.LearningDeliveryPeriodisedValues
                    ?.SingleOrDefault(att => att.AttributeName == Constants.Fm36MathEngBalPaymentAttributeName)?.Period5 ?? 0;

            model.DecemberDisadvantageEarnings =
                episodeAttribute?.PriceEpisodePeriodisedValues
                    ?.SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeFirstDisadvantagePaymentAttributeName)?.Period5 ?? 0
                + episodeAttribute?.PriceEpisodePeriodisedValues
                    ?.SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeSecondDisadvantagePaymentAttributeName)?.Period5 ?? 0;

            model.December1618AdditionalPaymentForEmployers =
                episodeAttribute?.PriceEpisodePeriodisedValues
                    ?.SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeFirstEmp1618PayAttributeName)?.Period5 ?? 0
                + episodeAttribute?.PriceEpisodePeriodisedValues
                    ?.SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeSecondEmp1618PayAttributeName)?.Period5 ?? 0;

            model.December1618AdditionalPaymentForProviders =
                episodeAttribute?.PriceEpisodePeriodisedValues
                    ?.SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeFirstProv1618PayAttributeName)?.Period5 ?? 0
                + episodeAttribute?.PriceEpisodePeriodisedValues
                    ?.SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeSecondProv1618PayAttributeName)?.Period5 ?? 0;

            model.DecemberAdditionalPaymentsForApprentices =
                episodeAttribute?.PriceEpisodePeriodisedValues
                    ?.SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeLearnerAdditionalPaymentAttributeName)?.Period5 ?? 0;

            model.December1618FrameworkUpliftOnProgrammePayment =
                episodeAttribute?.PriceEpisodePeriodisedValues
                    ?.SingleOrDefault(att =>
                        att.AttributeName ==
                        Constants.Fm36PriceEpisodeApplic1618FrameworkUpliftOnProgPaymentAttributeName)?.Period5 ?? 0;

            model.December1618FrameworkUpliftBalancingPayment =
                episodeAttribute?.PriceEpisodePeriodisedValues
                    ?.SingleOrDefault(att =>
                        att.AttributeName ==
                        Constants.Fm36PriceEpisodeApplic1618FrameworkUpliftBalancingAttributeName)?.Period5 ?? 0;

            model.December1618FrameworkUpliftCompletionPayment =
                episodeAttribute?.PriceEpisodePeriodisedValues
                    ?.SingleOrDefault(att =>
                        att.AttributeName ==
                        Constants.Fm36PriceEpisodeApplic1618FrameworkUpliftCompletionPaymentAttributeName)?.Period5 ?? 0;
        }
    }
}
