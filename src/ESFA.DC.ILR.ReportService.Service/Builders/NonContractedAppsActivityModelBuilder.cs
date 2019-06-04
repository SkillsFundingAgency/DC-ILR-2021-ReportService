using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ReportService.Interface.Builders;
using ESFA.DC.ILR.ReportService.Interface.Service;
using ESFA.DC.ILR.ReportService.Model.FCS;
using ESFA.DC.ILR.ReportService.Model.Lars;
using ESFA.DC.ILR.ReportService.Model.NonContractedAppsActivity;
using ESFA.DC.ILR.ReportService.Model.ReportModels;
using ESFA.DC.ILR.ReportService.Service.Extensions;

namespace ESFA.DC.ILR.ReportService.Service.Builders
{
    public class NonContractedAppsActivityModelBuilder : INonContractedAppsActivityModelBuilder
    {
        private readonly IPeriodProviderService _periodProviderService;

        public NonContractedAppsActivityModelBuilder(IPeriodProviderService periodProviderService)
        {
            _periodProviderService = periodProviderService;
        }

        public List<NonContractedAppsActivityModel> BuildModel(
            NonContractedAppsActivityILRInfo ilrInfo,
            NonContractedActivityRuleBaseInfo fm36Info,
            List<ContractAllocationInfo> fcsContractAllocationInfo,
            Dictionary<string, LarsLearningDelivery> larsLearningDeliveries)
        {
            var nonContractedAppsActivityModels = new List<NonContractedAppsActivityModel>();
            foreach (var learner in ilrInfo.Learners)
            {
                var aecLearningDeliveries = fm36Info.AECLearningDeliveries.Where(x => string.Equals(x.LearnRefNumber, learner.LearnRefNumber, StringComparison.OrdinalIgnoreCase)).ToList();

                foreach (var learningDelivery in learner.LearningDeliveries)
                {
                    if (learningDelivery.FundModel != 36)
                    {
                        continue;
                    }

                    LarsLearningDelivery larsDelivery = larsLearningDeliveries.SingleOrDefault(x => string.Equals(x.Key, learningDelivery.LearnAimRef, StringComparison.OrdinalIgnoreCase)).Value;
                    var aecLearningDeliveryInfo = aecLearningDeliveries?.SingleOrDefault(x => x.AimSeqNumber == learningDelivery.AimSeqNumber);
                    bool learnDelMathEng = aecLearningDeliveryInfo?.LearningDeliveryValues?.LearnDelMathEng ?? false;
                    if (learnDelMathEng)
                    {
                        List<ACTTemporalFAMPeriod> actTemporalFamPeriods = new List<ACTTemporalFAMPeriod>();
                        var period = 0;
                        foreach (var fundingLineType in aecLearningDeliveryInfo.LearningDeliveryPeriodisedTextValues.Periods)
                        {
                            if (fundingLineType.ToLower().Equals("none"))
                            {
                                period++;
                                continue;
                            }

                            var fundingStreamPeriodCode = GetFundingStreamPeriod(fundingLineType);
                            var contractExists = fcsContractAllocationInfo.Exists(x => x.FundingStreamPeriodCode.CaseInsensitiveEquals(fundingStreamPeriodCode));
                            if (!contractExists)
                            {
                                var fam = GetACTForThePeriod(period, learningDelivery);
                                if (fam != null)
                                {
                                    var actTemporalFamPeriod = actTemporalFamPeriods.SingleOrDefault(x =>
                                        x.FamInfo.LearnDelFAMAppliesFrom == fam.LearnDelFAMAppliesFrom &&
                                        x.FamInfo.LearnDelFAMAppliesTo == fam.LearnDelFAMAppliesTo &&
                                        x.FamInfo.LearnDelFAMCode.Equals(fam.LearnDelFAMCode) &&
                                        x.FamInfo.LearnDelFAMType.Equals(fam.LearnDelFAMType));
                                    if (actTemporalFamPeriod != null)
                                    {
                                        actTemporalFamPeriod.Periods[period] = CalculateMathsEnglishEarnings(period, aecLearningDeliveryInfo);
                                    }
                                    else
                                    {
                                        var actPeriod = new ACTTemporalFAMPeriod();
                                        actPeriod.FamInfo = fam;
                                        actPeriod.FundingLineType = fundingLineType;
                                        actPeriod.Periods[period] = CalculateMathsEnglishEarnings(period, aecLearningDeliveryInfo);
                                        actTemporalFamPeriods.Add(actPeriod);
                                    }
                                }
                            }

                            period++;
                        }

                        foreach (var actTemporalFamPeriod in actTemporalFamPeriods)
                        {
                            var nonContractedAppsActivityModel = BuildLineItem(learner, learningDelivery, aecLearningDeliveryInfo, null, larsDelivery, actTemporalFamPeriod, isMathsAndEnglish: true);
                            nonContractedAppsActivityModel.FundingLineType = actTemporalFamPeriod.FundingLineType;
                            nonContractedAppsActivityModels.Add(nonContractedAppsActivityModel);
                        }
                    }
                    else
                    {
                        List<PriceEpisodeInfo> episodesInRange = fm36Info.PriceEpisodes
                            .Where(p => learningDelivery.AimSeqNumber == p.PriceEpisodeValues.PriceEpisodeAimSeqNumber).ToList();

                        if (aecLearningDeliveryInfo != null)
                        {
                            foreach (var priceEpisode in episodesInRange)
                            {
                                var fundingLineType = GetFundingType(aecLearningDeliveryInfo.LearningDeliveryValues, priceEpisode.PriceEpisodeValues);
                                var fundingStreamPeriodCode = GetFundingStreamPeriod(fundingLineType);
                                var contractExists = fcsContractAllocationInfo.Exists(x => x.FundingStreamPeriodCode.CaseInsensitiveEquals(fundingStreamPeriodCode));
                                if (!contractExists)
                                {
                                    var nonContractedAppsActivityModel = BuildLineItem(learner, learningDelivery, aecLearningDeliveryInfo, priceEpisode, larsDelivery, actTemporalFamPeriod: null, isMathsAndEnglish: false);
                                    nonContractedAppsActivityModel.FundingLineType = fundingLineType;
                                    nonContractedAppsActivityModels.Add(nonContractedAppsActivityModel);
                                }
                            }
                        }
                    }
                }
            }

            return nonContractedAppsActivityModels;
        }

