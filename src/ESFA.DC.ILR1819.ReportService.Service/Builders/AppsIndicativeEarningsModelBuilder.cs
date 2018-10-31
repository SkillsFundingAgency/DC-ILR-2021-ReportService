using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.Data.LARS.Model;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.Lars;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;
using LearningDelivery = ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output.LearningDelivery;

namespace ESFA.DC.ILR1819.ReportService.Service.Builders
{
    public class AppsIndicativeEarningsModelBuilder : IAppsIndicativeEarningsModelBuilder
    {
        private readonly IList<IAppsIndicativeCommand> _commands;

        public AppsIndicativeEarningsModelBuilder(IList<IAppsIndicativeCommand> commands)
        {
            _commands = commands;
        }

        public AppsIndicativeEarningsModel BuildModel(
            ILearner learner,
            ILearningDelivery learningDelivery,
            LearningDelivery fm36DeliveryAttribute,
            PriceEpisode fm36EpisodeAttribute,
            LarsLearningDelivery larsLearningDelivery,
            LARS_Standard larsStandard,
            bool earliestEpisode,
            bool hasPriceEpisodes)
        {
            var employmentStatusDate = learner.LearnerEmploymentStatuses
                .Where(x => x.DateEmpStatApp <= learningDelivery.LearnStartDate).Max(x => x.DateEmpStatApp);
            var employmentStatus =
                learner.LearnerEmploymentStatuses?.SingleOrDefault(x => x.DateEmpStatApp == employmentStatusDate);

            var model = new AppsIndicativeEarningsModel
            {
                LearnRefNumber = learner.LearnRefNumber,
                UniqueLearnerNumber = learner.ULN,
                DateOfBirth = learner.DateOfBirthNullable?.ToString("dd/MM/yyyy"),
                PostcodePriorToEnrollment = learner.PostcodePrior,
                CampusIdentifier = learner.CampId,
                ProviderSpecifiedLearnerMonitoringA = learner.ProviderSpecLearnerMonitorings
                    ?.SingleOrDefault(x => x.ProvSpecLearnMonOccur == "A")?.ProvSpecLearnMon,
                ProviderSpecifiedLearnerMonitoringB = learner.ProviderSpecLearnerMonitorings
                    ?.SingleOrDefault(x => x.ProvSpecLearnMonOccur == "B")?.ProvSpecLearnMon,
                AimSeqNumber = learningDelivery.AimSeqNumber,
                LearningAimReference = learningDelivery.LearnAimRef,
                LearningAimTitle = larsLearningDelivery?.LearningAimTitle,
                SoftwareSupplierAimIdentifier = learningDelivery.SWSupAimId,
                LARS1618FrameworkUplift = fm36DeliveryAttribute?.LearningDeliveryValues
                    ?.LearnDelApplicProv1618FrameworkUplift,
                NotionalNVQLevel = larsLearningDelivery?.NotionalNvqLevel,
                StandardNotionalEndLevel = larsStandard?.NotionalEndLevel,
                Tier2SectorSubjectArea = larsLearningDelivery?.Tier2SectorSubjectArea,
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
                LearningDeliveryFAMTypeLearningSupportFunding = learningDelivery.LearningDeliveryFAMs
                    ?.SingleOrDefault(x => x.LearnDelFAMType == Constants.LearningDeliveryFAMCodeLSF)?.LearnDelFAMCode,
                LearningDeliveryFAMTypeLSFDateAppliesFrom = learningDelivery.LearningDeliveryFAMs
                    ?.SingleOrDefault(x => x.LearnDelFAMType == Constants.LearningDeliveryFAMCodeLSF)
                    ?.LearnDelFAMDateFromNullable?.ToString("dd/MM/yyyy"),
                LearningDeliveryFAMTypeLSFDateAppliesTo = learningDelivery.LearningDeliveryFAMs
                    ?.SingleOrDefault(x => x.LearnDelFAMType == Constants.LearningDeliveryFAMCodeLSF)
                    ?.LearnDelFAMDateToNullable?.ToString("dd/MM/yyyy"),
                LearningDeliveryFAMTypeLearningDeliveryMonitoringA = learningDelivery.LearningDeliveryFAMs
                    ?.SingleOrDefault(x => x.LearnDelFAMType == Constants.LearningDeliveryFAMCodeLDM1)?.LearnDelFAMCode,
                LearningDeliveryFAMTypeLearningDeliveryMonitoringB = learningDelivery.LearningDeliveryFAMs
                    ?.SingleOrDefault(x => x.LearnDelFAMType == Constants.LearningDeliveryFAMCodeLDM2)?.LearnDelFAMCode,
                LearningDeliveryFAMTypeLearningDeliveryMonitoringC = learningDelivery.LearningDeliveryFAMs
                    ?.SingleOrDefault(x => x.LearnDelFAMType == Constants.LearningDeliveryFAMCodeLDM3)?.LearnDelFAMCode,
                LearningDeliveryFAMTypeLearningDeliveryMonitoringD = learningDelivery.LearningDeliveryFAMs
                    ?.SingleOrDefault(x => x.LearnDelFAMType == Constants.LearningDeliveryFAMCodeLDM4)?.LearnDelFAMCode,
                LearningDeliveryFAMRestartIndicator = learningDelivery.LearningDeliveryFAMs
                    ?.SingleOrDefault(x => x.LearnDelFAMType == Constants.LearningDeliveryFAMCodeRES)?.LearnDelFAMCode,
                ProviderSpecifiedDeliveryMonitoringA = learner.ProviderSpecLearnerMonitorings
                    ?.SingleOrDefault(x => x.ProvSpecLearnMonOccur == "A")?.ProvSpecLearnMon,
                ProviderSpecifiedDeliveryMonitoringB = learner.ProviderSpecLearnerMonitorings
                    ?.SingleOrDefault(x => x.ProvSpecLearnMonOccur == "B")?.ProvSpecLearnMon,
                ProviderSpecifiedDeliveryMonitoringC = learner.ProviderSpecLearnerMonitorings
                    ?.SingleOrDefault(x => x.ProvSpecLearnMonOccur == "C")?.ProvSpecLearnMon,
                ProviderSpecifiedDeliveryMonitoringD = learner.ProviderSpecLearnerMonitorings
                    ?.SingleOrDefault(x => x.ProvSpecLearnMonOccur == "D")?.ProvSpecLearnMon,
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
                    ?.SingleOrDefault(x => x.ESMType == Constants.EmploymentStatusMonitoringTypeSEM)?.ESMCode,
                PriceEpisodeStartDate = fm36EpisodeAttribute?.PriceEpisodeValues?.EpisodeStartDate?.ToString("dd/MM/yyyy"),
                PriceEpisodeActualEndDate = fm36EpisodeAttribute?.PriceEpisodeValues?.PriceEpisodeActualEndDate?.ToString("dd/MM/yyyy"),
                FundingLineType = fm36DeliveryAttribute?.LearningDeliveryValues?.LearnDelMathEng ?? false
                    ? fm36DeliveryAttribute?.LearningDeliveryValues?.LearnDelInitialFundLineType
                    : fm36EpisodeAttribute?.PriceEpisodeValues?.PriceEpisodeFundLineType,
                TotalPriceApplicableToThisEpisode =
                    fm36EpisodeAttribute?.PriceEpisodeValues?.PriceEpisodeTotalTNPPrice,
                FundingBandUpperLimit = fm36EpisodeAttribute?.PriceEpisodeValues?.PriceEpisodeUpperBandLimit,
                PriceAmountAboveFundingBandLimit =
                    fm36EpisodeAttribute?.PriceEpisodeValues?.PriceEpisodeUpperLimitAdjustment,
                PriceAmountRemainingStartOfEpisode = fm36EpisodeAttribute?.PriceEpisodeValues
                    ?.PriceEpisodeCappedRemainingTNPAmount,
                CompletionElement = fm36EpisodeAttribute?.PriceEpisodeValues?.PriceEpisodeCompletionElement
            };

            CalculateApprenticeshipContractTypeFields(learningDelivery, model, fm36DeliveryAttribute, fm36EpisodeAttribute, hasPriceEpisodes);

            if (earliestEpisode)
            {
                CalculateAppFinTotals(model, learningDelivery);
            }

            foreach (var command in _commands)
            {
                command.Execute(model, fm36DeliveryAttribute, fm36EpisodeAttribute);
            }

            GetTotals(model);

            return model;
        }

