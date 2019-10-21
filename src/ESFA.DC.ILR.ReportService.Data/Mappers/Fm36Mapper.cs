using System.Linq;
using ESFA.DC.ILR.ReportService.Data.Interface.Mappers;
using ESFA.DC.ILR.ReportService.Models.Fm36;

namespace ESFA.DC.ILR.ReportService.Data.Mappers
{
    public class Fm36Mapper : IMapper<FundingService.FM36.FundingOutput.Model.Output.FM36Global, FM36Global>
    {
        public FM36Global MapData(FundingService.FM36.FundingOutput.Model.Output.FM36Global fm36Global)
        {
            return MapFm36Global(fm36Global);
        }

        private FM36Global MapFm36Global(FundingService.FM36.FundingOutput.Model.Output.FM36Global fm36Global)
        {
            return new FM36Global()
            {
                UKPRN = fm36Global.UKPRN,
                LARSVersion = fm36Global.LARSVersion,
                RulebaseVersion = fm36Global.RulebaseVersion,
                Year = fm36Global.Year,
                Learners = fm36Global.Learners?.Select(MapFm36Learner).ToList()
            };
        }

        private FM36Learner MapFm36Learner(FundingService.FM36.FundingOutput.Model.Output.FM36Learner fm36Learner)
        {
            return new FM36Learner()
            {
                LearnRefNumber = fm36Learner.LearnRefNumber,
                ULN = fm36Learner.ULN,
                PriceEpisodes = fm36Learner.PriceEpisodes?.Select(MapPriceEpisode).ToList(),
                LearningDeliveries = fm36Learner.LearningDeliveries?.Select(MapFm36LearningDelivery).ToList(),
                HistoricEarningOutputValues = fm36Learner.HistoricEarningOutputValues?.Select(MapHistoricEarningsOutputValues).ToList()
            };
        }

        private PriceEpisode MapPriceEpisode(FundingService.FM36.FundingOutput.Model.Output.PriceEpisode priceEpisode)
        {
            return new PriceEpisode()
            {
                PriceEpisodeIdentifier = priceEpisode.PriceEpisodeIdentifier,
                PriceEpisodeValues = MapPriceEpisodeValues(priceEpisode.PriceEpisodeValues),
                PriceEpisodePeriodisedValues = priceEpisode.PriceEpisodePeriodisedValues.Select(MapPriceEpisodePeriodisedValues).ToList()
            };
        }

