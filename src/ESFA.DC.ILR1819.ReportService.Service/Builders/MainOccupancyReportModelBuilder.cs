using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output;

namespace ESFA.DC.ILR1819.ReportService.Service.Builders
{
    using System.Linq;
    using ESFA.DC.ILR.FundingService.FM25.Model.Output;
    using ESFA.DC.ILR.Model.Interface;
    using ESFA.DC.ILR1819.ReportService.Interface.Service;
    using Model.Lars;
    using Model.ReportModels;

    public class MainOccupancyReportModelBuilder : IMainOccupancyReportModelBuilder
    {
        public MainOccupancyFM35Model BuildFm35Model(
            ILearner learner,
            ILearningDelivery learningDelivery,
            LarsLearningDelivery larsModel,
            LearningDelivery frameworkAim,
            ILR.FundingService.FM35.FundingOutput.Model.Output.LearningDelivery fm35Data)
        {
            var onProgPayment = fm35Data?.LearningDeliveryPeriodisedValues
                ?.SingleOrDefault(attr => attr.AttributeName == Constants.Fm35OnProgrammeAttributeName);
            var balancePayment = fm35Data?.LearningDeliveryPeriodisedValues
                ?.SingleOrDefault(attr => attr.AttributeName == Constants.Fm35BalancingAttributeName);
            var achievePayment = fm35Data?.LearningDeliveryPeriodisedValues
                ?.SingleOrDefault(attr => attr.AttributeName == Constants.Fm35AimAchievementAttributeName);
            var empOutcomePayment = fm35Data?.LearningDeliveryPeriodisedValues
                ?.SingleOrDefault(attr => attr.AttributeName == Constants.Fm35JobOutcomeAchievementAttributeName);
            var learnSuppFundCash = fm35Data?.LearningDeliveryPeriodisedValues
                ?.SingleOrDefault(attr => attr.AttributeName == Constants.Fm35LearningSupportAttributeName);

            var totalOnProgPayment = (onProgPayment?.Period1 ?? 0) + (onProgPayment?.Period2 ?? 0) +
                                     (onProgPayment?.Period3 ?? 0) + (onProgPayment?.Period4 ?? 0) +
                                     (onProgPayment?.Period5 ?? 0) + (onProgPayment?.Period6 ?? 0) +
                                     (onProgPayment?.Period7 ?? 0) + (onProgPayment?.Period8 ?? 0) +
                                     (onProgPayment?.Period9 ?? 0) + (onProgPayment?.Period10 ?? 0) +
                                     (onProgPayment?.Period11 ?? 0) + (onProgPayment?.Period12 ?? 0);

            var totalBalancePayment = (balancePayment?.Period1 ?? 0) + (balancePayment?.Period2 ?? 0) +
                                      (balancePayment?.Period3 ?? 0) + (balancePayment?.Period4 ?? 0) +
                                      (balancePayment?.Period5 ?? 0) + (balancePayment?.Period6 ?? 0) +
                                      (balancePayment?.Period7 ?? 0) + (balancePayment?.Period8 ?? 0) +
                                      (balancePayment?.Period9 ?? 0) + (balancePayment?.Period10 ?? 0) +
                                      (balancePayment?.Period11 ?? 0) + (balancePayment?.Period12 ?? 0);

            var totalAchievePayment = (achievePayment?.Period1 ?? 0) + (achievePayment?.Period2 ?? 0) +
                                      (achievePayment?.Period3 ?? 0) + (achievePayment?.Period4 ?? 0) +
                                      (achievePayment?.Period5 ?? 0) + (achievePayment?.Period6 ?? 0) +
                                      (achievePayment?.Period7 ?? 0) + (achievePayment?.Period8 ?? 0) +
                                      (achievePayment?.Period9 ?? 0) + (achievePayment?.Period10 ?? 0) +
                                      (achievePayment?.Period11 ?? 0) + (achievePayment?.Period12 ?? 0);

            var totalEmpOutcomePay = (empOutcomePayment?.Period1 ?? 0) + (empOutcomePayment?.Period2 ?? 0) +
                                     (empOutcomePayment?.Period3 ?? 0) + (empOutcomePayment?.Period4 ?? 0) +
                                     (empOutcomePayment?.Period5 ?? 0) + (empOutcomePayment?.Period6 ?? 0) +
                                     (empOutcomePayment?.Period7 ?? 0) + (empOutcomePayment?.Period8 ?? 0) +
                                     (empOutcomePayment?.Period9 ?? 0) + (empOutcomePayment?.Period10 ?? 0) +
                                     (empOutcomePayment?.Period11 ?? 0) + (empOutcomePayment?.Period12 ?? 0);

            var totalLearnSuppFundCash = (learnSuppFundCash?.Period1 ?? 0) + (learnSuppFundCash?.Period2 ?? 0) +
                                         (learnSuppFundCash?.Period3 ?? 0) + (learnSuppFundCash?.Period4 ?? 0) +
                                         (learnSuppFundCash?.Period5 ?? 0) + (learnSuppFundCash?.Period6 ?? 0) +
                                         (learnSuppFundCash?.Period7 ?? 0) + (learnSuppFundCash?.Period8 ?? 0) +
                                         (learnSuppFundCash?.Period9 ?? 0) + (learnSuppFundCash?.Period10 ?? 0) +
                                         (learnSuppFundCash?.Period11 ?? 0) + (learnSuppFundCash?.Period12 ?? 0);

            var aimPercent = fm35Data?.LearningDeliveryPeriodisedValues?.SingleOrDefault(attr =>
                attr.AttributeName == Constants.Fm35AimAchievementPercentAttributeName);

            return new MainOccupancyFM35Model
            {
                LearnRefNumber = learner.LearnRefNumber,
                Uln = learner.ULN,
                DateOfBirth = learner.DateOfBirthNullable?.ToString("dd/MM/yyyy"),
                PostcodePrior = learner.PostcodePrior,
                PmUkprn = learner.PMUKPRNNullable,
                CampId = learner.CampId,
                ProvSpecLearnMonA = learner.ProviderSpecLearnerMonitorings
                    ?.SingleOrDefault(x => x.ProvSpecLearnMonOccur == "A")?.ProvSpecLearnMon,
                ProvSpecLearnMonB = learner.ProviderSpecLearnerMonitorings
                    ?.SingleOrDefault(x => x.ProvSpecLearnMonOccur == "B")?.ProvSpecLearnMon,
                AimSeqNumber = learningDelivery.AimSeqNumber,
                LearnAimRef = learningDelivery.LearnAimRef,
                LearnAimRefTitle = larsModel.LearningAimTitle,
                SwSupAimId = learningDelivery.SWSupAimId,
                WeightedRateFromEsol = fm35Data?.LearningDeliveryValue?.WeightedRateFromESOL,
                ApplicWeightFundRate = fm35Data?.LearningDeliveryValue?.ApplicWeightFundRate,
                ApplicProgWeightFact = fm35Data?.LearningDeliveryValue?.ApplicProgWeightFact,
                AimValue = fm35Data?.LearningDeliveryValue?.AimValue,
                NotionalNvqLevelV2 = larsModel.NotionalNvqLevel,
                SectorSubjectAreaTier2 = larsModel.Tier2SectorSubjectArea,
                ProgType = learningDelivery.ProgTypeNullable,
                FworkCode = learningDelivery.FworkCodeNullable,
                PwayCode = learningDelivery.PwayCodeNullable,
                AimType = learningDelivery.AimType,
                FrameworkComponentType = frameworkAim.FrameworkComponentType,
                FundModel = learningDelivery.FundModel,
                PriorLearnFundAdj = learningDelivery.PriorLearnFundAdjNullable,
                OtherFundAdj = learningDelivery.OtherFundAdjNullable,
                OrigLearnStartDate = learningDelivery.OrigLearnStartDateNullable,
                LearnStartDate = learningDelivery.LearnStartDate,
                LearnPlanEndDate = learningDelivery.LearnPlanEndDate,
                CompStatus = learningDelivery.CompStatus,
                LearnActEndDate = learningDelivery.LearnActEndDateNullable,
                Outcome = learningDelivery.OutcomeNullable,
                AchDate = learningDelivery.AchDateNullable,
                AddHours = learningDelivery.AddHoursNullable,
                LearnDelFamCodeSof = learningDelivery.LearningDeliveryFAMs
                    ?.SingleOrDefault(x => x.LearnDelFAMType == "SOF")?.LearnDelFAMCode,
                LearnDelFamCodeFfi = learningDelivery.LearningDeliveryFAMs
                    ?.SingleOrDefault(x => x.LearnDelFAMType == "FFI")?.LearnDelFAMCode,
                LearnDelFamCodeEef = learningDelivery.LearningDeliveryFAMs
                    ?.SingleOrDefault(x => x.LearnDelFAMType == "EEF")?.LearnDelFAMCode,
                LearnDelFamCodeLsfHighest = learningDelivery.LearningDeliveryFAMs
                    ?.Where(x => x.LearnDelFAMType == "LSF").Max(x => x.LearnDelFAMCode),
                LearnDelFamCodeLsfEarliest = learningDelivery.LearningDeliveryFAMs
                    ?.Where(x => x.LearnDelFAMType == "LSF").Min(x => x.LearnDelFAMDateFromNullable)
                    ?.ToString("dd/MM/yyyy"),
                LearnDelFamCodeLsfLatest = learningDelivery.LearningDeliveryFAMs
                    ?.Where(x => x.LearnDelFAMType == "LSF")
                    .Min(x => x.LearnDelFAMDateToNullable)?.ToString("dd/MM/yyyy"),
                LearnDelFamCodeLdm1 = learningDelivery.LearningDeliveryFAMs
                    ?.SingleOrDefault(x => x.LearnDelFAMType == "LDM1")?.LearnDelFAMCode,
                LearnDelFamCodeLdm2 = learningDelivery.LearningDeliveryFAMs
                    ?.SingleOrDefault(x => x.LearnDelFAMType == "LDM2")?.LearnDelFAMCode,
                LearnDelFamCodeLdm3 = learningDelivery.LearningDeliveryFAMs
                    ?.SingleOrDefault(x => x.LearnDelFAMType == "LDM3")?.LearnDelFAMCode,
                LearnDelFamCodeLdm4 = learningDelivery.LearningDeliveryFAMs
                    ?.SingleOrDefault(x => x.LearnDelFAMType == "LDM4")?.LearnDelFAMCode,
                LearnDelFamCodeRes = learningDelivery.LearningDeliveryFAMs
                    ?.SingleOrDefault(x => x.LearnDelFAMType == "RES")?.LearnDelFAMCode,
                ProvSpecDelMonA = learningDelivery.ProviderSpecDeliveryMonitorings
                    ?.SingleOrDefault(x => x.ProvSpecDelMonOccur == "A")?.ProvSpecDelMon,
                ProvSpecDelMonB = learningDelivery.ProviderSpecDeliveryMonitorings
                    ?.SingleOrDefault(x => x.ProvSpecDelMonOccur == "B")?.ProvSpecDelMon,
                ProvSpecDelMonC = learningDelivery.ProviderSpecDeliveryMonitorings
                    ?.SingleOrDefault(x => x.ProvSpecDelMonOccur == "C")?.ProvSpecDelMon,
                ProvSpecDelMonD = learningDelivery.ProviderSpecDeliveryMonitorings
                    ?.SingleOrDefault(x => x.ProvSpecDelMonOccur == "D")?.ProvSpecDelMon,
                FundLine = fm35Data?.LearningDeliveryValue?.FundLine,
                PlannedNumOnProgInstalm = fm35Data?.LearningDeliveryValue?.PlannedNumOnProgInstalm,
                PlannedNumOnProgInstalmTrans = fm35Data?.LearningDeliveryValue?.PlannedNumOnProgInstalmTrans,
                StartPropTrans = fm35Data?.LearningDeliveryValue?.StartPropTrans,
                AchieveElement = fm35Data?.LearningDeliveryValue?.AchieveElement,
                AchievePercentage = aimPercent == null ? 0 : GetMaxPeriod(aimPercent),
                NonGovCont = fm35Data?.LearningDeliveryValue?.NonGovCont,
                PartnerUkprn = learningDelivery.PartnerUKPRNNullable,
                DelLocPostCode = learningDelivery.DelLocPostCode,
                AreaCostFactAdj = fm35Data?.LearningDeliveryValue?.AreaCostFactAdj,
                DisUpFactAdj = fm35Data?.LearningDeliveryValue?.DisUpFactAdj,
                LargeEmployerID = fm35Data?.LearningDeliveryValue?.LargeEmployerID,
                LargeEmployerFM35Fctr = fm35Data?.LearningDeliveryValue?.LargeEmployerFM35Fctr,
                CapFactor = fm35Data?.LearningDeliveryValue?.CapFactor,
                TraineeWorkPlacement = (fm35Data?.LearningDeliveryValue?.TrnWorkPlaceAim ?? false)
                                       || (fm35Data?.LearningDeliveryValue?.TrnWorkPrepAim ?? false),
                HigherApprentishipHeAim = fm35Data?.LearningDeliveryValue?.PrscHEAim ?? false,
                ApplicEmpFactDate = fm35Data?.LearningDeliveryValue?.ApplicEmpFactDate,
                ApplicFactDate = fm35Data?.LearningDeliveryValue?.ApplicFactDate,

                Period1OnProgPayment = onProgPayment?.Period1,
                Period1BalancePayment = balancePayment?.Period1,
                Period1AchievePayment = achievePayment?.Period1,
                Period1EmpOutcomePay = empOutcomePayment?.Period1,
                Period1LearnSuppFundCash = learnSuppFundCash?.Period1,

                Period2OnProgPayment = onProgPayment?.Period2,
                Period2BalancePayment = balancePayment?.Period2,
                Period2AchievePayment = achievePayment?.Period2,
                Period2EmpOutcomePay = empOutcomePayment?.Period2,
                Period2LearnSuppFundCash = learnSuppFundCash?.Period2,

                Period3OnProgPayment = onProgPayment?.Period3,
                Period3BalancePayment = balancePayment?.Period3,
                Period3AchievePayment = achievePayment?.Period3,
                Period3EmpOutcomePay = empOutcomePayment?.Period3,
                Period3LearnSuppFundCash = learnSuppFundCash?.Period3,

                Period4OnProgPayment = onProgPayment?.Period4,
                Period4BalancePayment = balancePayment?.Period4,
                Period4AchievePayment = achievePayment?.Period4,
                Period4EmpOutcomePay = empOutcomePayment?.Period4,
                Period4LearnSuppFundCash = learnSuppFundCash?.Period4,

                Period5OnProgPayment = onProgPayment?.Period5,
                Period5BalancePayment = balancePayment?.Period5,
                Period5AchievePayment = achievePayment?.Period5,
                Period5EmpOutcomePay = empOutcomePayment?.Period5,
                Period5LearnSuppFundCash = learnSuppFundCash?.Period5,

                Period6OnProgPayment = onProgPayment?.Period6,
                Period6BalancePayment = balancePayment?.Period6,
                Period6AchievePayment = achievePayment?.Period6,
                Period6EmpOutcomePay = empOutcomePayment?.Period6,
                Period6LearnSuppFundCash = learnSuppFundCash?.Period6,

                Period7OnProgPayment = onProgPayment?.Period7,
                Period7BalancePayment = balancePayment?.Period7,
                Period7AchievePayment = achievePayment?.Period7,
                Period7EmpOutcomePay = empOutcomePayment?.Period7,
                Period7LearnSuppFundCash = learnSuppFundCash?.Period7,

                Period8OnProgPayment = onProgPayment?.Period8,
                Period8BalancePayment = balancePayment?.Period8,
                Period8AchievePayment = achievePayment?.Period8,
                Period8EmpOutcomePay = empOutcomePayment?.Period8,
                Period8LearnSuppFundCash = learnSuppFundCash?.Period8,

                Period9OnProgPayment = onProgPayment?.Period9,
                Period9BalancePayment = balancePayment?.Period9,
                Period9AchievePayment = achievePayment?.Period9,
                Period9EmpOutcomePay = empOutcomePayment?.Period9,
                Period9LearnSuppFundCash = learnSuppFundCash?.Period9,

                Period10OnProgPayment = onProgPayment?.Period10,
                Period10BalancePayment = balancePayment?.Period10,
                Period10AchievePayment = achievePayment?.Period10,
                Period10EmpOutcomePay = empOutcomePayment?.Period10,
                Period10LearnSuppFundCash = learnSuppFundCash?.Period10,

                Period11OnProgPayment = onProgPayment?.Period11,
                Period11BalancePayment = balancePayment?.Period11,
                Period11AchievePayment = achievePayment?.Period11,
                Period11EmpOutcomePay = empOutcomePayment?.Period11,
                Period11LearnSuppFundCash = learnSuppFundCash?.Period11,

                Period12OnProgPayment = onProgPayment?.Period12,
                Period12BalancePayment = balancePayment?.Period12,
                Period12AchievePayment = achievePayment?.Period12,
                Period12EmpOutcomePay = empOutcomePayment?.Period12,
                Period12LearnSuppFundCash = learnSuppFundCash?.Period12,

                TotalOnProgPayment = totalOnProgPayment,
                TotalBalancePayment = totalBalancePayment,
                TotalAchievePayment = totalAchievePayment,
                TotalEmpOutcomePay = totalEmpOutcomePay,
                TotalLearnSuppFundCash = totalLearnSuppFundCash,
                TotalEarnedCash = totalOnProgPayment + totalBalancePayment + totalAchievePayment
                                  + totalEmpOutcomePay + totalLearnSuppFundCash
            };
        }

