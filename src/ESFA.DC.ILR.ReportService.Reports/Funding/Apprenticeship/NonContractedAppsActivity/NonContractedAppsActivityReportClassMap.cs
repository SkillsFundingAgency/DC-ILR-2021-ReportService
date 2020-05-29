using CsvHelper.Configuration;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Apprenticeship.NonContractedAppsActivity
{
    public class NonContractedAppsActivityReportClassMap : ClassMap<NonContractedAppsActivityReportModel>
    {
        private const string DecimalFormat = "0.00000";

        public NonContractedAppsActivityReportClassMap()
        {
            int i = 0;
            Map(m => m.LearnRefNumber).Index(i++).Name("Learner reference number");
            Map(m => m.Learner.ULN).Index(i++).Name("Unique learner number");
            Map(m => m.Learner.FamilyName).Index(i++).Name("Family name");
            Map(m => m.Learner.GivenNames).Index(i++).Name("Given names");
            Map(m => m.Learner.DateOfBirthNullable).Index(i++).Name("Date of birth");
            Map(m => m.Learner.CampId).Index(i++).Name("Campus identifier");
            Map(m => m.ProviderSpecLearnerMonitoring.A).Index(i++).Name("Provider specified learner monitoring (A)");
            Map(m => m.ProviderSpecLearnerMonitoring.B).Index(i++).Name("Provider specified learner monitoring (B)");
            Map(m => m.AimSeqNumber).Index(i++).Name("Aim sequence number");

            Map(m => m.LearningDelivery.LearnAimRef).Index(i++).Name("Learning aim reference");
            Map(m => m.LarsLearningDelivery.LearnAimRefTitle).Index(i++).Name("Learning aim title");

            Map(m => m.LearningDelivery.SWSupAimId).Index(i++).Name("Software supplier aim identifier");
            Map(m => m.LearningDelivery.ProgTypeNullable).Index(i++).Name("Programme type");
            Map(m => m.LearningDelivery.StdCodeNullable).Index(i++).Name("Standard code");
            Map(m => m.LearningDelivery.FworkCodeNullable).Index(i++).Name("Framework code");
            Map(m => m.LearningDelivery.PwayCodeNullable).Index(i++).Name("Apprenticeship pathway");
            Map(m => m.LearningDelivery.AimType).Index(i++).Name("Aim type");
            Map(m => m.LearningDelivery.OrigLearnStartDateNullable).Index(i++).Name("Original learning start date");
            Map(m => m.LearningDelivery.LearnStartDate).Index(i++).Name("Learning start date");
            Map(m => m.LearningDelivery.LearnPlanEndDate).Index(i++).Name("Learning planned end date");
            Map(m => m.LearningDelivery.LearnActEndDateNullable).Index(i++).Name("Learning actual end date");
            Map(m => m.LearningDelivery.AchDateNullable).Index(i++).Name("Achievement date");

            Map(m => m.LearningDeliveryFAMs.EEF).Index(i++).Name("Learning delivery funding and monitoring type - eligibility for enhanced apprenticeship funding");

            Map(m => m.ProviderSpecDeliveryMonitoring.A).Index(i++).Name("Provider specified delivery monitoring (A)");
            Map(m => m.ProviderSpecDeliveryMonitoring.B).Index(i++).Name("Provider specified delivery monitoring (B)");
            Map(m => m.ProviderSpecDeliveryMonitoring.C).Index(i++).Name("Provider specified delivery monitoring (C)");
            Map(m => m.ProviderSpecDeliveryMonitoring.D).Index(i++).Name("Provider specified delivery monitoring (D)");

            Map(m => m.PriceEpisodeValues.EpisodeStartDate).Index(i++).Name("Price episode start date");
            Map(m => m.PriceEpisodeValues.PriceEpisodeActualEndDate).Index(i++).Name("Price episode actual end date");
            Map(m => m.Fm36LearningDelivery.AppAdjLearnStartDate).Index(i++).Name("Apprenticeship adjusted learning start date");
            Map(m => m.Fm36LearningDelivery.AgeAtProgStart).Index(i++).Name("Age at programme start");
            Map(m => m.FundingLineType).Index(i++).Name("Funding line type");

            Map(m => m.LearningDeliveryFAMTypeApprenticeshipContractType).Index(i++).Name("Learning delivery funding and monitoring type - apprenticeship contract type");
            Map(m => m.LearningDeliveryFAMTypeACTDateAppliesFrom).Index(i++).Name("Learning delivery funding and monitoring type - ACT date applies from");
            Map(m => m.LearningDeliveryFAMTypeACTDateAppliesTo).Index(i++).Name("Learning delivery funding and monitoring type - ACT date applies to");

            Map(m => m.AugustTotal).Index(i++).Name("August total earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.SeptemberTotal).Index(i++).Name("September total earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.OctoberTotal).Index(i++).Name("October total earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.NovemberTotal).Index(i++).Name("November total earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.DecemberTotal).Index(i++).Name("December total earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.JanuaryTotal).Index(i++).Name("January total earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.FebruaryTotal).Index(i++).Name("February total earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.MarchTotal).Index(i++).Name("March total earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.AprilTotal).Index(i++).Name("April total earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.MayTotal).Index(i++).Name("May total earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.JuneTotal).Index(i++).Name("June total earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.JulyTotal).Index(i++).Name("July total earnings").TypeConverterOption.Format(DecimalFormat);
            Map(m => m.Total).Index(i++).Name("Total earnings").TypeConverterOption.Format(DecimalFormat);

            Map(m => m.OfficialSensitive).Index(i).Name("OFFICIAL - SENSITIVE");
        }
    }
}
