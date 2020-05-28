using CsvHelper.Configuration;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Apprenticeship.AppsIndicitave
{
    public sealed class AppsIndicativeEarningsReportClassMap : ClassMap<AppsIndicativeEarningsReportModel>
    {
        private const string DecimalFormat = "0.00000";

        public AppsIndicativeEarningsReportClassMap()
        {
            int i = 0;
            Map(m => m.LearnRefNumber).Index(i++).Name("Learner reference number");
            Map(m => m.Learner.ULN).Index(i++).Name("Unique learner number");
            Map(m => m.Learner.FamilyName).Index(i++).Name("Family name");
            Map(m => m.Learner.GivenNames).Index(i++).Name("Given names");
            Map(m => m.Learner.DateOfBirthNullable).Index(i++).Name("Date of birth");
            Map(m => m.Learner.PostcodePrior).Index(i++).Name("Postcode prior to enrolment");
            Map(m => m.Learner.CampId).Index(i++).Name("Campus identifier");
            Map(m => m.ProviderSpecLearnerMonitoring.A).Index(i++).Name("Provider specified learner monitoring (A)");
            Map(m => m.ProviderSpecLearnerMonitoring.B).Index(i++).Name("Provider specified learner monitoring (B)");
            Map(m => m.AimSeqNumber).Index(i++).Name("Aim sequence number");
            Map(m => m.LearningDelivery.LearnAimRef).Index(i++).Name("Learning aim reference");
            Map(m => m.LarsLearningDelivery.LearnAimRefTitle).Index(i++).Name("Learning aim title");
            Map(m => m.LearningDelivery.SWSupAimId).Index(i++).Name("Software supplier aim identifier");
            Map(m => m.Fm36LearningDelivery.LearnDelApplicProv1618FrameworkUplift).Index(i++).Name("LARS 16-18 framework uplift").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.LarsLearningDelivery.NotionalNVQLevel).Index(i++).Name("Notional NVQ level");
            Map(m => m.StandardNotionalEndLevel).Index(i++).Name("Standard notional end level");

            Map(m => m.LarsLearningDelivery.SectorSubjectAreaTier2).Index(i++).Name("Tier 2 sector subject area");
            Map(m => m.LearningDelivery.ProgTypeNullable).Index(i++).Name("Programme type");
            Map(m => m.LearningDelivery.StdCodeNullable).Index(i++).Name("Standard code");
            Map(m => m.LearningDelivery.FworkCodeNullable).Index(i++).Name("Framework code");
            Map(m => m.LearningDelivery.PwayCodeNullable).Index(i++).Name("Apprenticeship pathway");
            Map(m => m.LearningDelivery.AimType).Index(i++).Name("Aim type");
            Map(m => m.LarsLearningDelivery.FrameworkCommonComponent).Index(i++).Name("Common component code");
            Map(m => m.LearningDelivery.FundModel).Index(i++).Name("Funding model");
            Map(m => m.LearningDelivery.OrigLearnStartDateNullable).Index(i++).Name("Original learning start date");
            Map(m => m.LearningDelivery.LearnStartDate).Index(i++).Name("Learning start date");
            Map(m => m.LearningDelivery.LearnPlanEndDate).Index(i++).Name("Learning planned end date");
            Map(m => m.LearningDelivery.CompStatus).Index(i++).Name("Completion status");
            Map(m => m.LearningDelivery.LearnActEndDateNullable).Index(i++).Name("Learning actual end date");
            Map(m => m.LearningDelivery.AchDateNullable).Index(i++).Name("Achievement date");
            Map(m => m.LearningDelivery.OutcomeNullable).Index(i++).Name("Outcome");
            Map(m => m.LearningDelivery.PriorLearnFundAdjNullable).Index(i++).Name("Funding adjustment for prior learning");
            Map(m => m.LearningDelivery.OtherFundAdjNullable).Index(i++).Name("Other funding adjustment");
            Map(m => m.LearningDeliveryFAMs.LSF_Highest).Index(i++).Name("Learning delivery funding and monitoring type - learning support funding (if applicable)");
            Map(m => m.LearningDeliveryFAMs.LSF_EarliestDateFrom).Index(i++).Name("Learning delivery funding and monitoring type - LSF date applies from (earliest)");
            Map(m => m.LearningDeliveryFAMs.LSF_LatestDateTo).Index(i++).Name("Learning delivery funding and monitoring type - LSF date applies to (latest)");
            Map(m => m.LearningDeliveryFAMs.LDM1).Index(i++).Name("Learning delivery funding and monitoring type - learning delivery monitoring (A)");
            Map(m => m.LearningDeliveryFAMs.LDM2).Index(i++).Name("Learning delivery funding and monitoring type - learning delivery monitoring (B)");
            Map(m => m.LearningDeliveryFAMs.LDM3).Index(i++).Name("Learning delivery funding and monitoring type - learning delivery monitoring (C)");

            Map(m => m.LearningDeliveryFAMs.LDM4).Index(i++).Name("Learning delivery funding and monitoring type - learning delivery monitoring (D)");
            Map(m => m.LearningDeliveryFAMs.LDM5).Index(i++).Name("Learning delivery funding and monitoring type - learning delivery monitoring (E)");
            Map(m => m.LearningDeliveryFAMs.LDM6).Index(i++).Name("Learning delivery funding and monitoring type - learning delivery monitoring (F)");
            Map(m => m.LearningDeliveryFAMs.RES).Index(i++).Name("Learning delivery funding and monitoring type - restart indicator");
            Map(m => m.ProviderSpecDeliveryMonitoring.A).Index(i++).Name("Provider specified delivery monitoring (A)");
            Map(m => m.ProviderSpecDeliveryMonitoring.B).Index(i++).Name("Provider specified delivery monitoring (B)");
            Map(m => m.ProviderSpecDeliveryMonitoring.C).Index(i++).Name("Provider specified delivery monitoring (C)");
            Map(m => m.ProviderSpecDeliveryMonitoring.D).Index(i++).Name("Provider specified delivery monitoring (D)");
            Map(m => m.LearningDelivery.EPAOrgID).Index(i++).Name("End point assessment organisation");
            Map(m => m.Fm36LearningDelivery.PlannedNumOnProgInstalm).Index(i++).Name("Planned number of on programme instalments for aim");
            Map(m => m.LearningDelivery.PartnerUKPRNNullable).Index(i++).Name("Sub contracted or partnership UKPRN");
            Map(m => m.LearningDelivery.DelLocPostCode).Index(i++).Name("Delivery location postcode");
            Map(m => m.EmploymentStatus.EmpIdNullable).Index(i++).Name("Employer identifier on employment status date");
            Map(m => m.EmploymentStatus.EmpStat).Index(i++).Name("Employment status");
            Map(m => m.EmploymentStatus.DateEmpStatApp).Index(i++).Name("Employment status date");
            Map(m => m.EmpStatusMonitoringSmallEmployer).Index(i++).Name("Employment status monitoring - small employer");
            Map(m => m.PriceEpisodeValues.EpisodeStartDate).Index(i++).Name("Price episode start date");
            Map(m => m.PriceEpisodeValues.PriceEpisodeActualEndDate).Index(i++).Name("Price episode actual end date");

            Map(m => m.FundingLineType).Index(i++).Name("Funding line type");
            Map(m => m.PriceEpisodeValues.PriceEpisodeTotalTNPPrice).Index(i++).Name("Total price applicable to this episode").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PriceEpisodeValues.PriceEpisodeUpperBandLimit).Index(i++).Name("Funding band upper limit").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PriceEpisodeValues.PriceEpisodeUpperLimitAdjustment).Index(i++).Name("Price amount above funding band limit").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PriceEpisodeValues.PriceEpisodeCappedRemainingTNPAmount).Index(i++).Name("Price amount remaining (with upper limit applied) at start of this episode").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PriceEpisodeValues.PriceEpisodeCompletionElement).Index(i++).Name("Completion element (potential or actual earnings)").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.TotalPRMPreviousFundingYear).Index(i++).Name("Total employer contribution collected (PMR) in previous funding years").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.TotalPRMThisFundingYear).Index(i++).Name("Total employer contribution collected (PMR) in this funding year").TypeConverterOption.Format(DecimalFormat);

            Map(m => m.LearningDeliveryFAMTypeApprenticeshipContractType).Index(i++).Name("Learning delivery funding and monitoring type - apprenticeship contract type");
            Map(m => m.LearningDeliveryFAMTypeACTDateAppliesFrom).Index(i++).Name("Learning delivery funding and monitoring type - ACT date applies from");
            Map(m => m.LearningDeliveryFAMTypeACTDateAppliesTo).Index(i++).Name("Learning delivery funding and monitoring type - ACT date applies to");

            Map(m => m.PeriodisedValues.August.OnProgrammeEarnings).Index(i++).Name(@"August on programme earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.August.BalancingPaymentEarnings).Index(i++).Name(@"August balancing payment earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.August.AimCompletionEarnings).Index(i++).Name(@"August aim completion earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.August.LearningSupportEarnings).Index(i++).Name(@"August learning support earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.August.EnglishMathsOnProgrammeEarnings).Index(i++).Name(@"August English and maths on programme earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.August.EnglishMathsBalancingPaymentEarnings).Index(i++).Name(@"August English and maths balancing payment earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.August.DisadvantageEarnings).Index(i++).Name(@"August disadvantage earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.August.AdditionalPaymentForEmployers1618).Index(i++).Name(@"August 16-18 additional payments for employers").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.August.AdditionalPaymentForProviders1618).Index(i++).Name(@"August 16-18 additional payments for providers").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.August.AdditionalPaymentsForApprentices).Index(i++).Name(@"August additional payments for apprentices").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.August.FrameworkUpliftOnProgrammePayment1618).Index(i++).Name(@"August 16-18 framework uplift on programme payment").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.August.FrameworkUpliftBalancingPayment1618).Index(i++).Name(@"August 16-18 framework uplift balancing payment").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.August.FrameworkUpliftCompletionPayment1618).Index(i++).Name(@"August 16-18 framework uplift completion payment").TypeConverterOption.Format(DecimalFormat);


            Map(m => m.PeriodisedValues.September.OnProgrammeEarnings).Index(i++).Name(@"September on programme earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.September.BalancingPaymentEarnings).Index(i++).Name(@"September balancing payment earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.September.AimCompletionEarnings).Index(i++).Name(@"September aim completion earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.September.LearningSupportEarnings).Index(i++).Name(@"September learning support earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.September.EnglishMathsOnProgrammeEarnings).Index(i++).Name(@"September English and maths on programme earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.September.EnglishMathsBalancingPaymentEarnings).Index(i++).Name(@"September English and maths balancing payment earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.September.DisadvantageEarnings).Index(i++).Name(@"September disadvantage earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.September.AdditionalPaymentForEmployers1618).Index(i++).Name(@"September 16-18 additional payments for employers").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.September.AdditionalPaymentForProviders1618).Index(i++).Name(@"September 16-18 additional payments for providers").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.September.AdditionalPaymentsForApprentices).Index(i++).Name(@"September additional payments for apprentices").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.September.FrameworkUpliftOnProgrammePayment1618).Index(i++).Name(@"September 16-18 framework uplift on programme payment").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.September.FrameworkUpliftBalancingPayment1618).Index(i++).Name(@"September 16-18 framework uplift balancing payment").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.September.FrameworkUpliftCompletionPayment1618).Index(i++).Name(@"September 16-18 framework uplift completion payment").TypeConverterOption.Format(DecimalFormat);

            Map(m => m.PeriodisedValues.October.OnProgrammeEarnings).Index(i++).Name(@"October on programme earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.October.BalancingPaymentEarnings).Index(i++).Name(@"October balancing payment earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.October.AimCompletionEarnings).Index(i++).Name(@"October aim completion earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.October.LearningSupportEarnings).Index(i++).Name(@"October learning support earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.October.EnglishMathsOnProgrammeEarnings).Index(i++).Name(@"October English and maths on programme earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.October.EnglishMathsBalancingPaymentEarnings).Index(i++).Name(@"October English and maths balancing payment earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.October.DisadvantageEarnings).Index(i++).Name(@"October disadvantage earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.October.AdditionalPaymentForEmployers1618).Index(i++).Name(@"October 16-18 additional payments for employers").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.October.AdditionalPaymentForProviders1618).Index(i++).Name(@"October 16-18 additional payments for providers").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.October.AdditionalPaymentsForApprentices).Index(i++).Name(@"October additional payments for apprentices").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.October.FrameworkUpliftOnProgrammePayment1618).Index(i++).Name(@"October 16-18 framework uplift on programme payment").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.October.FrameworkUpliftBalancingPayment1618).Index(i++).Name(@"October 16-18 framework uplift balancing payment").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.October.FrameworkUpliftCompletionPayment1618).Index(i++).Name(@"October 16-18 framework uplift completion payment").TypeConverterOption.Format(DecimalFormat);


            Map(m => m.PeriodisedValues.November.OnProgrammeEarnings).Index(i++).Name(@"November on programme earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.November.BalancingPaymentEarnings).Index(i++).Name(@"November balancing payment earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.November.AimCompletionEarnings).Index(i++).Name(@"November aim completion earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.November.LearningSupportEarnings).Index(i++).Name(@"November learning support earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.November.EnglishMathsOnProgrammeEarnings).Index(i++).Name(@"November English and maths on programme earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.November.EnglishMathsBalancingPaymentEarnings).Index(i++).Name(@"November English and maths balancing payment earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.November.DisadvantageEarnings).Index(i++).Name(@"November disadvantage earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.November.AdditionalPaymentForEmployers1618).Index(i++).Name(@"November 16-18 additional payments for employers").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.November.AdditionalPaymentForProviders1618).Index(i++).Name(@"November 16-18 additional payments for providers").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.November.AdditionalPaymentsForApprentices).Index(i++).Name(@"November additional payments for apprentices").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.November.FrameworkUpliftOnProgrammePayment1618).Index(i++).Name(@"November 16-18 framework uplift on programme payment").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.November.FrameworkUpliftBalancingPayment1618).Index(i++).Name(@"November 16-18 framework uplift balancing payment").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.November.FrameworkUpliftCompletionPayment1618).Index(i++).Name(@"November 16-18 framework uplift completion payment").TypeConverterOption.Format(DecimalFormat);

            Map(m => m.PeriodisedValues.December.OnProgrammeEarnings).Index(i++).Name(@"December on programme earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.December.BalancingPaymentEarnings).Index(i++).Name(@"December balancing payment earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.December.AimCompletionEarnings).Index(i++).Name(@"December aim completion earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.December.LearningSupportEarnings).Index(i++).Name(@"December learning support earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.December.EnglishMathsOnProgrammeEarnings).Index(i++).Name(@"December English and maths on programme earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.December.EnglishMathsBalancingPaymentEarnings).Index(i++).Name(@"December English and maths balancing payment earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.December.DisadvantageEarnings).Index(i++).Name(@"December disadvantage earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.December.AdditionalPaymentForEmployers1618).Index(i++).Name(@"December 16-18 additional payments for employers").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.December.AdditionalPaymentForProviders1618).Index(i++).Name(@"December 16-18 additional payments for providers").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.December.AdditionalPaymentsForApprentices).Index(i++).Name(@"December additional payments for apprentices").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.December.FrameworkUpliftOnProgrammePayment1618).Index(i++).Name(@"December 16-18 framework uplift on programme payment").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.December.FrameworkUpliftBalancingPayment1618).Index(i++).Name(@"December 16-18 framework uplift balancing payment").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.December.FrameworkUpliftCompletionPayment1618).Index(i++).Name(@"December 16-18 framework uplift completion payment").TypeConverterOption.Format(DecimalFormat);


            Map(m => m.PeriodisedValues.January.OnProgrammeEarnings).Index(i++).Name(@"January on programme earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.January.BalancingPaymentEarnings).Index(i++).Name(@"January balancing payment earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.January.AimCompletionEarnings).Index(i++).Name(@"January aim completion earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.January.LearningSupportEarnings).Index(i++).Name(@"January learning support earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.January.EnglishMathsOnProgrammeEarnings).Index(i++).Name(@"January English and maths on programme earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.January.EnglishMathsBalancingPaymentEarnings).Index(i++).Name(@"January English and maths balancing payment earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.January.DisadvantageEarnings).Index(i++).Name(@"January disadvantage earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.January.AdditionalPaymentForEmployers1618).Index(i++).Name(@"January 16-18 additional payments for employers").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.January.AdditionalPaymentForProviders1618).Index(i++).Name(@"January 16-18 additional payments for providers").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.January.AdditionalPaymentsForApprentices).Index(i++).Name(@"January additional payments for apprentices").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.January.FrameworkUpliftOnProgrammePayment1618).Index(i++).Name(@"January 16-18 framework uplift on programme payment").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.January.FrameworkUpliftBalancingPayment1618).Index(i++).Name(@"January 16-18 framework uplift balancing payment").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.January.FrameworkUpliftCompletionPayment1618).Index(i++).Name(@"January 16-18 framework uplift completion payment").TypeConverterOption.Format(DecimalFormat);

            Map(m => m.PeriodisedValues.February.OnProgrammeEarnings).Index(i++).Name(@"February on programme earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.February.BalancingPaymentEarnings).Index(i++).Name(@"February balancing payment earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.February.AimCompletionEarnings).Index(i++).Name(@"February aim completion earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.February.LearningSupportEarnings).Index(i++).Name(@"February learning support earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.February.EnglishMathsOnProgrammeEarnings).Index(i++).Name(@"February English and maths on programme earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.February.EnglishMathsBalancingPaymentEarnings).Index(i++).Name(@"February English and maths balancing payment earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.February.DisadvantageEarnings).Index(i++).Name(@"February disadvantage earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.February.AdditionalPaymentForEmployers1618).Index(i++).Name(@"February 16-18 additional payments for employers").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.February.AdditionalPaymentForProviders1618).Index(i++).Name(@"February 16-18 additional payments for providers").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.February.AdditionalPaymentsForApprentices).Index(i++).Name(@"February additional payments for apprentices").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.February.FrameworkUpliftOnProgrammePayment1618).Index(i++).Name(@"February 16-18 framework uplift on programme payment").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.February.FrameworkUpliftBalancingPayment1618).Index(i++).Name(@"February 16-18 framework uplift balancing payment").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.February.FrameworkUpliftCompletionPayment1618).Index(i++).Name(@"February 16-18 framework uplift completion payment").TypeConverterOption.Format(DecimalFormat);


            Map(m => m.PeriodisedValues.March.OnProgrammeEarnings).Index(i++).Name(@"March on programme earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.March.BalancingPaymentEarnings).Index(i++).Name(@"March balancing payment earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.March.AimCompletionEarnings).Index(i++).Name(@"March aim completion earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.March.LearningSupportEarnings).Index(i++).Name(@"March learning support earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.March.EnglishMathsOnProgrammeEarnings).Index(i++).Name(@"March English and maths on programme earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.March.EnglishMathsBalancingPaymentEarnings).Index(i++).Name(@"March English and maths balancing payment earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.March.DisadvantageEarnings).Index(i++).Name(@"March disadvantage earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.March.AdditionalPaymentForEmployers1618).Index(i++).Name(@"March 16-18 additional payments for employers").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.March.AdditionalPaymentForProviders1618).Index(i++).Name(@"March 16-18 additional payments for providers").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.March.AdditionalPaymentsForApprentices).Index(i++).Name(@"March additional payments for apprentices").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.March.FrameworkUpliftOnProgrammePayment1618).Index(i++).Name(@"March 16-18 framework uplift on programme payment").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.March.FrameworkUpliftBalancingPayment1618).Index(i++).Name(@"March 16-18 framework uplift balancing payment").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.March.FrameworkUpliftCompletionPayment1618).Index(i++).Name(@"March 16-18 framework uplift completion payment").TypeConverterOption.Format(DecimalFormat);

            Map(m => m.PeriodisedValues.April.OnProgrammeEarnings).Index(i++).Name(@"April on programme earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.April.BalancingPaymentEarnings).Index(i++).Name(@"April balancing payment earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.April.AimCompletionEarnings).Index(i++).Name(@"April aim completion earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.April.LearningSupportEarnings).Index(i++).Name(@"April learning support earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.April.EnglishMathsOnProgrammeEarnings).Index(i++).Name(@"April English and maths on programme earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.April.EnglishMathsBalancingPaymentEarnings).Index(i++).Name(@"April English and maths balancing payment earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.April.DisadvantageEarnings).Index(i++).Name(@"April disadvantage earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.April.AdditionalPaymentForEmployers1618).Index(i++).Name(@"April 16-18 additional payments for employers").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.April.AdditionalPaymentForProviders1618).Index(i++).Name(@"April 16-18 additional payments for providers").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.April.AdditionalPaymentsForApprentices).Index(i++).Name(@"April additional payments for apprentices").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.April.FrameworkUpliftOnProgrammePayment1618).Index(i++).Name(@"April 16-18 framework uplift on programme payment").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.April.FrameworkUpliftBalancingPayment1618).Index(i++).Name(@"April 16-18 framework uplift balancing payment").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.April.FrameworkUpliftCompletionPayment1618).Index(i++).Name(@"April 16-18 framework uplift completion payment").TypeConverterOption.Format(DecimalFormat);

            Map(m => m.PeriodisedValues.May.OnProgrammeEarnings).Index(i++).Name(@"May on programme earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.May.BalancingPaymentEarnings).Index(i++).Name(@"May balancing payment earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.May.AimCompletionEarnings).Index(i++).Name(@"May aim completion earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.May.LearningSupportEarnings).Index(i++).Name(@"May learning support earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.May.EnglishMathsOnProgrammeEarnings).Index(i++).Name(@"May English and maths on programme earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.May.EnglishMathsBalancingPaymentEarnings).Index(i++).Name(@"May English and maths balancing payment earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.May.DisadvantageEarnings).Index(i++).Name(@"May disadvantage earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.May.AdditionalPaymentForEmployers1618).Index(i++).Name(@"May 16-18 additional payments for employers").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.May.AdditionalPaymentForProviders1618).Index(i++).Name(@"May 16-18 additional payments for providers").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.May.AdditionalPaymentsForApprentices).Index(i++).Name(@"May additional payments for apprentices").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.May.FrameworkUpliftOnProgrammePayment1618).Index(i++).Name(@"May 16-18 framework uplift on programme payment").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.May.FrameworkUpliftBalancingPayment1618).Index(i++).Name(@"May 16-18 framework uplift balancing payment").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.May.FrameworkUpliftCompletionPayment1618).Index(i++).Name(@"May 16-18 framework uplift completion payment").TypeConverterOption.Format(DecimalFormat);


            Map(m => m.PeriodisedValues.June.OnProgrammeEarnings).Index(i++).Name(@"June on programme earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.June.BalancingPaymentEarnings).Index(i++).Name(@"June balancing payment earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.June.AimCompletionEarnings).Index(i++).Name(@"June aim completion earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.June.LearningSupportEarnings).Index(i++).Name(@"June learning support earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.June.EnglishMathsOnProgrammeEarnings).Index(i++).Name(@"June English and maths on programme earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.June.EnglishMathsBalancingPaymentEarnings).Index(i++).Name(@"June English and maths balancing payment earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.June.DisadvantageEarnings).Index(i++).Name(@"June disadvantage earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.June.AdditionalPaymentForEmployers1618).Index(i++).Name(@"June 16-18 additional payments for employers").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.June.AdditionalPaymentForProviders1618).Index(i++).Name(@"June 16-18 additional payments for providers").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.June.AdditionalPaymentsForApprentices).Index(i++).Name(@"June additional payments for apprentices").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.June.FrameworkUpliftOnProgrammePayment1618).Index(i++).Name(@"June 16-18 framework uplift on programme payment").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.June.FrameworkUpliftBalancingPayment1618).Index(i++).Name(@"June 16-18 framework uplift balancing payment").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.June.FrameworkUpliftCompletionPayment1618).Index(i++).Name(@"June 16-18 framework uplift completion payment").TypeConverterOption.Format(DecimalFormat);

            Map(m => m.PeriodisedValues.July.OnProgrammeEarnings).Index(i++).Name(@"July on programme earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.July.BalancingPaymentEarnings).Index(i++).Name(@"July balancing payment earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.July.AimCompletionEarnings).Index(i++).Name(@"July aim completion earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.July.LearningSupportEarnings).Index(i++).Name(@"July learning support earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.July.EnglishMathsOnProgrammeEarnings).Index(i++).Name(@"July English and maths on programme earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.July.EnglishMathsBalancingPaymentEarnings).Index(i++).Name(@"July English and maths balancing payment earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.July.DisadvantageEarnings).Index(i++).Name(@"July disadvantage earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.July.AdditionalPaymentForEmployers1618).Index(i++).Name(@"July 16-18 additional payments for employers").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.July.AdditionalPaymentForProviders1618).Index(i++).Name(@"July 16-18 additional payments for providers").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.July.AdditionalPaymentsForApprentices).Index(i++).Name(@"July additional payments for apprentices").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.July.FrameworkUpliftOnProgrammePayment1618).Index(i++).Name(@"July 16-18 framework uplift on programme payment").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.July.FrameworkUpliftBalancingPayment1618).Index(i++).Name(@"July 16-18 framework uplift balancing payment").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.July.FrameworkUpliftCompletionPayment1618).Index(i++).Name(@"July 16-18 framework uplift completion payment").TypeConverterOption.Format(DecimalFormat);


            Map(m => m.PeriodisedValues.OnProgrammeEarningsTotal).Index(i++).Name("Total on programme earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.BalancingPaymentEarningsTotal).Index(i++).Name("Total balancing payment earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.AimCompletionEarningsTotal).Index(i++).Name("Total aim completion earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.LearningSupportEarningsTotal).Index(i++).Name("Total learning support earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.EnglishMathsOnProgrammeEarningsTotal).Index(i++).Name("Total English and maths on programme earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.EnglishMathsBalancingPaymentEarningsTotal).Index(i++).Name("Total English and maths balancing payment earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.DisadvantageEarningsTotal).Index(i++).Name("Total disadvantage earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.AdditionalPaymentForEmployers1618Total).Index(i++).Name("Total 16-18 additional payments for employers").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.AdditionalPaymentForProviders1618Total).Index(i++).Name("Total 16-18 additional payments for providers").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.AdditionalPaymentsForApprenticesTotal).Index(i++).Name("Total additional payments for apprentices").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.FrameworkUpliftOnProgrammePayment1618Total).Index(i++).Name("Total 16-18 framework uplift on programme payment").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.FrameworkUpliftBalancingPayment1618Total).Index(i++).Name("Total 16-18 framework uplift balancing payment").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.PeriodisedValues.FrameworkUpliftCompletionPayment1618Total).Index(i++).Name("Total 16-18 framework uplift completion payment").TypeConverterOption.Format(DecimalFormat);

            Map(m => m.OfficialSensitive).Index(i).Name("OFFICIAL - SENSITIVE");
        }
    }
}
