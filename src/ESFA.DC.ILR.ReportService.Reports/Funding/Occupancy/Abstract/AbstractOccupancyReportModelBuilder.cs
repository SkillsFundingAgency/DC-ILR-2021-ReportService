using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ReportService.Models.Fm25;
using ESFA.DC.ILR.ReportService.Models.Fm35;
using ESFA.DC.ILR.ReportService.Models.ReferenceData;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.LARS;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Extensions;
using ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Abstract.Model;
using ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Interface;
using ESFA.DC.ILR.ReportService.Reports.Model.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Abstract
{
    public abstract class AbstractOccupancyReportModelBuilder
    {
        protected IIlrModelMapper _ilrModelMapper;

        private const decimal _defaultDecimal = 0;

        protected AbstractOccupancyReportModelBuilder(IIlrModelMapper ilrModelMapper)
        {
            _ilrModelMapper = ilrModelMapper;
        }

        public LearningDeliveryPeriodisedValuesModel BuildFm35PeriodisedValuesModel(IEnumerable<LearningDeliveryPeriodisedValue> periodisedValues)
        {
            var periodisedValuesDictionary = BuildFm35PeriodisedValuesDictionary(periodisedValues);

            var onProgPaymentTotal = periodisedValuesDictionary.GetValueOrDefault(AttributeConstants.Fm35OnProgPayment)?.Sum() ?? _defaultDecimal;
            var balancePaymentTotal = periodisedValuesDictionary.GetValueOrDefault(AttributeConstants.Fm35BalancePayment)?.Sum() ?? _defaultDecimal;
            var achievePaymentTotal = periodisedValuesDictionary.GetValueOrDefault(AttributeConstants.Fm35AchievePayment)?.Sum() ?? _defaultDecimal;
            var empOutcomePayTotal = periodisedValuesDictionary.GetValueOrDefault(AttributeConstants.Fm35EmpOutcomePay)?.Sum() ?? _defaultDecimal;
            var learnSuppFundCashTotal = periodisedValuesDictionary.GetValueOrDefault(AttributeConstants.Fm35LearnSuppFundCash)?.Sum() ?? _defaultDecimal;

            var totalEarned = onProgPaymentTotal + balancePaymentTotal + achievePaymentTotal + empOutcomePayTotal + learnSuppFundCashTotal;

            return new LearningDeliveryPeriodisedValuesModel()
            {
                AchievePayPctMax = periodisedValuesDictionary.GetValueOrDefault(AttributeConstants.Fm35AchievePayPct)?.MaxOrDefault() ?? _defaultDecimal,
                August = BuildFm35PeriodisedValuesAttributes(periodisedValuesDictionary, 0),
                September = BuildFm35PeriodisedValuesAttributes(periodisedValuesDictionary, 1),
                October = BuildFm35PeriodisedValuesAttributes(periodisedValuesDictionary, 2),
                November = BuildFm35PeriodisedValuesAttributes(periodisedValuesDictionary, 3),
                December = BuildFm35PeriodisedValuesAttributes(periodisedValuesDictionary, 4),
                January = BuildFm35PeriodisedValuesAttributes(periodisedValuesDictionary, 5),
                February = BuildFm35PeriodisedValuesAttributes(periodisedValuesDictionary, 6),
                March = BuildFm35PeriodisedValuesAttributes(periodisedValuesDictionary, 7),
                April = BuildFm35PeriodisedValuesAttributes(periodisedValuesDictionary, 8),
                May = BuildFm35PeriodisedValuesAttributes(periodisedValuesDictionary, 9),
                June = BuildFm35PeriodisedValuesAttributes(periodisedValuesDictionary, 10),
                July = BuildFm35PeriodisedValuesAttributes(periodisedValuesDictionary, 11),
                OnProgPaymentTotal = onProgPaymentTotal,
                BalancePaymentTotal = balancePaymentTotal,
                AchievePaymentTotal = achievePaymentTotal,
                EmpOutcomePayTotal = empOutcomePayTotal,
                LearnSuppFundCashTotal = learnSuppFundCashTotal,
                TotalEarned = totalEarned,
            };
        }

        public LearningDeliveryPeriodisedValuesAttributesModel BuildFm35PeriodisedValuesAttributes(IDictionary<string, decimal[]> periodisedValues, int period)
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

        public LearningDeliveryPeriodisedValuesModel BuildFm25PeriodisedValuesModel(IEnumerable<LearnerPeriodisedValues> periodisedValues)
        {
            var periodisedValuesDictionary = BuildFm25PeriodisedValuesDictionary(periodisedValues);

            var onProgPaymentTotal = periodisedValuesDictionary.GetValueOrDefault(AttributeConstants.Fm25LrnOnProgPay)?.Sum() ?? _defaultDecimal;

            var totalEarned = onProgPaymentTotal;

            return new LearningDeliveryPeriodisedValuesModel()
            {
                August = BuildFm25PeriodisedValuesAttributes(periodisedValuesDictionary, 0),
                September = BuildFm25PeriodisedValuesAttributes(periodisedValuesDictionary, 1),
                October = BuildFm25PeriodisedValuesAttributes(periodisedValuesDictionary, 2),
                November = BuildFm25PeriodisedValuesAttributes(periodisedValuesDictionary, 3),
                December = BuildFm25PeriodisedValuesAttributes(periodisedValuesDictionary, 4),
                January = BuildFm25PeriodisedValuesAttributes(periodisedValuesDictionary, 5),
                February = BuildFm25PeriodisedValuesAttributes(periodisedValuesDictionary, 6),
                March = BuildFm25PeriodisedValuesAttributes(periodisedValuesDictionary, 7),
                April = BuildFm25PeriodisedValuesAttributes(periodisedValuesDictionary, 8),
                May = BuildFm25PeriodisedValuesAttributes(periodisedValuesDictionary, 9),
                June = BuildFm25PeriodisedValuesAttributes(periodisedValuesDictionary, 10),
                July = BuildFm25PeriodisedValuesAttributes(periodisedValuesDictionary, 11),
                OnProgPaymentTotal = onProgPaymentTotal,
                TotalEarned = totalEarned,
            };
        }

        public LearningDeliveryPeriodisedValuesAttributesModel BuildFm25PeriodisedValuesAttributes(IDictionary<string, decimal[]> periodisedValues, int period)
        {
            return new LearningDeliveryPeriodisedValuesAttributesModel()
            {
                OnProgPayment = periodisedValues.GetValueOrDefault(AttributeConstants.Fm25LrnOnProgPay)?[period] ?? _defaultDecimal,
            };
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

        public IEnumerable<T> Order<T>(IEnumerable<T> models)
            where T : IOrderableOccupancyReportModel
        {
            return models.OrderBy(m => m.LearnRefNumber).ThenBy(m => m.AimSeqNumber);
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

        protected IDictionary<string, FM25Learner> BuildFm25LearnerDictionary(FM25Global fm25Global)
        {
            return fm25Global?
                       .Learners?
                       .ToDictionary(
                           l => l.LearnRefNumber,
                           l => l,
                           StringComparer.OrdinalIgnoreCase)
                   ?? new Dictionary<string, FM25Learner>();
        }

        public LearningDeliveryPeriodisedValuesModel BuildFm99PeriodisedValuesModel(IEnumerable<ESFA.DC.ILR.ReportService.Models.Fm99.LearningDeliveryPeriodisedValue> periodisedValues)
        {
            var periodisedValuesDictionary = BuildFm99PeriodisedValuesDictionary(periodisedValues);

            var supportPaymentCashTotal = periodisedValuesDictionary.GetValueOrDefault(AttributeConstants.Fm99AlbSupportPayment)?.Sum() ?? _defaultDecimal;
            var onProgPaymentTotal = periodisedValuesDictionary.GetValueOrDefault(AttributeConstants.Fm99AreaUpliftOnProgPayment)?.Sum() ?? _defaultDecimal;
            var balancingPaymentTotal = periodisedValuesDictionary.GetValueOrDefault(AttributeConstants.Fm99AreaUpliftBalPayment)?.Sum() ?? _defaultDecimal;

            var totalEarned = onProgPaymentTotal + supportPaymentCashTotal + balancingPaymentTotal;

            return new LearningDeliveryPeriodisedValuesModel()
            {
                August = BuildFm99PeriodisedValuesAttributes(periodisedValuesDictionary, 0),
                September = BuildFm99PeriodisedValuesAttributes(periodisedValuesDictionary, 1),
                October = BuildFm99PeriodisedValuesAttributes(periodisedValuesDictionary, 2),
                November = BuildFm99PeriodisedValuesAttributes(periodisedValuesDictionary, 3),
                December = BuildFm99PeriodisedValuesAttributes(periodisedValuesDictionary, 4),
                January = BuildFm99PeriodisedValuesAttributes(periodisedValuesDictionary, 5),
                February = BuildFm99PeriodisedValuesAttributes(periodisedValuesDictionary, 6),
                March = BuildFm99PeriodisedValuesAttributes(periodisedValuesDictionary, 7),
                April = BuildFm99PeriodisedValuesAttributes(periodisedValuesDictionary, 8),
                May = BuildFm99PeriodisedValuesAttributes(periodisedValuesDictionary, 9),
                June = BuildFm99PeriodisedValuesAttributes(periodisedValuesDictionary, 10),
                July = BuildFm99PeriodisedValuesAttributes(periodisedValuesDictionary, 11),
                LearnSuppFundCashTotal = supportPaymentCashTotal,
                OnProgPaymentTotal = onProgPaymentTotal,
                BalancePaymentTotal = balancingPaymentTotal,
                TotalEarned = totalEarned,
            };
        }

        public LearningDeliveryPeriodisedValuesAttributesModel BuildFm99PeriodisedValuesAttributes(IDictionary<string, decimal[]> periodisedValues, int period)
        {
            var learnSuppFundCash = periodisedValues.GetValueOrDefault(AttributeConstants.Fm99AlbSupportPayment)?[period] ?? _defaultDecimal;
            var balancePayment = periodisedValues.GetValueOrDefault(AttributeConstants.Fm99AreaUpliftBalPayment)?[period] ??_defaultDecimal;
            var onProgPayment = periodisedValues.GetValueOrDefault(AttributeConstants.Fm99AreaUpliftOnProgPayment)?[period] ?? _defaultDecimal;

            var totalEarned = learnSuppFundCash + balancePayment + onProgPayment;

            return new LearningDeliveryPeriodisedValuesAttributesModel()
            {
                Code = periodisedValues.GetValueOrDefault(AttributeConstants.Fm99ALBCode)?[period],
                LearnSuppFundCash = learnSuppFundCash,
                BalancePayment = balancePayment,
                OnProgPayment = onProgPayment,
                TotalEarned = totalEarned,
            };
        }

        public IDictionary<string, decimal[]> BuildFm99PeriodisedValuesDictionary(IEnumerable<ESFA.DC.ILR.ReportService.Models.Fm99.LearningDeliveryPeriodisedValue> periodisedValues)
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

        public LearningDeliveryPeriodisedValuesModel BuildFm81PeriodisedValuesModel(IEnumerable<ESFA.DC.ILR.ReportService.Models.Fm81.LearningDeliveryPeriodisedValue> periodisedValues)
        {
            var periodisedValuesDictionary = BuildFm81PeriodisedValuesDictionary(periodisedValues);

            var coreGovContPayment = periodisedValuesDictionary.GetValueOrDefault(AttributeConstants.Fm81CoreGovContPayment)?.Sum() ?? _defaultDecimal;
            var mathEngOnProgPayment = periodisedValuesDictionary.GetValueOrDefault(AttributeConstants.Fm81MathEngOnProgPayment)?.Sum() ?? _defaultDecimal;
            var mathEngBalPayment = periodisedValuesDictionary.GetValueOrDefault(AttributeConstants.Fm81MathEngBalPayment)?.Sum() ?? _defaultDecimal;
            var learnSuppFundCash = periodisedValuesDictionary.GetValueOrDefault(AttributeConstants.Fm81LearnSuppFundCash)?.Sum() ?? _defaultDecimal;
            var smallBusPayment = periodisedValuesDictionary.GetValueOrDefault(AttributeConstants.Fm81SmallBusPayment)?.Sum() ?? _defaultDecimal;
            var youngAppPayment = periodisedValuesDictionary.GetValueOrDefault(AttributeConstants.Fm81YoungAppPayment)?.Sum() ?? _defaultDecimal;
            var achPayment = periodisedValuesDictionary.GetValueOrDefault(AttributeConstants.Fm81AchPayment)?.Sum() ?? _defaultDecimal;

            var totalEarned = coreGovContPayment + mathEngOnProgPayment + mathEngBalPayment + learnSuppFundCash + smallBusPayment + youngAppPayment + achPayment;

            return new LearningDeliveryPeriodisedValuesModel()
            {
                August = BuildFm81PeriodisedValuesAttributes(periodisedValuesDictionary, 0),
                September = BuildFm81PeriodisedValuesAttributes(periodisedValuesDictionary, 1),
                October = BuildFm81PeriodisedValuesAttributes(periodisedValuesDictionary, 2),
                November = BuildFm81PeriodisedValuesAttributes(periodisedValuesDictionary, 3),
                December = BuildFm81PeriodisedValuesAttributes(periodisedValuesDictionary, 4),
                January = BuildFm81PeriodisedValuesAttributes(periodisedValuesDictionary, 5),
                February = BuildFm81PeriodisedValuesAttributes(periodisedValuesDictionary, 6),
                March = BuildFm81PeriodisedValuesAttributes(periodisedValuesDictionary, 7),
                April = BuildFm81PeriodisedValuesAttributes(periodisedValuesDictionary, 8),
                May = BuildFm81PeriodisedValuesAttributes(periodisedValuesDictionary, 9),
                June = BuildFm81PeriodisedValuesAttributes(periodisedValuesDictionary, 10),
                July = BuildFm81PeriodisedValuesAttributes(periodisedValuesDictionary, 11),
                CoreGovContPaymentTotal = coreGovContPayment,
                MathEngOnProgPaymentTotal = mathEngOnProgPayment,
                MathEngBalPaymentTotal = mathEngBalPayment,
                LearnSuppFundCashTotal = learnSuppFundCash,
                SmallBusPaymentTotal = smallBusPayment,
                YoungAppPaymentTotal = youngAppPayment,
                AchievePaymentTotal = achPayment,
                TotalEarned = totalEarned,
            };
        }

        public LearningDeliveryPeriodisedValuesAttributesModel BuildFm81PeriodisedValuesAttributes(IDictionary<string, decimal[]> periodisedValues, int period)
        {
            var coreGovContPayment = periodisedValues.GetValueOrDefault(AttributeConstants.Fm81CoreGovContPayment)?[period] ?? _defaultDecimal;
            var mathEngOnProgPayment = periodisedValues.GetValueOrDefault(AttributeConstants.Fm81MathEngOnProgPayment)?[period] ?? _defaultDecimal;
            var mathEngBalPayment = periodisedValues.GetValueOrDefault(AttributeConstants.Fm81MathEngBalPayment)?[period] ?? _defaultDecimal;
            var learnSuppFundCash = periodisedValues.GetValueOrDefault(AttributeConstants.Fm81LearnSuppFundCash)?[period] ?? _defaultDecimal;
            var smallBusPayment = periodisedValues.GetValueOrDefault(AttributeConstants.Fm81SmallBusPayment)?[period] ?? _defaultDecimal;
            var youngAppPayment = periodisedValues.GetValueOrDefault(AttributeConstants.Fm81YoungAppPayment)?[period] ?? _defaultDecimal;
            var achPayment = periodisedValues.GetValueOrDefault(AttributeConstants.Fm81AchPayment)?[period] ?? _defaultDecimal;

            var totalEarned = coreGovContPayment + mathEngOnProgPayment + mathEngBalPayment + learnSuppFundCash + smallBusPayment + youngAppPayment + achPayment;

            return new LearningDeliveryPeriodisedValuesAttributesModel()
            {
                CoreGovContPayment = coreGovContPayment,
                MathEngOnProgPayment = mathEngOnProgPayment,
                MathEngBalPayment = mathEngBalPayment,
                LearnSuppFundCash = learnSuppFundCash,
                SmallBusPayment = smallBusPayment,
                YoungAppPayment = youngAppPayment,
                AchievePayment = achPayment,
                TotalEarned = totalEarned,
            };
        }

        public IDictionary<string, decimal[]> BuildFm81PeriodisedValuesDictionary(IEnumerable<Models.Fm81.LearningDeliveryPeriodisedValue> periodisedValues)
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
    }
}