        private PriceEpisodeValues MapPriceEpisodeValues(FundingService.FM36.FundingOutput.Model.Output.PriceEpisodeValues priceEpisodeValues)
        {
            return new PriceEpisodeValues()
            {
                EpisodeStartDate = priceEpisodeValues.EpisodeStartDate,
                TNP1 = priceEpisodeValues.TNP1,
                TNP2 = priceEpisodeValues.TNP2,
                TNP3 = priceEpisodeValues.TNP3,
                TNP4 = priceEpisodeValues.TNP4,
                PriceEpisodeActualEndDateIncEPA = priceEpisodeValues.PriceEpisodeActualEndDateIncEPA,
                PriceEpisode1618FUBalValue = priceEpisodeValues.PriceEpisode1618FUBalValue,
                PriceEpisodeApplic1618FrameworkUpliftCompElement = priceEpisodeValues.PriceEpisodeApplic1618FrameworkUpliftCompElement,
                PriceEpisode1618FrameworkUpliftTotPrevEarnings = priceEpisodeValues.PriceEpisode1618FrameworkUpliftTotPrevEarnings,
                PriceEpisode1618FrameworkUpliftRemainingAmount = priceEpisodeValues.PriceEpisode1618FrameworkUpliftRemainingAmount,
                PriceEpisode1618FUMonthInstValue = priceEpisodeValues.PriceEpisode1618FUMonthInstValue,
                PriceEpisode1618FUTotEarnings = priceEpisodeValues.PriceEpisode1618FUTotEarnings,
                PriceEpisodeUpperBandLimit = priceEpisodeValues.PriceEpisodeUpperBandLimit,
                PriceEpisodePlannedEndDate = priceEpisodeValues.PriceEpisodePlannedEndDate,
                PriceEpisodeActualEndDate = priceEpisodeValues.PriceEpisodeActualEndDate,
                PriceEpisodeTotalTNPPrice = priceEpisodeValues.PriceEpisodeTotalTNPPrice,
                PriceEpisodeUpperLimitAdjustment = priceEpisodeValues.PriceEpisodeUpperLimitAdjustment,
                PriceEpisodePlannedInstalments = priceEpisodeValues.PriceEpisodePlannedInstalments,
                PriceEpisodeActualInstalments = priceEpisodeValues.PriceEpisodeActualInstalments,
                PriceEpisodeInstalmentsThisPeriod = priceEpisodeValues.PriceEpisodeInstalmentsThisPeriod,
                PriceEpisodeCompletionElement = priceEpisodeValues.PriceEpisodeCompletionElement,
                PriceEpisodePreviousEarnings = priceEpisodeValues.PriceEpisodePreviousEarnings,
                PriceEpisodeInstalmentValue = priceEpisodeValues.PriceEpisodeInstalmentValue,
                PriceEpisodeOnProgPayment = priceEpisodeValues.PriceEpisodeOnProgPayment,
                PriceEpisodeTotalEarnings = priceEpisodeValues.PriceEpisodeTotalEarnings,
                PriceEpisodeBalanceValue = priceEpisodeValues.PriceEpisodeBalanceValue,
                PriceEpisodeBalancePayment = priceEpisodeValues.PriceEpisodeBalancePayment,
                PriceEpisodeCompleted = priceEpisodeValues.PriceEpisodeCompleted,
                PriceEpisodeCompletionPayment = priceEpisodeValues.PriceEpisodeCompletionPayment,
                PriceEpisodeRemainingTNPAmount = priceEpisodeValues.PriceEpisodeRemainingTNPAmount,
                PriceEpisodeRemainingAmountWithinUpperLimit = priceEpisodeValues.PriceEpisodeRemainingAmountWithinUpperLimit,
                PriceEpisodeCappedRemainingTNPAmount = priceEpisodeValues.PriceEpisodeCappedRemainingTNPAmount,
                PriceEpisodeExpectedTotalMonthlyValue = priceEpisodeValues.PriceEpisodeExpectedTotalMonthlyValue,
                PriceEpisodeAimSeqNumber = priceEpisodeValues.PriceEpisodeAimSeqNumber,
                PriceEpisodeFirstDisadvantagePayment = priceEpisodeValues.PriceEpisodeFirstDisadvantagePayment,
                PriceEpisodeSecondDisadvantagePayment = priceEpisodeValues.PriceEpisodeSecondDisadvantagePayment,
                PriceEpisodeApplic1618FrameworkUpliftBalancing = priceEpisodeValues.PriceEpisodeApplic1618FrameworkUpliftBalancing,
                PriceEpisodeApplic1618FrameworkUpliftCompletionPayment = priceEpisodeValues.PriceEpisodeApplic1618FrameworkUpliftCompletionPayment,
                PriceEpisodeApplic1618FrameworkUpliftOnProgPayment = priceEpisodeValues.PriceEpisodeApplic1618FrameworkUpliftOnProgPayment,
                PriceEpisodeSecondProv1618Pay = priceEpisodeValues.PriceEpisodeSecondProv1618Pay,
                PriceEpisodeFirstEmp1618Pay = priceEpisodeValues.PriceEpisodeFirstEmp1618Pay,
                PriceEpisodeSecondEmp1618Pay = priceEpisodeValues.PriceEpisodeSecondEmp1618Pay,
                PriceEpisodeFirstProv1618Pay = priceEpisodeValues.PriceEpisodeFirstProv1618Pay,
                PriceEpisodeLSFCash = priceEpisodeValues.PriceEpisodeLSFCash,
                PriceEpisodeFundLineType = priceEpisodeValues.PriceEpisodeFundLineType,
                PriceEpisodeSFAContribPct = priceEpisodeValues.PriceEpisodeSFAContribPct,
                PriceEpisodeLevyNonPayInd = priceEpisodeValues.PriceEpisodeLevyNonPayInd,
                EpisodeEffectiveTNPStartDate = priceEpisodeValues.EpisodeEffectiveTNPStartDate,
                PriceEpisodeFirstAdditionalPaymentThresholdDate = priceEpisodeValues.PriceEpisodeFirstAdditionalPaymentThresholdDate,
                PriceEpisodeSecondAdditionalPaymentThresholdDate = priceEpisodeValues.PriceEpisodeSecondAdditionalPaymentThresholdDate,
                PriceEpisodeContractType = priceEpisodeValues.PriceEpisodeContractType,
                PriceEpisodePreviousEarningsSameProvider = priceEpisodeValues.PriceEpisodePreviousEarningsSameProvider,
                PriceEpisodeTotProgFunding = priceEpisodeValues.PriceEpisodeTotProgFunding,
                PriceEpisodeProgFundIndMinCoInvest = priceEpisodeValues.PriceEpisodeProgFundIndMinCoInvest,
                PriceEpisodeProgFundIndMaxEmpCont = priceEpisodeValues.PriceEpisodeProgFundIndMaxEmpCont,
                PriceEpisodeTotalPMRs = priceEpisodeValues.PriceEpisodeTotalPMRs,
                PriceEpisodeCumulativePMRs = priceEpisodeValues.PriceEpisodeCumulativePMRs,
                PriceEpisodeCompExemCode = priceEpisodeValues.PriceEpisodeCompExemCode,
                PriceEpisodeLearnerAdditionalPaymentThresholdDate = priceEpisodeValues.PriceEpisodeLearnerAdditionalPaymentThresholdDate,
                PriceEpisodeAgreeId = priceEpisodeValues.PriceEpisodeAgreeId,
                PriceEpisodeRedStartDate = priceEpisodeValues.PriceEpisodeRedStartDate,
                PriceEpisodeRedStatusCode = priceEpisodeValues.PriceEpisodeRedStatusCode,
            };
        }

