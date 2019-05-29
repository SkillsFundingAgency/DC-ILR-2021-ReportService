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
            Dictionary<string, LarsLearningDelivery> larsLearningDeliveries,
            int returnPeriod)
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
                                var nonContractedAppsActivityModel = BuildLineItem(learner, learningDelivery, aecLearningDeliveryInfo, priceEpisode, larsDelivery);
                                nonContractedAppsActivityModel.FundingLineType = fundingLineType;
                                CalculateLearningDeliveryFamFields(learningDelivery, nonContractedAppsActivityModel, aecLearningDeliveryInfo, priceEpisode, returnPeriod);
                                nonContractedAppsActivityModels.Add(nonContractedAppsActivityModel);
                            }
                        }
                    }
                }
            }

            return nonContractedAppsActivityModels;
        }

        private NonContractedAppsActivityModel BuildLineItem(NonContractedAppsActivityLearnerInfo learner, NonContractedAppsActivityLearningDeliveryInfo learningDeliveryInfo, AECLearningDeliveryInfo aecLearningDeliveryInfo, PriceEpisodeInfo priceEpisode, LarsLearningDelivery larsLearningDelivery)
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
                LearningActualEndDate = learningDeliveryInfo.LearnActualEndDate.GetValueOrDefault().ToString("dd/MM/yyyy"),
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
                AgeAtProgrammeStart = aecLearningDeliveryInfo.LearningDeliveryValues.AgeAtProgStart,
                AugustTotalEarnings = CalculateEarnings(0,  aecLearningDeliveryInfo, priceEpisode.AECApprenticeshipPriceEpisodePeriodisedValues),
                SeptemberTotalEarnings = CalculateEarnings(1,  aecLearningDeliveryInfo, priceEpisode.AECApprenticeshipPriceEpisodePeriodisedValues),
                OctoberTotalEarnings = CalculateEarnings(2,  aecLearningDeliveryInfo, priceEpisode.AECApprenticeshipPriceEpisodePeriodisedValues),
                NovemberTotalEarnings = CalculateEarnings(3,  aecLearningDeliveryInfo, priceEpisode.AECApprenticeshipPriceEpisodePeriodisedValues),
                DecemberTotalEarnings = CalculateEarnings(4,  aecLearningDeliveryInfo, priceEpisode.AECApprenticeshipPriceEpisodePeriodisedValues),
                JanuaryTotalEarnings = CalculateEarnings(5,  aecLearningDeliveryInfo, priceEpisode.AECApprenticeshipPriceEpisodePeriodisedValues),
                FebruaryTotalEarnings = CalculateEarnings(6,  aecLearningDeliveryInfo, priceEpisode.AECApprenticeshipPriceEpisodePeriodisedValues),
                MarchTotalEarnings = CalculateEarnings(7,  aecLearningDeliveryInfo, priceEpisode.AECApprenticeshipPriceEpisodePeriodisedValues),
                AprilTotalEarnings = CalculateEarnings(8,  aecLearningDeliveryInfo, priceEpisode.AECApprenticeshipPriceEpisodePeriodisedValues),
                MayTotalEarnings = CalculateEarnings(9,  aecLearningDeliveryInfo, priceEpisode.AECApprenticeshipPriceEpisodePeriodisedValues),
                JuneTotalEarnings = CalculateEarnings(10,  aecLearningDeliveryInfo, priceEpisode.AECApprenticeshipPriceEpisodePeriodisedValues),
                JulyTotalEarnings = CalculateEarnings(11,  aecLearningDeliveryInfo, priceEpisode.AECApprenticeshipPriceEpisodePeriodisedValues)
            };

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
            AECLearningDeliveryInfo fm36LearningDeliveryInfo,
            PriceEpisodeInfo fm36PriceEpisodeInfo,
            int returnPeriod)
        {
            bool learnDelMathEng = fm36LearningDeliveryInfo?.LearningDeliveryValues?.LearnDelMathEng ?? false;
            var acts = learningDelivery.LearningDeliveryFams.Where(x =>
                string.Equals(x.LearnDelFAMType, Constants.LearningDeliveryFAMCodeACT, StringComparison.OrdinalIgnoreCase)).ToArray();

            if (!learnDelMathEng)
            {
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
            else
            {
                NonContractedAppsActivityLearningDeliveryFAMInfo act;
                if (learningDelivery.LearnActualEndDate != null)
                {
                    act = acts.SingleOrDefault(x => x.LearnDelFAMAppliesTo == learningDelivery.LearnActualEndDate);
                }
                else
                {
                    var periodEndDate = GetPeriodEndDate(returnPeriod);
                    act = acts.FirstOrDefault(x => x.LearnDelFAMAppliesTo != null ? (x.LearnDelFAMAppliesFrom > periodEndDate && x.LearnDelFAMAppliesTo < periodEndDate) : x.LearnDelFAMAppliesFrom > periodEndDate);
                }

                model.LearningDeliveryFAMTypeACTDateAppliesFrom = act?.LearnDelFAMAppliesFrom?.ToString("dd/MM/yyyy");
                model.LearningDeliveryFAMTypeACTDateAppliesTo = act?.LearnDelFAMAppliesTo?.ToString("dd/MM/yyyy");
                model.LearningDeliveryFAMTypeApprenticeshipContractType = act?.LearnDelFAMCode;
            }
        }

        private DateTime GetPeriodEndDate(int returnPeriod)
        {
            int year = returnPeriod >= 8 ? 2018 : 2019; //Todo: Year needs to be calculated from the jobContextMessage, for eg: collectionName
            return new DateTime(year, _periodProviderService.MonthFromPeriod(returnPeriod), DateTime.DaysInMonth(year, _periodProviderService.MonthFromPeriod(returnPeriod)));
        }

        private decimal CalculateEarnings(int month, AECLearningDeliveryInfo learningDelivery, List<AECApprenticeshipPriceEpisodePeriodisedValuesInfo> priceEpisodePeriodisedValues)
        {
            decimal result = 0;
            if (learningDelivery.LearningDeliveryValues.LearnDelMathEng)
            {
                result = learningDelivery.LearningDeliveryPeriodisedValues.SingleOrDefault(x => x.AttributeName.Equals(Constants.Fm36MathEngOnProgPayment))?.Periods[month] ?? 0 +
                               learningDelivery.LearningDeliveryPeriodisedValues.SingleOrDefault(x => x.AttributeName.Equals(Constants.Fm36MathEngBalPayment))?.Periods[month] ?? 0 +
                               learningDelivery.LearningDeliveryPeriodisedValues.SingleOrDefault(x => x.AttributeName.Equals(Constants.Fm36LearnSuppFundCash))?.Periods[month] ?? 0;
            }
            else
            {
                result = priceEpisodePeriodisedValues.SingleOrDefault(x => x.AttributeName.Equals(Constants.Fm36PriceEpisodeOnProgPaymentAttributeName))?.Periods[month] ?? 0 +
                         priceEpisodePeriodisedValues.SingleOrDefault(x => x.AttributeName.Equals(Constants.Fm3PriceEpisodeBalancePaymentAttributeName))?.Periods[month] ?? 0 +
                         priceEpisodePeriodisedValues.SingleOrDefault(x => x.AttributeName.Equals(Constants.Fm36PriceEpisodeCompletionPaymentAttributeName))?.Periods[month] ?? 0 +
                         priceEpisodePeriodisedValues.SingleOrDefault(x => x.AttributeName.Equals(Constants.Fm36PriceEpisodeLSFCashAttributeName))?.Periods[month] ?? 0 +
                         priceEpisodePeriodisedValues.SingleOrDefault(x => x.AttributeName.Equals(Constants.Fm36PriceEpisodeFirstDisadvantagePaymentAttributeName))?.Periods[month] ?? 0 +
                         priceEpisodePeriodisedValues.SingleOrDefault(x => x.AttributeName.Equals(Constants.Fm36PriceEpisodeSecondDisadvantagePaymentAttributeName))?.Periods[month] ?? 0 +
                         priceEpisodePeriodisedValues.SingleOrDefault(x => x.AttributeName.Equals(Constants.Fm36PriceEpisodeFirstEmp1618PayAttributeName))?.Periods[month] ?? 0 +
                         priceEpisodePeriodisedValues.SingleOrDefault(x => x.AttributeName.Equals(Constants.Fm36PriceEpisodeSecondEmp1618PayAttributeName))?.Periods[month] ?? 0 +
                         priceEpisodePeriodisedValues.SingleOrDefault(x => x.AttributeName.Equals(Constants.Fm36PriceEpisodeFirstProv1618PayAttributeName))?.Periods[month] ?? 0 +
                         priceEpisodePeriodisedValues.SingleOrDefault(x => x.AttributeName.Equals(Constants.Fm36PriceEpisodeSecondProv1618PayAttributeName))?.Periods[month] ?? 0 +
                         priceEpisodePeriodisedValues.SingleOrDefault(x => x.AttributeName.Equals(Constants.Fm36PriceEpisodeApplic1618FrameworkUpliftOnProgPaymentAttributeName))?.Periods[month] ?? 0 +
                         priceEpisodePeriodisedValues.SingleOrDefault(x => x.AttributeName.Equals(Constants.Fm36PriceEpisodeApplic1618FrameworkUpliftBalancingAttributeName))?.Periods[month] ?? 0 +
                         priceEpisodePeriodisedValues.SingleOrDefault(x => x.AttributeName.Equals(Constants.Fm36PriceEpisodeApplic1618FrameworkUpliftCompletionPaymentAttributeName))?.Periods[month] ?? 0 +
                         priceEpisodePeriodisedValues.SingleOrDefault(x => x.AttributeName.Equals(Constants.Fm36PriceEpisodeLearnerAdditionalPaymentAttributeName))?.Periods[month] ?? 0;
            }

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

            return string.Empty;
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