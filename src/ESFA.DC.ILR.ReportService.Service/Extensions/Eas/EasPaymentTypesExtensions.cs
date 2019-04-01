using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR1819.ReportService.Model.Eas;

namespace ESFA.DC.ILR1819.ReportService.Service.Extensions.Eas
{
    public static class EasPaymentTypesExtensions
    {
        public static EasPaymentType ToEasPaymentTypes(this EAS1819.EF.PaymentTypes paymentTypes)
        {
            return new EasPaymentType()
            {
                PaymentId = paymentTypes.PaymentId,
                PaymentName = paymentTypes.PaymentName
            };
        }
    }
}