        private NonContractedAppsActivityModel BuildLineItem(
            NonContractedAppsActivityLearnerInfo learner,
            NonContractedAppsActivityLearningDeliveryInfo learningDeliveryInfo,
            AECLearningDeliveryInfo aecLearningDeliveryInfo,
            PriceEpisodeInfo priceEpisode,
            LarsLearningDelivery larsLearningDelivery,
            ACTTemporalFAMPeriod actTemporalFamPeriod,
            bool isMathsAndEnglish)
        {
            var learnDelFamCodeEef = learningDeliveryInfo.LearningDeliveryFams?.SingleOrDefault(x => string.Equals(x.LearnDelFAMType, "EEF", StringComparison.OrdinalIgnoreCase))?.LearnDelFAMCode;

            var model = new NonContractedAppsActivityModel
            {
                LearnerReferenceNumber = learner.LearnRefNumber,
                UniqueLearnerNumber = learner.UniqueLearnerNumber,
                DateOfBirth = learner.DateOfBirth.ToString("dd/MM/yyyy"),
                CampusIdentifier = learner.CampId,
                ProviderSpecifiedLearnerMonitoringA = learner.ProviderSpecLearnerMonitorings
                    ?.SingleOrDefault(x => string.Equals(x.ProvSpecLearnMonOccur, "A", StringComparison.OrdinalIgnoreCase))?.ProvSpecLearnMon,
                ProviderSpecifiedLearnerMonitoringB = learner.ProviderSpecLearnerMonitorings
                    ?.SingleOrDefault(x => string.Equals(x.ProvSpecLearnMonOccur, "B", StringComparison.OrdinalIgnoreCase))?.ProvSpecLearnMon,
                AimSeqNumber = aecLearningDeliveryInfo.AimSeqNumber,
                LearningAimReference = learningDeliveryInfo.LearnAimRef,
                LearningAimTitle = larsLearningDelivery?.LearningAimTitle,
                SoftwareSupplierAimIdentifier = learningDeliveryInfo.SWSupAimId,
                ProgrammeType = learningDeliveryInfo.ProgType,
                StandardCode = learningDeliveryInfo.StdCode,
                FrameworkCode = learningDeliveryInfo.FworkCode,
                ApprenticeshipPathway = learningDeliveryInfo.PwayCode,
                AimType = learningDeliveryInfo.AimType,
                OriginalLearningStartDate = learningDeliveryInfo.OriginalLearnStartDate.GetValueOrDefault().ToString("dd/MM/yyyy"),
                LearningStartDate = learningDeliveryInfo.LearnStartDate.ToString("dd/MM/yyyy"),
                LearningPlannedEndDate = learningDeliveryInfo.LearningPlannedEndDate.ToString("dd/MM/yyyy"),
                LearningActualEndDate = learningDeliveryInfo.LearnActualEndDate?.ToString("dd/MM/yyyy"),
                LearningDeliveryFAMTypeEEF = learnDelFamCodeEef,
                ProviderSpecifiedDeliveryMonitoringA = learningDeliveryInfo.ProviderSpecDeliveryMonitorings?.SingleOrDefault(x =>
                    string.Equals(x.ProvSpecDelMonOccur, "A", StringComparison.OrdinalIgnoreCase))?.ProvSpecDelMon,
                ProviderSpecifiedDeliveryMonitoringB = learningDeliveryInfo.ProviderSpecDeliveryMonitorings?.SingleOrDefault(x =>
                    string.Equals(x.ProvSpecDelMonOccur, "B", StringComparison.OrdinalIgnoreCase))?.ProvSpecDelMon,
                ProviderSpecifiedDeliveryMonitoringC = learningDeliveryInfo.ProviderSpecDeliveryMonitorings?.SingleOrDefault(x =>
                    string.Equals(x.ProvSpecDelMonOccur, "C", StringComparison.OrdinalIgnoreCase))?.ProvSpecDelMon,
                ProviderSpecifiedDeliveryMonitoringD = learningDeliveryInfo.ProviderSpecDeliveryMonitorings?.SingleOrDefault(x =>
                    string.Equals(x.ProvSpecDelMonOccur, "D", StringComparison.OrdinalIgnoreCase))?.ProvSpecDelMon,
                PriceEpisodeStartDate = priceEpisode?.PriceEpisodeValues?.EpisodeStartDate.ToString("dd/MM/yyyy"),
                PriceEpisodeActualEndDate = priceEpisode?.PriceEpisodeValues?.PriceEpisodeActualEndDate.ToString("dd/MM/yyyy"),
                AppAdjLearnStartDate = aecLearningDeliveryInfo.LearningDeliveryValues.AppAdjLearnStartDate.ToString("dd/MM/yyyy"),
                AgeAtProgrammeStart = aecLearningDeliveryInfo.LearningDeliveryValues.AgeAtProgStart
            };

            if (isMathsAndEnglish)
            {
                model.AugustTotalEarnings = actTemporalFamPeriod.Periods[0];
                model.SeptemberTotalEarnings = actTemporalFamPeriod.Periods[1];
                model.OctoberTotalEarnings = actTemporalFamPeriod.Periods[2];
                model.NovemberTotalEarnings = actTemporalFamPeriod.Periods[3];
                model.DecemberTotalEarnings = actTemporalFamPeriod.Periods[4];
                model.JanuaryTotalEarnings = actTemporalFamPeriod.Periods[5];
                model.FebruaryTotalEarnings = actTemporalFamPeriod.Periods[6];
                model.MarchTotalEarnings = actTemporalFamPeriod.Periods[7];
                model.AprilTotalEarnings = actTemporalFamPeriod.Periods[8];
                model.MayTotalEarnings = actTemporalFamPeriod.Periods[9];
                model.JuneTotalEarnings = actTemporalFamPeriod.Periods[10];
                model.JulyTotalEarnings = actTemporalFamPeriod.Periods[11];
                model.LearningDeliveryFAMTypeACTDateAppliesFrom = actTemporalFamPeriod.FamInfo.LearnDelFAMAppliesFrom.GetValueOrDefault().ToString("dd/MM/yyyy");
                model.LearningDeliveryFAMTypeACTDateAppliesTo = actTemporalFamPeriod.FamInfo.LearnDelFAMAppliesTo?.ToString("dd/MM/yyyy");
                model.LearningDeliveryFAMTypeApprenticeshipContractType = actTemporalFamPeriod.FamInfo.LearnDelFAMCode;
            }
            else
            {
                model.AugustTotalEarnings = CalculateNonMathsAndEnglishAimEarnings(0, priceEpisode.AECApprenticeshipPriceEpisodePeriodisedValues);
                model.SeptemberTotalEarnings = CalculateNonMathsAndEnglishAimEarnings(1, priceEpisode.AECApprenticeshipPriceEpisodePeriodisedValues);
                model.OctoberTotalEarnings = CalculateNonMathsAndEnglishAimEarnings(2, priceEpisode.AECApprenticeshipPriceEpisodePeriodisedValues);
                model.NovemberTotalEarnings = CalculateNonMathsAndEnglishAimEarnings(3, priceEpisode.AECApprenticeshipPriceEpisodePeriodisedValues);
                model.DecemberTotalEarnings = CalculateNonMathsAndEnglishAimEarnings(4, priceEpisode.AECApprenticeshipPriceEpisodePeriodisedValues);
                model.JanuaryTotalEarnings = CalculateNonMathsAndEnglishAimEarnings(5, priceEpisode.AECApprenticeshipPriceEpisodePeriodisedValues);
                model.FebruaryTotalEarnings = CalculateNonMathsAndEnglishAimEarnings(6, priceEpisode.AECApprenticeshipPriceEpisodePeriodisedValues);
                model.MarchTotalEarnings = CalculateNonMathsAndEnglishAimEarnings(7, priceEpisode.AECApprenticeshipPriceEpisodePeriodisedValues);
                model.AprilTotalEarnings = CalculateNonMathsAndEnglishAimEarnings(8, priceEpisode.AECApprenticeshipPriceEpisodePeriodisedValues);
                model.MayTotalEarnings = CalculateNonMathsAndEnglishAimEarnings(9, priceEpisode.AECApprenticeshipPriceEpisodePeriodisedValues);
                model.JuneTotalEarnings = CalculateNonMathsAndEnglishAimEarnings(10, priceEpisode.AECApprenticeshipPriceEpisodePeriodisedValues);
                model.JulyTotalEarnings = CalculateNonMathsAndEnglishAimEarnings(11, priceEpisode.AECApprenticeshipPriceEpisodePeriodisedValues);
                CalculateLearningDeliveryFamFields(learningDeliveryInfo, model, priceEpisode);
            }

            model.TotalEarnings = model.AugustTotalEarnings + model.SeptemberTotalEarnings +
                                  model.OctoberTotalEarnings + model.NovemberTotalEarnings +
                                  model.DecemberTotalEarnings + model.JanuaryTotalEarnings +
                                  model.FebruaryTotalEarnings + model.MarchTotalEarnings +
                                  model.AprilTotalEarnings + model.MayTotalEarnings + model.JuneTotalEarnings +
                                  model.JulyTotalEarnings;
            return model;
        }

