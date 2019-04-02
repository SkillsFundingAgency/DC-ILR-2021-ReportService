﻿
namespace ESFA.DC.ILR.ReportService.Model.ReportModels.PeriodEnd
{
    public class AppsCoInvestmentContributionsModel
    {
        public string LearnRefNumber { get; set; }

        public long UniqueLearnerNumber { get; set; }

        public string LearningStartDate { get; set; }

        public int? ProgType { get; set; }

        public int? StandardCode { get; set; }

        public int? FrameworkCode { get; set; }

        public int? ApprenticeshipPathway { get; set; }

        public string SoftwareSupplierAimIdentifier { get; set; }

        public string LearningDeliveryFAMTypeApprenticeshipContractType { get; set; }

        public int? EmployerIdentifierAtStartOfLearning { get; set; }

        public string EmployerNameFromApprenticeshipService { get; set; }

        public decimal TotalPMRPreviousFundingYears { get; set; }

        public decimal TotalCoInvestmentDueFromEmployerInPreviousFundingYears { get; set; }

        public decimal TotalPMRThisFundingYear { get; set; }

        public decimal TotalCoInvestmentDueFromEmployerThisFundingYear { get; set; }

        public decimal PercentageOfCoInvestmentCollected { get; set; }

        public decimal CoInvestmentDueFromEmployerForAugust { get; set; }

        public decimal CoInvestmentDueFromEmployerForSeptember { get; set; }

        public decimal CoInvestmentDueFromEmployerForOctober { get; set; }

        public decimal CoInvestmentDueFromEmployerForNovember { get; set; }

        public decimal CoInvestmentDueFromEmployerForDecember { get; set; }

        public decimal CoInvestmentDueFromEmployerForJanuary { get; set; }

        public decimal CoInvestmentDueFromEmployerForFebruary { get; set; }

        public decimal CoInvestmentDueFromEmployerForMarch { get; set; }

        public decimal CoInvestmentDueFromEmployerForApril { get; set; }

        public decimal CoInvestmentDueFromEmployerForMay { get; set; }

        public decimal CoInvestmentDueFromEmployerForJune { get; set; }

        public decimal CoInvestmentDueFromEmployerForJuly { get; set; }

        public decimal CoInvestmentDueFromEmployerForR13 { get; set; }

        public decimal CoInvestmentDueFromEmployerForR14 { get; set; }

        public string OfficialSensitive { get; }

    }
}