using System;

namespace ESFA.DC.ILR.ReportService.Models.EAS
{
    public class EasPaymentValue
    {
        public EasPaymentValue(Decimal? paymentValue, int? devolvedAraSofs)
        {
            this.PaymentValue = paymentValue;
            this.DevolvedAreaSofs = devolvedAraSofs;
        }

        public Decimal? PaymentValue { get; set; }

        public int? DevolvedAreaSofs { get; set; }
    }
}
