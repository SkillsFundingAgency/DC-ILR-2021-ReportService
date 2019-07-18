using CsvHelper.Configuration;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.DevolvedOccupancy.Model
{
    public class DevolvedAdultEducationOccupancyReportClassMap : ClassMap<DevolvedAdultEducationOccupancyReportModel>
    {
        public DevolvedAdultEducationOccupancyReportClassMap()
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
            Map(m => m.Fm35LearningDelivery.ApplicWeightFundRate).Name(@"Applicable funding rate").Index(++index);
            Map(m => m.Fm35LearningDelivery.ApplicProgWeightFact).Name(@"Applicable programme weighting").Index(++index);
            Map(m => m.Fm35LearningDelivery.AimValue).Name(@"Aim value").Index(++index);
            Map(m => m.LarsLearningDelivery.NotionalNVQLevelv2).Name(@"Notional NVQ level").Index(++index);
            Map(m => m.LarsLearningDelivery.SectorSubjectAreaTier2).Name(@"Tier 2 sector subject area").Index(++index);
            Map(m => m.LearningDelivery.FundModel).Name(@"Funding model").Index(++index);
            Map(m => m.LearningDelivery.PriorLearnFundAdjNullable).Name(@"Funding adjustment for prior learning").Index(++index);
            Map(m => m.LearningDelivery.OtherFundAdjNullable).Name(@"Other funding adjustment").Index(++index);
            Map(m => m.LearningDelivery.OrigLearnStartDateNullable).Name(@"Original learning start date").Index(++index);
            Map(m => m.LearningDelivery.LearnStartDate).Name(@"Learning start date").Index(++index);
            Map(m => m.LearningDelivery.LearnPlanEndDate).Name(@"Learning planned end date").Index(++index);
            Map(m => m.LearningDelivery.CompStatus).Name(@"Completion status").Index(++index);
            Map(m => m.LearningDelivery.LearnActEndDateNullable).Name(@"Learning actual end date").Index(++index);
            Map(m => m.LearningDelivery.OutcomeNullable).Name(@"Outcome").Index(++index);
            Map(m => m.LearningDelivery.AddHoursNullable).Name(@"Additional delivery hours").Index(++index);
            Map(m => m.LearningDelivery.LSDPostcode).Name(@"Learning start date postcode").Index(++index);
            Map().Name(@"Applicable area from source of funding").Index(++index);      // TODO
            Map().Name(@"Learning delivery funding and monitoring type – source of funding").Index(++index);
            Map().Name(@"Learning delivery funding and monitoring type – full or co funding indicator ").Index(++index);
            Map().Name(@"Learning delivery funding and monitoring type – learning support funding (highest applicable)").Index(++index);
            Map().Name(@"Learning delivery funding and monitoring type - LSF date applies from (earliest)").Index(++index);
            Map().Name(@"Learning delivery funding and monitoring type - LSF date applies to (latest)").Index(++index);
            Map().Name(@"Learning delivery funding and monitoring type – learning delivery monitoring (A)").Index(++index);
            Map().Name(@"Learning delivery funding and monitoring type – learning delivery monitoring (B)").Index(++index);
            Map().Name(@"Learning delivery funding and monitoring type – learning delivery monitoring (C)").Index(++index);
            Map().Name(@"Learning delivery funding and monitoring type – learning delivery monitoring (D)").Index(++index);
            Map().Name(@"Learning delivery funding and monitoring type – learning delivery monitoring (E)").Index(++index);
            Map().Name(@"Learning delivery funding and monitoring type – learning delivery monitoring (F)").Index(++index);
            Map().Name(@"Learning delivery funding and monitoring type - devolved area monitoring (A)").Index(++index);
            Map().Name(@"Learning delivery funding and monitoring type - devolved area monitoring (B)").Index(++index);
            Map().Name(@"Learning delivery funding and monitoring type - devolved area monitoring (C)").Index(++index);
            Map().Name(@"Learning delivery funding and monitoring type - devolved area monitoring (D)").Index(++index);
            Map().Name(@"Learning delivery funding and monitoring type – restart indicator").Index(++index);
            Map().Name(@"Provider specified delivery monitoring (A)").Index(++index);
            Map().Name(@"Provider specified delivery monitoring (B)").Index(++index);
            Map().Name(@"Provider specified delivery monitoring (C)").Index(++index);
            Map().Name(@"Provider specified delivery monitoring (D)").Index(++index);
            Map(m => m.Fm35LearningDelivery.FundLine).Name(@"Funding line type").Index(++index);
            Map(m => m.Fm35LearningDelivery.PlannedNumOnProgInstalm).Name(@"Planned number of on programme instalments").Index(++index);
            Map(m => m.Fm35LearningDelivery.AchieveElement).Name(@"Achievement element (potential or actual earned cash)").Index(++index);
            Map().Name(@"Achievement percentage (aggregated maximum value)").Index(++index);    // TODO
            Map(m => m.Fm35LearningDelivery.NonGovCont).Name(@"Non-public-funded contribution").Index(++index);
            Map(m => m.Fm35LearningDelivery.CapFactor).Name(@"Capping factor").Index(++index);
            Map(m => m.LearningDelivery.PartnerUKPRNNullable).Name(@"Sub contracted or partnership UKPRN").Index(++index);
            Map(m => m.LearningDelivery.DelLocPostCode).Name(@"Delivery location postcode").Index(++index);
            Map(m => m.Fm35LearningDelivery.AreaCostFactAdj).Name(@"Area uplift").Index(++index);
            Map(m => m.Fm35LearningDelivery.DisUpFactAdj).Name(@"Disadvantage uplift").Index(++index);
            Map(m => m.Fm35LearningDelivery.ApplicFactDate).Name(@"Date used for uplifts and other lookups").Index(++index);
            Map().Name(@"August on programme earned cash").Index(++index);
            Map().Name(@"August balancing payment earned cash").Index(++index);
            Map().Name(@"August aim achievement earned cash").Index(++index);
            Map().Name(@"August job outcome earned cash").Index(++index);
            Map().Name(@"August learning support earned cash").Index(++index);
            Map().Name(@"September on programme earned cash").Index(++index);
            Map().Name(@"September balancing payment earned cash").Index(++index);
            Map().Name(@"September aim achievement earned cash").Index(++index);
            Map().Name(@"September job outcome earned cash").Index(++index);
            Map().Name(@"September learning support earned cash").Index(++index);
            Map().Name(@"October on programme earned cash").Index(++index);
            Map().Name(@"October balancing payment earned cash").Index(++index);
            Map().Name(@"October aim achievement earned cash").Index(++index);
            Map().Name(@"October job outcome earned cash").Index(++index);
            Map().Name(@"October learning support earned cash").Index(++index);
            Map().Name(@"November on programme earned cash").Index(++index);
            Map().Name(@"November balancing payment earned cash").Index(++index);
            Map().Name(@"November aim achievement earned cash").Index(++index);
            Map().Name(@"November job outcome earned cash").Index(++index);
            Map().Name(@"November learning support earned cash").Index(++index);
            Map().Name(@"December on programme earned cash").Index(++index);
            Map().Name(@"December balancing payment earned cash").Index(++index);
            Map().Name(@"December aim achievement earned cash").Index(++index);
            Map().Name(@"December job outcome earned cash").Index(++index);
            Map().Name(@"December learning support earned cash").Index(++index);
            Map().Name(@"January on programme earned cash").Index(++index);
            Map().Name(@"January balancing payment earned cash").Index(++index);
            Map().Name(@"January aim achievement earned cash").Index(++index);
            Map().Name(@"January job outcome earned cash").Index(++index);
            Map().Name(@"January learning support earned cash").Index(++index);
            Map().Name(@"February on programme earned cash").Index(++index);
            Map().Name(@"February balancing payment earned cash").Index(++index);
            Map().Name(@"February aim achievement earned cash").Index(++index);
            Map().Name(@"February job outcome earned cash").Index(++index);
            Map().Name(@"February learning support earned cash").Index(++index);
            Map().Name(@"March on programme earned cash").Index(++index);
            Map().Name(@"March balancing payment earned cash").Index(++index);
            Map().Name(@"March aim achievement earned cash").Index(++index);
            Map().Name(@"March job outcome earned cash").Index(++index);
            Map().Name(@"March learning support earned cash").Index(++index);
            Map().Name(@"April on programme earned cash").Index(++index);
            Map().Name(@"April balancing payment earned cash").Index(++index);
            Map().Name(@"April aim achievement earned cash").Index(++index);
            Map().Name(@"April job outcome earned cash").Index(++index);
            Map().Name(@"April learning support earned cash").Index(++index);
            Map().Name(@"May on programme earned cash").Index(++index);
            Map().Name(@"May balancing payment earned cash").Index(++index);
            Map().Name(@"May aim achievement earned cash").Index(++index);
            Map().Name(@"May job outcome earned cash").Index(++index);
            Map().Name(@"May learning support earned cash").Index(++index);
            Map().Name(@"June on programme earned cash").Index(++index);
            Map().Name(@"June balancing payment earned cash").Index(++index);
            Map().Name(@"June aim achievement earned cash").Index(++index);
            Map().Name(@"June job outcome earned cash").Index(++index);
            Map().Name(@"June learning support earned cash").Index(++index);
            Map().Name(@"July on programme earned cash").Index(++index);
            Map().Name(@"July balancing payment earned cash").Index(++index);
            Map().Name(@"July aim achievement earned cash").Index(++index);
            Map().Name(@"July job outcome earned cash").Index(++index);
            Map().Name(@"July learning support earned cash").Index(++index);
            Map().Name(@"Total on programme earned cash").Index(++index);
            Map().Name(@"Total balancing payment earned cash").Index(++index);
            Map().Name(@"Total aim achievement earned cash").Index(++index);
            Map().Name(@"Total job outcome earned cash").Index(++index);
            Map().Name(@"Total learning support earned cash").Index(++index);
            Map().Name(@"Total earned cash").Index(++index);
            Map().Name(@"OFFICIAL – SENSITIVE").Constant(@"OFFICIAL – SENSITIVE").Index(++index);
        }

        
    }
}