        private PriceEpisodePeriodisedValues MapPriceEpisodePeriodisedValues(FundingService.FM36.FundingOutput.Model.Output.PriceEpisodePeriodisedValues priceEpisodePeriodisedValues)
        {
            return new PriceEpisodePeriodisedValues()
            {
                AttributeName = priceEpisodePeriodisedValues.AttributeName,
                Period1 = priceEpisodePeriodisedValues.Period1,
                Period2 = priceEpisodePeriodisedValues.Period2,
                Period3 = priceEpisodePeriodisedValues.Period3,
                Period4 = priceEpisodePeriodisedValues.Period4,
                Period5 = priceEpisodePeriodisedValues.Period5,
                Period6 = priceEpisodePeriodisedValues.Period6,
                Period7 = priceEpisodePeriodisedValues.Period7,
                Period8 = priceEpisodePeriodisedValues.Period8,
                Period9 = priceEpisodePeriodisedValues.Period9,
                Period10 = priceEpisodePeriodisedValues.Period10,
                Period11 = priceEpisodePeriodisedValues.Period11,
                Period12 = priceEpisodePeriodisedValues.Period12
            };
        }

        private LearningDelivery MapFm36LearningDelivery(FundingService.FM36.FundingOutput.Model.Output.LearningDelivery fm36LearningDelivery)
        {
            return new LearningDelivery()
            {
                AimSeqNumber = fm36LearningDelivery.AimSeqNumber,
                LearningDeliveryValues = MapFm36LearningDeliveryValues(fm36LearningDelivery.LearningDeliveryValues),
                LearningDeliveryPeriodisedValues = fm36LearningDelivery.LearningDeliveryPeriodisedValues?.Select(MapFm36LearningDeliveryPeriodisedValues).ToList(),
                LearningDeliveryPeriodisedTextValues = fm36LearningDelivery.LearningDeliveryPeriodisedTextValues?.Select(MapFm36LearningDeliveryPeriodisedTextValues).ToList()
            };
        }

