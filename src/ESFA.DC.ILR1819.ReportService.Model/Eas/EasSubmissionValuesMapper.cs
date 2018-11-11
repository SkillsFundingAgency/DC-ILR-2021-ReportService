namespace ESFA.DC.ILR1819.ReportService.Model.Eas
{
    public static class EasSubmissionValuesMapper
    {
        public static EasSubmissionValues ToEasSubmissionValues(this EAS1819.EF.EasSubmissionValues easSubmissionValues)
        {
            return new EasSubmissionValues()
            {
                CollectionPeriod = easSubmissionValues.CollectionPeriod,
                PaymentId =  easSubmissionValues.PaymentId,
                PaymentValue =  easSubmissionValues.PaymentValue
            };
        }
    }
}