        private void CalculateAppFinTotals(AppsIndicativeEarningsModel model, ILearningDelivery learningDelivery)
        {
            if (learningDelivery.AppFinRecords == null)
            {
                model.TotalPRMPreviousFundingYear = 0;
                model.TotalPRMThisFundingYear = 0;
                return;
            }

            DateTime firstOfAugust = new DateTime(2018, 8, 1);
            DateTime endOfYear = new DateTime(2019, 7, 31, 23, 59, 59);

            List<IAppFinRecord> previousYearData = learningDelivery.AppFinRecords
                .Where(x => x.AFinDate < firstOfAugust && x.AFinType == "PMR").ToList();
            List<IAppFinRecord> currentYearData = learningDelivery.AppFinRecords
                .Where(x => x.AFinDate >= firstOfAugust && x.AFinDate <= endOfYear && x.AFinType == "PMR").ToList();

            model.TotalPRMPreviousFundingYear =
                previousYearData.Where(x => x.AFinCode == 1 || x.AFinCode == 2).Sum(x => x.AFinAmount) -
                previousYearData.Where(x => x.AFinCode == 3).Sum(x => x.AFinAmount);

            model.TotalPRMThisFundingYear =
                currentYearData.Where(x => x.AFinCode == 1 || x.AFinCode == 2).Sum(x => x.AFinAmount) -
                currentYearData.Where(x => x.AFinCode == 3).Sum(x => x.AFinAmount);
        }

