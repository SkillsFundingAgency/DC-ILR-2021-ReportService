namespace ESFA.DC.ILR.ReportService.Reports.Constants
{
    public static class AppFinRecordConstants
    {
        public static class Types
        {
            /// <summary>
            /// The total negotiated price
            /// </summary>
            public const string TotalNegotiatedPrice = "TNP";

            /// <summary>
            /// The payment record
            /// </summary>
            public const string PaymentRecord = "PMR";
        }

        /// <summary>
        /// Total Negotiated Price Codes / TNP Codes
        /// </summary>
        public static class TotalNegotiatedPriceCodes
        {
            /// <summary>
            /// Total training price / 1
            /// </summary>
            public const int TotalTrainingPrice = 1;

            /// <summary>
            /// Total training price / 1
            /// </summary>
            public const int TotalAssessmentPrice = 2;

            /// <summary>
            /// Total assessment price / 3
            /// </summary>
            public const int ResidualTrainingPrice = 3;
        }

        /// <summary>
        /// Payment Record Codes / PMR Codes
        /// </summary>
        public static class PaymentRecordCodes
        {
            /// <summary>
            /// Training Payment / 1
            /// </summary>
            public const int TrainingPayment = 1;

            /// <summary>
            /// Assessment Payment / 2
            /// </summary>
            public const int AssessmentPayment = 2;

            /// <summary>
            /// Employer payment reinmbursed by provider / 3
            /// </summary>
            public const int EmployerPaymentReimbursedByProvider = 3;
        }
    }
}
