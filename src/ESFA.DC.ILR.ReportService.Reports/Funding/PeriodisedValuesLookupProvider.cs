using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.FundingService.ALB.FundingOutput.Model.Output;
using ESFA.DC.ILR.FundingService.FM25.Model.Output;
using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using ESFA.DC.ILR.FundingService.FM81.FundingOutput.Model.Output;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReportService.Reports.Funding.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.Model;
using ESFA.DC.ILR.ReportService.Reports.Funding.Model.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding
{
    public class PeriodisedValuesLookupProvider : IPeriodisedValuesLookupProvider
    {
        public IPeriodisedValuesLookup Provide(IEnumerable<FundingDataSources> fundingDataSources, IReportServiceDependentData reportServiceDependentData)
        {
            var periodisedValuesLookup = new PeriodisedValuesLookup();

            if (fundingDataSources.Contains(FundingDataSources.FM35))
            {
                periodisedValuesLookup[FundingDataSources.FM35] = BuildFm35Dictionary(reportServiceDependentData.Get<FM35Global>());
            }

            if (fundingDataSources.Contains(FundingDataSources.FM81))
            {
                periodisedValuesLookup[FundingDataSources.FM81] = BuildFm81Dictionary(reportServiceDependentData.Get<FM81Global>());
            }

            if (fundingDataSources.Contains(FundingDataSources.FM25))
            {
                periodisedValuesLookup[FundingDataSources.FM25] = BuildFm25Dictionary(reportServiceDependentData.Get<FM25Global>());
            }

            if (fundingDataSources.Contains(FundingDataSources.FM36))
            {
                periodisedValuesLookup[FundingDataSources.FM36] = BuildFm36Dictionary(reportServiceDependentData.Get<FM36Global>());
            }

            if (fundingDataSources.Contains(FundingDataSources.FM99))
            {
                periodisedValuesLookup[FundingDataSources.FM99] = BuildFm99Dictionary(reportServiceDependentData.Get<ALBGlobal>());
            }

            if (fundingDataSources.Contains(FundingDataSources.EAS))
            {
                periodisedValuesLookup[FundingDataSources.EAS] = BuildEASDictionary(reportServiceDependentData.Get<ReferenceDataRoot>());
            }
            
            return periodisedValuesLookup;
        }

        public Dictionary<string, Dictionary<string, decimal?[][]>> BuildFm25Dictionary(FM25Global fm25Global)
        {
            return fm25Global
                       .Learners?
                       .GroupBy(ld => ld.FundLine, StringComparer.OrdinalIgnoreCase)
                       .ToDictionary(k => k.Key,
                           v => v.SelectMany(ld => ld.LearnerPeriodisedValues)
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

        public Dictionary<string, Dictionary<string, decimal?[][]>> BuildFm36Dictionary(FM36Global fm35Global)
        {
            return fm35Global?
                       .Learners?
                       .SelectMany(l => l.LearningDeliveries)
                       .GroupBy(ld => ld.LearningDeliveryValues.LearnDelInitialFundLineType, StringComparer.OrdinalIgnoreCase)
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

        public Dictionary<string, Dictionary<string, decimal?[][]>> BuildFm81Dictionary(FM81Global fm81Global)
        {
            return fm81Global
                    .Learners?
                    .SelectMany(l => l.LearningDeliveries)
                    .GroupBy(ld => ld.LearningDeliveryValues.FundLine, StringComparer.OrdinalIgnoreCase)
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

        public Dictionary<string, Dictionary<string, decimal?[][]>> BuildFm99Dictionary(ALBGlobal fm99Global)
        {
            return fm99Global
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

        public Dictionary<string, Dictionary<string, decimal?[][]>> BuildEASDictionary(ReferenceDataRoot referenceDataRoot)
        {
            // TODO when ReferenceDataRoot has EAS Data
            return new Dictionary<string, Dictionary<string, decimal?[][]>>();
        }
    }
}