        private void CalculateApprenticeshipContractTypeFields(
            ILearningDelivery learningDelivery,
            AppsIndicativeEarningsModel model,
            LearningDelivery fm36DeliveryAttribute,
            PriceEpisode fm36PriceEpisodeAttribute,
            bool hasPriceEpisodes)
        {
            bool useDeliveryAttributeDate =
                fm36DeliveryAttribute?.LearningDeliveryValues?.LearnDelMathEng ?? false ||
                (!(fm36DeliveryAttribute?.LearningDeliveryValues?.LearnDelMathEng ?? false) && !hasPriceEpisodes);

            if (learningDelivery.LearningDeliveryFAMs?.SingleOrDefault(x => x.LearnDelFAMType == Constants.LearningDeliveryFAMCodeACT)
                    ?.LearnDelFAMDateFromNullable == null)
            {
                return;
            }

            var contractAppliesFrom = learningDelivery.LearningDeliveryFAMs
                ?.Where(x => x.LearnDelFAMType == Constants.LearningDeliveryFAMCodeACT &&
                            ((useDeliveryAttributeDate &&
                              learningDelivery.LearnStartDate >= x.LearnDelFAMDateFromNullable) ||
                             (!useDeliveryAttributeDate &&
                              fm36PriceEpisodeAttribute?.PriceEpisodeValues.EpisodeStartDate >=
                              x.LearnDelFAMDateFromNullable)))
                .Max(x => x.LearnDelFAMDateFromNullable);

            var contractAppliesTo = learningDelivery.LearningDeliveryFAMs
                ?.Where(x => x.LearnDelFAMType == Constants.LearningDeliveryFAMCodeACT &&
                            ((useDeliveryAttributeDate &&
                              learningDelivery.LearnStartDate <= x.LearnDelFAMDateToNullable) ||
                             (!useDeliveryAttributeDate &&
                              fm36PriceEpisodeAttribute?.PriceEpisodeValues.EpisodeStartDate <=
                              x.LearnDelFAMDateToNullable)))
                .Min(x => x.LearnDelFAMDateFromNullable);

            model.LearningDeliveryFAMTypeACTDateAppliesFrom = contractAppliesFrom?.ToString("dd/MM/yyyy");
            model.LearningDeliveryFAMTypeACTDateAppliesTo = contractAppliesTo?.ToString("dd/MM/yyyy");

            model.LearningDeliveryFAMTypeApprenticeshipContractType = learningDelivery.LearningDeliveryFAMs
                ?.FirstOrDefault(x => x.LearnDelFAMType == Constants.LearningDeliveryFAMCodeACT &&
                    x.LearnDelFAMDateFromNullable == contractAppliesFrom && x.LearnDelFAMDateToNullable == contractAppliesTo)?.LearnDelFAMCode;
        }

