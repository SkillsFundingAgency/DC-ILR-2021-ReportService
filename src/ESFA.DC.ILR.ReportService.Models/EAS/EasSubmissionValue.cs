using System.Collections.Generic;

namespace ESFA.DC.ILR.ReportService.Models.EAS
{
    public class EasSubmissionValue
    {
        public string PaymentName { get; set; }

        public string AdjustmentTypeName { get; set; }

        public List<EasPaymentValue> Period1 { get; set; }

        public List<EasPaymentValue> Period2 { get; set; }

        public List<EasPaymentValue> Period3 { get; set; }

        public List<EasPaymentValue> Period4 { get; set; }

        public List<EasPaymentValue> Period5 { get; set; }

        public List<EasPaymentValue> Period6 { get; set; }

        public List<EasPaymentValue> Period7 { get; set; }

        public List<EasPaymentValue> Period8 { get; set; }

        public List<EasPaymentValue> Period9 { get; set; }

        public List<EasPaymentValue> Period10 { get; set; }

        public List<EasPaymentValue> Period11 { get; set; }

        public List<EasPaymentValue> Period12 { get; set; }
    }
}