        private void CalculateLearningDeliveryFamFields(
            NonContractedAppsActivityLearningDeliveryInfo learningDelivery,
            NonContractedAppsActivityModel model,
            PriceEpisodeInfo fm36PriceEpisodeInfo)
        {
            var acts = learningDelivery.LearningDeliveryFams.Where(x =>
                string.Equals(x.LearnDelFAMType, Constants.LearningDeliveryFAMCodeACT, StringComparison.OrdinalIgnoreCase)).ToArray();

            DateTime contractAppliesFrom = acts.Where(x => fm36PriceEpisodeInfo?.PriceEpisodeValues.EpisodeStartDate >= x.LearnDelFAMAppliesFrom)
                .Select(x => x.LearnDelFAMAppliesFrom ?? DateTime.MinValue)
                .DefaultIfEmpty(DateTime.MinValue)
                .Max();

            DateTime contractAppliesTo = acts.Where(x => (fm36PriceEpisodeInfo?.PriceEpisodeValues.EpisodeStartDate <= x.LearnDelFAMAppliesTo))
                .Select(x => x.LearnDelFAMAppliesTo ?? DateTime.MinValue)
                .DefaultIfEmpty(DateTime.MinValue)
                .Min();

            if (contractAppliesTo == DateTime.MinValue)
            {
                model.LearningDeliveryFAMTypeApprenticeshipContractType = acts.FirstOrDefault(x => x.LearnDelFAMAppliesTo == null)?.LearnDelFAMCode;
            }
            else
            {
                model.LearningDeliveryFAMTypeApprenticeshipContractType = acts.FirstOrDefault(x =>
                    x.LearnDelFAMAppliesFrom == contractAppliesFrom &&
                    x.LearnDelFAMAppliesTo == contractAppliesTo)?.LearnDelFAMCode;
            }

            model.LearningDeliveryFAMTypeACTDateAppliesFrom = contractAppliesFrom == DateTime.MinValue ? $"" : contractAppliesFrom.ToString("dd/MM/yyyy");
            model.LearningDeliveryFAMTypeACTDateAppliesTo = contractAppliesTo == DateTime.MinValue ? $"" : contractAppliesTo.ToString("dd/MM/yyyy");
        }

