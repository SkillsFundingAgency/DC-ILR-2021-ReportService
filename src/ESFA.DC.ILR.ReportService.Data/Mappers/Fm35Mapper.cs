using System.Linq;
using ESFA.DC.ILR.ReportService.Models.Fm35;
using ESFA.DC.ILR.ReportService.Service.Interface.Mappers;

namespace ESFA.DC.ILR.ReportService.Data.Mappers
{
    public class Fm35Mapper : IFm35Mapper
    {
        public FM35Global MapData(FundingService.FM35.FundingOutput.Model.Output.FM35Global fm35Global)
        {
            return MapFm35Global(fm35Global);
        }

        private FM35Global MapFm35Global(FundingService.FM35.FundingOutput.Model.Output.FM35Global fm35Global)
        {
            return new FM35Global()
            {
                UKPRN = fm35Global.UKPRN,
                CurFundYr = fm35Global.CurFundYr,
                LARSVersion = fm35Global.LARSVersion,
                RulebaseVersion = fm35Global.RulebaseVersion,
                OrgVersion = fm35Global.OrgVersion,
                PostcodeDisadvantageVersion = fm35Global.PostcodeDisadvantageVersion,
                Learners = fm35Global.Learners?.Select(MapFm35Learner).ToList()
            };
        }

        private FM35Learner MapFm35Learner(FundingService.FM35.FundingOutput.Model.Output.FM35Learner fm35Learner)
        {
            return new FM35Learner()
            {
                LearnRefNumber = fm35Learner.LearnRefNumber,
                LearningDeliveries = fm35Learner.LearningDeliveries?.Select(MapFm35LearningDelivery).ToList()
            };
        }

        private LearningDelivery MapFm35LearningDelivery(FundingService.FM35.FundingOutput.Model.Output.LearningDelivery fm35LearningDelivery)
        {
            return new LearningDelivery()
            {
                AimSeqNumber = fm35LearningDelivery.AimSeqNumber,
                LearningDeliveryValue = MapFm35LearningDeliveryValue(fm35LearningDelivery.LearningDeliveryValue),
                LearningDeliveryPeriodisedValues = fm35LearningDelivery.LearningDeliveryPeriodisedValues?.Select(MapFm35LearningDeliveryPeriodisedValue).ToList()   
            };
        }