        private LearningDeliveryValues MapFm36LearningDeliveryValues(FundingService.FM36.FundingOutput.Model.Output.LearningDeliveryValues fm36LearningDeliveryValues)
        {
            return new LearningDeliveryValues()
            {
                ActualDaysIL = fm36LearningDeliveryValues.ActualDaysIL,
                ActualNumInstalm = fm36LearningDeliveryValues.ActualNumInstalm,
                AdjStartDate = fm36LearningDeliveryValues.AdjStartDate,
                AgeAtProgStart = fm36LearningDeliveryValues.AgeAtProgStart,
                AppAdjLearnStartDate = fm36LearningDeliveryValues.AppAdjLearnStartDate,
                AppAdjLearnStartDateMatchPathway = fm36LearningDeliveryValues.AppAdjLearnStartDateMatchPathway,
                ApplicCompDate = fm36LearningDeliveryValues.ApplicCompDate,
                CombinedAdjProp = fm36LearningDeliveryValues.CombinedAdjProp,
                Completed = fm36LearningDeliveryValues.Completed,
                FirstIncentiveThresholdDate = fm36LearningDeliveryValues.FirstIncentiveThresholdDate,
                FundStart = fm36LearningDeliveryValues.FundStart,
                FworkCode = fm36LearningDeliveryValues.FworkCode,
                LDApplic1618FrameworkUpliftTotalActEarnings = fm36LearningDeliveryValues.LDApplic1618FrameworkUpliftTotalActEarnings,
                LearnAimRef = fm36LearningDeliveryValues.LearnAimRef,
                LearnStartDate = fm36LearningDeliveryValues.LearnStartDate,
                LearnDel1618AtStart = fm36LearningDeliveryValues.LearnDel1618AtStart,
                LearnDelAccDaysILCareLeavers = fm36LearningDeliveryValues.LearnDelAccDaysILCareLeavers,
                LearnDelAppAccDaysIL = fm36LearningDeliveryValues.LearnDelAppAccDaysIL,
                LearnDelApplicCareLeaverIncentive = fm36LearningDeliveryValues.LearnDelApplicCareLeaverIncentive,
                LearnDelApplicDisadvAmount = fm36LearningDeliveryValues.LearnDelApplicDisadvAmount,
                LearnDelApplicEmp1618Incentive = fm36LearningDeliveryValues.LearnDelApplicEmp1618Incentive,
                LearnDelApplicEmpDate = fm36LearningDeliveryValues.LearnDelApplicEmpDate,
                LearnDelApplicProv1618FrameworkUplift = fm36LearningDeliveryValues.LearnDelApplicProv1618FrameworkUplift,
                LearnDelApplicProv1618Incentive = fm36LearningDeliveryValues.LearnDelApplicProv1618Incentive,
                LearnDelAppPrevAccDaysIL = fm36LearningDeliveryValues.LearnDelAppPrevAccDaysIL,
                LearnDelDaysIL = fm36LearningDeliveryValues.LearnDelDaysIL,
                LearnDelDisadAmount = fm36LearningDeliveryValues.LearnDelDisadAmount,
                LearnDelEligDisadvPayment = fm36LearningDeliveryValues.LearnDelEligDisadvPayment,
                LearnDelEmpIdFirstAdditionalPaymentThreshold = fm36LearningDeliveryValues.LearnDelEmpIdFirstAdditionalPaymentThreshold,
                LearnDelEmpIdSecondAdditionalPaymentThreshold = fm36LearningDeliveryValues.LearnDelEmpIdSecondAdditionalPaymentThreshold,
                LearnDelHistDaysCareLeavers = fm36LearningDeliveryValues.LearnDelHistDaysCareLeavers,
                LearnDelHistDaysThisApp = fm36LearningDeliveryValues.LearnDelHistDaysThisApp,
                LearnDelHistProgEarnings = fm36LearningDeliveryValues.LearnDelHistProgEarnings,
                LearnDelInitialFundLineType = fm36LearningDeliveryValues.LearnDelInitialFundLineType,
                LearnDelLearnerAddPayThresholdDate = fm36LearningDeliveryValues.LearnDelLearnerAddPayThresholdDate,
                LearnDelMathEng = fm36LearningDeliveryValues.LearnDelMathEng,
                LearnDelNonLevyProcured = fm36LearningDeliveryValues.LearnDelNonLevyProcured,
                LearnDelPrevAccDaysILCareLeavers = fm36LearningDeliveryValues.LearnDelPrevAccDaysILCareLeavers,
                LearnDelProgEarliestACT2Date = fm36LearningDeliveryValues.LearnDelProgEarliestACT2Date,
                LearnDelRedCode = fm36LearningDeliveryValues.LearnDelRedCode,
                LearnDelRedStartDate = fm36LearningDeliveryValues.LearnDelRedStartDate,
                MathEngAimValue = fm36LearningDeliveryValues.MathEngAimValue,
                OutstandNumOnProgInstalm = fm36LearningDeliveryValues.OutstandNumOnProgInstalm,
                PlannedNumOnProgInstalm = fm36LearningDeliveryValues.PlannedNumOnProgInstalm,
                PlannedTotalDaysIL = fm36LearningDeliveryValues.PlannedTotalDaysIL,
                ProgType = fm36LearningDeliveryValues.ProgType,
                PwayCode = fm36LearningDeliveryValues.PwayCode,
                SecondIncentiveThresholdDate = fm36LearningDeliveryValues.SecondIncentiveThresholdDate,
                StdCode = fm36LearningDeliveryValues.StdCode,
                ThresholdDays = fm36LearningDeliveryValues.ThresholdDays,
            };
        }

