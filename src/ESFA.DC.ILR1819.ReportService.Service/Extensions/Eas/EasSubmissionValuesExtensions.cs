using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR1819.ReportService.Model.Eas;

namespace ESFA.DC.ILR1819.ReportService.Service.Extensions.Eas
{
    public static class EasSubmissionValuesExtensions
    {
        public static EasSubmissionValues ToEasSubmissionValues(this EAS1819.EF.EasSubmissionValues easSubmissionValues)
        {
            return new EasSubmissionValues()
            {
                CollectionPeriod = easSubmissionValues.CollectionPeriod,
                PaymentId = easSubmissionValues.PaymentId,
                PaymentValue = easSubmissionValues.PaymentValue
            };
        }
    }
}
