using System;

namespace ESFA.DC.ILR1819.ReportService.Service
{
    public static class Constants
    {
        public const string EmploymentStatusMonitoringTypeSEM = "SEM";

        // LearningDelivery FAM Codes
        public const string LearningDeliveryFAMCodeLSF = "LSF";
        public const string LearningDeliveryFAMCodeLDM1 = "LDM1";
        public const string LearningDeliveryFAMCodeLDM2 = "LDM2";
        public const string LearningDeliveryFAMCodeLDM3 = "LDM3";
        public const string LearningDeliveryFAMCodeLDM4 = "LDM4";
        public const string LearningDeliveryFAMCodeRES = "RES";
        public const string LearningDeliveryFAMCodeACT = "ACT";

        // FM25 FundLines
        public const string DirectFundedStudents1416FundLine = "14-16 Direct Funded Students";
        public const string Students1619FundLine = "16-19 Students (excluding High Needs Students)";
        public const string HighNeedsStudents1619FundLine = "16-19 High Needs Students";
        public const string StudentsWithEHCP1924FundLine = "19-24 Students with an EHCP";
        public const string ContinuingStudents19PlusFundLine = "19+ Continuing Students (excluding EHCP)";

        // FM25 Attributes
        public const string Fm25OnProgrammeAttributeName = "OnProgPayment";

        // FM35 Attributes
        public const string Fm35OnProgrammeAttributeName = "OnProgPayment";
        public const string Fm35BalancingAttributeName = "BalancePayment";
        public const string Fm35JobOutcomeAchievementAttributeName = "EmpOutcomePay";
        public const string Fm35AimAchievementAttributeName = "AchievePayment";
        public const string Fm35LearningSupportAttributeName = "LearnSuppFundCash";
        public const string Fm35AimAchievementPercentAttributeName = "AchievePayPct";

        // FM36 Attributes
        public const string Fm36PriceEpisodeOnProgPaymentAttributeName = "PriceEpisodeOnProgPayment";
        public const string Fm3PriceEpisodeBalancePaymentAttributeName = "PriceEpisodeBalancePayment";
        public const string Fm36PriceEpisodeCompletionPaymentAttributeName = "PriceEpisodeCompletionPayment";
        public const string Fm36LearnSuppFundCashAttributeName = "LearnSuppFundCash";
        public const string Fm36PriceEpisodeLSFCashAttributeName = "PriceEpisodeLSFCash";
        public const string Fm36MathEngOnProgPaymentAttributeName = "MathEngOnProgPayment";
        public const string Fm36PriceEpisodeFirstDisadvantagePaymentAttributeName = "PriceEpisodeFirstDisadvantagePayment";
        public const string Fm36PriceEpisodeSecondDisadvantagePaymentAttributeName = "PriceEpisodeSecondDisadvantagePayment";
        public const string Fm36PriceEpisodeFirstEmp1618PayAttributeName = "PriceEpisodeFirstEmp1618Pay";
        public const string Fm36PriceEpisodeSecondEmp1618PayAttributeName = "PriceEpisodeSecondEmp1618Pay";
        public const string Fm36PriceEpisodeFirstProv1618PayAttributeName = "PriceEpisodeFirstProv1618Pay";
        public const string Fm36PriceEpisodeSecondProv1618PayAttributeName = "PriceEpisodeSecondProv1618Pay";
        public const string Fm36PriceEpisodeLearnerAdditionalPaymentAttributeName = "PriceEpisodeLearnerAdditionalPayment";
        public const string Fm36PriceEpisodeApplic1618FrameworkUpliftOnProgPaymentAttributeName = "PriceEpisodeApplic1618FrameworkUpliftOnProgPayment";
        public const string Fm36PriceEpisodeApplic1618FrameworkUpliftBalancingAttributeName = "PriceEpisodeApplic1618FrameworkUpliftBalancing";
        public const string Fm36PriceEpisodeApplic1618FrameworkUpliftCompletionPaymentAttributeName = "PriceEpisodeApplic1618FrameworkUpliftCompletionPayment";
        public const string Fm36ProgrammeAimOnProgPayment = "ProgrammeAimOnProgPayment";
        public const string Fm36ProgrammeAimBalPayment = "ProgrammeAimBalPayment";
        public const string Fm36ProgrammeAimCompletionPayment = "ProgrammeAimCompletionPayment";
        public const string Fm36ProgrammeAimProgFundIndMinCoInvest = "ProgrammeAimProgFundIndMinCoInvest";
        public const string Fm36MathEngOnProgPayment = "MathEngOnProgPayment";
        public const string Fm36MathEngBalPayment = "MathEngBalPayment";
        public const string Fm36LDApplic1618FrameworkUpliftBalancingPayment = "LDApplic1618FrameworkUpliftBalancingPayment";
        public const string Fm36LDApplic1618FrameworkUpliftCompletionPayment = "LDApplic1618FrameworkUpliftCompletionPayment";
        public const string Fm36LDApplic1618FrameworkUpliftOnProgPayment = "LDApplic1618FrameworkUpliftOnProgPayment";
        public const string Fm36DisadvFirstPayment = "DisadvFirstPayment";
        public const string Fm36DisadvSecondPayment = "DisadvSecondPayment";
        public const string Fm36LearnDelFirstProv1618Pay = "LearnDelFirstProv1618Pay";
        public const string Fm36LearnDelSecondProv1618Pay = "LearnDelSecondProv1618Pay";
        public const string Fm36LearnDelFirstEmp1618Pay = "LearnDelFirstEmp1618Pay";
        public const string Fm36LearnDelSecondEmp1618Pay = "LearnDelSecondEmp1618Pay";
        public const string Fm36LearnSuppFundCash = "LearnSuppFundCash";
        public const string Fm36LearnDelLearnAddPayment = "LearnDelLearnAddPayment";

        // FM81
        public const string Fm81CoreGovContPayment = "CoreGovContPayment";
        public const string Fm81MathEngBalPayment = "MathEngBalPayment";
        public const string Fm81MathEngOnProgPayment = "MathEngOnProgPayment";
        public const string Fm81AchPayment = "AchPayment";
        public const string Fm81SmallBusPayment = "SmallBusPayment";
        public const string Fm81YoungAppPayment = "YoungAppPayment";
        public const string Fm81LearnSuppFundCash = "LearnSuppFundCash";

        // Funding Summary
        public const string ALBInfoText = "Please note that loads bursary funding for learners who are funded within the Career Learning Pilot is not included here. Please refer to the separate Career Learning Pilot report.";

        // Dates
        public static readonly DateTime FirstOfAugust = new DateTime(2018, 8, 1);
        public static readonly DateTime EndOfYear = new DateTime(2019, 7, 31, 23, 59, 59);
    }
}
