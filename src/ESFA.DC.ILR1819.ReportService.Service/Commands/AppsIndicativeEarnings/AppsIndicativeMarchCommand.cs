using System.Linq;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;

namespace ESFA.DC.ILR1819.ReportService.Service.Commands.AppsIndicativeEarnings
{
    public class AppsIndicativeMarchCommand : IAppsIndicativeCommand
    {
        public void Execute(AppsIndicativeEarningsModel model, LearningDelivery LearningDelivery, PriceEpisode episodeAttribute)
        {
            model.MarchOnProgrammeEarnings =
                episodeAttribute?.PriceEpisodePeriodisedValues
                    ?.SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeOnProgPaymentAttributeName)?.Period8 ?? 0;

            model.MarchBalancingPaymentEarnings =
                episodeAttribute?.PriceEpisodePeriodisedValues
                    ?.SingleOrDefault(att => att.AttributeName == Constants.Fm3PriceEpisodeBalancePaymentAttributeName)?.Period8 ?? 0;

            model.MarchAimCompletionEarnings =
                episodeAttribute?.PriceEpisodePeriodisedValues
                    ?.SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeCompletionPaymentAttributeName)?.Period8 ?? 0;

            model.MarchLearningSupportEarnings = LearningDelivery?.LearningDeliveryValues?.LearnDelMathEng ?? false
                    ? LearningDelivery.LearningDeliveryPeriodisedValues
                          ?.SingleOrDefault(att => att.AttributeName == Constants.Fm36LearnSuppFundCashAttributeName)?.Period8 ?? 0
                    : episodeAttribute?.PriceEpisodePeriodisedValues
                          ?.SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeLSFCashAttributeName)?.Period8 ?? 0;

            model.MarchEnglishMathsOnProgrammeEarnings =
                LearningDelivery?.LearningDeliveryPeriodisedValues
                    ?.SingleOrDefault(att => att.AttributeName == Constants.Fm36MathEngOnProgPaymentAttributeName)?.Period8 ?? 0;

            model.MarchEnglishMathsBalancingPaymentEarnings =
                LearningDelivery?.LearningDeliveryPeriodisedValues
                    ?.SingleOrDefault(att => att.AttributeName == Constants.Fm36MathEngBalPaymentAttributeName)?.Period8 ?? 0;

            model.MarchDisadvantageEarnings =
                episodeAttribute?.PriceEpisodePeriodisedValues
                    ?.SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeFirstDisadvantagePaymentAttributeName)?.Period8 ?? 0
                + episodeAttribute?.PriceEpisodePeriodisedValues
                    ?.SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeSecondDisadvantagePaymentAttributeName)?.Period8 ?? 0;

            model.March1618AdditionalPaymentForEmployers =
                episodeAttribute?.PriceEpisodePeriodisedValues
                    ?.SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeFirstEmp1618PayAttributeName)?.Period8 ?? 0
                + episodeAttribute?.PriceEpisodePeriodisedValues
                    ?.SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeSecondEmp1618PayAttributeName)?.Period8 ?? 0;

            model.March1618AdditionalPaymentForProviders =
                episodeAttribute?.PriceEpisodePeriodisedValues
                    ?.SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeFirstProv1618PayAttributeName)?.Period8 ?? 0
                + episodeAttribute?.PriceEpisodePeriodisedValues
                    ?.SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeSecondProv1618PayAttributeName)?.Period8 ?? 0;

            model.MarchAdditionalPaymentsForApprentices =
                episodeAttribute?.PriceEpisodePeriodisedValues
                    ?.SingleOrDefault(att => att.AttributeName == Constants.Fm36PriceEpisodeLearnerAdditionalPaymentAttributeName)?.Period8 ?? 0;

            model.March1618FrameworkUpliftOnProgrammePayment =
                episodeAttribute?.PriceEpisodePeriodisedValues
                    ?.SingleOrDefault(att =>
                        att.AttributeName ==
                        Constants.Fm36PriceEpisodeApplic1618FrameworkUpliftOnProgPaymentAttributeName)?.Period8 ?? 0;

            model.March1618FrameworkUpliftBalancingPayment =
                episodeAttribute?.PriceEpisodePeriodisedValues
                    ?.SingleOrDefault(att =>
                        att.AttributeName ==
                        Constants.Fm36PriceEpisodeApplic1618FrameworkUpliftBalancingAttributeName)?.Period8 ?? 0;

            model.March1618FrameworkUpliftCompletionPayment =
                episodeAttribute?.PriceEpisodePeriodisedValues
                    ?.SingleOrDefault(att =>
                        att.AttributeName ==
                        Constants.Fm36PriceEpisodeApplic1618FrameworkUpliftCompletionPaymentAttributeName)?.Period8 ?? 0;
        }
    }
}
