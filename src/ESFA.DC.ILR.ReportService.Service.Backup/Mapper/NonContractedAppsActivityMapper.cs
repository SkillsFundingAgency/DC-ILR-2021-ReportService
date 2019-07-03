using CsvHelper.Configuration;
using ESFA.DC.ILR.ReportService.Model.ReportModels;

namespace ESFA.DC.ILR.ReportService.Service.Mapper
{
    public class NonContractedAppsActivityMapper : ClassMap<NonContractedAppsActivityModel>
    {
        public NonContractedAppsActivityMapper()
        {
            int i = 0;
            Map(m => m.LearnerReferenceNumber).Index(i++).Name("Learner reference number");
            Map(m => m.UniqueLearnerNumber).Index(i++).Name("Unique learner number");
            Map(m => m.DateOfBirth).Index(i++).Name("Date of birth");
            Map(m => m.CampusIdentifier).Index(i++).Name("Campus identifier");
            Map(m => m.ProviderSpecifiedLearnerMonitoringA).Index(i++).Name("Provider specified learner monitoring (A)");
            Map(m => m.ProviderSpecifiedLearnerMonitoringB).Index(i++).Name("Provider specified learner monitoring (B)");
            Map(m => m.AimSeqNumber).Index(i++).Name("Aim sequence number");
            Map(m => m.LearningAimReference).Index(i++).Name("Learning aim reference");
            Map(m => m.LearningAimTitle).Index(i++).Name("Learning aim title");
            Map(m => m.SoftwareSupplierAimIdentifier).Index(i++).Name("Software supplier aim identifier");
            Map(m => m.ProgrammeType).Index(i++).Name("Programme type");
            Map(m => m.StandardCode).Index(i++).Name("Standard code");
            Map(m => m.FrameworkCode).Index(i++).Name("Framework code");
            Map(m => m.ApprenticeshipPathway).Index(i++).Name("Apprenticeship pathway");
            Map(m => m.AimType).Index(i++).Name("Aim type");
            Map(m => m.OriginalLearningStartDate).Index(i++).Name("Original learning start date");
            Map(m => m.LearningStartDate).Index(i++).Name("Learning start date");
            Map(m => m.LearningPlannedEndDate).Index(i++).Name("Learning planned end date");
            Map(m => m.LearningActualEndDate).Index(i++).Name("Learning actual end date");
            Map(m => m.LearningDeliveryFAMTypeEEF).Index(i++).Name("Learning delivery funding and monitoring type – eligibility for enhanced apprenticeship funding");
            Map(m => m.ProviderSpecifiedDeliveryMonitoringA).Index(i++).Name("Provider specified delivery monitoring (A)");
            Map(m => m.ProviderSpecifiedDeliveryMonitoringB).Index(i++).Name("Provider specified delivery monitoring (B)");
            Map(m => m.ProviderSpecifiedDeliveryMonitoringC).Index(i++).Name("Provider specified delivery monitoring (C)");
            Map(m => m.ProviderSpecifiedDeliveryMonitoringD).Index(i++).Name("Provider specified delivery monitoring (D)");
            Map(m => m.PriceEpisodeStartDate).Index(i++).Name("Price episode start date");
            Map(m => m.PriceEpisodeActualEndDate).Index(i++).Name("Price episode actual end date");
            Map(m => m.AppAdjLearnStartDate).Index(i++).Name("Apprenticeship adjusted learning start date");
            Map(m => m.AgeAtProgrammeStart).Index(i++).Name("Age at programme start");
            Map(m => m.FundingLineType).Index(i++).Name("Funding line type");
            Map(m => m.LearningDeliveryFAMTypeApprenticeshipContractType).Index(i++).Name("Learning delivery funding and monitoring type - apprenticeship contract type");
            Map(m => m.LearningDeliveryFAMTypeACTDateAppliesFrom).Index(i++).Name("Learning delivery funding and monitoring type - ACT date applies from");
            Map(m => m.LearningDeliveryFAMTypeACTDateAppliesTo).Index(i++).Name("Learning delivery funding and monitoring type - ACT date applies to");
            Map(m => m.AugustTotalEarnings).Index(i++).Name("August total earnings");
            Map(m => m.SeptemberTotalEarnings).Index(i++).Name("September total earnings");
            Map(m => m.OctoberTotalEarnings).Index(i++).Name("October total earnings");
            Map(m => m.NovemberTotalEarnings).Index(i++).Name("November total earnings");
            Map(m => m.DecemberTotalEarnings).Index(i++).Name("December total earnings");
            Map(m => m.JanuaryTotalEarnings).Index(i++).Name("January total earnings");
            Map(m => m.FebruaryTotalEarnings).Index(i++).Name("February total earnings");
            Map(m => m.MarchTotalEarnings).Index(i++).Name("March total earnings");
            Map(m => m.AprilTotalEarnings).Index(i++).Name("April total earnings");
            Map(m => m.MayTotalEarnings).Index(i++).Name("May total earnings");
            Map(m => m.JuneTotalEarnings).Index(i++).Name("June total earnings");
            Map(m => m.JulyTotalEarnings).Index(i++).Name("July total earnings");
            Map(m => m.TotalEarnings).Index(i++).Name("Total earnings");
            Map(m => m.OfficialSensitive).Index(i++).Name("OFFICIAL - SENSITIVE");
        }
    }
}
