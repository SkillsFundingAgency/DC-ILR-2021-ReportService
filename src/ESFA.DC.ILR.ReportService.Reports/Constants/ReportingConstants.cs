using System;

namespace ESFA.DC.ILR.ReportService.Reports.Constants
{
    public static class ReportingConstants
    {
        public const string EmploymentStatusMonitoringTypeSEM = "SEM";

        // LearningDelivery FAM Codes
        public const string LearningDeliveryFAMCodeLSF = "LSF";
        public const string LearningDeliveryFAMCodeLDM = "LDM";
        public const string LearningDeliveryFAMCodeRES = "RES";
        public const string LearningDeliveryFAMCodeACT = "ACT";
        public const string LearningDeliveryFAMCodeSOF = "SOF";

        public const string LearningDeliveryFAMCode107 = "107";

        // learner FAM codes
        public const string LearnerFAMCodeEHC = "EHC";
        public const string LearnerFAMCodeHNS = "HNS";

        // Funding Summary
        public const string ALBInfoText = "Please note that loads bursary funding for learners who are funded within the Career Learning Pilot is not included here. Please refer to the separate Career Learning Pilot report.";

        // Exceptional Learning Support
        public const string ExceptionalLearningInfoText = "Exceptional learning support is paid out of a separate budget, not the budgets noted above. This is provided for information only and you will be informed separately of any payments made. Note payments are made following the last ILR collection of the funding year.";

        // ALB
        public const string ALBSupportPayment = "ALBSupportPayment";
        public const string AreaUpliftBalPayment = "AreaUpliftBalPayment";
        public const string AreaUpliftOnProgPayment = "AreaUpliftOnProgPayment";

        // Fundline types
        public const string AdvancedLearnerLoansBursary = "Advanced Learner Loans Bursary";
        public const string AdvancedLearnerLoansBursary_ExcessSupport = "Excess Support: Advanced Learner Loans Bursary";
        public const string AdvancedLearnerLoansBursary_AuthorisedClaims = "Authorised Claims: Advanced Learner Loans Bursary";

        public const string AEBOtherLearning = "AEB - Other Learning";
        public const string AEBOtherLearning_AuthorisedClaims = "Authorised Claims: AEB-Other Learning";
        public const string AEBOtherLearning_PrincesTrust = "Princes Trust: AEB-Other Learning";
        public const string AEBOtherLearning_ExcessLearningSupport = "Excess Learning Support: AEB-Other Learning";

        public const string Traineeships1924 = "19-24 Traineeships";
        public const string Traineeships1924_NonProcured = "19-24 Traineeship (non-procured)";
        public const string Traineeships1924_LearnerSupport = "Learner Support: 19-24 Traineeships";
        public const string Traineeships1924_ExcessLearningSupport = "Excess Learning Support: 19-24 Traineeships";
        public const string Traineeships1924_AuthorisedClaims = "Authorised Claims: 19-24 Traineeships";

        // Dates
        public const string Year = "2018/19";
        
        // Value Provider
        public const string Zero = "0";
        public const string NotApplicable = "n/a";
        public static string DateTimeMin = DateTime.MinValue.ToString("dd/MM/yyyy");

        // Dates
        public static readonly DateTime BeginningOfYear = new DateTime(2018, 8, 1);
        public static readonly DateTime EndOfYear = new DateTime(2019, 7, 31, 23, 59, 59);
    }
}