        private LearningDeliveryValue MapFm35LearningDeliveryValue(FundingService.FM35.FundingOutput.Model.Output.LearningDeliveryValue fm35LearningDeliveryValue)
        {
            return new LearningDeliveryValue()
            {
                AchApplicDate = fm35LearningDeliveryValue.AchApplicDate,
                Achieved = fm35LearningDeliveryValue.Achieved,
                AchieveElement = fm35LearningDeliveryValue.AchieveElement,
                AchievePayElig = fm35LearningDeliveryValue.AchievePayElig,
                AchievePayPctPreTrans = fm35LearningDeliveryValue.AchievePayPctPreTrans,
                AchPayTransHeldBack = fm35LearningDeliveryValue.AchPayTransHeldBack,
                ActualDaysIL = fm35LearningDeliveryValue.ActualDaysIL,
                ActualNumInstalm = fm35LearningDeliveryValue.ActualNumInstalm,
                ActualNumInstalmPreTrans = fm35LearningDeliveryValue.ActualNumInstalmPreTrans,
                ActualNumInstalmTrans = fm35LearningDeliveryValue.ActualNumInstalmTrans,
                AdjLearnStartDate = fm35LearningDeliveryValue.AdjLearnStartDate,
                AdltLearnResp = fm35LearningDeliveryValue.AdltLearnResp,
                AgeAimStart = fm35LearningDeliveryValue.AgeAimStart,
                AimValue = fm35LearningDeliveryValue.AimValue,
                AppAdjLearnStartDate = fm35LearningDeliveryValue.AppAdjLearnStartDate,
                AppAgeFact = fm35LearningDeliveryValue.AppAgeFact,
                AppATAGTA = fm35LearningDeliveryValue.AppATAGTA,
                AppCompetency = fm35LearningDeliveryValue.AppCompetency,
                AppFuncSkill = fm35LearningDeliveryValue.AppFuncSkill,
                AppFuncSkill1618AdjFact = fm35LearningDeliveryValue.AppFuncSkill1618AdjFact,
                AppKnowl = fm35LearningDeliveryValue.AppKnowl,
                AppLearnStartDate = fm35LearningDeliveryValue.AppLearnStartDate,
                ApplicEmpFactDate = fm35LearningDeliveryValue.ApplicEmpFactDate,
                ApplicFactDate = fm35LearningDeliveryValue.ApplicFactDate,
                ApplicFundRateDate = fm35LearningDeliveryValue.ApplicFundRateDate,
                ApplicProgWeightFact = fm35LearningDeliveryValue.ApplicProgWeightFact,
                ApplicUnweightFundRate = fm35LearningDeliveryValue.ApplicUnweightFundRate,
                ApplicWeightFundRate = fm35LearningDeliveryValue.ApplicWeightFundRate,
                AppNonFund = fm35LearningDeliveryValue.AppNonFund,
                AreaCostFactAdj = fm35LearningDeliveryValue.AreaCostFactAdj,
                BalInstalmPreTrans = fm35LearningDeliveryValue.BalInstalmPreTrans,
                BaseValueUnweight = fm35LearningDeliveryValue.BaseValueUnweight,
                CapFactor = fm35LearningDeliveryValue.CapFactor,
                DisUpFactAdj = fm35LearningDeliveryValue.DisUpFactAdj,
                EmpOutcomePayElig = fm35LearningDeliveryValue.EmpOutcomePayElig,
                EmpOutcomePctHeldBackTrans = fm35LearningDeliveryValue.EmpOutcomePctHeldBackTrans,
                EmpOutcomePctPreTrans = fm35LearningDeliveryValue.EmpOutcomePctPreTrans,
                EmpRespOth = fm35LearningDeliveryValue.EmpRespOth,
                ESOL = fm35LearningDeliveryValue.ESOL,
                FullyFund = fm35LearningDeliveryValue.FullyFund,
                FundLine = fm35LearningDeliveryValue.FundLine,
                FundStart = fm35LearningDeliveryValue.FundStart,
                LargeEmployerID = fm35LearningDeliveryValue.LargeEmployerID,
                LargeEmployerFM35Fctr = fm35LearningDeliveryValue.LargeEmployerFM35Fctr,
                LargeEmployerStatusDate = fm35LearningDeliveryValue.LargeEmployerStatusDate,
                LearnDelFundOrgCode = fm35LearningDeliveryValue.LearnDelFundOrgCode,
                LTRCUpliftFctr = fm35LearningDeliveryValue.LTRCUpliftFctr,
                NonGovCont = fm35LearningDeliveryValue.NonGovCont,
                OLASSCustody = fm35LearningDeliveryValue.OLASSCustody,
                OnProgPayPctPreTrans = fm35LearningDeliveryValue.OnProgPayPctPreTrans,
                OutstndNumOnProgInstalm = fm35LearningDeliveryValue.OutstndNumOnProgInstalm,
                OutstndNumOnProgInstalmTrans = fm35LearningDeliveryValue.OutstndNumOnProgInstalmTrans,
                PlannedNumOnProgInstalm = fm35LearningDeliveryValue.PlannedNumOnProgInstalm,
                PlannedNumOnProgInstalmTrans = fm35LearningDeliveryValue.PlannedNumOnProgInstalmTrans,
                PlannedTotalDaysIL = fm35LearningDeliveryValue.PlannedTotalDaysIL,
                PlannedTotalDaysILPreTrans = fm35LearningDeliveryValue.PlannedTotalDaysILPreTrans,
                PropFundRemain = fm35LearningDeliveryValue.PropFundRemain,
                PropFundRemainAch = fm35LearningDeliveryValue.PropFundRemainAch,
                PrscHEAim = fm35LearningDeliveryValue.PrscHEAim,
                Residential = fm35LearningDeliveryValue.Residential,
                Restart = fm35LearningDeliveryValue.Restart,
                SpecResUplift = fm35LearningDeliveryValue.SpecResUplift,
                StartPropTrans = fm35LearningDeliveryValue.StartPropTrans,
                ThresholdDays = fm35LearningDeliveryValue.ThresholdDays,
                Traineeship = fm35LearningDeliveryValue.Traineeship,
                Trans = fm35LearningDeliveryValue.Trans,
                TrnAdjLearnStartDate = fm35LearningDeliveryValue.TrnAdjLearnStartDate,
                TrnWorkPlaceAim = fm35LearningDeliveryValue.TrnWorkPlaceAim,
                TrnWorkPrepAim = fm35LearningDeliveryValue.TrnWorkPrepAim,
                UnWeightedRateFromESOL = fm35LearningDeliveryValue.UnWeightedRateFromESOL,
                UnweightedRateFromLARS = fm35LearningDeliveryValue.UnweightedRateFromLARS,
                WeightedRateFromESOL = fm35LearningDeliveryValue.WeightedRateFromESOL,
                WeightedRateFromLARS = fm35LearningDeliveryValue.WeightedRateFromLARS
            };
        }

        private LearningDeliveryPeriodisedValue MapFm35LearningDeliveryPeriodisedValue(FundingService.FM35.FundingOutput.Model.Output.LearningDeliveryPeriodisedValue fm35LearningDeliveryPeriodisedValue)
        {
            return new LearningDeliveryPeriodisedValue()
            {
                AttributeName = fm35LearningDeliveryPeriodisedValue.AttributeName,
                Period1 = fm35LearningDeliveryPeriodisedValue.Period1,
                Period2 = fm35LearningDeliveryPeriodisedValue.Period2,
                Period3 = fm35LearningDeliveryPeriodisedValue.Period3,
                Period4 = fm35LearningDeliveryPeriodisedValue.Period4,
                Period5 = fm35LearningDeliveryPeriodisedValue.Period5,
                Period6 = fm35LearningDeliveryPeriodisedValue.Period6,
                Period7 = fm35LearningDeliveryPeriodisedValue.Period7,
                Period8 = fm35LearningDeliveryPeriodisedValue.Period8,
                Period9 = fm35LearningDeliveryPeriodisedValue.Period9,
                Period10 = fm35LearningDeliveryPeriodisedValue.Period10,
                Period11 = fm35LearningDeliveryPeriodisedValue.Period11,
                Period12 = fm35LearningDeliveryPeriodisedValue.Period12
            };
        }
    }
}
