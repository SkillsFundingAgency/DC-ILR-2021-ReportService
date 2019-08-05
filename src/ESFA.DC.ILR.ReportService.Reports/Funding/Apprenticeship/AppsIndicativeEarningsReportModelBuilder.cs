using System;
using ESFA.DC.ILR.ReportService.Service.Interface;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReferenceDataService.Model.LARS;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Extensions;
using ESFA.DC.ILR.ReportService.Reports.Funding.Apprenticeship.Model;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Apprenticeship
{
    public class AppsIndicativeEarningsReportModelBuilder : IModelBuilder<IEnumerable<AppsIndicativeEarningsReportModel>>
    {
        private const decimal _defaultDecimal = 0;

        public IEnumerable<AppsIndicativeEarningsReportModel> Build(IReportServiceContext reportServiceContext, IReportServiceDependentData reportServiceDependentData)
        {

            var message = reportServiceDependentData.Get<IMessage>();
            var fm36Data = reportServiceDependentData.Get<FM36Global>();    
            var appsIndicativeEarningsModels = new List<AppsIndicativeEarningsReportModel>();

            var referenceData = reportServiceDependentData.Get<ReferenceDataRoot>();
            IDictionary<string, LARSLearningDelivery> larsLearningDeliveries = BuildLarsLearningDeliveryDictionary(referenceData);


            foreach (var learner in message?.Learners?.Where(l => l != null) ?? Enumerable.Empty<ILearner>())
            {
                FM36Learner fm36Learner = fm36Data?.Learners?.SingleOrDefault(x => string.Equals(x.LearnRefNumber, learner.LearnRefNumber, StringComparison.OrdinalIgnoreCase));

                foreach (ILearningDelivery learningDelivery in learner.LearningDeliveries)
                {
                    if (learningDelivery.FundModel != 36)
                    {
                        continue;
                    }

                    string larsStandard = null;
                    if (learningDelivery.StdCodeNullable != null)
                    {
                        larsStandard = referenceData.LARSStandards.SingleOrDefault(l => l.StandardCode == learningDelivery.StdCodeNullable.Value)?.NotionalEndLevel??"NA";
                    }

                    LARSLearningDelivery larsDelivery = larsLearningDeliveries.SingleOrDefault(x => string.Equals(x.Key, learningDelivery.LearnAimRef, StringComparison.OrdinalIgnoreCase)).Value;
                    LearningDelivery fm36LearningDelivery = fm36Learner?.LearningDeliveries
                        ?.SingleOrDefault(x => x.AimSeqNumber == learningDelivery.AimSeqNumber);

                    if (fm36Learner?.PriceEpisodes.Any() ?? false)
                    {
                        List<PriceEpisode> episodesInRange = fm36Learner.PriceEpisodes
                            .Where(p => p.PriceEpisodeValues.EpisodeStartDate >= ReportingConstants.BeginningOfYear &&
                                        p.PriceEpisodeValues.EpisodeStartDate <= ReportingConstants.EndOfYear
                                        && learningDelivery.AimSeqNumber == p.PriceEpisodeValues.PriceEpisodeAimSeqNumber).ToList();

                        if (episodesInRange.Any())
                        {
                            DateTime earliestEpisodeDate = episodesInRange.Select(x => x.PriceEpisodeValues.EpisodeStartDate ?? DateTime.MaxValue).Min();
                            bool earliestEpisode = false;
                            foreach (PriceEpisode episodeAttribute in episodesInRange)
                            {
                                if (episodeAttribute.PriceEpisodeValues.EpisodeStartDate == earliestEpisodeDate)
                                {
                                    earliestEpisode = true;
                                }
                                appsIndicativeEarningsModels.Add(
                                    BuildLineModel(
                                        learner,
                                        learningDelivery,
                                        fm36LearningDelivery,
                                        episodeAttribute,
                                        larsDelivery,
                                        larsStandard,
                                        earliestEpisode,
                                        true));

                                earliestEpisode = false;

                            }
                            continue;
                        }

                    }

                }
            }

            return new List<AppsIndicativeEarningsReportModel>();
        }

        private IDictionary<string, LARSLearningDelivery> BuildLarsLearningDeliveryDictionary(ReferenceDataRoot referenceDataRoot)
        {
            return referenceDataRoot?.LARSLearningDeliveries?.ToDictionary(ld => ld.LearnAimRef, ld => ld, StringComparer.OrdinalIgnoreCase) ?? new Dictionary<string, LARSLearningDelivery>();
        }

        private AppsIndicativeEarningsReportModel BuildLineModel(
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
            ILearnerEmploymentStatus employmentStatus =
                learner.LearnerEmploymentStatuses?.SingleOrDefault(x => x.DateEmpStatApp == employmentStatusDate);
            LearningDeliveryFamSimple[] learningDeliveryFams = GetLearningDeliveryFams(learningDelivery);
            string fundingLineType = GetFundingType(fm36DeliveryAttribute?.LearningDeliveryValues, fm36EpisodeAttribute?.PriceEpisodeValues);
            var ldms = GetArrayEntries(learningDelivery.LearningDeliveryFAMs?.Where(x => string.Equals(x.LearnDelFAMType, ReportingConstants.LearningDeliveryFAMCodeLDM, StringComparison.OrdinalIgnoreCase)), 4);

            var model = new AppsIndicativeEarningsReportModel
            {
                LearnRefNumber = learner.LearnRefNumber,
                UniqueLearnerNumber = learner.ULN,
                DateOfBirth = learner.DateOfBirthNullable?.ToString("dd/MM/yyyy"),
                PostcodePriorToEnrollment = learner.PostcodePrior,
                CampusIdentifier = learner.CampId,
                ProviderSpecifiedLearnerMonitoringA = learner.ProviderSpecLearnerMonitorings
                    ?.SingleOrDefault(x => string.Equals(x.ProvSpecLearnMonOccur, "A", StringComparison.OrdinalIgnoreCase))?.ProvSpecLearnMon,
                ProviderSpecifiedLearnerMonitoringB = learner.ProviderSpecLearnerMonitorings
                    ?.SingleOrDefault(x => string.Equals(x.ProvSpecLearnMonOccur, "B", StringComparison.OrdinalIgnoreCase))?.ProvSpecLearnMon,
                AimSeqNumber = learningDelivery.AimSeqNumber,
                LearningAimReference = learningDelivery.LearnAimRef,
                LearningAimTitle = larsLearningDelivery?.LearnAimRefTitle,
                SoftwareSupplierAimIdentifier = learningDelivery.SWSupAimId,
                LARS1618FrameworkUplift = fm36DeliveryAttribute?.LearningDeliveryValues
                    ?.LearnDelApplicProv1618FrameworkUplift,
                NotionalNVQLevel = larsLearningDelivery?.NotionalNVQLevel,
                StandardNotionalEndLevel = notionalEndLevel,
                Tier2SectorSubjectArea = larsLearningDelivery?.SectorSubjectAreaTier2,
                ProgrammeType = learningDelivery.ProgTypeNullable,
                StandardCode = learningDelivery.StdCodeNullable,
                FrameworkCode = learningDelivery.FworkCodeNullable,
                ApprenticeshipPathway = learningDelivery.PwayCodeNullable,
                AimType = learningDelivery.AimType,
                CommonComponentCode = larsLearningDelivery?.FrameworkCommonComponent,
                FundingModel = learningDelivery.FundModel,
                OriginalLearningStartDate = learningDelivery.OrigLearnStartDateNullable?.ToString("dd/MM/yyyy"),
                LearningStartDate = learningDelivery.LearnStartDate.ToString("dd/MM/yyyy"),
                LearningPlannedEndDate = learningDelivery.LearnPlanEndDate.ToString("dd/MM/yyyy"),
                CompletionStatus = learningDelivery.CompStatus,
                LearningActualEndDate = learningDelivery.LearnActEndDateNullable?.ToString("dd/MM/yyyy"),
                Outcome = learningDelivery.OutcomeNullable,
                FundingAdjustmentForPriorLearning = learningDelivery.PriorLearnFundAdjNullable,
                OtherFundingAdjustment = learningDelivery.OtherFundAdjNullable,
                LearningDeliveryFAMTypeLearningSupportFunding = learningDeliveryFams.Select(x => x.Code).Max(),
                LearningDeliveryFAMTypeLSFDateAppliesFrom =
                    learningDeliveryFams.Select(x => x.From).Min() == DateTime.MinValue ? string.Empty : learningDeliveryFams.Select(x => x.From).Min().ToString("dd/MM/yyyy"),
                LearningDeliveryFAMTypeLSFDateAppliesTo =
                learningDeliveryFams.Select(x => x.To).Max() == DateTime.MinValue ? string.Empty : learningDeliveryFams.Select(x => x.To).Max().ToString("dd/MM/yyyy"),
                LearningDeliveryFAMTypeLearningDeliveryMonitoringA = ldms[0],
                LearningDeliveryFAMTypeLearningDeliveryMonitoringB = ldms[1],
                LearningDeliveryFAMTypeLearningDeliveryMonitoringC = ldms[2],
                LearningDeliveryFAMTypeLearningDeliveryMonitoringD = ldms[3],
                LearningDeliveryFAMRestartIndicator = learningDelivery.LearningDeliveryFAMs
                    ?.SingleOrDefault(x => string.Equals(x.LearnDelFAMType, ReportingConstants.LearningDeliveryFAMCodeRES, StringComparison.OrdinalIgnoreCase))?.LearnDelFAMCode,
                ProviderSpecifiedDeliveryMonitoringA = learningDelivery.ProviderSpecDeliveryMonitorings
                    ?.SingleOrDefault(x => string.Equals(x.ProvSpecDelMonOccur, "A", StringComparison.OrdinalIgnoreCase))?.ProvSpecDelMon,
                ProviderSpecifiedDeliveryMonitoringB = learningDelivery.ProviderSpecDeliveryMonitorings
                    ?.SingleOrDefault(x => string.Equals(x.ProvSpecDelMonOccur, "B", StringComparison.OrdinalIgnoreCase))?.ProvSpecDelMon,
                ProviderSpecifiedDeliveryMonitoringC = learningDelivery.ProviderSpecDeliveryMonitorings
                    ?.SingleOrDefault(x => string.Equals(x.ProvSpecDelMonOccur, "C", StringComparison.OrdinalIgnoreCase))?.ProvSpecDelMon,
                ProviderSpecifiedDeliveryMonitoringD = learningDelivery.ProviderSpecDeliveryMonitorings
                    ?.SingleOrDefault(x => string.Equals(x.ProvSpecDelMonOccur, "D", StringComparison.OrdinalIgnoreCase))?.ProvSpecDelMon,
                EndPointAssessmentOrganisation = learningDelivery.EPAOrgID,
                PlannedNoOfOnProgrammeInstallmentsForAim =
                    fm36DeliveryAttribute?.LearningDeliveryValues?.PlannedNumOnProgInstalm,
                SubContractedOrPartnershipUKPRN = learningDelivery.PartnerUKPRNNullable,
                DeliveryLocationPostcode = learningDelivery.DelLocPostCode,
                EmployerIdentifier = employmentStatus?.EmpIdNullable,
                AgreementIdentifier = fm36EpisodeAttribute?.PriceEpisodeValues?.PriceEpisodeAgreeId,
                EmploymentStatus = employmentStatus?.EmpStat,
                EmploymentStatusDate = employmentStatus?.DateEmpStatApp.ToString("dd/MM/yyyy"),
                EmpStatusMonitoringSmallEmployer = employmentStatus?.EmploymentStatusMonitorings
                    ?.SingleOrDefault(x => string.Equals(x.ESMType, ReportingConstants.EmploymentStatusMonitoringTypeSEM, StringComparison.OrdinalIgnoreCase))?.ESMCode,
                PriceEpisodeStartDate =
                    fm36EpisodeAttribute?.PriceEpisodeValues?.EpisodeStartDate,
                PriceEpisodeActualEndDate =
                    fm36EpisodeAttribute?.PriceEpisodeValues?.PriceEpisodeActualEndDate?.ToString("dd/MM/yyyy"),
                FundingLineType = fundingLineType,
                TotalPriceApplicableToThisEpisode =
                    fm36EpisodeAttribute?.PriceEpisodeValues?.PriceEpisodeTotalTNPPrice,
                FundingBandUpperLimit = fm36EpisodeAttribute?.PriceEpisodeValues?.PriceEpisodeUpperBandLimit,
                PriceAmountAboveFundingBandLimit =
                    fm36EpisodeAttribute?.PriceEpisodeValues?.PriceEpisodeUpperLimitAdjustment,
                PriceAmountRemainingStartOfEpisode = fm36EpisodeAttribute?.PriceEpisodeValues
                    ?.PriceEpisodeCappedRemainingTNPAmount,
                CompletionElement = fm36EpisodeAttribute?.PriceEpisodeValues?.PriceEpisodeCompletionElement
            };

            if (learningDelivery?.LearningDeliveryFAMs != null)
            {
                CalculateApprenticeshipContractTypeFields(
                    learningDelivery,
                    model,
                    fm36DeliveryAttribute,
                    fm36EpisodeAttribute,
                    hasPriceEpisodes);
            }

            if (earliestEpisode)
            {
                CalculateAppFinTotals(model, learningDelivery);
            }

            var isMathsEngLearningDelivery = fm36DeliveryAttribute?.LearningDeliveryValues?.LearnDelMathEng ?? false;

            var periodisedValuesModel = BuildPeriodisedValuesModel(fm36EpisodeAttribute?.PriceEpisodePeriodisedValues,
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

            var disadvantageEarningsTotal = priceEpisodePeriodisedValuesDictionary.GetValueOrDefault(AttributeConstants.Fm36PriceEpisodeFirstDisadvantagePaymentAttributeName)?.Sum() ?? _defaultDecimal +
                                            priceEpisodePeriodisedValuesDictionary.GetValueOrDefault(AttributeConstants.Fm36PriceEpisodeSecondDisadvantagePaymentAttributeName)?.Sum() ?? _defaultDecimal;

            var additionalPaymentForEmployers1618Total = priceEpisodePeriodisedValuesDictionary.GetValueOrDefault(AttributeConstants.Fm36PriceEpisodeFirstEmp1618PayAttributeName)?.Sum() ?? _defaultDecimal +
                                                         priceEpisodePeriodisedValuesDictionary.GetValueOrDefault(AttributeConstants.Fm36PriceEpisodeSecondEmp1618PayAttributeName)?.Sum() ?? _defaultDecimal;

            var additionalPaymentForProviders1618Total = priceEpisodePeriodisedValuesDictionary.GetValueOrDefault(AttributeConstants.Fm36PriceEpisodeFirstProv1618PayAttributeName)?.Sum() ?? _defaultDecimal +
                                                         priceEpisodePeriodisedValuesDictionary.GetValueOrDefault(AttributeConstants.Fm36PriceEpisodeSecondProv1618PayAttributeName)?.Sum() ?? _defaultDecimal;

            var additionalPaymentsForApprenticesTotal = priceEpisodePeriodisedValuesDictionary.GetValueOrDefault(AttributeConstants.Fm36PriceEpisodeLearnerAdditionalPaymentAttributeName)?.Sum() ?? _defaultDecimal;
            var frameworkUpliftOnProgrammePayment1618Total = priceEpisodePeriodisedValuesDictionary.GetValueOrDefault(AttributeConstants.Fm36PriceEpisodeApplic1618FrameworkUpliftOnProgPaymentAttributeName)?.Sum() ?? _defaultDecimal;
            var frameworkUpliftBalancingPayment1618Total = priceEpisodePeriodisedValuesDictionary.GetValueOrDefault(AttributeConstants.Fm36PriceEpisodeApplic1618FrameworkUpliftBalancingAttributeName)?.Sum() ?? _defaultDecimal;
            var frameworkUpliftCompletionPayment1618Total = priceEpisodePeriodisedValuesDictionary.GetValueOrDefault(AttributeConstants.Fm36PriceEpisodeApplic1618FrameworkUpliftCompletionPaymentAttributeName)?.Sum() ?? _defaultDecimal;


            
            var englishMathsOnProgrammeEarningsTotal = learningDeliveryPeriodisedValuesDictionary.GetValueOrDefault(AttributeConstants.Fm36MathEngOnProgPayment)?.Sum() ?? _defaultDecimal;
            var englishMathsBalancingPaymentEarningsTotal = learningDeliveryPeriodisedValuesDictionary.GetValueOrDefault(AttributeConstants.Fm36MathEngBalPayment)?.Sum() ?? _defaultDecimal;


            var totalEarned = onProgrammeEarningsTotal + balancingPaymentEarningsTotal + aimCompletionEarningsTotal + learningSupportEarningsTotal + disadvantageEarningsTotal +
                              additionalPaymentForEmployers1618Total + additionalPaymentForProviders1618Total + additionalPaymentsForApprenticesTotal +
                              frameworkUpliftOnProgrammePayment1618Total + frameworkUpliftBalancingPayment1618Total + frameworkUpliftCompletionPayment1618Total +
                              englishMathsOnProgrammeEarningsTotal + englishMathsBalancingPaymentEarningsTotal;

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
                TotalEarned = totalEarned,
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

                DisadvantageEarnings = priceEpisodePeriodisedValues.GetValueOrDefault(AttributeConstants.Fm36PriceEpisodeFirstDisadvantagePaymentAttributeName)?[period] ?? _defaultDecimal +
                                       priceEpisodePeriodisedValues.GetValueOrDefault(AttributeConstants.Fm36PriceEpisodeSecondDisadvantagePaymentAttributeName)?[period] ?? _defaultDecimal,

                AdditionalPaymentForEmployers1618 = priceEpisodePeriodisedValues.GetValueOrDefault(AttributeConstants.Fm36PriceEpisodeFirstEmp1618PayAttributeName)?[period] ?? _defaultDecimal +
                                                    priceEpisodePeriodisedValues.GetValueOrDefault(AttributeConstants.Fm36PriceEpisodeSecondEmp1618PayAttributeName)?[period] ?? _defaultDecimal,

                AdditionalPaymentForProviders1618 = priceEpisodePeriodisedValues.GetValueOrDefault(AttributeConstants.Fm36PriceEpisodeFirstProv1618PayAttributeName)?[period] ?? _defaultDecimal +
                                                    priceEpisodePeriodisedValues.GetValueOrDefault(AttributeConstants.Fm36PriceEpisodeSecondProv1618PayAttributeName)?[period] ?? _defaultDecimal,

                AdditionalPaymentsForApprentices = priceEpisodePeriodisedValues.GetValueOrDefault(AttributeConstants.Fm36PriceEpisodeLearnerAdditionalPaymentAttributeName)?[period] ?? _defaultDecimal,

                FrameworkUpliftOnProgrammePayment1618 = priceEpisodePeriodisedValues.GetValueOrDefault(AttributeConstants.Fm36PriceEpisodeApplic1618FrameworkUpliftOnProgPaymentAttributeName)?[period] ?? _defaultDecimal,

                FrameworkUpliftBalancingPayment1618 = priceEpisodePeriodisedValues.GetValueOrDefault(AttributeConstants.Fm36PriceEpisodeApplic1618FrameworkUpliftBalancingAttributeName)?[period] ?? _defaultDecimal,

                FrameworkUpliftCompletionPayment1618 = priceEpisodePeriodisedValues.GetValueOrDefault(AttributeConstants.Fm36PriceEpisodeApplic1618FrameworkUpliftCompletionPaymentAttributeName)?[period] ?? _defaultDecimal,

                EnglishMathsOnProgrammeEarnings = learningDeliveryPeriodisedValues.GetValueOrDefault(AttributeConstants.Fm36MathEngOnProgPayment)?[period] ?? _defaultDecimal,
                EnglishMathsBalancingPaymentEarnings = learningDeliveryPeriodisedValues.GetValueOrDefault(AttributeConstants.Fm36MathEngBalPayment)?[period] ?? _defaultDecimal

            };
        }


        private string GetFundingType(
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

        public string[] GetArrayEntries(IEnumerable<ILearningDeliveryFAM> availableValues, int size)
        {
            if (size < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(size), $"{nameof(size)} should be greater than 0");
            }

            string[] values = new string[size];
            int pointer = 0;
            foreach (ILearningDeliveryFAM learningDeliveryFam in availableValues ?? Enumerable.Empty<ILearningDeliveryFAM>())
            {
                values[pointer++] = learningDeliveryFam.LearnDelFAMCode;
                if (pointer == size)
                {
                    break;
                }
            }

            return values;
        }

        private LearningDeliveryFamSimple[] GetLearningDeliveryFams(ILearningDelivery learningDelivery)
        {
            List<LearningDeliveryFamSimple> ret = new List<LearningDeliveryFamSimple>();

            ILearningDeliveryFAM[] lsfs = learningDelivery.LearningDeliveryFAMs
                ?.Where(x =>
                    string.Equals(x.LearnDelFAMType, ReportingConstants.LearningDeliveryFAMCodeLSF, StringComparison.OrdinalIgnoreCase))
                .ToArray();
            if (lsfs == null || !lsfs.Any())
            {
                ret.Add(new LearningDeliveryFamSimple(string.Empty, DateTime.MinValue, DateTime.MinValue));
            }
            else
            {
                foreach (ILearningDeliveryFAM learningDeliveryFam in lsfs)
                {
                    ret.Add(new LearningDeliveryFamSimple(learningDeliveryFam.LearnDelFAMCode, learningDeliveryFam.LearnDelFAMDateFromNullable.GetValueOrDefault(DateTime.MinValue), learningDeliveryFam.LearnDelFAMDateToNullable.GetValueOrDefault(DateTime.MinValue)));
                }
            }

            return ret.ToArray();
        }

        private void CalculateApprenticeshipContractTypeFields(
          ILearningDelivery learningDelivery,
          AppsIndicativeEarningsReportModel model,
          LearningDelivery fm36DeliveryAttribute,
          PriceEpisode fm36PriceEpisodeAttribute,
          bool hasPriceEpisodes)
        {
            bool learnDelMathEng = fm36DeliveryAttribute?.LearningDeliveryValues?.LearnDelMathEng ?? false;
            bool useDeliveryAttributeDate = learnDelMathEng || !hasPriceEpisodes;
            ILearningDeliveryFAM[] acts = learningDelivery.LearningDeliveryFAMs.Where(x =>
                string.Equals(x.LearnDelFAMType, ReportingConstants.LearningDeliveryFAMCodeACT, StringComparison.OrdinalIgnoreCase)).ToArray();

            if (acts.All(x => x.LearnDelFAMDateFromNullable == null))
            {
                return;
            }

            DateTime contractAppliesFrom = acts.Where(x =>
                (useDeliveryAttributeDate && learningDelivery.LearnStartDate >= x.LearnDelFAMDateFromNullable) ||
                (!useDeliveryAttributeDate && fm36PriceEpisodeAttribute?.PriceEpisodeValues.EpisodeStartDate >= x.LearnDelFAMDateFromNullable))
                .Select(x => x.LearnDelFAMDateFromNullable ?? DateTime.MinValue)
                .DefaultIfEmpty(DateTime.MinValue)
                .Max();

            DateTime contractAppliesTo = acts.Where(x =>
                (useDeliveryAttributeDate && learningDelivery.LearnStartDate <= x.LearnDelFAMDateToNullable) ||
                (!useDeliveryAttributeDate && fm36PriceEpisodeAttribute?.PriceEpisodeValues.EpisodeStartDate <= x.LearnDelFAMDateToNullable))
                .Select(x => x.LearnDelFAMDateToNullable ?? DateTime.MinValue)
                .DefaultIfEmpty(DateTime.MinValue)
                .Min();

            model.LearningDeliveryFAMTypeACTDateAppliesFrom = contractAppliesFrom == DateTime.MinValue ? $"" : contractAppliesFrom.ToString("dd/MM/yyyy");
            model.LearningDeliveryFAMTypeACTDateAppliesTo = contractAppliesTo == DateTime.MinValue ? $"" : contractAppliesTo.ToString("dd/MM/yyyy");

            if (contractAppliesTo == DateTime.MinValue)
            {
                model.LearningDeliveryFAMTypeApprenticeshipContractType = acts.FirstOrDefault(x => x.LearnDelFAMDateToNullable == null)?.LearnDelFAMCode;
            }
            else
            {
                model.LearningDeliveryFAMTypeApprenticeshipContractType = acts.FirstOrDefault(x =>
                    x.LearnDelFAMDateFromNullable == contractAppliesFrom &&
                    x.LearnDelFAMDateToNullable == contractAppliesTo)?.LearnDelFAMCode;
            }
        }

        private void CalculateAppFinTotals(AppsIndicativeEarningsReportModel model, ILearningDelivery learningDelivery)
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