        private NonContractedAppsActivityLearningDeliveryFAMInfo GetACTForThePeriod(int period, NonContractedAppsActivityLearningDeliveryInfo learningDelivery)
        {
            var acts = learningDelivery.LearningDeliveryFams.Where(x =>
                string.Equals(x.LearnDelFAMType, Constants.LearningDeliveryFAMCodeACT, StringComparison.OrdinalIgnoreCase)).ToArray();

            NonContractedAppsActivityLearningDeliveryFAMInfo act;
            if (learningDelivery.LearnActualEndDate != null)
            {
                act = acts.SingleOrDefault(x => x.LearnDelFAMAppliesTo == learningDelivery.LearnActualEndDate);
            }
            else
            {
                var periodEndDate = GetPeriodEndDate(period);
                act = acts.FirstOrDefault(x => x.LearnDelFAMAppliesTo != null ? (x.LearnDelFAMAppliesFrom <= periodEndDate && x.LearnDelFAMAppliesTo >= periodEndDate) : x.LearnDelFAMAppliesFrom <= periodEndDate);
            }

            return act;
        }

        private DateTime GetPeriodEndDate(int period)
        {
            int year = period >= 6 ? Constants.EndOfYear.Year : Constants.BeginningOfYear.Year;
            return new DateTime(year, _periodProviderService.MonthFromPeriod(period), DateTime.DaysInMonth(year, _periodProviderService.MonthFromPeriod(period)));
        }

