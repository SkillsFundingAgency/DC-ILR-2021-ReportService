using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS1920.EF.Interface;
using ESFA.DC.ILR.ReportService.Models.EAS;
using ESFA.DC.ILR.ReportService.Service.Interface;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.ILR.ReportService.Data.Eas.Providers
{
    public class EasProvider : IExternalDataProvider
    {
        private readonly Func<IEasdbContext> _easContext;

        public EasProvider(Func<IEasdbContext> easContext)
        {
            _easContext = easContext;
        }

        public async Task<object> ProvideAsync(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            using (var context = _easContext())
            {
                var ukprnString = reportServiceContext.Ukprn.ToString();

                var data = await context.EasSubmissionValues
                    .Where(sv => sv.EasSubmission.Ukprn == ukprnString)
                    .Select(sv => new
                    {
                        sv.Payment.PaymentName,
                        FundingLineName = sv.Payment.FundingLine.Name,
                        AdjustmentTypeName = sv.Payment.AdjustmentType.Name,
                        sv.CollectionPeriod,
                        sv.PaymentValue,
                        sv.DevolvedAreaSoF,
                    }).ToListAsync(cancellationToken);

                return data
                    .GroupBy(d => d.FundingLineName)
                    .Select(d => new EasFundingLine()
                    {
                        FundLine = d.Key,
                        EasSubmissionValues =
                            d.Select(sv => new EasSubmissionValue()
                            {
                                AdjustmentTypeName = sv.AdjustmentTypeName,
                                PaymentName = sv.PaymentName,
                                Period1 = d.Where(f => f.CollectionPeriod == 1).Select(g => new EasPaymentValue(g.PaymentValue, MapDevolvedAreaSoF(g.DevolvedAreaSoF))).ToList(),
                                Period2 = d.Where(f => f.CollectionPeriod == 2).Select(g => new EasPaymentValue(g.PaymentValue, MapDevolvedAreaSoF(g.DevolvedAreaSoF))).ToList(),
                                Period3 = d.Where(f => f.CollectionPeriod == 3).Select(g => new EasPaymentValue(g.PaymentValue, MapDevolvedAreaSoF(g.DevolvedAreaSoF))).ToList(),
                                Period4 = d.Where(f => f.CollectionPeriod == 4).Select(g => new EasPaymentValue(g.PaymentValue, MapDevolvedAreaSoF(g.DevolvedAreaSoF))).ToList(),
                                Period5 = d.Where(f => f.CollectionPeriod == 5).Select(g => new EasPaymentValue(g.PaymentValue, MapDevolvedAreaSoF(g.DevolvedAreaSoF))).ToList(),
                                Period6 = d.Where(f => f.CollectionPeriod == 6).Select(g => new EasPaymentValue(g.PaymentValue, MapDevolvedAreaSoF(g.DevolvedAreaSoF))).ToList(),
                                Period7 = d.Where(f => f.CollectionPeriod == 7).Select(g => new EasPaymentValue(g.PaymentValue, MapDevolvedAreaSoF(g.DevolvedAreaSoF))).ToList(),
                                Period8 = d.Where(f => f.CollectionPeriod == 8).Select(g => new EasPaymentValue(g.PaymentValue, MapDevolvedAreaSoF(g.DevolvedAreaSoF))).ToList(),
                                Period9 = d.Where(f => f.CollectionPeriod == 9).Select(g => new EasPaymentValue(g.PaymentValue, MapDevolvedAreaSoF(g.DevolvedAreaSoF))).ToList(),
                                Period10 = d.Where(f => f.CollectionPeriod == 10).Select(g => new EasPaymentValue(g.PaymentValue, MapDevolvedAreaSoF(g.DevolvedAreaSoF))).ToList(),
                                Period11 = d.Where(f => f.CollectionPeriod == 11).Select(g => new EasPaymentValue(g.PaymentValue, MapDevolvedAreaSoF(g.DevolvedAreaSoF))).ToList(),
                                Period12 = d.Where(f => f.CollectionPeriod == 12).Select(g => new EasPaymentValue(g.PaymentValue, MapDevolvedAreaSoF(g.DevolvedAreaSoF))).ToList(),
                            }).ToList()
                    }).ToList();
            }
        }

        private int? MapDevolvedAreaSoF(int devolvedAreaSoF)
        {
            return devolvedAreaSoF == -1 ? null : (int?)devolvedAreaSoF;
        }
    }
}
