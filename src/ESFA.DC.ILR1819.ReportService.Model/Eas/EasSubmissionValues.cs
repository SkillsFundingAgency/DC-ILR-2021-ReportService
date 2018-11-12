using System;

namespace ESFA.DC.ILR1819.ReportService.Model.Eas
{
    public class EasSubmissionValues
    {
        public int CollectionPeriod { get; set; }

        public int PaymentId { get; set; }

        public Decimal PaymentValue { get; set; }

        public string PaymentTypeName { get; set; }
    }
}