        private void GetTotals(AppsIndicativeEarningsModel model)
        {
            model.TotalOnProgrammeEarnings =
                model.AugustOnProgrammeEarnings ?? 0 +
                model.SeptemberOnProgrammeEarnings ?? 0 +
                model.OctoberOnProgrammeEarnings ?? 0 +
                model.NovemberOnProgrammeEarnings ?? 0 +
                model.DecemberOnProgrammeEarnings ?? 0 +
                model.JanuaryOnProgrammeEarnings ?? 0 +
                model.FebruaryOnProgrammeEarnings ?? 0 +
                model.MarchOnProgrammeEarnings ?? 0 +
                model.AprilOnProgrammeEarnings ?? 0 +
                model.MayOnProgrammeEarnings ?? 0 +
                model.JuneOnProgrammeEarnings ?? 0 +
                model.JulyOnProgrammeEarnings ?? 0;

            model.TotalBalancingPaymentEarnings =
                model.AugustBalancingPaymentEarnings ?? 0 +
                model.SeptemberBalancingPaymentEarnings ?? 0 +
                model.OctoberBalancingPaymentEarnings ?? 0 +
                model.NovemberBalancingPaymentEarnings ?? 0 +
                model.DecemberBalancingPaymentEarnings ?? 0 +
                model.JanuaryBalancingPaymentEarnings ?? 0 +
                model.FebruaryBalancingPaymentEarnings ?? 0 +
                model.MarchBalancingPaymentEarnings ?? 0 +
                model.AprilBalancingPaymentEarnings ?? 0 +
                model.MayBalancingPaymentEarnings ?? 0 +
                model.JuneBalancingPaymentEarnings ?? 0 +
                model.JulyBalancingPaymentEarnings ?? 0;

            model.TotalAimCompletionEarnings =
                model.AugustAimCompletionEarnings ?? 0 +
                model.SeptemberAimCompletionEarnings ?? 0 +
                model.OctoberAimCompletionEarnings ?? 0 +
                model.NovemberAimCompletionEarnings ?? 0 +
                model.DecemberAimCompletionEarnings ?? 0 +
                model.JanuaryAimCompletionEarnings ?? 0 +
                model.FebruaryAimCompletionEarnings ?? 0 +
                model.MarchAimCompletionEarnings ?? 0 +
                model.AprilAimCompletionEarnings ?? 0 +
                model.MayAimCompletionEarnings ?? 0 +
                model.JuneAimCompletionEarnings ?? 0 +
                model.JulyAimCompletionEarnings ?? 0;

            model.TotalLearningSupportEarnings =
                model.AugustLearningSupportEarnings ?? 0 +
                model.SeptemberLearningSupportEarnings ?? 0 +
                model.OctoberLearningSupportEarnings ?? 0 +
                model.NovemberLearningSupportEarnings ?? 0 +
                model.DecemberLearningSupportEarnings ?? 0 +
                model.JanuaryLearningSupportEarnings ?? 0 +
                model.FebruaryLearningSupportEarnings ?? 0 +
                model.MarchLearningSupportEarnings ?? 0 +
                model.AprilLearningSupportEarnings ?? 0 +
                model.MayLearningSupportEarnings ?? 0 +
                model.JuneLearningSupportEarnings ?? 0 +
                model.JulyLearningSupportEarnings ?? 0;

            model.TotalEnglishMathsOnProgrammeEarnings =
                model.AugustEnglishMathsOnProgrammeEarnings ?? 0 +
                model.SeptemberEnglishMathsOnProgrammeEarnings ?? 0 +
                model.OctoberEnglishMathsOnProgrammeEarnings ?? 0 +
                model.NovemberEnglishMathsOnProgrammeEarnings ?? 0 +
                model.DecemberEnglishMathsOnProgrammeEarnings ?? 0 +
                model.JanuaryEnglishMathsOnProgrammeEarnings ?? 0 +
                model.FebruaryEnglishMathsOnProgrammeEarnings ?? 0 +
                model.MarchEnglishMathsOnProgrammeEarnings ?? 0 +
                model.AprilEnglishMathsOnProgrammeEarnings ?? 0 +
                model.MayEnglishMathsOnProgrammeEarnings ?? 0 +
                model.JuneEnglishMathsOnProgrammeEarnings ?? 0 +
                model.JulyEnglishMathsOnProgrammeEarnings ?? 0;

            model.TotalEnglishMathsBalancingPaymentEarnings =
                model.AugustEnglishMathsBalancingPaymentEarnings ?? 0 +
                model.SeptemberEnglishMathsBalancingPaymentEarnings ?? 0 +
                model.OctoberEnglishMathsBalancingPaymentEarnings ?? 0 +
                model.NovemberEnglishMathsBalancingPaymentEarnings ?? 0 +
                model.DecemberEnglishMathsBalancingPaymentEarnings ?? 0 +
                model.JanuaryEnglishMathsBalancingPaymentEarnings ?? 0 +
                model.FebruaryEnglishMathsBalancingPaymentEarnings ?? 0 +
                model.MarchEnglishMathsBalancingPaymentEarnings ?? 0 +
                model.AprilEnglishMathsBalancingPaymentEarnings ?? 0 +
                model.MayEnglishMathsBalancingPaymentEarnings ?? 0 +
                model.JuneEnglishMathsBalancingPaymentEarnings ?? 0 +
                model.JulyEnglishMathsBalancingPaymentEarnings ?? 0;

            model.TotalDisadvantageEarnings =
                model.AugustDisadvantageEarnings ?? 0 +
                model.SeptemberDisadvantageEarnings ?? 0 +
                model.OctoberDisadvantageEarnings ?? 0 +
                model.NovemberDisadvantageEarnings ?? 0 +
                model.DecemberDisadvantageEarnings ?? 0 +
                model.JanuaryDisadvantageEarnings ?? 0 +
                model.FebruaryDisadvantageEarnings ?? 0 +
                model.MarchDisadvantageEarnings ?? 0 +
                model.AprilDisadvantageEarnings ?? 0 +
                model.MayDisadvantageEarnings ?? 0 +
                model.JuneDisadvantageEarnings ?? 0 +
                model.JulyDisadvantageEarnings ?? 0;

            model.Total1618AdditionalPaymentForEmployers =
                model.August1618AdditionalPaymentForEmployers ?? 0 +
                model.September1618AdditionalPaymentForEmployers ?? 0 +
                model.October1618AdditionalPaymentForEmployers ?? 0 +
                model.November1618AdditionalPaymentForEmployers ?? 0 +
                model.December1618AdditionalPaymentForEmployers ?? 0 +
                model.January1618AdditionalPaymentForEmployers ?? 0 +
                model.February1618AdditionalPaymentForEmployers ?? 0 +
                model.March1618AdditionalPaymentForEmployers ?? 0 +
                model.April1618AdditionalPaymentForEmployers ?? 0 +
                model.May1618AdditionalPaymentForEmployers ?? 0 +
                model.June1618AdditionalPaymentForEmployers ?? 0 +
                model.July1618AdditionalPaymentForEmployers ?? 0;

            model.Total1618AdditionalPaymentForProviders =
                model.August1618AdditionalPaymentForProviders ?? 0 +
                model.September1618AdditionalPaymentForProviders ?? 0 +
                model.October1618AdditionalPaymentForProviders ?? 0 +
                model.November1618AdditionalPaymentForProviders ?? 0 +
                model.December1618AdditionalPaymentForProviders ?? 0 +
                model.January1618AdditionalPaymentForProviders ?? 0 +
                model.February1618AdditionalPaymentForProviders ?? 0 +
                model.March1618AdditionalPaymentForProviders ?? 0 +
                model.April1618AdditionalPaymentForProviders ?? 0 +
                model.May1618AdditionalPaymentForProviders ?? 0 +
                model.June1618AdditionalPaymentForProviders ?? 0 +
                model.July1618AdditionalPaymentForProviders ?? 0;

            model.TotalAdditionalPaymentsForApprentices =
                model.AugustAdditionalPaymentsForApprentices ?? 0 +
                model.SeptemberAdditionalPaymentsForApprentices ?? 0 +
                model.OctoberAdditionalPaymentsForApprentices ?? 0 +
                model.NovemberAdditionalPaymentsForApprentices ?? 0 +
                model.DecemberAdditionalPaymentsForApprentices ?? 0 +
                model.JanuaryAdditionalPaymentsForApprentices ?? 0 +
                model.FebruaryAdditionalPaymentsForApprentices ?? 0 +
                model.MarchAdditionalPaymentsForApprentices ?? 0 +
                model.AprilAdditionalPaymentsForApprentices ?? 0 +
                model.MayAdditionalPaymentsForApprentices ?? 0 +
                model.JuneAdditionalPaymentsForApprentices ?? 0 +
                model.JulyAdditionalPaymentsForApprentices ?? 0;

            model.Total1618FrameworkUpliftOnProgrammePayment =
                model.August1618FrameworkUpliftOnProgrammePayment ?? 0 +
                model.September1618FrameworkUpliftOnProgrammePayment ?? 0 +
                model.October1618FrameworkUpliftOnProgrammePayment ?? 0 +
                model.November1618FrameworkUpliftOnProgrammePayment ?? 0 +
                model.December1618FrameworkUpliftOnProgrammePayment ?? 0 +
                model.January1618FrameworkUpliftOnProgrammePayment ?? 0 +
                model.February1618FrameworkUpliftOnProgrammePayment ?? 0 +
                model.March1618FrameworkUpliftOnProgrammePayment ?? 0 +
                model.April1618FrameworkUpliftOnProgrammePayment ?? 0 +
                model.May1618FrameworkUpliftOnProgrammePayment ?? 0 +
                model.June1618FrameworkUpliftOnProgrammePayment ?? 0 +
                model.July1618FrameworkUpliftOnProgrammePayment ?? 0;

            model.Total1618FrameworkUpliftBalancingPayment =
                model.August1618FrameworkUpliftBalancingPayment ?? 0 +
                model.September1618FrameworkUpliftBalancingPayment ?? 0 +
                model.October1618FrameworkUpliftBalancingPayment ?? 0 +
                model.November1618FrameworkUpliftBalancingPayment ?? 0 +
                model.December1618FrameworkUpliftBalancingPayment ?? 0 +
                model.January1618FrameworkUpliftBalancingPayment ?? 0 +
                model.February1618FrameworkUpliftBalancingPayment ?? 0 +
                model.March1618FrameworkUpliftBalancingPayment ?? 0 +
                model.April1618FrameworkUpliftBalancingPayment ?? 0 +
                model.May1618FrameworkUpliftBalancingPayment ?? 0 +
                model.June1618FrameworkUpliftBalancingPayment ?? 0 +
                model.July1618FrameworkUpliftBalancingPayment ?? 0;

            model.Total1618FrameworkUpliftCompletionPayment =
                model.August1618FrameworkUpliftCompletionPayment ?? 0 +
                model.September1618FrameworkUpliftCompletionPayment ?? 0 +
                model.October1618FrameworkUpliftCompletionPayment ?? 0 +
                model.November1618FrameworkUpliftCompletionPayment ?? 0 +
                model.December1618FrameworkUpliftCompletionPayment ?? 0 +
                model.January1618FrameworkUpliftCompletionPayment ?? 0 +
                model.February1618FrameworkUpliftCompletionPayment ?? 0 +
                model.March1618FrameworkUpliftCompletionPayment ?? 0 +
                model.April1618FrameworkUpliftCompletionPayment ?? 0 +
                model.May1618FrameworkUpliftCompletionPayment ?? 0 +
                model.June1618FrameworkUpliftCompletionPayment ?? 0 +
                model.July1618FrameworkUpliftCompletionPayment ?? 0;
        }
    }
}