        public MainOccupancyFM25Model BuildFm25Model(
            ILearner learner,
            ILearningDelivery learningDelivery,
            FM25Learner fm25Data)
        {
            var onProgPayment = fm25Data?.LearnerPeriodisedValues?.SingleOrDefault(attr =>
                attr.AttributeName == Constants.Fm25OnProgrammeAttributeName);

            var onProgPaymentTotal = onProgPayment?.Period1
                                     + onProgPayment?.Period2
                                     + onProgPayment?.Period3
                                     + onProgPayment?.Period4
                                     + onProgPayment?.Period5
                                     + onProgPayment?.Period6
                                     + onProgPayment?.Period7
                                     + onProgPayment?.Period8
                                     + onProgPayment?.Period9
                                     + onProgPayment?.Period10
                                     + onProgPayment?.Period11
                                     + onProgPayment?.Period12;

            return new MainOccupancyFM25Model
            {
                LearnRefNumber = learner.LearnRefNumber,
                Uln = learner.ULN,
                DateOfBirth = learner.DateOfBirthNullable?.ToString("dd/MM/yyyy"),
                PostcodePrior = learner.PostcodePrior,
                PmUkprn = learner.PMUKPRNNullable,
                CampId = learner.CampId,
                ProvSpecLearnMonA = learner.ProviderSpecLearnerMonitorings
                    ?.SingleOrDefault(x => x.ProvSpecLearnMonOccur == "A")?.ProvSpecLearnMon,
                ProvSpecLearnMonB = learner.ProviderSpecLearnerMonitorings
                    ?.SingleOrDefault(x => x.ProvSpecLearnMonOccur == "B")?.ProvSpecLearnMon,
                NatRate = fm25Data?.NatRate,
                FundModel = learningDelivery.FundModel,
                LearnerStartDate = fm25Data?.LearnerStartDate,
                LearnerPlanEndDate = fm25Data?.LearnerPlanEndDate,
                LearnerActEndDate = fm25Data?.LearnerActEndDate,
                FundLine = fm25Data?.FundLine,
                Period1OnProgPayment = onProgPayment?.Period1,
                Period2OnProgPayment = onProgPayment?.Period2,
                Period3OnProgPayment = onProgPayment?.Period3,
                Period4OnProgPayment = onProgPayment?.Period4,
                Period5OnProgPayment = onProgPayment?.Period5,
                Period6OnProgPayment = onProgPayment?.Period6,
                Period7OnProgPayment = onProgPayment?.Period7,
                Period8OnProgPayment = onProgPayment?.Period8,
                Period9OnProgPayment = onProgPayment?.Period9,
                Period10OnProgPayment = onProgPayment?.Period10,
                Period11OnProgPayment = onProgPayment?.Period11,
                Period12OnProgPayment = onProgPayment?.Period12,
                PeriodOnProgPaymentTotal = onProgPaymentTotal,
                Total = onProgPaymentTotal
            };
        }

