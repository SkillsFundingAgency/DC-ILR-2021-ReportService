﻿using System;
using System.Collections.Generic;
using System.Text;
using CsvHelper.Configuration;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Main
{
    public class AbstractOccupancyReportClassMap<T> : ClassMap<T>
        where T : MainOccupancyReportModel
    {
        public AbstractOccupancyReportClassMap()
        {
            int index = 0;

            Map(m => m.Learner.LearnRefNumber).Name(@"Learner reference number").Index(++index);
            Map(m => m.Learner.ULN).Name(@"Unique learner number").Index(++index);
            Map(m => m.Learner.FamilyName).Name(@"Family name").Index(++index);
            Map(m => m.Learner.GivenNames).Name(@"Given names").Index(++index);
            Map(m => m.Learner.DateOfBirthNullable).Name(@"Date of birth").Index(++index);
            Map(m => m.Learner.PostcodePrior).Name(@"Postcode prior to enrolment").Index(++index);
            Map(m => m.Learner.PMUKPRNNullable).Name(@"Pre-merger UKPRN").Index(++index);
            Map(m => m.Learner.CampId).Name(@"Campus identifier").Index(++index);
            Map(m => m.ProviderSpecLearnerMonitoring.A).Name(@"Provider specified learner monitoring (A)").Index(++index);
            Map(m => m.ProviderSpecLearnerMonitoring.B).Name(@"Provider specified learner monitoring (B)").Index(++index);
            Map(m => m.FundModelAgnosticModel.AimSequenceNumber).Name(@"Aim sequence number").Index(++index);
            Map(m => m.LearningDelivery.LearnAimRef).Name(@"Learning aim reference").Index(++index);
            Map(m => m.LarsLearningDelivery.LearnAimRefTitle).Name(@"Learning aim title").Index(++index);
            Map(m => m.LearningDelivery.SWSupAimId).Name(@"Software supplier aim identifier").Index(++index);
            Map(m => m.Fm35LearningDelivery.WeightedRateFromESOL).Name(@"Applicable funding rate from ESOL hours").Index(++index);
            Map(m => m.Fm35LearningDelivery.ReservedUpliftRate1).Name(@"Reserved Uplift Rate").Index(++index);
            Map(m => m.FundModelAgnosticModel.ApplicableFundingRate).Name(@"Applicable funding rate").Index(++index);
            Map(m => m.Fm35LearningDelivery.ApplicProgWeightFact).Name(@"Applicable programme weighting").Index(++index);
            Map(m => m.Fm35LearningDelivery.AimValue).Name(@"Aim value").Index(++index);
            Map(m => m.LarsLearningDelivery.NotionalNVQLevel).Name(@"Notional NVQ level").Index(++index);
            Map(m => m.LarsLearningDelivery.SectorSubjectAreaTier2).Name(@"Tier 2 sector subject area").Index(++index);
            Map(m => m.LearningDelivery.ProgTypeNullable).Name(@"Programme type").Index(++index);
            Map(m => m.LearningDelivery.FworkCodeNullable).Name(@"Framework code").Index(++index);
            Map(m => m.LearningDelivery.PwayCodeNullable).Name(@"Apprenticeship pathway").Index(++index);
            Map(m => m.FundModelAgnosticModel.AimType).Name(@"Aim type").Index(++index);
            Map(m => m.LarsFrameworkAim.FrameworkComponentType).Name(@"Framework component type code").Index(++index);
            Map(m => m.FundModelAgnosticModel.FundModel).Name(@"Funding model").Index(++index);
            Map(m => m.LearningDelivery.PriorLearnFundAdjNullable).Name(@"Funding adjustment for prior learning").Index(++index);
            Map(m => m.LearningDelivery.OtherFundAdjNullable).Name(@"Other funding adjustment").Index(++index);
            Map(m => m.LearningDelivery.OrigLearnStartDateNullable).Name(@"Original learning start date").Index(++index);
            Map(m => m.FundModelAgnosticModel.LearningStartDate).Name(@"Learning start date").Index(++index);
            Map(m => m.FundModelAgnosticModel.LearningPlannedEndDate).Name(@"Learning planned end date").Index(++index);
            Map(m => m.FundModelAgnosticModel.CompStatus).Name(@"Completion status").Index(++index);
            Map(m => m.FundModelAgnosticModel.LearningActualEndDate).Name(@"Learning actual end date").Index(++index);
            Map(m => m.LearningDelivery.OutcomeNullable).Name(@"Outcome").Index(++index);
            Map(m => m.LearningDelivery.AchDateNullable).Name(@"Achievement date").Index(++index);
            Map(m => m.LearningDelivery.AddHoursNullable).Name(@"Additional delivery hours").Index(++index);
            Map(m => m.LearningDelivery.LSDPostcode).Name(@"Learning start date postcode").Index(++index);
            Map(m => m.LearningDeliveryFAMs.SOF).Name(@"Learning delivery funding and monitoring type - source of funding").Index(++index);
            Map(m => m.LearningDeliveryFAMs.FFI).Name(@"Learning delivery funding and monitoring type - full or co funding indicator").Index(++index);
            Map(m => m.LearningDeliveryFAMs.EEF).Name(@"Learning delivery funding and monitoring type - eligibility for enhanced apprenticeship funding").Index(++index);
            Map(m => m.LearningDeliveryFAMs.LSF_Highest).Name(@"Learning delivery funding and monitoring type - learning support funding (highest applicable)").Index(++index);
            Map(m => m.LearningDeliveryFAMs.LSF_EarliestDateFrom).Name(@"Learning delivery funding and monitoring type - LSF date applies from (earliest)").Index(++index);
            Map(m => m.LearningDeliveryFAMs.LSF_LatestDateTo).Name(@"Learning delivery funding and monitoring type - LSF date applies to (latest)").Index(++index);
            Map(m => m.LearningDeliveryFAMs.LDM1).Name(@"Learning delivery funding and monitoring type - learning delivery monitoring (A)").Index(++index);
            Map(m => m.LearningDeliveryFAMs.LDM2).Name(@"Learning delivery funding and monitoring type - learning delivery monitoring (B)").Index(++index);
            Map(m => m.LearningDeliveryFAMs.LDM3).Name(@"Learning delivery funding and monitoring type - learning delivery monitoring (C)").Index(++index);
            Map(m => m.LearningDeliveryFAMs.LDM4).Name(@"Learning delivery funding and monitoring type - learning delivery monitoring (D)").Index(++index);
            Map(m => m.LearningDeliveryFAMs.LDM5).Name(@"Learning delivery funding and monitoring type - learning delivery monitoring (E)").Index(++index);
            Map(m => m.LearningDeliveryFAMs.LDM6).Name(@"Learning delivery funding and monitoring type - learning delivery monitoring (F)").Index(++index);
            Map(m => m.LearningDeliveryFAMs.RES).Name(@"Learning delivery funding and monitoring type - restart indicator").Index(++index);
            Map(m => m.ProviderSpecDeliveryMonitoring.A).Name(@"Provider specified delivery monitoring (A)").Index(++index);
            Map(m => m.ProviderSpecDeliveryMonitoring.B).Name(@"Provider specified delivery monitoring (B)").Index(++index);
            Map(m => m.ProviderSpecDeliveryMonitoring.C).Name(@"Provider specified delivery monitoring (C)").Index(++index);
            Map(m => m.ProviderSpecDeliveryMonitoring.D).Name(@"Provider specified delivery monitoring (D)").Index(++index);
            Map(m => m.FundModelAgnosticModel.FundLine).Name(@"Funding line type").Index(++index);
            Map(m => m.Fm35LearningDelivery.PlannedNumOnProgInstalm).Name(@"Planned number of on programme instalments").Index(++index);
            Map(m => m.Fm35LearningDelivery.PlannedNumOnProgInstalmTrans).Name(@"Transitional planned number of programme instalments from 1 August 2013").Index(++index);
            Map(m => m.Fm35LearningDelivery.StartPropTrans).Name(@"Transitional start proportion").Index(++index);
            Map(m => m.Fm35LearningDelivery.AchieveElement).Name(@"Achievement element (potential or actual earned cash)").Index(++index);
            Map(m => m.PeriodisedValues.AchievePayPctMax).Name(@"Achievement percentage (aggregated maximum value)").Index(++index);
            Map(m => m.Fm35LearningDelivery.NonGovCont).Name(@"Non-Govt contribution").Index(++index);
            Map(m => m.LearningDelivery.PartnerUKPRNNullable).Name(@"Sub contracted or partnership UKPRN").Index(++index);
            Map(m => m.LearningDelivery.DelLocPostCode).Name(@"Delivery location postcode").Index(++index);
            Map(m => m.Fm35LearningDelivery.AreaCostFactAdj).Name(@"Area uplift").Index(++index);
            Map(m => m.Fm35LearningDelivery.DisUpFactAdj).Name(@"Disadvantage uplift").Index(++index);
            Map(m => m.Fm35LearningDelivery.LargeEmployerID).Name(@"Employer identifier").Index(++index);
            Map(m => m.Fm35LearningDelivery.LargeEmployerFM35Fctr).Name(@"Large employer factor").Index(++index);
            Map(m => m.Fm35LearningDelivery.CapFactor).Name(@"Capping factor").Index(++index);
            Map(m => m.TraineeshipWorkPlacementOrWorkPreparation).Name(@"Traineeship work placement or work preparation").Index(++index);
            Map(m => m.HigherApprenticeshipPrescribedHeAim).Name(@"Higher apprenticeship prescribed HE aim").Index(++index);
            Map(m => m.Fm35LearningDelivery.ApplicEmpFactDate).Name(@"Date used for employment factor lookups").Index(++index);
            Map(m => m.Fm35LearningDelivery.ApplicFactDate).Name(@"Date used for other factor lookups").Index(++index);
            Map(m => m.PeriodisedValues.August.OnProgPayment).Name(@"August on programme earned cash").Index(++index);
            Map(m => m.PeriodisedValues.August.BalancePayment).Name(@"August balancing payment earned cash").Index(++index);
            Map(m => m.PeriodisedValues.August.AchievePayment).Name(@"August aim achievement earned cash").Index(++index);
            Map(m => m.PeriodisedValues.August.EmpOutcomePay).Name(@"August job outcome earned cash").Index(++index);
            Map(m => m.PeriodisedValues.August.LearnSuppFundCash).Name(@"August learning support earned cash").Index(++index);
            Map(m => m.PeriodisedValues.September.OnProgPayment).Name(@"September on programme earned cash").Index(++index);
            Map(m => m.PeriodisedValues.September.BalancePayment).Name(@"September balancing payment earned cash").Index(++index);
            Map(m => m.PeriodisedValues.September.AchievePayment).Name(@"September aim achievement earned cash").Index(++index);
            Map(m => m.PeriodisedValues.September.EmpOutcomePay).Name(@"September job outcome earned cash").Index(++index);
            Map(m => m.PeriodisedValues.September.LearnSuppFundCash).Name(@"September learning support earned cash").Index(++index);
            Map(m => m.PeriodisedValues.October.OnProgPayment).Name(@"October on programme earned cash").Index(++index);
            Map(m => m.PeriodisedValues.October.BalancePayment).Name(@"October balancing payment earned cash").Index(++index);
            Map(m => m.PeriodisedValues.October.AchievePayment).Name(@"October aim achievement earned cash").Index(++index);
            Map(m => m.PeriodisedValues.October.EmpOutcomePay).Name(@"October job outcome earned cash").Index(++index);
            Map(m => m.PeriodisedValues.October.LearnSuppFundCash).Name(@"October learning support earned cash").Index(++index);
            Map(m => m.PeriodisedValues.November.OnProgPayment).Name(@"November on programme earned cash").Index(++index);
            Map(m => m.PeriodisedValues.November.BalancePayment).Name(@"November balancing payment earned cash").Index(++index);
            Map(m => m.PeriodisedValues.November.AchievePayment).Name(@"November aim achievement earned cash").Index(++index);
            Map(m => m.PeriodisedValues.November.EmpOutcomePay).Name(@"November job outcome earned cash").Index(++index);
            Map(m => m.PeriodisedValues.November.LearnSuppFundCash).Name(@"November learning support earned cash").Index(++index);
            Map(m => m.PeriodisedValues.December.OnProgPayment).Name(@"December on programme earned cash").Index(++index);
            Map(m => m.PeriodisedValues.December.BalancePayment).Name(@"December balancing payment earned cash").Index(++index);
            Map(m => m.PeriodisedValues.December.AchievePayment).Name(@"December aim achievement earned cash").Index(++index);
            Map(m => m.PeriodisedValues.December.EmpOutcomePay).Name(@"December job outcome earned cash").Index(++index);
            Map(m => m.PeriodisedValues.December.LearnSuppFundCash).Name(@"December learning support earned cash").Index(++index);
            Map(m => m.PeriodisedValues.January.OnProgPayment).Name(@"January on programme earned cash").Index(++index);
            Map(m => m.PeriodisedValues.January.BalancePayment).Name(@"January balancing payment earned cash").Index(++index);
            Map(m => m.PeriodisedValues.January.AchievePayment).Name(@"January aim achievement earned cash").Index(++index);
            Map(m => m.PeriodisedValues.January.EmpOutcomePay).Name(@"January job outcome earned cash").Index(++index);
            Map(m => m.PeriodisedValues.January.LearnSuppFundCash).Name(@"January learning support earned cash").Index(++index);
            Map(m => m.PeriodisedValues.February.OnProgPayment).Name(@"February on programme earned cash").Index(++index);
            Map(m => m.PeriodisedValues.February.BalancePayment).Name(@"February balancing payment earned cash").Index(++index);
            Map(m => m.PeriodisedValues.February.AchievePayment).Name(@"February aim achievement earned cash").Index(++index);
            Map(m => m.PeriodisedValues.February.EmpOutcomePay).Name(@"February job outcome earned cash").Index(++index);
            Map(m => m.PeriodisedValues.February.LearnSuppFundCash).Name(@"February learning support earned cash").Index(++index);
            Map(m => m.PeriodisedValues.March.OnProgPayment).Name(@"March on programme earned cash").Index(++index);
            Map(m => m.PeriodisedValues.March.BalancePayment).Name(@"March balancing payment earned cash").Index(++index);
            Map(m => m.PeriodisedValues.March.AchievePayment).Name(@"March aim achievement earned cash").Index(++index);
            Map(m => m.PeriodisedValues.March.EmpOutcomePay).Name(@"March job outcome earned cash").Index(++index);
            Map(m => m.PeriodisedValues.March.LearnSuppFundCash).Name(@"March learning support earned cash").Index(++index);
            Map(m => m.PeriodisedValues.April.OnProgPayment).Name(@"April on programme earned cash").Index(++index);
            Map(m => m.PeriodisedValues.April.BalancePayment).Name(@"April balancing payment earned cash").Index(++index);
            Map(m => m.PeriodisedValues.April.AchievePayment).Name(@"April aim achievement earned cash").Index(++index);
            Map(m => m.PeriodisedValues.April.EmpOutcomePay).Name(@"April job outcome earned cash").Index(++index);
            Map(m => m.PeriodisedValues.April.LearnSuppFundCash).Name(@"April learning support earned cash").Index(++index);
            Map(m => m.PeriodisedValues.May.OnProgPayment).Name(@"May on programme earned cash").Index(++index);
            Map(m => m.PeriodisedValues.May.BalancePayment).Name(@"May balancing payment earned cash").Index(++index);
            Map(m => m.PeriodisedValues.May.AchievePayment).Name(@"May aim achievement earned cash").Index(++index);
            Map(m => m.PeriodisedValues.May.EmpOutcomePay).Name(@"May job outcome earned cash").Index(++index);
            Map(m => m.PeriodisedValues.May.LearnSuppFundCash).Name(@"May learning support earned cash").Index(++index);
            Map(m => m.PeriodisedValues.June.OnProgPayment).Name(@"June on programme earned cash").Index(++index);
            Map(m => m.PeriodisedValues.June.BalancePayment).Name(@"June balancing payment earned cash").Index(++index);
            Map(m => m.PeriodisedValues.June.AchievePayment).Name(@"June aim achievement earned cash").Index(++index);
            Map(m => m.PeriodisedValues.June.EmpOutcomePay).Name(@"June job outcome earned cash").Index(++index);
            Map(m => m.PeriodisedValues.June.LearnSuppFundCash).Name(@"June learning support earned cash").Index(++index);
            Map(m => m.PeriodisedValues.July.OnProgPayment).Name(@"July on programme earned cash").Index(++index);
            Map(m => m.PeriodisedValues.July.BalancePayment).Name(@"July balancing payment earned cash").Index(++index);
            Map(m => m.PeriodisedValues.July.AchievePayment).Name(@"July aim achievement earned cash").Index(++index);
            Map(m => m.PeriodisedValues.July.EmpOutcomePay).Name(@"July job outcome earned cash").Index(++index);
            Map(m => m.PeriodisedValues.July.LearnSuppFundCash).Name(@"July learning support earned cash").Index(++index);
            Map(m => m.PeriodisedValues.OnProgPaymentTotal).Name(@"Total on programme earned cash").Index(++index);
            Map(m => m.PeriodisedValues.BalancePaymentTotal).Name(@"Total balancing payment earned cash").Index(++index);
            Map(m => m.PeriodisedValues.AchievePaymentTotal).Name(@"Total aim achievement earned cash").Index(++index);
            Map(m => m.PeriodisedValues.EmpOutcomePayTotal).Name(@"Total job outcome earned cash").Index(++index);
            Map(m => m.PeriodisedValues.LearnSuppFundCashTotal).Name(@"Total learning support earned cash").Index(++index);
            Map(m => m.PeriodisedValues.TotalEarned).Name(@"Total earned cash").Index(++index);
            Map().Name(@"OFFICIAL - SENSITIVE").Constant(string.Empty).Index(++index);
        }
    }
}