        private LearningDeliveryPeriodisedValues MapFm36LearningDeliveryPeriodisedValues(FundingService.FM36.FundingOutput.Model.Output.LearningDeliveryPeriodisedValues fm36LearningDeliveryPeriodisedValue)
        {
            return new LearningDeliveryPeriodisedValues()
            {
                AttributeName = fm36LearningDeliveryPeriodisedValue.AttributeName,
                Period1 = fm36LearningDeliveryPeriodisedValue.Period1,
                Period2 = fm36LearningDeliveryPeriodisedValue.Period2,
                Period3 = fm36LearningDeliveryPeriodisedValue.Period3,
                Period4 = fm36LearningDeliveryPeriodisedValue.Period4,
                Period5 = fm36LearningDeliveryPeriodisedValue.Period5,
                Period6 = fm36LearningDeliveryPeriodisedValue.Period6,
                Period7 = fm36LearningDeliveryPeriodisedValue.Period7,
                Period8 = fm36LearningDeliveryPeriodisedValue.Period8,
                Period9 = fm36LearningDeliveryPeriodisedValue.Period9,
                Period10 = fm36LearningDeliveryPeriodisedValue.Period10,
                Period11 = fm36LearningDeliveryPeriodisedValue.Period11,
                Period12 = fm36LearningDeliveryPeriodisedValue.Period12
            };
        }

