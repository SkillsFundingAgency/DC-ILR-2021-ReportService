using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.FundingService.FM81.FundingOutput.Model.Output;
using ESFA.DC.ILR.ReportService.Interface.Builders;
using ESFA.DC.ILR.ReportService.Model.ReportModels;

namespace ESFA.DC.ILR.ReportService.Service.Builders
{
    public class TrailblazerEmployerIncentivesModelBuilder : ITrailblazerEmployerIncentivesModelBuilder
    {
        public TrailblazerEmployerIncentivesModel BuildTrailblazerEmployerIncentivesModel(long empIdentifier, Dictionary<string, int> employerIdentifier, List<LearningDelivery> fm81Data)
        {
            var smallBusPaymentList = fm81Data?.Where(x => x.LearningDeliveryValues?.EmpIdSmallBusDate == empIdentifier).FirstOrDefault()?.LearningDeliveryPeriodisedValues.Where(l => l.AttributeName == Constants.Fm81SmallBusPayment).ToList();
            var youngAppFirstPaymentList = fm81Data?.Where(x => x.LearningDeliveryValues?.EmpIdFirstYoungAppDate == empIdentifier).FirstOrDefault()?.LearningDeliveryPeriodisedValues.Where(l => l.AttributeName == Constants.Fm81YoungAppPayment).ToList();
            var youngAppSecondPaymentList = fm81Data?.Where(x => x.LearningDeliveryValues?.EmpIdSecondYoungAppDate == empIdentifier).FirstOrDefault()?.LearningDeliveryPeriodisedValues.Where(l => l.AttributeName == "YoungAppSecondPayment").ToList();
            var achPaymentList = fm81Data?.Where(x => x.LearningDeliveryValues?.EmpIdAchDate == empIdentifier).FirstOrDefault()?.LearningDeliveryPeriodisedValues.Where(l => l.AttributeName == Constants.Fm81AchPayment).ToList();

            var trailblazerEmployerIncentiveModel = new TrailblazerEmployerIncentivesModel()
            {
                EmployerIdentifier = empIdentifier,
                Period1SmallEmployerIncentive = smallBusPaymentList?.Sum(x => x.Period1) ?? decimal.Zero,
                Period2SmallEmployerIncentive = smallBusPaymentList?.Sum(x => x.Period2) ?? decimal.Zero,
                Period3SmallEmployerIncentive = smallBusPaymentList?.Sum(x => x.Period3) ?? decimal.Zero,
                Period4SmallEmployerIncentive = smallBusPaymentList?.Sum(x => x.Period4) ?? decimal.Zero,
                Period5SmallEmployerIncentive = smallBusPaymentList?.Sum(x => x.Period5) ?? decimal.Zero,
                Period6SmallEmployerIncentive = smallBusPaymentList?.Sum(x => x.Period6) ?? decimal.Zero,
                Period7SmallEmployerIncentive = smallBusPaymentList?.Sum(x => x.Period7) ?? decimal.Zero,
                Period8SmallEmployerIncentive = smallBusPaymentList?.Sum(x => x.Period8) ?? decimal.Zero,
                Period9SmallEmployerIncentive = smallBusPaymentList?.Sum(x => x.Period9) ?? decimal.Zero,
                Period10SmallEmployerIncentive = smallBusPaymentList?.Sum(x => x.Period10) ?? decimal.Zero,
                Period11SmallEmployerIncentive = smallBusPaymentList?.Sum(x => x.Period11) ?? decimal.Zero,
                Period12SmallEmployerIncentive = smallBusPaymentList?.Sum(x => x.Period12) ?? decimal.Zero,

                Period11618ApprenticeIncentive = youngAppFirstPaymentList?.Sum(x => x.Period1) + youngAppSecondPaymentList?.Sum(x => x.Period1) ?? decimal.Zero,
                Period21618ApprenticeIncentive = youngAppFirstPaymentList?.Sum(x => x.Period2) + youngAppSecondPaymentList?.Sum(x => x.Period2) ?? decimal.Zero,
                Period31618ApprenticeIncentive = youngAppFirstPaymentList?.Sum(x => x.Period3) + youngAppSecondPaymentList?.Sum(x => x.Period3) ?? decimal.Zero,
                Period41618ApprenticeIncentive = youngAppFirstPaymentList?.Sum(x => x.Period4) + youngAppSecondPaymentList?.Sum(x => x.Period4) ?? decimal.Zero,
                Period51618ApprenticeIncentive = youngAppFirstPaymentList?.Sum(x => x.Period5) + youngAppSecondPaymentList?.Sum(x => x.Period5) ?? decimal.Zero,
                Period61618ApprenticeIncentive = youngAppFirstPaymentList?.Sum(x => x.Period6) + youngAppSecondPaymentList?.Sum(x => x.Period6) ?? decimal.Zero,
                Period71618ApprenticeIncentive = youngAppFirstPaymentList?.Sum(x => x.Period7) + youngAppSecondPaymentList?.Sum(x => x.Period7) ?? decimal.Zero,
                Period81618ApprenticeIncentive = youngAppFirstPaymentList?.Sum(x => x.Period8) + youngAppSecondPaymentList?.Sum(x => x.Period8) ?? decimal.Zero,
                Period91618ApprenticeIncentive = youngAppFirstPaymentList?.Sum(x => x.Period9) + youngAppSecondPaymentList?.Sum(x => x.Period9) ?? decimal.Zero,
                Period101618ApprenticeIncentive = youngAppFirstPaymentList?.Sum(x => x.Period10) + youngAppSecondPaymentList?.Sum(x => x.Period10) ?? decimal.Zero,
                Period111618ApprenticeIncentive = youngAppFirstPaymentList?.Sum(x => x.Period11) + youngAppSecondPaymentList?.Sum(x => x.Period11) ?? decimal.Zero,
                Period121618ApprenticeIncentive = youngAppFirstPaymentList?.Sum(x => x.Period12) + youngAppSecondPaymentList?.Sum(x => x.Period12) ?? decimal.Zero,

                Period1AchievementIncentive = achPaymentList?.Sum(x => x.Period1) ?? decimal.Zero,
                Period2AchievementIncentive = achPaymentList?.Sum(x => x.Period2) ?? decimal.Zero,
                Period3AchievementIncentive = achPaymentList?.Sum(x => x.Period3) ?? decimal.Zero,
                Period4AchievementIncentive = achPaymentList?.Sum(x => x.Period4) ?? decimal.Zero,
                Period5AchievementIncentive = achPaymentList?.Sum(x => x.Period5) ?? decimal.Zero,
                Period6AchievementIncentive = achPaymentList?.Sum(x => x.Period6) ?? decimal.Zero,
                Period7AchievementIncentive = achPaymentList?.Sum(x => x.Period7) ?? decimal.Zero,
                Period8AchievementIncentive = achPaymentList?.Sum(x => x.Period8) ?? decimal.Zero,
                Period9AchievementIncentive = achPaymentList?.Sum(x => x.Period9) ?? decimal.Zero,
                Period10AchievementIncentive = achPaymentList?.Sum(x => x.Period10) ?? decimal.Zero,
                Period11AchievementIncentive = achPaymentList?.Sum(x => x.Period11) ?? decimal.Zero,
                Period12AchievementIncentive = achPaymentList?.Sum(x => x.Period12) ?? decimal.Zero
            };

            trailblazerEmployerIncentiveModel.Period1Total = trailblazerEmployerIncentiveModel.Period1SmallEmployerIncentive + trailblazerEmployerIncentiveModel.Period11618ApprenticeIncentive + trailblazerEmployerIncentiveModel.Period1AchievementIncentive;
            trailblazerEmployerIncentiveModel.Period2Total = trailblazerEmployerIncentiveModel.Period2SmallEmployerIncentive + trailblazerEmployerIncentiveModel.Period21618ApprenticeIncentive + trailblazerEmployerIncentiveModel.Period2AchievementIncentive;
            trailblazerEmployerIncentiveModel.Period3Total = trailblazerEmployerIncentiveModel.Period3SmallEmployerIncentive + trailblazerEmployerIncentiveModel.Period31618ApprenticeIncentive + trailblazerEmployerIncentiveModel.Period3AchievementIncentive;
            trailblazerEmployerIncentiveModel.Period4Total = trailblazerEmployerIncentiveModel.Period4SmallEmployerIncentive + trailblazerEmployerIncentiveModel.Period41618ApprenticeIncentive + trailblazerEmployerIncentiveModel.Period4AchievementIncentive;
            trailblazerEmployerIncentiveModel.Period5Total = trailblazerEmployerIncentiveModel.Period5SmallEmployerIncentive + trailblazerEmployerIncentiveModel.Period51618ApprenticeIncentive + trailblazerEmployerIncentiveModel.Period5AchievementIncentive;
            trailblazerEmployerIncentiveModel.Period6Total = trailblazerEmployerIncentiveModel.Period6SmallEmployerIncentive + trailblazerEmployerIncentiveModel.Period61618ApprenticeIncentive + trailblazerEmployerIncentiveModel.Period6AchievementIncentive;
            trailblazerEmployerIncentiveModel.Period7Total = trailblazerEmployerIncentiveModel.Period7SmallEmployerIncentive + trailblazerEmployerIncentiveModel.Period71618ApprenticeIncentive + trailblazerEmployerIncentiveModel.Period7AchievementIncentive;
            trailblazerEmployerIncentiveModel.Period8Total = trailblazerEmployerIncentiveModel.Period8SmallEmployerIncentive + trailblazerEmployerIncentiveModel.Period81618ApprenticeIncentive + trailblazerEmployerIncentiveModel.Period8AchievementIncentive;
            trailblazerEmployerIncentiveModel.Period9Total = trailblazerEmployerIncentiveModel.Period9SmallEmployerIncentive + trailblazerEmployerIncentiveModel.Period91618ApprenticeIncentive + trailblazerEmployerIncentiveModel.Period9AchievementIncentive;
            trailblazerEmployerIncentiveModel.Period10Total = trailblazerEmployerIncentiveModel.Period10SmallEmployerIncentive + trailblazerEmployerIncentiveModel.Period101618ApprenticeIncentive + trailblazerEmployerIncentiveModel.Period10AchievementIncentive;
            trailblazerEmployerIncentiveModel.Period11Total = trailblazerEmployerIncentiveModel.Period10SmallEmployerIncentive + trailblazerEmployerIncentiveModel.Period111618ApprenticeIncentive + trailblazerEmployerIncentiveModel.Period11AchievementIncentive;
            trailblazerEmployerIncentiveModel.Period12Total = trailblazerEmployerIncentiveModel.Period12SmallEmployerIncentive + trailblazerEmployerIncentiveModel.Period121618ApprenticeIncentive + trailblazerEmployerIncentiveModel.Period12AchievementIncentive;

            trailblazerEmployerIncentiveModel.SmallEmployerIncentiveTotal =
                trailblazerEmployerIncentiveModel.Period1SmallEmployerIncentive +
                trailblazerEmployerIncentiveModel.Period2SmallEmployerIncentive +
                trailblazerEmployerIncentiveModel.Period3SmallEmployerIncentive +
                trailblazerEmployerIncentiveModel.Period4SmallEmployerIncentive +
                trailblazerEmployerIncentiveModel.Period5SmallEmployerIncentive +
                trailblazerEmployerIncentiveModel.Period6SmallEmployerIncentive +
                trailblazerEmployerIncentiveModel.Period7SmallEmployerIncentive +
                trailblazerEmployerIncentiveModel.Period8SmallEmployerIncentive +
                trailblazerEmployerIncentiveModel.Period9SmallEmployerIncentive +
                trailblazerEmployerIncentiveModel.Period10SmallEmployerIncentive +
                trailblazerEmployerIncentiveModel.Period11SmallEmployerIncentive +
                trailblazerEmployerIncentiveModel.Period12SmallEmployerIncentive;

            trailblazerEmployerIncentiveModel.Apprentice1618IncentiveTotal =
                trailblazerEmployerIncentiveModel.Period11618ApprenticeIncentive +
                trailblazerEmployerIncentiveModel.Period21618ApprenticeIncentive +
                trailblazerEmployerIncentiveModel.Period31618ApprenticeIncentive +
                trailblazerEmployerIncentiveModel.Period41618ApprenticeIncentive +
                trailblazerEmployerIncentiveModel.Period51618ApprenticeIncentive +
                trailblazerEmployerIncentiveModel.Period61618ApprenticeIncentive +
                trailblazerEmployerIncentiveModel.Period71618ApprenticeIncentive +
                trailblazerEmployerIncentiveModel.Period81618ApprenticeIncentive +
                trailblazerEmployerIncentiveModel.Period91618ApprenticeIncentive +
                trailblazerEmployerIncentiveModel.Period101618ApprenticeIncentive +
                trailblazerEmployerIncentiveModel.Period111618ApprenticeIncentive +
                trailblazerEmployerIncentiveModel.Period121618ApprenticeIncentive;

            trailblazerEmployerIncentiveModel.AchievementTotal =
                trailblazerEmployerIncentiveModel.Period1AchievementIncentive +
                trailblazerEmployerIncentiveModel.Period2AchievementIncentive +
                trailblazerEmployerIncentiveModel.Period3AchievementIncentive +
                trailblazerEmployerIncentiveModel.Period4AchievementIncentive +
                trailblazerEmployerIncentiveModel.Period5AchievementIncentive +
                trailblazerEmployerIncentiveModel.Period6AchievementIncentive +
                trailblazerEmployerIncentiveModel.Period7AchievementIncentive +
                trailblazerEmployerIncentiveModel.Period8AchievementIncentive +
                trailblazerEmployerIncentiveModel.Period9AchievementIncentive +
                trailblazerEmployerIncentiveModel.Period10AchievementIncentive +
                trailblazerEmployerIncentiveModel.Period11AchievementIncentive +
                trailblazerEmployerIncentiveModel.Period12AchievementIncentive;

            trailblazerEmployerIncentiveModel.GrandTotal =
                trailblazerEmployerIncentiveModel.Period1Total + trailblazerEmployerIncentiveModel.Period2Total +
                trailblazerEmployerIncentiveModel.Period3Total + trailblazerEmployerIncentiveModel.Period4Total +
                trailblazerEmployerIncentiveModel.Period5Total + trailblazerEmployerIncentiveModel.Period6Total +
                trailblazerEmployerIncentiveModel.Period7Total + trailblazerEmployerIncentiveModel.Period8Total +
                trailblazerEmployerIncentiveModel.Period9Total + trailblazerEmployerIncentiveModel.Period10Total +
                trailblazerEmployerIncentiveModel.Period11Total + trailblazerEmployerIncentiveModel.Period12Total;

            return trailblazerEmployerIncentiveModel;
        }

        private decimal? GetPeriodValueForEmployerIdentifier(KeyValuePair<string, int> empIdentifier, int period, LearningDeliveryPeriodisedValue ldPeriodisedValue)
        {
            if (empIdentifier.Key == Constants.Fm81SmallBusPayment)
            {
                return ldPeriodisedValue.Period1;
            }

            return 0;
        }
    }
}