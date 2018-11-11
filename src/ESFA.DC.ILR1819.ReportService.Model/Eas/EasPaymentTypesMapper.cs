namespace ESFA.DC.ILR1819.ReportService.Model.Eas
{
    public static class EasPaymentTypesMapper
    { 
        public static EasPaymentTypes ToEasPaymentTypes(this EAS1819.EF.PaymentTypes paymentTypes)
        {
            return new EasPaymentTypes()
            {
                PaymentId = paymentTypes.PaymentId,
                PaymentName =  paymentTypes.PaymentName
            };
        }
    }
}