        private LearningDeliveryPeriodisedTextValues MapFm36LearningDeliveryPeriodisedTextValues(FundingService.FM36.FundingOutput.Model.Output.LearningDeliveryPeriodisedTextValues fm36LearningDeliveryPeriodisedTextValue)
        {
            return new LearningDeliveryPeriodisedTextValues()
            {
                AttributeName = fm36LearningDeliveryPeriodisedTextValue.AttributeName,
                Period1 = fm36LearningDeliveryPeriodisedTextValue.Period1,
                Period2 = fm36LearningDeliveryPeriodisedTextValue.Period2,
                Period3 = fm36LearningDeliveryPeriodisedTextValue.Period3,
                Period4 = fm36LearningDeliveryPeriodisedTextValue.Period4,
                Period5 = fm36LearningDeliveryPeriodisedTextValue.Period5,
                Period6 = fm36LearningDeliveryPeriodisedTextValue.Period6,
                Period7 = fm36LearningDeliveryPeriodisedTextValue.Period7,
                Period8 = fm36LearningDeliveryPeriodisedTextValue.Period8,
                Period9 = fm36LearningDeliveryPeriodisedTextValue.Period9,
                Period10 = fm36LearningDeliveryPeriodisedTextValue.Period10,
                Period11 = fm36LearningDeliveryPeriodisedTextValue.Period11,
                Period12 = fm36LearningDeliveryPeriodisedTextValue.Period12
            };

        }

        private HistoricEarningOutputValues MapHistoricEarningsOutputValues(FundingService.FM36.FundingOutput.Model.Output.HistoricEarningOutputValues historicEarningOutputValues)
        {
            return new HistoricEarningOutputValues()
            {
                AppIdentifierOutput = historicEarningOutputValues.AppIdentifierOutput,
                AppProgCompletedInTheYearOutput = historicEarningOutputValues.AppProgCompletedInTheYearOutput,
                HistoricDaysInYearOutput = historicEarningOutputValues.HistoricDaysInYearOutput,
                HistoricEffectiveTNPStartDateOutput = historicEarningOutputValues.HistoricEffectiveTNPStartDateOutput,
                HistoricEmpIdEndWithinYearOutput = historicEarningOutputValues.HistoricEmpIdEndWithinYearOutput,
                HistoricEmpIdStartWithinYearOutput = historicEarningOutputValues.HistoricEmpIdStartWithinYearOutput,
                HistoricFworkCodeOutput = historicEarningOutputValues.HistoricFworkCodeOutput,
                HistoricLearner1618AtStartOutput = historicEarningOutputValues.HistoricLearner1618AtStartOutput,
                HistoricPMRAmountOutput = historicEarningOutputValues.HistoricPMRAmountOutput,
                HistoricProgrammeStartDateIgnorePathwayOutput = historicEarningOutputValues.HistoricProgrammeStartDateIgnorePathwayOutput,
                HistoricProgrammeStartDateMatchPathwayOutput = historicEarningOutputValues.HistoricProgrammeStartDateMatchPathwayOutput,
                HistoricProgTypeOutput = historicEarningOutputValues.HistoricProgTypeOutput,
                HistoricPwayCodeOutput = historicEarningOutputValues.HistoricPwayCodeOutput,
                HistoricSTDCodeOutput = historicEarningOutputValues.HistoricSTDCodeOutput,
                HistoricTNP1Output = historicEarningOutputValues.HistoricTNP1Output,
                HistoricTNP2Output = historicEarningOutputValues.HistoricTNP2Output,
                HistoricTNP3Output = historicEarningOutputValues.HistoricTNP3Output,
                HistoricTNP4Output = historicEarningOutputValues.HistoricTNP4Output,
                HistoricTotal1618UpliftPaymentsInTheYear = historicEarningOutputValues.HistoricTotal1618UpliftPaymentsInTheYear,
                HistoricTotalProgAimPaymentsInTheYear = historicEarningOutputValues.HistoricTotalProgAimPaymentsInTheYear,
                HistoricULNOutput = historicEarningOutputValues.HistoricULNOutput,
                HistoricUptoEndDateOutput = historicEarningOutputValues.HistoricUptoEndDateOutput,
                HistoricVirtualTNP3EndofThisYearOutput = historicEarningOutputValues.HistoricVirtualTNP3EndofThisYearOutput,
                HistoricVirtualTNP4EndofThisYearOutput = historicEarningOutputValues.HistoricVirtualTNP4EndofThisYearOutput,
                HistoricLearnDelProgEarliestACT2DateOutput = historicEarningOutputValues.HistoricLearnDelProgEarliestACT2DateOutput,
            };
        }
    }
}
