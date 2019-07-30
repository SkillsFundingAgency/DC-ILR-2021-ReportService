using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output;
using ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Devolved.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESFA.DC.ILR.FundingService.FM25.Model.Output;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReferenceDataService.Model.LARS;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Extensions;
using ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Abstract.Model;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Abstract
{
    public abstract class AbstractOccupancyReportModelBuilder
    {
        private const decimal _defaultDecimal = 0;

        public LearningDeliveryPeriodisedValuesModel BuildPeriodisedValuesModel(IDictionary<string, decimal[]> periodisedValuesDictionary)
        {
            var onProgPaymentTotal = periodisedValuesDictionary.GetValueOrDefault(AttributeConstants.Fm35OnProgPayment)?.Sum() ?? _defaultDecimal;
            var balancePaymentTotal = periodisedValuesDictionary.GetValueOrDefault(AttributeConstants.Fm35BalancePayment)?.Sum() ?? _defaultDecimal;
            var achievePaymentTotal = periodisedValuesDictionary.GetValueOrDefault(AttributeConstants.Fm35AchievePayment)?.Sum() ?? _defaultDecimal;
            var empOutcomePayTotal = periodisedValuesDictionary.GetValueOrDefault(AttributeConstants.Fm35EmpOutcomePay)?.Sum() ?? _defaultDecimal;
            var learnSuppFundCashTotal = periodisedValuesDictionary.GetValueOrDefault(AttributeConstants.Fm35LearnSuppFundCash)?.Sum() ?? _defaultDecimal;

            var totalEarned = onProgPaymentTotal + balancePaymentTotal + achievePaymentTotal + empOutcomePayTotal + learnSuppFundCashTotal;

            return new LearningDeliveryPeriodisedValuesModel()
            {
                AchievePayPctMax = periodisedValuesDictionary.GetValueOrDefault(AttributeConstants.Fm35AchievePayPct)?.MaxOrDefault() ?? _defaultDecimal,
                August = BuildPeriodisedValuesAttributes(periodisedValuesDictionary, 0),
                September = BuildPeriodisedValuesAttributes(periodisedValuesDictionary, 1),
                October = BuildPeriodisedValuesAttributes(periodisedValuesDictionary, 2),
                November = BuildPeriodisedValuesAttributes(periodisedValuesDictionary, 3),
                December = BuildPeriodisedValuesAttributes(periodisedValuesDictionary, 4),
                January = BuildPeriodisedValuesAttributes(periodisedValuesDictionary, 5),
                February = BuildPeriodisedValuesAttributes(periodisedValuesDictionary, 6),
                March = BuildPeriodisedValuesAttributes(periodisedValuesDictionary, 7),
                April = BuildPeriodisedValuesAttributes(periodisedValuesDictionary, 8),
                May = BuildPeriodisedValuesAttributes(periodisedValuesDictionary, 9),
                June = BuildPeriodisedValuesAttributes(periodisedValuesDictionary, 10),
                July = BuildPeriodisedValuesAttributes(periodisedValuesDictionary, 11),
                OnProgPaymentTotal = onProgPaymentTotal,
                BalancePaymentTotal = balancePaymentTotal,
                AchievePaymentTotal = achievePaymentTotal,
                EmpOutcomePayTotal = empOutcomePayTotal,
                LearnSuppFundCashTotal = learnSuppFundCashTotal,
                TotalEarned = totalEarned,
            };
        }

        public LearningDeliveryPeriodisedValuesAttributesModel BuildPeriodisedValuesAttributes(IDictionary<string, decimal[]> periodisedValues, int period)
        {
            return new LearningDeliveryPeriodisedValuesAttributesModel()
            {
                OnProgPayment = periodisedValues.GetValueOrDefault(AttributeConstants.Fm35OnProgPayment)?[period] ?? _defaultDecimal,
                BalancePayment = periodisedValues.GetValueOrDefault(AttributeConstants.Fm35BalancePayment)?[period] ?? _defaultDecimal,
                AchievePayment = periodisedValues.GetValueOrDefault(AttributeConstants.Fm35AchievePayment)?[period] ?? _defaultDecimal,
                EmpOutcomePay = periodisedValues.GetValueOrDefault(AttributeConstants.Fm35EmpOutcomePay)?[period] ?? _defaultDecimal,
                LearnSuppFundCash = periodisedValues.GetValueOrDefault(AttributeConstants.Fm35LearnSuppFundCash)?[period] ?? _defaultDecimal,
            };
        }

        public IDictionary<string, decimal[]> BuildFm35PeriodisedValuesDictionary(IEnumerable<LearningDeliveryPeriodisedValue> periodisedValues)
        {
            return periodisedValues?
                .ToDictionary(
                    pv => pv.AttributeName,
                    pv => new decimal[]
                    {
                        pv.Period1 ?? 0,
                        pv.Period2 ?? 0,
                        pv.Period3 ?? 0,
                        pv.Period4 ?? 0,
                        pv.Period5 ?? 0,
                        pv.Period6 ?? 0,
                        pv.Period7 ?? 0,
                        pv.Period8 ?? 0,
                        pv.Period9 ?? 0,
                        pv.Period10 ?? 0,
                        pv.Period11 ?? 0,
                        pv.Period12 ?? 0,
                    },
                    StringComparer.OrdinalIgnoreCase)
                ?? new Dictionary<string, decimal[]>();
        }

        public IDictionary<string, decimal[]> BuildFm25PeriodisedValuesDictionary(IEnumerable<LearnerPeriodisedValues> periodisedValues)
        {
            return periodisedValues?
                       .ToDictionary(
                           pv => pv.AttributeName,
                           pv => new decimal[]
                           {
                               pv.Period1 ?? 0,
                               pv.Period2 ?? 0,
                               pv.Period3 ?? 0,
                               pv.Period4 ?? 0,
                               pv.Period5 ?? 0,
                               pv.Period6 ?? 0,
                               pv.Period7 ?? 0,
                               pv.Period8 ?? 0,
                               pv.Period9 ?? 0,
                               pv.Period10 ?? 0,
                               pv.Period11 ?? 0,
                               pv.Period12 ?? 0,
                           },
                           StringComparer.OrdinalIgnoreCase)
                   ?? new Dictionary<string, decimal[]>();
        }

        public IEnumerable<DevolvedAdultEducationOccupancyReportModel> Order(IEnumerable<DevolvedAdultEducationOccupancyReportModel> models)
        {
            return models.OrderBy(m => m.Learner.LearnRefNumber).ThenBy(m => m.LearningDelivery.AimSeqNumber);
        }

        protected IDictionary<string, LARSLearningDelivery> BuildLarsLearningDeliveryDictionary(ReferenceDataRoot referenceDataRoot)
        {
            return referenceDataRoot?.LARSLearningDeliveries?.ToDictionary(ld => ld.LearnAimRef, ld => ld, StringComparer.OrdinalIgnoreCase) ?? new Dictionary<string, LARSLearningDelivery>();
        }

        protected IDictionary<string, Dictionary<int, LearningDelivery>> BuildFm35LearningDeliveryDictionary(FM35Global fm35Global)
        {
            return fm35Global?
                .Learners?
                .ToDictionary(
                    l => l.LearnRefNumber,
                    l => l.LearningDeliveries
                        .Where(ld => ld.AimSeqNumber.HasValue)
                        .ToDictionary(
                            ld => ld.AimSeqNumber.Value,
                            ld => ld),
                    StringComparer.OrdinalIgnoreCase)
                ?? new Dictionary<string, Dictionary<int, LearningDelivery>>();
        }
    }
}
