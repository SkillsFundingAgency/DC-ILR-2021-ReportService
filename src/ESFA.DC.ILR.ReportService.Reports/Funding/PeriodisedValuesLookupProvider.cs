using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output;
using ESFA.DC.ILR.ReportService.Reports.Funding.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.Model;
using ESFA.DC.ILR.ReportService.Reports.Funding.Model.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding
{
    public class PeriodisedValuesLookupProvider : IPeriodisedValuesLookupProvider
    {
        public IPeriodisedValuesLookup Provide(IEnumerable<FundModels> fundModels, IReportServiceDependentData reportServiceDependentData)
        {
            var periodisedValuesLookup = new PeriodisedValuesLookup();

            if (fundModels.Contains(FundModels.FM35))
            {
                periodisedValuesLookup[FundModels.FM35] =
                    BuildFm35Dictionary(reportServiceDependentData.Get<FM35Global>());
            }

            return periodisedValuesLookup;
        }


        public Dictionary<string, Dictionary<string, decimal?[][]>> BuildFm35Dictionary(FM35Global fm35Global)
        {
            return fm35Global?
                       .Learners?
                       .SelectMany(l => l.LearningDeliveries)
                       .GroupBy(ld => ld.LearningDeliveryValue.FundLine, StringComparer.OrdinalIgnoreCase)
                       .ToDictionary(k => k.Key,
                           v => v.SelectMany(ld => ld.LearningDeliveryPeriodisedValues)
                               .GroupBy(ldpv => ldpv.AttributeName, StringComparer.OrdinalIgnoreCase)
                               .ToDictionary(k => k.Key, value =>
                                       value.Select(pvGroup => new decimal?[]
                                       {
                                           pvGroup.Period1,
                                           pvGroup.Period2,
                                           pvGroup.Period3,
                                           pvGroup.Period4,
                                           pvGroup.Period5,
                                           pvGroup.Period6,
                                           pvGroup.Period7,
                                           pvGroup.Period8,
                                           pvGroup.Period9,
                                           pvGroup.Period10,
                                           pvGroup.Period11,
                                           pvGroup.Period12,
                                       }).ToArray(),
                                   StringComparer.OrdinalIgnoreCase),
                           StringComparer.OrdinalIgnoreCase)
                   ?? new Dictionary<string, Dictionary<string, decimal?[][]>>();
        }

    }
}