        private static decimal GetMaxPeriod(LearningDeliveryPeriodisedValue learningDeliveryPeriodisedAttribute)
        {
            decimal max = int.MinValue;
            if (learningDeliveryPeriodisedAttribute.Period1 > max)
            {
                max = learningDeliveryPeriodisedAttribute.Period1 ?? 0;
            }

            if (learningDeliveryPeriodisedAttribute.Period2 > max)
            {
                max = learningDeliveryPeriodisedAttribute.Period2 ?? 0;
            }

            if (learningDeliveryPeriodisedAttribute.Period3 > max)
            {
                max = learningDeliveryPeriodisedAttribute.Period3 ?? 0;
            }

            if (learningDeliveryPeriodisedAttribute.Period4 > max)
            {
                max = learningDeliveryPeriodisedAttribute.Period4 ?? 0;
            }

            if (learningDeliveryPeriodisedAttribute.Period5 > max)
            {
                max = learningDeliveryPeriodisedAttribute.Period5 ?? 0;
            }

            if (learningDeliveryPeriodisedAttribute.Period6 > max)
            {
                max = learningDeliveryPeriodisedAttribute.Period6 ?? 0;
            }

            if (learningDeliveryPeriodisedAttribute.Period7 > max)
            {
                max = learningDeliveryPeriodisedAttribute.Period7 ?? 0;
            }

            if (learningDeliveryPeriodisedAttribute.Period8 > max)
            {
                max = learningDeliveryPeriodisedAttribute.Period8 ?? 0;
            }

            if (learningDeliveryPeriodisedAttribute.Period9 > max)
            {
                max = learningDeliveryPeriodisedAttribute.Period9 ?? 0;
            }

            if (learningDeliveryPeriodisedAttribute.Period10 > max)
            {
                max = learningDeliveryPeriodisedAttribute.Period10 ?? 0;
            }

            if (learningDeliveryPeriodisedAttribute.Period11 > max)
            {
                max = learningDeliveryPeriodisedAttribute.Period11 ?? 0;
            }

            if (learningDeliveryPeriodisedAttribute.Period12 > max)
            {
                max = learningDeliveryPeriodisedAttribute.Period12 ?? 0;
            }

            return max;
        }
    }
}