        private decimal CalculateMathsEnglishEarnings(int month, AECLearningDeliveryInfo learningDelivery)
        {
            decimal result = 0;
            result = learningDelivery.LearningDeliveryPeriodisedValues.FirstOrDefault(x => x.AttributeName.Equals(Constants.Fm36MathEngOnProgPayment))?.Periods[month] ?? 0 +
                           learningDelivery.LearningDeliveryPeriodisedValues.FirstOrDefault(x => x.AttributeName.Equals(Constants.Fm36MathEngBalPayment))?.Periods[month] ?? 0 +
                           learningDelivery.LearningDeliveryPeriodisedValues.FirstOrDefault(x => x.AttributeName.Equals(Constants.Fm36LearnSuppFundCash))?.Periods[month] ?? 0;

            return result;
        }

        private decimal CalculateNonMathsAndEnglishAimEarnings(int month, List<AECApprenticeshipPriceEpisodePeriodisedValuesInfo> priceEpisodePeriodisedValues)
        {
            decimal result = 0;
            result = priceEpisodePeriodisedValues.FirstOrDefault(x => x.AttributeName.Equals(Constants.Fm36PriceEpisodeOnProgPaymentAttributeName))?.Periods[month] ?? 0 +
                     priceEpisodePeriodisedValues.FirstOrDefault(x => x.AttributeName.Equals(Constants.Fm3PriceEpisodeBalancePaymentAttributeName))?.Periods[month] ?? 0 +
                     priceEpisodePeriodisedValues.FirstOrDefault(x => x.AttributeName.Equals(Constants.Fm36PriceEpisodeCompletionPaymentAttributeName))?.Periods[month] ?? 0 +
                     priceEpisodePeriodisedValues.FirstOrDefault(x => x.AttributeName.Equals(Constants.Fm36PriceEpisodeLSFCashAttributeName))?.Periods[month] ?? 0 +
                     priceEpisodePeriodisedValues.FirstOrDefault(x => x.AttributeName.Equals(Constants.Fm36PriceEpisodeFirstDisadvantagePaymentAttributeName))?.Periods[month] ?? 0 +
                     priceEpisodePeriodisedValues.FirstOrDefault(x => x.AttributeName.Equals(Constants.Fm36PriceEpisodeSecondDisadvantagePaymentAttributeName))?.Periods[month] ?? 0 +
                     priceEpisodePeriodisedValues.FirstOrDefault(x => x.AttributeName.Equals(Constants.Fm36PriceEpisodeFirstEmp1618PayAttributeName))?.Periods[month] ?? 0 +
                     priceEpisodePeriodisedValues.FirstOrDefault(x => x.AttributeName.Equals(Constants.Fm36PriceEpisodeSecondEmp1618PayAttributeName))?.Periods[month] ?? 0 +
                     priceEpisodePeriodisedValues.FirstOrDefault(x => x.AttributeName.Equals(Constants.Fm36PriceEpisodeFirstProv1618PayAttributeName))?.Periods[month] ?? 0 +
                     priceEpisodePeriodisedValues.FirstOrDefault(x => x.AttributeName.Equals(Constants.Fm36PriceEpisodeSecondProv1618PayAttributeName))?.Periods[month] ?? 0 +
                     priceEpisodePeriodisedValues.FirstOrDefault(x => x.AttributeName.Equals(Constants.Fm36PriceEpisodeApplic1618FrameworkUpliftOnProgPaymentAttributeName))?.Periods[month] ?? 0 +
                     priceEpisodePeriodisedValues.FirstOrDefault(x => x.AttributeName.Equals(Constants.Fm36PriceEpisodeApplic1618FrameworkUpliftBalancingAttributeName))?.Periods[month] ?? 0 +
                     priceEpisodePeriodisedValues.FirstOrDefault(x => x.AttributeName.Equals(Constants.Fm36PriceEpisodeApplic1618FrameworkUpliftCompletionPaymentAttributeName))?.Periods[month] ?? 0 +
                     priceEpisodePeriodisedValues.FirstOrDefault(x => x.AttributeName.Equals(Constants.Fm36PriceEpisodeLearnerAdditionalPaymentAttributeName))?.Periods[month] ?? 0;
            return result;
        }

        private string GetFundingType(AECLearningDeliveryValuesInfo learningDeliveryValues, PriceEpisodeValuesInfo priceEpisodeValues)
        {
            if (learningDeliveryValues.LearnDelMathEng)
            {
                return learningDeliveryValues.LearnDelInitialFundLineType;
            }
            else
            {
                return priceEpisodeValues.PriceEpisodeFundLineType;
            }
        }

        private string GetFundingStreamPeriod(string fundingLineType)
        {
            switch (fundingLineType)
            {
                case "16-18 Apprenticeship (From May 2017) Levy Contract":
                case "19+ Apprenticeship (From May 2017) Levy Contract":
                    return "LEVY1799";
                case "16-18 Apprenticeship (From May 2017) Non-Levy Contract":
                case "16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured)":
                case "19+ Apprenticeship (From May 2017) Non-Levy Contract":
                case "19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured)":
                    return "APPS1819";
                case "16-18 Apprenticeship Non-Levy Contract (procured)":
                    return "16-18NLAP2018";
                case "19+ Apprenticeship Non-Levy Contract (procured)":
                    return "ANLAP2018 ";
                default:
                    return string.Empty;
            }
        }
    }
}