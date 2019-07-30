using CsvHelper.Configuration;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Main
{
    public class MainOccupancyReportClassMap : ClassMap<MainOccupancyReportModel>
    {
        public MainOccupancyReportClassMap()
        {
            int index = 0;

            Map(m => m.Learner.LearnRefNumber).Name(@"Learner reference number").Index(++index);
            Map(m => m.Learner.ULN).Name(@"Unique learner number").Index(++index);
            Map(m => m.Learner.DateOfBirthNullable).Name(@"Date of birth").Index(++index);
            Map(m => m.Learner.PostcodePrior).Name(@"Postcode prior to enrolment").Index(++index);
            Map(m => m.Learner.PMUKPRNNullable).Name(@"Pre-merger UKPRN").Index(++index);
            Map(m => m.Learner.CampId).Name(@"Campus identifier").Index(++index);
            Map(m => m.ProviderSpecLearnerMonitoring.A).Name(@"Provider specified learner monitoring (A)").Index(++index);
            Map(m => m.ProviderSpecLearnerMonitoring.B).Name(@"Provider specified learner monitoring (B)").Index(++index);
            Map(m => m.LearningDelivery.AimSeqNumber).Name(@"Aim sequence number").Index(++index);
            Map(m => m.LearningDelivery.LearnAimRef).Name(@"Learning aim reference").Index(++index);
            Map(m => m.LarsLearningDelivery.LearnAimRefTitle).Name(@"Learning aim title").Index(++index);
            Map(m => m.LearningDelivery.SWSupAimId).Name(@"Software supplier aim identifier").Index(++index);
            Map(m => m.Fm35LearningDelivery.WeightedRateFromESOL).Name(@"Applicable funding rate from ESOL hours").Index(++index);
            Map(m => m.FundModelAgnosticModel.ApplicableFundingRate).Name(@"Applicable funding rate").Index(++index);
            Map(m => m.Fm35LearningDelivery.ApplicProgWeightFact).Name(@"Applicable programme weighting").Index(++index);
            Map(m => m.Fm35LearningDelivery.AimValue).Name(@"Aim value").Index(++index);
            Map(m => m.LarsLearningDelivery.NotionalNVQLevel).Name(@"Notional NVQ level").Index(++index);
            Map(m => m.LarsLearningDelivery.SectorSubjectAreaTier2).Name(@"Tier 2 sector subject area").Index(++index);
            Map(m => m.LearningDelivery.ProgTypeNullable).Name(@"Programme type").Index(++index);
            Map(m => m.LearningDelivery.FworkCodeNullable).Name(@"Framework code").Index(++index);
            Map(m => m.LearningDelivery.PwayCodeNullable).Name(@"Apprenticeship pathway").Index(++index);
            Map(m => m.LearningDelivery.AimType).Name(@"Aim type").Index(++index);
            Map(m => m.LarsLearningDelivery.FrameworkCommonComponent).Name(@"Framework component type code").Index(++index);
            Map(m => m.LearningDelivery.FundModel).Name(@"Funding model").Index(++index);
            Map(m => m.LearningDelivery.PriorLearnFundAdjNullable).Name(@"Funding adjustment for prior learning").Index(++index);
            Map(m => m.LearningDelivery.OtherFundAdjNullable).Name(@"Other funding adjustment").Index(++index);
            Map(m => m.LearningDelivery.OrigLearnStartDateNullable).Name(@"Original learning start date").Index(++index);
            Map(m => m.FundModelAgnosticModel.LearningStartDate).Name(@"Learning start date").Index(++index);
            Map(m => m.FundModelAgnosticModel.LearningPlannedEndDate).Name(@"Learning planned end date").Index(++index);
            Map(m => m.LearningDelivery.CompStatus).Name(@"Completion status").Index(++index);
            Map(m => m.FundModelAgnosticModel.LearningActualEndDate).Name(@"Learning actual end date").Index(++index);
            Map(m => m.LearningDelivery.OutcomeNullable).Name(@"Outcome").Index(++index);
            Map(m => m.LearningDelivery.AchDateNullable).Name(@"Achievement date").Index(++index);
            Map(m => m.LearningDelivery.AddHoursNullable).Name(@"Additional delivery hours").Index(++index);
            Map(m => m.LearningDelivery.LSDPostcode).Name(@"Learning start date postcode").Index(++index);
            Map(m => m.LearningDeliveryFAMs.SOF).Name(@"Learning delivery funding and monitoring type – source of funding").Index(++index);
            Map(m => m.LearningDeliveryFAMs.FFI).Name(@"Learning delivery funding and monitoring type – full or co funding indicator").Index(++index);
            Map(m => m.LearningDeliveryFAMs.EEF).Name(@"Learning delivery funding and monitoring type – eligibility for enhanced apprenticeship funding").Index(++index);
            Map(m => m.LearningDeliveryFAMs.LSF_Highest).Name(@"Learning delivery funding and monitoring type – learning support funding (highest applicable)").Index(++index);
            Map(m => m.LearningDeliveryFAMs.LSF_EarliestDateFrom).Name(@"Learning delivery funding and monitoring type - LSF date applies from (earliest)").Index(++index);
            Map(m => m.LearningDeliveryFAMs.LSF_LatestDateTo).Name(@"Learning delivery funding and monitoring type - LSF date applies to (latest)").Index(++index);
            Map(m => m.LearningDeliveryFAMs.LDM1).Name(@"Learning delivery funding and monitoring type – learning delivery monitoring (A)").Index(++index);
            Map(m => m.LearningDeliveryFAMs.LDM2).Name(@"Learning delivery funding and monitoring type – learning delivery monitoring (B)").Index(++index);
            Map(m => m.LearningDeliveryFAMs.LDM3).Name(@"Learning delivery funding and monitoring type – learning delivery monitoring (C)").Index(++index);
            Map(m => m.LearningDeliveryFAMs.LDM4).Name(@"Learning delivery funding and monitoring type – learning delivery monitoring (D)").Index(++index);
            Map(m => m.LearningDeliveryFAMs.LDM5).Name(@"Learning delivery funding and monitoring type – learning delivery monitoring (E)").Index(++index);
            Map(m => m.LearningDeliveryFAMs.LDM6).Name(@"Learning delivery funding and monitoring type – learning delivery monitoring (F)").Index(++index);
            Map(m => m.LearningDeliveryFAMs.RES).Name(@"Learning delivery funding and monitoring type – restart indicator").Index(++index);
            Map(m => m.ProviderSpecDeliveryMonitoring.A).Name(@"Provider specified delivery monitoring (A)").Index(++index);
            Map(m => m.ProviderSpecDeliveryMonitoring.B).Name(@"Provider specified delivery monitoring (B)").Index(++index);
            Map(m => m.ProviderSpecDeliveryMonitoring.C).Name(@"Provider specified delivery monitoring (C)").Index(++index);
            Map(m => m.ProviderSpecDeliveryMonitoring.D).Name(@"Provider specified delivery monitoring (D)").Index(++index);
            Map(m => m.FundModelAgnosticModel.FundLine).Name(@"Funding line type").Index(++index);
            Map(m => m.Fm35LearningDelivery.PlannedNumOnProgInstalm).Name(@"Planned number of on programme instalments").Index(++index);
            Map(m => m.Fm35LearningDelivery.PlannedNumOnProgInstalmTrans).Name(@"Transitional planned number of programme instalments from 1 August 2013").Index(++index);
            Map(m => m.Fm35LearningDelivery.StartPropTrans).Name(@"Transitional start proportion").Index(++index);
            Map(m => m.Fm35LearningDelivery.AchieveElement).Name(@"Achievement element (potential or actual earned cash)").Index(++index);
            Map().Name(@"Achievement percentage (aggregated maximum value)").Index(++index);     //TODO
            Map(m => m.Fm35LearningDelivery.NonGovCont).Name(@"Non-Govt contribution").Index(++index);
            Map(m => m.LearningDelivery.PartnerUKPRNNullable).Name(@"Sub contracted or partnership UKPRN").Index(++index);
            Map(m => m.LearningDelivery.DelLocPostCode).Name(@"Delivery location postcode").Index(++index);
            Map(m => m.Fm35LearningDelivery.AreaCostFactAdj).Name(@"Area uplift").Index(++index);
            Map(m => m.Fm35LearningDelivery.DisUpFactAdj).Name(@"Disadvantage uplift").Index(++index);
            Map(m => m.Fm35LearningDelivery.LargeEmployerID).Name(@"Employer identifier").Index(++index);
            Map(m => m.Fm35LearningDelivery.LargeEmployerFM35Fctr).Name(@"Large employer factor").Index(++index);
            Map(m => m.Fm35LearningDelivery.CapFactor).Name(@"Capping factor").Index(++index);
            Map(m => m).Name(@"Traineeship work placement or work preparation").Index(++index);  // TODO
            Map(m => m).Name(@"Higher apprenticeship prescribed HE aim").Index(++index);         // TODO
            Map(m => m.Fm35LearningDelivery.ApplicEmpFactDate).Name(@"Date used for employment factor lookups").Index(++index);
            Map(m => m.Fm35LearningDelivery.ApplicFactDate).Name(@"Date used for other factor lookups").Index(++index);
            Map(m => m).Name(@"August on programme earned cash").Index(++index);                 // TODO
            Map(m => m).Name(@"August balancing payment earned cash").Index(++index);            // TODO
            Map(m => m).Name(@"August aim achievement earned cash").Index(++index);              // TODO
            Map(m => m).Name(@"August job outcome earned cash").Index(++index);                  // TODO
            Map(m => m).Name(@"August learning support earned cash").Index(++index);             // TODO
            Map(m => m).Name(@"September on programme earned cash").Index(++index);              // TODO
            Map(m => m).Name(@"September balancing payment earned cash").Index(++index);         // TODO
            Map(m => m).Name(@"September aim achievement earned cash").Index(++index);           // TODO
            Map(m => m).Name(@"September job outcome earned cash").Index(++index);               // TODO
            Map(m => m).Name(@"September learning support earned cash").Index(++index);          // TODO
            Map(m => m).Name(@"October on programme earned cash").Index(++index);                // TODO
            Map(m => m).Name(@"October balancing payment earned cash").Index(++index);           // TODO
            Map(m => m).Name(@"October aim achievement earned cash").Index(++index);             // TODO
            Map(m => m).Name(@"October job outcome earned cash").Index(++index);                 // TODO
            Map(m => m).Name(@"October learning support earned cash").Index(++index);            // TODO
            Map(m => m).Name(@"November on programme earned cash").Index(++index);               // TODO
            Map(m => m).Name(@"November balancing payment earned cash").Index(++index);          // TODO
            Map(m => m).Name(@"November aim achievement earned cash").Index(++index);            // TODO
            Map(m => m).Name(@"November job outcome earned cash").Index(++index);                // TODO
            Map(m => m).Name(@"November learning support earned cash").Index(++index);           // TODO
            Map(m => m).Name(@"December on programme earned cash").Index(++index);               // TODO
            Map(m => m).Name(@"December balancing payment earned cash").Index(++index);          // TODO
            Map(m => m).Name(@"December aim achievement earned cash").Index(++index);            // TODO
            Map(m => m).Name(@"December job outcome earned cash").Index(++index);                // TODO
            Map(m => m).Name(@"December learning support earned cash").Index(++index);           // TODO
            Map(m => m).Name(@"January on programme earned cash").Index(++index);                // TODO
            Map(m => m).Name(@"January balancing payment earned cash").Index(++index);           // TODO
            Map(m => m).Name(@"January aim achievement earned cash").Index(++index);             // TODO
            Map(m => m).Name(@"January job outcome earned cash").Index(++index);                 // TODO
            Map(m => m).Name(@"January learning support earned cash").Index(++index);            // TODO
            Map(m => m).Name(@"February on programme earned cash").Index(++index);               // TODO
            Map(m => m).Name(@"February balancing payment earned cash").Index(++index);          // TODO
            Map(m => m).Name(@"February aim achievement earned cash").Index(++index);            // TODO
            Map(m => m).Name(@"February job outcome earned cash").Index(++index);                // TODO
            Map(m => m).Name(@"February learning support earned cash").Index(++index);           // TODO
            Map(m => m).Name(@"March on programme earned cash").Index(++index);                  // TODO
            Map(m => m).Name(@"March balancing payment earned cash").Index(++index);             // TODO
            Map(m => m).Name(@"March aim achievement earned cash").Index(++index);               // TODO
            Map(m => m).Name(@"March job outcome earned cash").Index(++index);                   // TODO
            Map(m => m).Name(@"March learning support earned cash").Index(++index);              // TODO
            Map(m => m).Name(@"April on programme earned cash").Index(++index);                  // TODO
            Map(m => m).Name(@"April balancing payment earned cash").Index(++index);             // TODO
            Map(m => m).Name(@"April aim achievement earned cash").Index(++index);               // TODO
            Map(m => m).Name(@"April job outcome earned cash").Index(++index);                   // TODO
            Map(m => m).Name(@"April learning support earned cash").Index(++index);              // TODO
            Map(m => m).Name(@"May on programme earned cash").Index(++index);                    // TODO
            Map(m => m).Name(@"May balancing payment earned cash").Index(++index);               // TODO
            Map(m => m).Name(@"May aim achievement earned cash").Index(++index);                 // TODO
            Map(m => m).Name(@"May job outcome earned cash").Index(++index);                     // TODO
            Map(m => m).Name(@"May learning support earned cash").Index(++index);                // TODO
            Map(m => m).Name(@"June on programme earned cash").Index(++index);                   // TODO
            Map(m => m).Name(@"June balancing payment earned cash").Index(++index);              // TODO
            Map(m => m).Name(@"June aim achievement earned cash").Index(++index);                // TODO
            Map(m => m).Name(@"June job outcome earned cash").Index(++index);                    // TODO
            Map(m => m).Name(@"June learning support earned cash").Index(++index);               // TODO
            Map(m => m).Name(@"July on programme earned cash").Index(++index);                   // TODO
            Map(m => m).Name(@"July balancing payment earned cash").Index(++index);              // TODO
            Map(m => m).Name(@"July aim achievement earned cash").Index(++index);                // TODO
            Map(m => m).Name(@"July job outcome earned cash").Index(++index);                    // TODO
            Map(m => m).Name(@"July learning support earned cash").Index(++index);               // TODO
            Map(m => m).Name(@"Total on programme earned cash").Index(++index);                  // TODO
            Map(m => m).Name(@"Total balancing payment earned cash").Index(++index);             // TODO
            Map(m => m).Name(@"Total aim achievement earned cash").Index(++index);               // TODO
            Map(m => m).Name(@"Total job outcome earned cash").Index(++index);                   // TODO
            Map(m => m).Name(@"Total learning support earned cash").Index(++index);              // TODO
            Map(m => m).Name(@"Total earned cash").Index(++index);                               // TODO
            Map().Name(@"OFFICIAL - SENSITIVE").Constant(@"OFFICIAL - SENSITIVE").Index(++index);

        }
    }
}
