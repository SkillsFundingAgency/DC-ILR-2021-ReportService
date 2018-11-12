namespace ESFA.DC.ILR1819.ReportService.Model.Eas
{
    public static class EasPaymentTypesMapper
    { 
        public static EasPaymentType ToEasPaymentTypes(this EAS1819.EF.PaymentTypes paymentTypes)
        {
            return new EasPaymentType()
            {
                PaymentId = paymentTypes.PaymentId,
                PaymentName =  paymentTypes.PaymentName
            };
        }
    }
}
