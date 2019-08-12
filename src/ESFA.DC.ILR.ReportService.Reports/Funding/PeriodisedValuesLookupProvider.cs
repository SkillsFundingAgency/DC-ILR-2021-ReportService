using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.FundingService.ALB.FundingOutput.Model.Output;
using ESFA.DC.ILR.FundingService.FM25.Model.Output;
using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using ESFA.DC.ILR.FundingService.FM81.FundingOutput.Model.Output;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Extensions;
using ESFA.DC.ILR.ReportService.Reports.Funding.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.Model;
using ESFA.DC.ILR.ReportService.Reports.Funding.Model.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;
using LearningDelivery = ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output.LearningDelivery;

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

        public Dictionary<string, Dictionary<string, decimal?[][]>> BuildFm36Dictionary(FM36Global fm36Global)
        {
            var learningDeliveriesByPeriod = fm36Global?
                .Learners?
                .SelectMany(
                    l => 
                    l.LearningDeliveries?
                    .SelectMany(ld =>
                {
                    // Get Fund Line By Period
                    var fundLinePeriodisedValues = ld.LearningDeliveryPeriodisedTextValues?.FirstOrDefault(t => t.AttributeName.CaseInsensitiveEquals(AttributeConstants.Fm36FundLineType));

                    if (fundLinePeriodisedValues != null)
                    {
                        // Flatten
                        return ld.LearningDeliveryPeriodisedValues.SelectMany(pv => new[]
                        {
                            new FlattenedPeriodisedValue(l.LearnRefNumber, ld.AimSeqNumber, 0, fundLinePeriodisedValues.Period1, pv.AttributeName, pv.Period1),
                            new FlattenedPeriodisedValue(l.LearnRefNumber, ld.AimSeqNumber, 1, fundLinePeriodisedValues.Period2, pv.AttributeName, pv.Period2),
                            new FlattenedPeriodisedValue(l.LearnRefNumber, ld.AimSeqNumber, 2, fundLinePeriodisedValues.Period3, pv.AttributeName, pv.Period3),
                            new FlattenedPeriodisedValue(l.LearnRefNumber, ld.AimSeqNumber, 3, fundLinePeriodisedValues.Period4, pv.AttributeName, pv.Period4),
                            new FlattenedPeriodisedValue(l.LearnRefNumber, ld.AimSeqNumber, 4, fundLinePeriodisedValues.Period5, pv.AttributeName, pv.Period5),
                            new FlattenedPeriodisedValue(l.LearnRefNumber, ld.AimSeqNumber, 5, fundLinePeriodisedValues.Period6, pv.AttributeName, pv.Period6),
                            new FlattenedPeriodisedValue(l.LearnRefNumber, ld.AimSeqNumber, 6, fundLinePeriodisedValues.Period7, pv.AttributeName, pv.Period7),
                            new FlattenedPeriodisedValue(l.LearnRefNumber, ld.AimSeqNumber, 7, fundLinePeriodisedValues.Period8, pv.AttributeName, pv.Period8),
                            new FlattenedPeriodisedValue(l.LearnRefNumber, ld.AimSeqNumber, 8, fundLinePeriodisedValues.Period9, pv.AttributeName, pv.Period9),
                            new FlattenedPeriodisedValue(l.LearnRefNumber, ld.AimSeqNumber, 9, fundLinePeriodisedValues.Period10, pv.AttributeName, pv.Period10),
                            new FlattenedPeriodisedValue(l.LearnRefNumber, ld.AimSeqNumber, 10, fundLinePeriodisedValues.Period11, pv.AttributeName, pv.Period11),
                            new FlattenedPeriodisedValue(l.LearnRefNumber, ld.AimSeqNumber, 11, fundLinePeriodisedValues.Period12, pv.AttributeName, pv.Period12),
                        });
                    }

                    return Enumerable.Empty<FlattenedPeriodisedValue>();
                }));
            
            return learningDeliveriesByPeriod?
                .GroupBy(p => p.FundLine, StringComparer.OrdinalIgnoreCase) // Fund Lines
                .ToDictionary(k => k.Key,
                    v => v.GroupBy(a => a.Attribute, StringComparer.OrdinalIgnoreCase) // Attributes
                        .ToDictionary(k => k.Key, 
                            flattenedValuesSet => flattenedValuesSet
                                .GroupBy(fv => new { fv.LearnRefNumber, fv.AimSeqNumber }) // Learning Deliveries
                                .Select(ld => 
                                {


                                    var array = new decimal?[12];

                                    foreach (var periodisedValue in ld)
                                    {
                                        array[periodisedValue.PeriodIndex] = periodisedValue.Value;  // Periods
                                    }

                                    return array;
                                }).ToArray(),
                        StringComparer.OrdinalIgnoreCase),
                    StringComparer.OrdinalIgnoreCase) ?? new Dictionary<string, Dictionary<string, decimal?[][]>>();
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
            return referenceDataRoot?
                       .EasFundingLines?
                       .GroupBy(fl => fl.FundLine, StringComparer.OrdinalIgnoreCase)
                       .ToDictionary(k => k.Key,
                           v => v.SelectMany(ld => ld.EasSubmissionValues)
                               .GroupBy(easv => easv.AdjustmentTypeName, StringComparer.OrdinalIgnoreCase)
                               .ToDictionary(k => k.Key, value =>
                                       value.Select(pvGroup => new decimal?[]
                                       {
                                           pvGroup.Period1.PaymentValue,
                                           pvGroup.Period2.PaymentValue,
                                           pvGroup.Period3.PaymentValue,
                                           pvGroup.Period4.PaymentValue,
                                           pvGroup.Period5.PaymentValue,
                                           pvGroup.Period6.PaymentValue,
                                           pvGroup.Period7.PaymentValue,
                                           pvGroup.Period8.PaymentValue,
                                           pvGroup.Period9.PaymentValue,
                                           pvGroup.Period10.PaymentValue,
                                           pvGroup.Period11.PaymentValue,
                                           pvGroup.Period12.PaymentValue,
                                       }).ToArray(),
                                   StringComparer.OrdinalIgnoreCase),
                           StringComparer.OrdinalIgnoreCase)
                   ?? new Dictionary<string, Dictionary<string, decimal?[][]>>();
        }

        private struct FlattenedPeriodisedValue
        {
            public FlattenedPeriodisedValue(
                string learnRefNumber,
                int aimSeqNumber,
                int periodIndex,
                string fundLine,
                string attribute,
                decimal? value)
            {
                LearnRefNumber = learnRefNumber;
                AimSeqNumber = aimSeqNumber;
                PeriodIndex = periodIndex;
                FundLine = fundLine;
                Attribute = attribute;
                Value = value;
            }

            public string LearnRefNumber { get; }

            public int AimSeqNumber { get; }

            public int PeriodIndex { get; }

            public string FundLine { get; }

            public string Attribute { get; }

            public decimal? Value { get; }
        }

        private struct FundLineAttributeValue
        {
            public string FundLine { get; }

            public string Attribute { get; }

            public decimal? Value { get; }
        }
    }
}
