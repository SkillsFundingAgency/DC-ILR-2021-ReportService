using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using CsvHelper.Configuration.Attributes;
using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReferenceDataService.Model.LARS;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Extensions;
using ESFA.DC.ILR.ReportService.Reports.Funding.DevolvedOccupancy.Model;
using ESFA.DC.ILR.ReportService.Service.Extensions;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.DevolvedOccupancy
{
    public class DevolvedAdultEducationOccupancyReportModelBuilder : IModelBuilder<IEnumerable<DevolvedAdultEducationOccupancyReportModel>>
    {
        private readonly IEnumerable<string> _sofLearnDelFamCodes = new HashSet<string>()
        {
            LearningDeliveryFAMCodeConstants.SOF_GreaterManchesterCombinedAuthority,
            LearningDeliveryFAMCodeConstants.SOF_LiverpoolCityRegionCombinedAuthority,
            LearningDeliveryFAMCodeConstants.SOF_WestMidlandsCombinedAuthority,
            LearningDeliveryFAMCodeConstants.SOF_WestOfEnglandCombinedAuthority,
            LearningDeliveryFAMCodeConstants.SOF_TeesValleyCombinedAuthority,
            LearningDeliveryFAMCodeConstants.SOF_CambridgeshireAndPeterboroughCombinedAuthority,
            LearningDeliveryFAMCodeConstants.SOF_GreaterLondonAuthority,
        };
        
        private const decimal _defaultDecimal = 0;

        public IEnumerable<DevolvedAdultEducationOccupancyReportModel> Build(IReportServiceContext reportServiceContext, IReportServiceDependentData reportServiceDependentData)
        {
            var message = reportServiceDependentData.Get<IMessage>();
            var fm35 = reportServiceDependentData.Get<FM35Global>();
            var referenceData = reportServiceDependentData.Get<ReferenceDataRoot>();

            var larsLearningDeliveries = BuildLarsLearningDeliveryDictionary(referenceData);
            var fm35LearningDeliveries = BuildFm35LearningDeliveryDictionary(fm35);

            var models = new List<DevolvedAdultEducationOccupancyReportModel>();

            foreach (var learner in message.Learners)
            {
                foreach (var learningDelivery in learner.LearningDeliveries.Where(LearningDeliveryReportFilter))
                {
                    var fm35LearningDelivery = fm35LearningDeliveries.GetValueOrDefault(learner.LearnRefNumber).GetValueOrDefault(learningDelivery.AimSeqNumber);
                    var larsLearningDelivery = larsLearningDeliveries.GetValueOrDefault(learningDelivery.LearnAimRef);
                    var providerSpecLearnerMonitoring = BuildProviderSpecLearnerMonitoring(learner.ProviderSpecLearnerMonitorings);
                    var providerSpecDeliveryMonitoring = BuildProviderSpecDeliveryMonitoring(learningDelivery.ProviderSpecDeliveryMonitorings);
                    var learningDeliveryFams = BuildLearningDeliveryFAMsModel(learningDelivery.LearningDeliveryFAMs);
                    var periodisedValues = BuildPeriodisedValuesModel(fm35LearningDelivery.LearningDeliveryPeriodisedValues);


                    models.Add(new DevolvedAdultEducationOccupancyReportModel()
                    {
                        Learner = learner,
                        ProviderSpecLearnerMonitoring = providerSpecLearnerMonitoring,
                        LearningDelivery = learningDelivery,
                        ProviderSpecDeliveryMonitoring = providerSpecDeliveryMonitoring,
                        LearningDeliveryFAMs = learningDeliveryFams,
                        Fm35LearningDelivery = fm35LearningDelivery?.LearningDeliveryValue,
                        LarsLearningDelivery = larsLearningDelivery,
                        PeriodisedValues = periodisedValues,

                        // devolved
                    });
                }
            }

            return Order(models);
        }

        public bool LearningDeliveryReportFilter(ILearningDelivery learningDelivery)
        {
            return learningDelivery
                .LearningDeliveryFAMs?
                .Any(
                    ldfam => 
                        ldfam.LearnDelFAMType.CaseInsensitiveEquals(LearningDeliveryFAMTypeConstants.SOF)
                           && _sofLearnDelFamCodes.Contains(ldfam.LearnDelFAMCode))
                ?? false;
        }

        public LearningDeliveryPeriodisedValuesModel BuildPeriodisedValuesModel(IEnumerable<LearningDeliveryPeriodisedValue> periodisedValues)
        {
            var periodisedValuesDictionary = BuildPeriodisedValuesDictionary(periodisedValues);

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

        public IDictionary<string, decimal[]> BuildPeriodisedValuesDictionary(IEnumerable<LearningDeliveryPeriodisedValue> periodisedValues)
        {
            return periodisedValues.ToDictionary(
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
                StringComparer.OrdinalIgnoreCase);
        }

        public IEnumerable<DevolvedAdultEducationOccupancyReportModel> Order(IEnumerable<DevolvedAdultEducationOccupancyReportModel> models)
        {
            return models.OrderBy(m => m.Learner.LearnRefNumber).ThenBy(m => m.LearningDelivery.AimSeqNumber);
        }

        public ProviderSpecLearnerMonitoringModel BuildProviderSpecLearnerMonitoring(IEnumerable<IProviderSpecLearnerMonitoring> monitorings)
        {
            return new ProviderSpecLearnerMonitoringModel()
            {
                A = monitorings.FirstOrDefault(m => m.ProvSpecLearnMonOccur.CaseInsensitiveEquals("A"))?.ProvSpecLearnMon,
                B = monitorings.FirstOrDefault(m => m.ProvSpecLearnMonOccur.CaseInsensitiveEquals("B"))?.ProvSpecLearnMon,
            };
        }

        public ProviderSpecDeliveryMonitoringModel BuildProviderSpecDeliveryMonitoring(IEnumerable<IProviderSpecDeliveryMonitoring> monitorings)
        {
            return new ProviderSpecDeliveryMonitoringModel()
            {
                A = monitorings.FirstOrDefault(m => m.ProvSpecDelMonOccur.CaseInsensitiveEquals("A"))?.ProvSpecDelMon,
                B = monitorings.FirstOrDefault(m => m.ProvSpecDelMonOccur.CaseInsensitiveEquals("B"))?.ProvSpecDelMon,
                C = monitorings.FirstOrDefault(m => m.ProvSpecDelMonOccur.CaseInsensitiveEquals("C"))?.ProvSpecDelMon,
                D = monitorings.FirstOrDefault(m => m.ProvSpecDelMonOccur.CaseInsensitiveEquals("D"))?.ProvSpecDelMon,
            };
        }

        public LearningDeliveryFAMsModel BuildLearningDeliveryFAMsModel(IEnumerable<ILearningDeliveryFAM> learningDeliveryFams)
        {
            var famDictionary = learningDeliveryFams.GroupBy(fam => fam.LearnDelFAMType).ToDictionary(g => g.Key, g => g.ToArray(), StringComparer.OrdinalIgnoreCase);

            var ldmsArray = famDictionary.GetValueOrDefault(LearningDeliveryFAMTypeConstants.LDM).ToFixedLengthArray(6);
            var damsArray = famDictionary.GetValueOrDefault(LearningDeliveryFAMTypeConstants.DAM).ToFixedLengthArray(4);

            var lsf = famDictionary.GetValueOrDefault(LearningDeliveryFAMTypeConstants.LSF);

            return new LearningDeliveryFAMsModel()
            {
                SOF = GetLearningDeliveryFAMCode(LearningDeliveryFAMTypeConstants.SOF, famDictionary),
                FFI = GetLearningDeliveryFAMCode(LearningDeliveryFAMTypeConstants.FFI, famDictionary),
                LSF_Highest = lsf?.MaxOrDefault(f => f.LearnDelFAMCode),
                LSF_EarliestDateFrom = lsf?.MinOrDefault(f => f.LearnDelFAMDateFromNullable),
                LSF_LatestDateTo = lsf?.MaxOrDefault(f => f.LearnDelFAMDateToNullable),
                LDM1 = ldmsArray[0]?.LearnDelFAMCode,
                LDM2 = ldmsArray[1]?.LearnDelFAMCode,
                LDM3 = ldmsArray[2]?.LearnDelFAMCode,
                LDM4 = ldmsArray[3]?.LearnDelFAMCode,
                LDM5 = ldmsArray[4]?.LearnDelFAMCode,
                LDM6 = ldmsArray[5]?.LearnDelFAMCode,
                DAM1 = damsArray[0]?.LearnDelFAMCode,
                DAM2 = damsArray[1]?.LearnDelFAMCode,
                DAM3 = damsArray[2]?.LearnDelFAMCode,
                DAM4 = damsArray[3]?.LearnDelFAMCode,
                RES = GetLearningDeliveryFAMCode(LearningDeliveryFAMTypeConstants.RES, famDictionary)
            };
        }

        private string GetLearningDeliveryFAMCode(string famType, IDictionary<string, ILearningDeliveryFAM[]> learningDeliveryFamDictionary)
        {
            return learningDeliveryFamDictionary.GetValueOrDefault(famType)?.FirstOrDefault()?.LearnDelFAMCode;
        }

        private IDictionary<string, LARSLearningDelivery> BuildLarsLearningDeliveryDictionary(ReferenceDataRoot referenceDataRoot)
        {
            return referenceDataRoot.LARSLearningDeliveries.ToDictionary(ld => ld.LearnAimRef, ld => ld, StringComparer.OrdinalIgnoreCase);
        }


        private IDictionary<string, Dictionary<int, LearningDelivery>> BuildFm35LearningDeliveryDictionary(FM35Global fm35Global)
        {
            return fm35Global
                .Learners
                .ToDictionary(
                    l => l.LearnRefNumber,
                    l => l.LearningDeliveries
                        .Where(ld => ld.AimSeqNumber.HasValue)
                        .ToDictionary(
                            ld => ld.AimSeqNumber.Value,
                            ld => ld),
                    StringComparer.OrdinalIgnoreCase);
        }
    }
}
