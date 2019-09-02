using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.FundingService.ALB.FundingOutput.Model.Output;
using ESFA.DC.ILR.FundingService.FM25.Model.Output;
using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using ESFA.DC.ILR.FundingService.FM81.FundingOutput.Model.Output;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReferenceDataService.Model.EAS;
using ESFA.DC.ILR.ReportService.Reports.Funding;
using FluentAssertions;
using Xunit;
using LearningDelivery = ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output.LearningDelivery;
using LearningDeliveryPeriodisedValue = ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output.LearningDeliveryPeriodisedValue;
using LearningDeliveryValue = ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output.LearningDeliveryValue;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding
{
    public class PeriodisedValuesLookupProviderTests
    {
        [Fact]
        public void MapFM25()
        {
            var fundLine1 = "FundLine1";
            var fundLine2 = "FundLine2";

            var attribute1 = "Attribute1";
            var attribute2 = "Attribute2";

            var global = new FM25Global()
            {
                Learners = Enumerable.Range(0, 1000)
                    .Select(i => new FM25Learner()
                    {
                        FundLine = i % 2 == 0 ? fundLine1 : fundLine2,
                        LearnerPeriodisedValues = Enumerable.Range(0, 16)
                            .Select(k => new LearnerPeriodisedValues()
                            {
                                AttributeName = k % 2 == 0 ? attribute1 : attribute2
                            }).ToList(),
                    }).ToList()
            };

            var mapped = NewProvider().BuildFm25Dictionary(global);

            mapped.Should().HaveCount(2);

            mapped[fundLine1].Should().HaveCount(2);
            mapped[fundLine2].Should().HaveCount(2);

            mapped[fundLine1][attribute1].Should().HaveCount(4000);
            mapped[fundLine1][attribute2].Should().HaveCount(4000);

            mapped[fundLine2][attribute1].Should().HaveCount(4000);
            mapped[fundLine2][attribute2].Should().HaveCount(4000);
        }

        [Fact]
        public void MapFM35()
        {
            var fundLine1 = "FundLine1";
            var fundLine2 = "FundLine2";

            var attribute1 = "Attribute1";
            var attribute2 = "Attribute2";

            var global = new FM35Global()
            {
                Learners = Enumerable.Range(0, 1000)
                    .Select(i => new FM35Learner()
                    {
                        LearningDeliveries = Enumerable.Range(0, 3)
                            .Select(j => new LearningDelivery()
                            {
                                LearningDeliveryValue = new LearningDeliveryValue()
                                {
                                    FundLine = j % 2 == 0 ? fundLine1 : fundLine2
                                },
                                LearningDeliveryPeriodisedValues = Enumerable.Range(0, 16)
                                    .Select(k => new LearningDeliveryPeriodisedValue()
                                    {
                                        AttributeName = k % 2 == 0 ? attribute1 : attribute2
                                    }).ToList(),
                            }).ToList(),
                    }).ToList()
            };

            var mapped = NewProvider().BuildFm35Dictionary(global);

            mapped.Should().HaveCount(2);

            mapped[fundLine1].Should().HaveCount(2);
            mapped[fundLine2].Should().HaveCount(2);

            mapped[fundLine1][attribute1].Should().HaveCount(16000);
            mapped[fundLine1][attribute2].Should().HaveCount(16000);

            mapped[fundLine2][attribute1].Should().HaveCount(8000);
            mapped[fundLine2][attribute2].Should().HaveCount(8000);
        }

        [Fact]
        public void MapFM36()
        {
            var fundLine1 = "FundLine1";
            var fundLine2 = "FundLine2";
            
            var global = new FM36Global()
            {
                Learners = Enumerable.Range(0, 1000)
                    .Select(i => new FM36Learner()
                    {
                        LearnRefNumber = i.ToString(),
                        LearningDeliveries = Enumerable.Range(0, 3)
                            .Select(j => new FundingService.FM36.FundingOutput.Model.Output.LearningDelivery()
                            {
                                AimSeqNumber = j,
                                LearningDeliveryValues = new FundingService.FM36.FundingOutput.Model.Output.LearningDeliveryValues()
                                {
                                    LearnDelInitialFundLineType = j % 2 == 0 ? fundLine1 : fundLine2
                                },
                                LearningDeliveryPeriodisedTextValues = new List<LearningDeliveryPeriodisedTextValues>()
                                {
                                    new LearningDeliveryPeriodisedTextValues()
                                    {
                                        AttributeName = "FundLineType", Period1 =  fundLine1, Period2 = fundLine2, Period3 = fundLine1, Period4 = fundLine2, Period5 = fundLine1, Period6 = fundLine2, Period7 = fundLine1, Period8 = fundLine2, Period9 = fundLine1, Period10 = fundLine2, Period11 = fundLine1, Period12 =  fundLine2
                                    }
                                },
                                LearningDeliveryPeriodisedValues = Enumerable.Range(0, 16)
                                    .Select(k => new FundingService.FM36.FundingOutput.Model.Output.LearningDeliveryPeriodisedValues()
                                    {
                                        AttributeName = k.ToString(),
                                        Period1 = 1,
                                        Period2 = 2,
                                    }).ToList(),
                            }).ToList(),
                    }).ToList()
            };

            var mapped = NewProvider().BuildFm36Dictionary(global);

            mapped.Should().HaveCount(2);

            mapped[fundLine1].Should().HaveCount(16);
            mapped[fundLine2].Should().HaveCount(16);

            mapped[fundLine1]["1"].Should().HaveCount(3000);
            mapped[fundLine1]["2"].Should().HaveCount(3000);

            mapped[fundLine2]["1"].Should().HaveCount(3000);
            mapped[fundLine2]["2"].Should().HaveCount(3000);

            mapped[fundLine1]["1"][0][0].Should().Be(1);
            mapped[fundLine1]["1"][0][1].Should().BeNull();
            
            mapped[fundLine2]["1"][0][0].Should().BeNull();
            mapped[fundLine2]["1"][0][1].Should().Be(2);
        }

        [Fact]
        public void MapFM81()
        {
            var fundLine1 = "FundLine1";
            var fundLine2 = "FundLine2";

            var attribute1 = "Attribute1";
            var attribute2 = "Attribute2";

            var global = new FM81Global()
            {
                Learners = Enumerable.Range(0, 1000)
                    .Select(i => new FM81Learner()
                    {
                        LearningDeliveries = Enumerable.Range(0, 3)
                            .Select(j => new FundingService.FM81.FundingOutput.Model.Output.LearningDelivery()
                            {
                                LearningDeliveryValues = new FundingService.FM81.FundingOutput.Model.Output.LearningDeliveryValue()
                                {
                                    FundLine = j % 2 == 0 ? fundLine1 : fundLine2
                                },
                                LearningDeliveryPeriodisedValues = Enumerable.Range(0, 16)
                                    .Select(k => new FundingService.FM81.FundingOutput.Model.Output.LearningDeliveryPeriodisedValue()
                                    {
                                        AttributeName = k % 2 == 0 ? attribute1 : attribute2
                                    }).ToList(),
                            }).ToList(),
                    }).ToList()
            };

            var mapped = NewProvider().BuildFm81Dictionary(global);

            mapped.Should().HaveCount(2);

            mapped[fundLine1].Should().HaveCount(2);
            mapped[fundLine2].Should().HaveCount(2);

            mapped[fundLine1][attribute1].Should().HaveCount(16000);
            mapped[fundLine1][attribute2].Should().HaveCount(16000);

            mapped[fundLine2][attribute1].Should().HaveCount(8000);
            mapped[fundLine2][attribute2].Should().HaveCount(8000);
        }

        [Fact]
        public void MapFM99()
        {
            var fundLine1 = "FundLine1";
            var fundLine2 = "FundLine2";

            var attribute1 = "Attribute1";
            var attribute2 = "Attribute2";

            var global = new ALBGlobal()
            {
                Learners = Enumerable.Range(0, 1000)
                    .Select(i => new ALBLearner()
                    {
                        LearningDeliveries = Enumerable.Range(0, 3)
                            .Select(j => new FundingService.ALB.FundingOutput.Model.Output.LearningDelivery()
                            {
                                LearningDeliveryValue = new FundingService.ALB.FundingOutput.Model.Output.LearningDeliveryValue()
                                {
                                    FundLine = j % 2 == 0 ? fundLine1 : fundLine2
                                },
                                LearningDeliveryPeriodisedValues = Enumerable.Range(0, 16)
                                    .Select(k => new FundingService.ALB.FundingOutput.Model.Output.LearningDeliveryPeriodisedValue()
                                    {
                                        AttributeName = k % 2 == 0 ? attribute1 : attribute2
                                    }).ToList(),
                            }).ToList(),
                    }).ToList()
            };

            var mapped = NewProvider().BuildFm99Dictionary(global);

            mapped.Should().HaveCount(2);

            mapped[fundLine1].Should().HaveCount(2);
            mapped[fundLine2].Should().HaveCount(2);

            mapped[fundLine1][attribute1].Should().HaveCount(16000);
            mapped[fundLine1][attribute2].Should().HaveCount(16000);

            mapped[fundLine2][attribute1].Should().HaveCount(8000);
            mapped[fundLine2][attribute2].Should().HaveCount(8000);
        }

        [Fact]
        public void MapEAS()
        {
            var fundLine1 = "FundLine1";
            var fundLine2 = "FundLine2";

            var adjustment1 = "Adjustment1";
            var adjustment2 = "Adjustment2";

            var payment1 = "Payment1";
            var payment2 = "Payment2";
            var payment3 = "Payment3";

            var referenceDataRoot = new ReferenceDataRoot()
            {
                EasFundingLines = new List<EasFundingLine>
                {
                    new EasFundingLine
                    {
                        FundLine =  fundLine1,
                        EasSubmissionValues = new List<EasSubmissionValue>
                        {
                            new EasSubmissionValue
                            {
                                AdjustmentTypeName = adjustment1,
                                PaymentName =  payment1,
                                Period1 = null,
                                Period2 = null,
                                Period3 = null,
                                Period4 = null,
                                Period5 = null,
                                Period6 = null,
                                Period7 = null,
                                Period8 = null,
                                Period9 = null,
                                Period10 = null,
                                Period11 = null,
                                Period12 = null
                            },
                            new EasSubmissionValue
                            {
                                AdjustmentTypeName = adjustment2,
                                PaymentName = payment2,
                                Period1 = new List<EasPaymentValue>(),
                                Period2 = new List<EasPaymentValue>(),
                                Period3 = new List<EasPaymentValue>(),
                                Period4 = new List<EasPaymentValue>(),
                                Period5 = new List<EasPaymentValue>(),
                                Period6 = new List<EasPaymentValue>(),
                                Period7 = new List<EasPaymentValue>(),
                                Period8 = new List<EasPaymentValue>(),
                                Period9 = new List<EasPaymentValue>(),
                                Period10 = new List<EasPaymentValue>(),
                                Period11 = new List<EasPaymentValue>(),
                                Period12 = new List<EasPaymentValue>()
                            }
                        },                    
                    },
                    new EasFundingLine
                    {
                        FundLine = fundLine2,
                        EasSubmissionValues = new List<EasSubmissionValue>
                        {
                            new EasSubmissionValue
                            {
                                AdjustmentTypeName = adjustment1,
                                PaymentName =  payment1,
                                Period1 = null,
                                Period2 = null,
                                Period3 = null,
                                Period4 = null,
                                Period5 = null,
                                Period6 = null,
                                Period7 = null,
                                Period8 = null,
                                Period9 = null,
                                Period10 = null,
                                Period11 = null,
                                Period12 = null
                            },
                            new EasSubmissionValue
                            {
                                AdjustmentTypeName = adjustment2,
                                PaymentName = payment2,
                                Period1 = new List<EasPaymentValue>
                                {
                                    new EasPaymentValue(1m, null),
                                    new EasPaymentValue(2m, null),
                                },
                                Period2 = new List<EasPaymentValue>
                                {
                                    new EasPaymentValue(1m, 105),
                                    new EasPaymentValue(2m, 115),
                                },
                                Period3 = new List<EasPaymentValue>
                                {
                                    new EasPaymentValue(1m, 110),
                                    new EasPaymentValue(2m, null),
                                },
                                Period4 = new List<EasPaymentValue>(),
                                Period5 = new List<EasPaymentValue>(),
                                Period6 = new List<EasPaymentValue>(),
                                Period7 = new List<EasPaymentValue>(),
                                Period8 = new List<EasPaymentValue>(),
                                Period9 = new List<EasPaymentValue>(),
                                Period10 = new List<EasPaymentValue>(),
                                Period11 = new List<EasPaymentValue>(),
                                Period12 = new List<EasPaymentValue>()
                            },
                            new EasSubmissionValue
                            {
                                AdjustmentTypeName = adjustment2,
                                PaymentName = payment3,
                                Period1 = null,
                                Period2 = null,
                                Period3 = null,
                                Period4 = null,
                                Period5 = null,
                                Period6 = null,
                                Period7 = null,
                                Period8 = new List<EasPaymentValue>(),
                                Period9 = new List<EasPaymentValue>(),
                                Period10 = new List<EasPaymentValue>(),
                                Period11 = new List<EasPaymentValue>(),
                                Period12 = new List<EasPaymentValue>()
                            }
                        }
                    }
                }
            };

            var mapped = NewProvider().BuildEASDictionary(referenceDataRoot);

            mapped.Should().HaveCount(2);

            mapped[fundLine1].Should().HaveCount(2);
            mapped[fundLine2].Should().HaveCount(2);

            mapped[fundLine1][adjustment1].Should().HaveCount(1);
            mapped[fundLine1][adjustment2].Should().HaveCount(1);

            mapped[fundLine2][adjustment1].Should().HaveCount(1);
            mapped[fundLine2][adjustment2].Should().HaveCount(2);
        }


        private PeriodisedValuesLookupProvider NewProvider()
        {
            return new PeriodisedValuesLookupProvider();
        }
    }
}
