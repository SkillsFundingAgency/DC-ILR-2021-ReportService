using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Extensions;
using ESFA.DC.ILR.ReportService.Reports.Funding.Apprenticeship.AppsIndicitave.Model;
using ESFA.DC.ILR.ReportService.Reports.Model.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ReportService.Models.Fm36;
using ESFA.DC.ILR.ReportService.Models.ReferenceData;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.LARS;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Apprenticeship.AppsIndicitave
{
    public class AppsIndicativeEarningsReportModelBuilder : IModelBuilder<IEnumerable<AppsIndicativeEarningsReportModel>>
    {
        private readonly IIlrModelMapper _ilrModelMapper;
        private const decimal _defaultDecimal = 0;

        public AppsIndicativeEarningsReportModelBuilder(IIlrModelMapper ilrModelMapper)
        {
            _ilrModelMapper = ilrModelMapper;
        }

        public IEnumerable<AppsIndicativeEarningsReportModel> Build(IReportServiceContext reportServiceContext, IReportServiceDependentData reportServiceDependentData)
        {

            var message = reportServiceDependentData.Get<IMessage>();
            var fm36Data = reportServiceDependentData.Get<FM36Global>();
            var appsIndicativeEarningsModels = new List<AppsIndicativeEarningsReportModel>();
            var referenceData = reportServiceDependentData.Get<ReferenceDataRoot>();
            IDictionary<string, LARSLearningDelivery> larsLearningDeliveries = BuildLarsLearningDeliveryDictionary(referenceData);
            IDictionary<int, LARSStandard> larsStandards = BuildLarsStandardDictionary(referenceData);
            IDictionary<string, FM36Learner> fm36Learners = BuildFm36LearnerDeliveryDictionary(fm36Data);

            foreach (var learner in message?.Learners?.Where(l => l != null) ?? Enumerable.Empty<ILearner>())
            {
                FM36Learner fm36Learner = fm36Learners.GetValueOrDefault(learner.LearnRefNumber);

                foreach (var learningDelivery in learner.LearningDeliveries ?? Enumerable.Empty<ILearningDelivery>())
                {
                    if (learningDelivery.FundModel != FundModelConstants.FM36)
                    {
                        continue;
                    }

                    LARSLearningDelivery larsDelivery = larsLearningDeliveries.GetValueOrDefault(learningDelivery.LearnAimRef);
                    LearningDelivery fm36LearningDelivery = fm36Learner?.LearningDeliveries?.FirstOrDefault(x => x.AimSeqNumber == learningDelivery.AimSeqNumber);
                    string larsStandard = null;

                    if (learningDelivery.StdCodeNullable != null)
                    {
                        larsStandard = larsStandards.GetValueOrDefault(learningDelivery.StdCodeNullable.Value)?.NotionalEndLevel ??"NA";
                    }

                    if (fm36Learner?.PriceEpisodes.Any() ?? false)
                    {
                        List<PriceEpisode> episodesInRange = fm36Learner.PriceEpisodes
                            .Where(p => p.PriceEpisodeValues.EpisodeStartDate >= ReportingConstants.BeginningOfYear &&
                                        p.PriceEpisodeValues.EpisodeStartDate <= ReportingConstants.EndOfYear &&
                                        learningDelivery.AimSeqNumber == p.PriceEpisodeValues.PriceEpisodeAimSeqNumber)
                            .ToList();

                        if (episodesInRange.Any())
                        {
                            DateTime earliestEpisodeDate = episodesInRange.Select(x => x.PriceEpisodeValues.EpisodeStartDate ?? DateTime.MaxValue).Min();
                            
                            foreach (PriceEpisode episodeAttribute in episodesInRange)
                            {
                                appsIndicativeEarningsModels.Add(BuildLineModel(learner,
                                                                                learningDelivery,
                                                                                fm36LearningDelivery,
                                                                                episodeAttribute,
                                                                                larsDelivery,
                                                                                larsStandard,
                                                                                episodeAttribute.PriceEpisodeValues.EpisodeStartDate == earliestEpisodeDate,
                                                                                true));
                            }
                            continue;
                        }
                    }

                    appsIndicativeEarningsModels.Add(BuildLineModel(learner,
                        learningDelivery,
                        fm36LearningDelivery,
                        null,
                        larsDelivery,
                        larsStandard,
                        false,
                        false));
                }
            }

            return appsIndicativeEarningsModels
                .OrderBy(x => x.LearnRefNumber)
                .ThenByDescending(x => x.AimSeqNumber)
                .ThenByDescending(x => x.PriceEpisodeStartDate);
        }

        public IDictionary<string, LARSLearningDelivery> BuildLarsLearningDeliveryDictionary(ReferenceDataRoot referenceDataRoot)
        {
            return referenceDataRoot?.LARSLearningDeliveries?.ToDictionary(ld => ld.LearnAimRef, ld => ld, StringComparer.OrdinalIgnoreCase) ?? new Dictionary<string, LARSLearningDelivery>();
        }

        public IDictionary<int, LARSStandard> BuildLarsStandardDictionary(ReferenceDataRoot referenceDataRoot)
        {
            return referenceDataRoot?.LARSStandards?.ToDictionary(ld => ld.StandardCode, ld => ld) ?? new Dictionary<int, LARSStandard>();
        }

        public IDictionary<string, FM36Learner> BuildFm36LearnerDeliveryDictionary(FM36Global fm36Global)
        {
            return fm36Global?.Learners?.ToDictionary(ld => ld.LearnRefNumber, ld => ld, StringComparer.OrdinalIgnoreCase) ?? new Dictionary<string, FM36Learner>();
        }

        public AppsIndicativeEarningsReportModel BuildLineModel(
           ILearner learner,
           ILearningDelivery learningDelivery,
           LearningDelivery fm36DeliveryAttribute,
           PriceEpisode fm36EpisodeAttribute,
           LARSLearningDelivery larsLearningDelivery,
           string notionalEndLevel,
           bool earliestEpisode,
           bool hasPriceEpisodes)
        {
            DateTime employmentStatusDate = learner.LearnerEmploymentStatuses?
                .Where(x => x.DateEmpStatApp <= learningDelivery.LearnStartDate).Select(x => x.DateEmpStatApp)
                .DefaultIfEmpty(DateTime.MinValue).Max() ?? DateTime.MinValue;

            var model = new AppsIndicativeEarningsReportModel
            {
                Learner = learner,
                ProviderSpecLearnerMonitoring =
                    _ilrModelMapper.MapProviderSpecLearnerMonitorings(learner.ProviderSpecLearnerMonitorings),
                ProviderSpecDeliveryMonitoring =
                    _ilrModelMapper.MapProviderSpecDeliveryMonitorings(learningDelivery
                        .ProviderSpecDeliveryMonitorings),
                LearningDeliveryFAMs = _ilrModelMapper.MapLearningDeliveryFAMs(learningDelivery.LearningDeliveryFAMs),
                LearningDelivery = learningDelivery,
                LarsLearningDelivery = larsLearningDelivery,
                EmploymentStatus =
                    learner.LearnerEmploymentStatuses?.SingleOrDefault(x => x.DateEmpStatApp == employmentStatusDate),
                PriceEpisodeValues = fm36EpisodeAttribute?.PriceEpisodeValues,
                StandardNotionalEndLevel = notionalEndLevel
            };

            model.EmpStatusMonitoringSmallEmployer = model.EmploymentStatus?.EmploymentStatusMonitorings
                ?.FirstOrDefault(x => string.Equals(x.ESMType, ReportingConstants.EmploymentStatusMonitoringTypeSEM,
                    StringComparison.OrdinalIgnoreCase))?.ESMCode;
            model.FundingLineType = GetFundingType(fm36DeliveryAttribute?.LearningDeliveryValues, fm36EpisodeAttribute?.PriceEpisodeValues);
            model.Fm36LearningDelivery = fm36DeliveryAttribute?.LearningDeliveryValues;

            if (learningDelivery?.LearningDeliveryFAMs != null)
            {
                CalculateApprenticeshipContractTypeFields(
                    learningDelivery,
                    model,
                    fm36DeliveryAttribute,
                    fm36EpisodeAttribute,
                    hasPriceEpisodes);
            }

            if (earliestEpisode || !hasPriceEpisodes)
            {
                CalculateAppFinTotals(model, learningDelivery);
            }

            var isMathsEngLearningDelivery = fm36DeliveryAttribute?.LearningDeliveryValues?.LearnDelMathEng ?? false;

            model.PeriodisedValues = BuildPeriodisedValuesModel(fm36EpisodeAttribute?.PriceEpisodePeriodisedValues,
                fm36DeliveryAttribute?.LearningDeliveryPeriodisedValues, isMathsEngLearningDelivery);
            return model;
        }

        public PeriodisedValuesModel BuildPeriodisedValuesModel(IEnumerable<PriceEpisodePeriodisedValues> priceEpisodePeriodisedValues,
                                                                IEnumerable<LearningDeliveryPeriodisedValues> learningDelieryPeriodisedValues,
                                                                bool isMathsEngLearningDelivery)
        {
            var priceEpisodePeriodisedValuesDictionary = BuildPriceEpisodePeriodisedValuesDictionary(priceEpisodePeriodisedValues);
            var learningDeliveryPeriodisedValuesDictionary = BuildFm36PeriodisedValuesDictionary(learningDelieryPeriodisedValues);


            var onProgrammeEarningsTotal = priceEpisodePeriodisedValuesDictionary.GetValueOrDefault(AttributeConstants.Fm36PriceEpisodeOnProgPaymentAttributeName)?.Sum() ?? _defaultDecimal;
            var balancingPaymentEarningsTotal = priceEpisodePeriodisedValuesDictionary.GetValueOrDefault(AttributeConstants.Fm3PriceEpisodeBalancePaymentAttributeName)?.Sum() ?? _defaultDecimal;
            var aimCompletionEarningsTotal = priceEpisodePeriodisedValuesDictionary.GetValueOrDefault(AttributeConstants.Fm36PriceEpisodeCompletionPaymentAttributeName)?.Sum() ?? _defaultDecimal;
            var learningSupportEarningsTotal = isMathsEngLearningDelivery ? learningDeliveryPeriodisedValuesDictionary.GetValueOrDefault(AttributeConstants.Fm36LearnSuppFundCash)?.Sum() ?? _defaultDecimal :
                                                            priceEpisodePeriodisedValuesDictionary.GetValueOrDefault(AttributeConstants.Fm36PriceEpisodeLSFCashAttributeName)?.Sum() ?? _defaultDecimal;

            var disadvantageEarningsTotal = (priceEpisodePeriodisedValuesDictionary.GetValueOrDefault(AttributeConstants.Fm36PriceEpisodeFirstDisadvantagePaymentAttributeName)?.Sum() ?? _defaultDecimal) +
                                            (priceEpisodePeriodisedValuesDictionary.GetValueOrDefault(AttributeConstants.Fm36PriceEpisodeSecondDisadvantagePaymentAttributeName)?.Sum() ?? _defaultDecimal);

            var additionalPaymentForEmployers1618Total = (priceEpisodePeriodisedValuesDictionary.GetValueOrDefault(AttributeConstants.Fm36PriceEpisodeFirstEmp1618PayAttributeName)?.Sum() ?? _defaultDecimal) +
                                                         (priceEpisodePeriodisedValuesDictionary.GetValueOrDefault(AttributeConstants.Fm36PriceEpisodeSecondEmp1618PayAttributeName)?.Sum() ?? _defaultDecimal);

            var additionalPaymentForProviders1618Total = (priceEpisodePeriodisedValuesDictionary.GetValueOrDefault(AttributeConstants.Fm36PriceEpisodeFirstProv1618PayAttributeName)?.Sum() ?? _defaultDecimal) +
                                                         (priceEpisodePeriodisedValuesDictionary.GetValueOrDefault(AttributeConstants.Fm36PriceEpisodeSecondProv1618PayAttributeName)?.Sum() ?? _defaultDecimal);

            var additionalPaymentsForApprenticesTotal = priceEpisodePeriodisedValuesDictionary.GetValueOrDefault(AttributeConstants.Fm36PriceEpisodeLearnerAdditionalPaymentAttributeName)?.Sum() ?? _defaultDecimal;
            var frameworkUpliftOnProgrammePayment1618Total = priceEpisodePeriodisedValuesDictionary.GetValueOrDefault(AttributeConstants.Fm36PriceEpisodeApplic1618FrameworkUpliftOnProgPaymentAttributeName)?.Sum() ?? _defaultDecimal;
            var frameworkUpliftBalancingPayment1618Total = priceEpisodePeriodisedValuesDictionary.GetValueOrDefault(AttributeConstants.Fm36PriceEpisodeApplic1618FrameworkUpliftBalancingAttributeName)?.Sum() ?? _defaultDecimal;
            var frameworkUpliftCompletionPayment1618Total = priceEpisodePeriodisedValuesDictionary.GetValueOrDefault(AttributeConstants.Fm36PriceEpisodeApplic1618FrameworkUpliftCompletionPaymentAttributeName)?.Sum() ?? _defaultDecimal;



            var englishMathsOnProgrammeEarningsTotal = learningDeliveryPeriodisedValuesDictionary.GetValueOrDefault(AttributeConstants.Fm36MathEngOnProgPayment)?.Sum() ?? _defaultDecimal;
            var englishMathsBalancingPaymentEarningsTotal = learningDeliveryPeriodisedValuesDictionary.GetValueOrDefault(AttributeConstants.Fm36MathEngBalPayment)?.Sum() ?? _defaultDecimal;


            return new PeriodisedValuesModel()
            {
                August = BuildPeriodisedValuesAttributes(priceEpisodePeriodisedValuesDictionary, learningDeliveryPeriodisedValuesDictionary, isMathsEngLearningDelivery, 0),
                September = BuildPeriodisedValuesAttributes(priceEpisodePeriodisedValuesDictionary, learningDeliveryPeriodisedValuesDictionary, isMathsEngLearningDelivery, 1),
                October = BuildPeriodisedValuesAttributes(priceEpisodePeriodisedValuesDictionary, learningDeliveryPeriodisedValuesDictionary, isMathsEngLearningDelivery, 2),
                November = BuildPeriodisedValuesAttributes(priceEpisodePeriodisedValuesDictionary, learningDeliveryPeriodisedValuesDictionary, isMathsEngLearningDelivery, 3),
                December = BuildPeriodisedValuesAttributes(priceEpisodePeriodisedValuesDictionary, learningDeliveryPeriodisedValuesDictionary, isMathsEngLearningDelivery, 4),
                January = BuildPeriodisedValuesAttributes(priceEpisodePeriodisedValuesDictionary, learningDeliveryPeriodisedValuesDictionary, isMathsEngLearningDelivery, 5),
                February = BuildPeriodisedValuesAttributes(priceEpisodePeriodisedValuesDictionary, learningDeliveryPeriodisedValuesDictionary, isMathsEngLearningDelivery, 6),
                March = BuildPeriodisedValuesAttributes(priceEpisodePeriodisedValuesDictionary, learningDeliveryPeriodisedValuesDictionary, isMathsEngLearningDelivery, 7),
                April = BuildPeriodisedValuesAttributes(priceEpisodePeriodisedValuesDictionary, learningDeliveryPeriodisedValuesDictionary, isMathsEngLearningDelivery, 8),
                May = BuildPeriodisedValuesAttributes(priceEpisodePeriodisedValuesDictionary, learningDeliveryPeriodisedValuesDictionary, isMathsEngLearningDelivery, 9),
                June = BuildPeriodisedValuesAttributes(priceEpisodePeriodisedValuesDictionary, learningDeliveryPeriodisedValuesDictionary, isMathsEngLearningDelivery, 10),
                July = BuildPeriodisedValuesAttributes(priceEpisodePeriodisedValuesDictionary, learningDeliveryPeriodisedValuesDictionary, isMathsEngLearningDelivery, 11),
                OnProgrammeEarningsTotal = onProgrammeEarningsTotal,
                BalancingPaymentEarningsTotal = balancingPaymentEarningsTotal,
                AimCompletionEarningsTotal = aimCompletionEarningsTotal,
                LearningSupportEarningsTotal = learningSupportEarningsTotal,
                DisadvantageEarningsTotal = disadvantageEarningsTotal,
                AdditionalPaymentForEmployers1618Total = additionalPaymentForEmployers1618Total,
                AdditionalPaymentForProviders1618Total = additionalPaymentForProviders1618Total,
                AdditionalPaymentsForApprenticesTotal = additionalPaymentsForApprenticesTotal,
                FrameworkUpliftOnProgrammePayment1618Total = frameworkUpliftOnProgrammePayment1618Total,
                FrameworkUpliftBalancingPayment1618Total = frameworkUpliftBalancingPayment1618Total,
                FrameworkUpliftCompletionPayment1618Total = frameworkUpliftCompletionPayment1618Total,
                EnglishMathsOnProgrammeEarningsTotal = englishMathsOnProgrammeEarningsTotal,
                EnglishMathsBalancingPaymentEarningsTotal = englishMathsBalancingPaymentEarningsTotal,
            };
        }


        public IDictionary<string, decimal[]> BuildPriceEpisodePeriodisedValuesDictionary(IEnumerable<PriceEpisodePeriodisedValues> periodisedValues)
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

        public IDictionary<string, decimal[]> BuildFm36PeriodisedValuesDictionary(IEnumerable<LearningDeliveryPeriodisedValues> periodisedValues)
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

        public PeriodisedValuesAttributesModel BuildPeriodisedValuesAttributes(IDictionary<string, decimal[]> priceEpisodePeriodisedValues,
                                                                                IDictionary<string, decimal[]> learningDeliveryPeriodisedValues,
                                                                                bool isMathsEngLearningDelivery,
                                                                                int period)
        {
            return new PeriodisedValuesAttributesModel()
            {
                OnProgrammeEarnings = priceEpisodePeriodisedValues.GetValueOrDefault(AttributeConstants.Fm36PriceEpisodeOnProgPaymentAttributeName)?[period] ?? _defaultDecimal,
                BalancingPaymentEarnings = priceEpisodePeriodisedValues.GetValueOrDefault(AttributeConstants.Fm3PriceEpisodeBalancePaymentAttributeName)?[period] ?? _defaultDecimal,
                AimCompletionEarnings = priceEpisodePeriodisedValues.GetValueOrDefault(AttributeConstants.Fm36PriceEpisodeCompletionPaymentAttributeName)?[period] ?? _defaultDecimal,
                LearningSupportEarnings = isMathsEngLearningDelivery ? learningDeliveryPeriodisedValues.GetValueOrDefault(AttributeConstants.Fm36LearnSuppFundCash)?[period] ?? _defaultDecimal :
                                                priceEpisodePeriodisedValues.GetValueOrDefault(AttributeConstants.Fm36PriceEpisodeLSFCashAttributeName)?[period] ?? _defaultDecimal,

                DisadvantageEarnings = (priceEpisodePeriodisedValues.GetValueOrDefault(AttributeConstants.Fm36PriceEpisodeFirstDisadvantagePaymentAttributeName)?[period] ?? _defaultDecimal) +
                                       (priceEpisodePeriodisedValues.GetValueOrDefault(AttributeConstants.Fm36PriceEpisodeSecondDisadvantagePaymentAttributeName)?[period] ?? _defaultDecimal),

                AdditionalPaymentForEmployers1618 = (priceEpisodePeriodisedValues.GetValueOrDefault(AttributeConstants.Fm36PriceEpisodeFirstEmp1618PayAttributeName)?[period] ?? _defaultDecimal) +
                                                    (priceEpisodePeriodisedValues.GetValueOrDefault(AttributeConstants.Fm36PriceEpisodeSecondEmp1618PayAttributeName)?[period] ?? _defaultDecimal),

                AdditionalPaymentForProviders1618 = (priceEpisodePeriodisedValues.GetValueOrDefault(AttributeConstants.Fm36PriceEpisodeFirstProv1618PayAttributeName)?[period] ?? _defaultDecimal) +
                                                    (priceEpisodePeriodisedValues.GetValueOrDefault(AttributeConstants.Fm36PriceEpisodeSecondProv1618PayAttributeName)?[period] ?? _defaultDecimal),

                AdditionalPaymentsForApprentices = priceEpisodePeriodisedValues.GetValueOrDefault(AttributeConstants.Fm36PriceEpisodeLearnerAdditionalPaymentAttributeName)?[period] ?? _defaultDecimal,

                FrameworkUpliftOnProgrammePayment1618 = priceEpisodePeriodisedValues.GetValueOrDefault(AttributeConstants.Fm36PriceEpisodeApplic1618FrameworkUpliftOnProgPaymentAttributeName)?[period] ?? _defaultDecimal,

                FrameworkUpliftBalancingPayment1618 = priceEpisodePeriodisedValues.GetValueOrDefault(AttributeConstants.Fm36PriceEpisodeApplic1618FrameworkUpliftBalancingAttributeName)?[period] ?? _defaultDecimal,

                FrameworkUpliftCompletionPayment1618 = priceEpisodePeriodisedValues.GetValueOrDefault(AttributeConstants.Fm36PriceEpisodeApplic1618FrameworkUpliftCompletionPaymentAttributeName)?[period] ?? _defaultDecimal,

                EnglishMathsOnProgrammeEarnings = learningDeliveryPeriodisedValues.GetValueOrDefault(AttributeConstants.Fm36MathEngOnProgPayment)?[period] ?? _defaultDecimal,
                EnglishMathsBalancingPaymentEarnings = learningDeliveryPeriodisedValues.GetValueOrDefault(AttributeConstants.Fm36MathEngBalPayment)?[period] ?? _defaultDecimal

            };
        }
        public string GetFundingType(
             LearningDeliveryValues learningDeliveryValues,
             PriceEpisodeValues priceEpisodeValues)
        {
            if (learningDeliveryValues != null && learningDeliveryValues.LearnDelMathEng.GetValueOrDefault(false))
            {
                return learningDeliveryValues.LearnDelInitialFundLineType;
            }

            if (priceEpisodeValues != null)
            {
                return priceEpisodeValues.PriceEpisodeFundLineType;
            }

            return string.Empty;
        }

        public void CalculateApprenticeshipContractTypeFields(
           ILearningDelivery learningDelivery,
           AppsIndicativeEarningsReportModel model,
           LearningDelivery fm36DeliveryAttribute,
           PriceEpisode fm36PriceEpisodeAttribute,
           bool hasPriceEpisodes)
        {
            bool learnDelMathEng = fm36DeliveryAttribute?.LearningDeliveryValues?.LearnDelMathEng ?? false;
            bool useDeliveryAttributeDate = learnDelMathEng || !hasPriceEpisodes;
            ILearningDeliveryFAM[] acts = learningDelivery.LearningDeliveryFAMs.Where(x => x.LearnDelFAMType.CaseInsensitiveEquals(LearningDeliveryFAMTypeConstants.ACT)).ToArray();

            if (acts.All(x => x.LearnDelFAMDateFromNullable == null))
            {
                return;
            }


            model.LearningDeliveryFAMTypeACTDateAppliesFrom = acts.Where(x =>
                    (useDeliveryAttributeDate && learningDelivery.LearnStartDate >= x.LearnDelFAMDateFromNullable) ||
                    (!useDeliveryAttributeDate && fm36PriceEpisodeAttribute?.PriceEpisodeValues.EpisodeStartDate >=
                     x.LearnDelFAMDateFromNullable))
                .Select(x => x.LearnDelFAMDateFromNullable)
                .MaxOrDefault();

            model.LearningDeliveryFAMTypeACTDateAppliesTo = acts.Where(x =>
                    (useDeliveryAttributeDate && learningDelivery.LearnStartDate <= x.LearnDelFAMDateToNullable) ||
                    (!useDeliveryAttributeDate && fm36PriceEpisodeAttribute?.PriceEpisodeValues.EpisodeStartDate <= x.LearnDelFAMDateToNullable))
                .Select(x => x.LearnDelFAMDateToNullable)
                .MinOrDefault();

            if (model.LearningDeliveryFAMTypeACTDateAppliesTo == null)
            {
                model.LearningDeliveryFAMTypeApprenticeshipContractType = acts.FirstOrDefault(x => x.LearnDelFAMDateToNullable == null)?.LearnDelFAMCode;
            }
            else
            {
                model.LearningDeliveryFAMTypeApprenticeshipContractType = acts.FirstOrDefault(x =>
                    x.LearnDelFAMDateFromNullable == model.LearningDeliveryFAMTypeACTDateAppliesFrom &&
                    x.LearnDelFAMDateToNullable == model.LearningDeliveryFAMTypeACTDateAppliesTo)?.LearnDelFAMCode;
            }
        }

        public void CalculateAppFinTotals(AppsIndicativeEarningsReportModel model, ILearningDelivery learningDelivery)
        {
            if (learningDelivery.AppFinRecords == null)
            {
                return;
            }

            List<IAppFinRecord> previousYearData = learningDelivery.AppFinRecords
                .Where(x => x.AFinDate < ReportingConstants.BeginningOfYear &&
                            string.Equals(x.AFinType, "PMR", StringComparison.OrdinalIgnoreCase)).ToList();
            List<IAppFinRecord> currentYearData = learningDelivery.AppFinRecords
                .Where(x => x.AFinDate >= ReportingConstants.BeginningOfYear && x.AFinDate <= ReportingConstants.EndOfYear &&
                            string.Equals(x.AFinType, "PMR", StringComparison.OrdinalIgnoreCase)).ToList();

            model.TotalPRMPreviousFundingYear =
                previousYearData.Where(x => x.AFinCode == 1 || x.AFinCode == 2).Sum(x => x.AFinAmount) -
                previousYearData.Where(x => x.AFinCode == 3).Sum(x => x.AFinAmount);

            model.TotalPRMThisFundingYear =
                currentYearData.Where(x => x.AFinCode == 1 || x.AFinCode == 2).Sum(x => x.AFinAmount) -
                currentYearData.Where(x => x.AFinCode == 3).Sum(x => x.AFinAmount);
        }

    }
}
