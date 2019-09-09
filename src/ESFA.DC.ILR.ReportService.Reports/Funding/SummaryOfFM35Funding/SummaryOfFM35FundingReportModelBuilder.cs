using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Model.ILR;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.SummaryOfFM35Funding
{
    public class SummaryOfFM35FundingReportModelBuilder : IModelBuilder<IEnumerable<SummaryOfFM35FundingReportModel>>
    {
        private readonly HashSet<string> _attributes = new HashSet<string>
        {
            AttributeConstants.Fm35OnProgPayment,
            AttributeConstants.Fm35BalancePayment,
            AttributeConstants.Fm35EmpOutcomePay,
            AttributeConstants.Fm35AchievePayment,
            AttributeConstants.Fm35LearnSuppFundCash
        };

        private readonly HashSet<string> _fundLines = new HashSet<string>
        {
            FundLineConstants.Apprenticeship1618,
            FundLineConstants.Apprenticeship1923,
            FundLineConstants.Apprenticeship24Plus,
            FundLineConstants.Traineeship1924NonProcured,
            FundLineConstants.Traineeship1924ProcuredFromNov2017,
            FundLineConstants.AebOtherLearningNonProcured,
            FundLineConstants.AebOtherLearningProcuredFromNov2017,
        };

        public SummaryOfFM35FundingReportModelBuilder()
        {
        }

        public IEnumerable<SummaryOfFM35FundingReportModel> Build(IReportServiceContext reportServiceContext, IReportServiceDependentData reportServiceDependentData)
        {
            var ukprn = reportServiceContext.Ukprn;
            var message = reportServiceDependentData.Get<IMessage>();
            var fm35 = reportServiceDependentData.Get<FM35Global>();

            var fm35LearningDeliveries = GetFM35LearningDeliveryPeriodisedValues(fm35, reportServiceContext.Ukprn);
            var fm35LearningDeliveryDictionary = BuildFm35LearningDeliveryDictionary(fm35LearningDeliveries);
            var fundlines = fm35LearningDeliveries.Select(f => f.FundLine).Distinct().ToList();

            var models = new List<SummaryOfFM35FundingReportModel>();

            foreach (var fundline in _fundLines)
            {
                int i = 1;
                while (i > 0 && i < 13)
                {
                   models.Add(
                   new SummaryOfFM35FundingReportModel
                   {
                       UKPRN = ukprn,
                       FundingLineType = fundline,
                       Period = i,
                       OnProgramme = fm35LearningDeliveryDictionary[fundline][AttributeConstants.Fm35OnProgPayment].Select(p => GetPeriodValue(p, i)).FirstOrDefault(),
                       Balancing= fm35LearningDeliveryDictionary[fundline][AttributeConstants.Fm35BalancePayment].Select(p => GetPeriodValue(p, i)).FirstOrDefault(),
                       JobOutcomeAchievement = fm35LearningDeliveryDictionary[fundline][AttributeConstants.Fm35EmpOutcomePay].Select(p => GetPeriodValue(p, i)).FirstOrDefault(),
                       AimAchievement = fm35LearningDeliveryDictionary[fundline][AttributeConstants.Fm35AchievePayment].Select(p => GetPeriodValue(p, i)).FirstOrDefault(),
                       LearningSupport = fm35LearningDeliveryDictionary[fundline][AttributeConstants.Fm35LearnSuppFundCash].Select(p => GetPeriodValue(p, i)).FirstOrDefault(),
                   });
                }

                i++;
            }
          
            return models;
        }

        private decimal? SumTotalAchievement(decimal?[] periodisedValues, int period)
        {
            return periodisedValues[period - 1] != null ? periodisedValues[period - 1] : 0m;
        }

        private decimal? GetPeriodValue(decimal?[] periodisedValues, int period)
        {
            return periodisedValues[period - 1] != null ? periodisedValues[period - 1] : 0m;
        }

        private Dictionary<string, Dictionary<string, decimal?[][]>> BuildFm35LearningDeliveryDictionary(List<FM35LearningDeliveryValues> fm35LearningDeliveryValues)
        {
            return fm35LearningDeliveryValues?
               .Where(a => _attributes.Contains(a.AttributeName, StringComparer.OrdinalIgnoreCase))
               .GroupBy(f => f.FundLine)
               .ToDictionary(
               k1 => k1.Key,
               v1 => v1.Select(g => g)
               .GroupBy(a => a.AttributeName)
               .ToDictionary(
                   k2 => k2.Key,
                   v2 => v2.Select(pvGroup => new decimal?[]
                   {
                         pvGroup.Period1,
                         pvGroup.Period2,
                         pvGroup.Period3,
                         pvGroup.Period4,
                         pvGroup.Period5,
                         pvGroup.Period6,
                         pvGroup.Period7,
                         pvGroup.Period8,
                         pvGroup.Period9,
                         pvGroup.Period10,
                         pvGroup.Period11,
                         pvGroup.Period12,
                   }).ToArray()))
                   ?? new Dictionary<string, Dictionary<string, decimal?[][]>>();
        }

        private static List<FM35LearningDeliveryValues> GetFM35LearningDeliveryPeriodisedValues(FM35Global fm35Global, int ukPrn)
        {
            var result = new List<FM35LearningDeliveryValues>();
            if (fm35Global?.Learners == null)
            {
                return result;
            }

            foreach (var learner in fm35Global.Learners)
            {
                if (learner.LearningDeliveries == null)
                {
                    continue;
                }

                foreach (var ld in learner.LearningDeliveries)
                {
                    if (ld.LearningDeliveryPeriodisedValues == null)
                    {
                        continue;
                    }

                    foreach (var ldpv in ld.LearningDeliveryPeriodisedValues)
                    {
                        result.Add(new FM35LearningDeliveryValues
                        {
                            AimSeqNumber = ld.AimSeqNumber ?? 0,
                            LearnRefNumber = learner.LearnRefNumber,
                            AttributeName = ldpv.AttributeName,
                            Period1 = ldpv.Period1,
                            Period2 = ldpv.Period2,
                            Period3 = ldpv.Period3,
                            Period4 = ldpv.Period4,
                            Period5 = ldpv.Period5,
                            Period6 = ldpv.Period6,
                            Period7 = ldpv.Period7,
                            Period8 = ldpv.Period8,
                            Period9 = ldpv.Period9,
                            Period10 = ldpv.Period10,
                            Period11 = ldpv.Period11,
                            Period12 = ldpv.Period12,
                            UKPRN = ukPrn,
                            FundLine = ld.LearningDeliveryValue?.FundLine
                        });
                    }
                }
            }

            return result;
        }
    }
}
