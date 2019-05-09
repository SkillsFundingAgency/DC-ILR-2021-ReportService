namespace ESFA.DC.ILR.ReportService.Model.ReportModels
{
    public class NonContractedAppsActivityModel
    {
        public string LearnerReferenceNumber { get; set; }

        public long UniqueLearnerNumber { get; set; }

        public string DateOfBirth { get; set; }

        public string CampusIdentifier { get; set; }

        public string ProviderSpecifiedLearnerMonitoringA { get; set; }

        public string ProviderSpecifiedLearnerMonitoringB { get; set; }

        public int? AimSeqNumber { get; set; }

        public string LearningAimReference { get; set; }

        public string LearningAimTitle { get; set; }

        public string SoftwareSupplierAimIdentifier { get; set; }

        public int? ProgrammeType { get; set; }

        public int? StandardCode { get; set; }

        public int? FrameworkCode { get; set; }

        public int? ApprenticeshipPathway { get; set; }

        public int AimType { get; set; }

        public string OriginalLearningStartDate { get; set; }

        public string LearningStartDate { get; set; }

        public string LearningPlannedEndDate { get; set; }

        public string LearningActualEndDate { get; set; }

        public string LearningDeliveryFAMTypeEEF { get; set; }

        public string ProviderSpecifiedDeliveryMonitoringA { get; set; }

        public string ProviderSpecifiedDeliveryMonitoringB { get; set; }

        public string ProviderSpecifiedDeliveryMonitoringC { get; set; }

        public string ProviderSpecifiedDeliveryMonitoringD { get; set; }

        public string PriceEpisodeStartDate { get; set; }

        public string PriceEpisodeActualEndDate { get; set; }

        public string AppAdjLearnStartDate { get; set; }

        public string AgeAtProgrammeStart { get; set; }

        public string FundingLineType { get; set; }

        public string LearningDeliveryFAMTypeApprenticeshipContractType { get; set; }

        public string LearningDeliveryFAMTypeACTDateAppliesFrom { get; set; }

        public string LearningDeliveryFAMTypeACTDateAppliesTo { get; set; }

        public decimal AugustTotalEarnings { get; set; }

        public decimal SeptemberTotalEarnings { get; set; }

        public decimal OctoberTotalEarnings { get; set; }

        public decimal NovemberTotalEarnings { get; set; }

        public decimal DecemberTotalEarnings { get; set; }

        public decimal JanuaryTotalEarnings { get; set; }

        public decimal FebruaryTotalEarnings { get; set; }

        public decimal MarchTotalEarnings { get; set; }

        public decimal AprilTotalEarnings { get; set; }

        public decimal MayTotalEarnings { get; set; }

        public decimal JuneTotalEarnings { get; set; }

        public decimal JulyTotalEarnings { get; set; }

        public decimal TotalEarnings { get; set; }

        public string OfficialSensitive { get; }
    }
}