using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReferenceDataService.Model.FCS;
using ESFA.DC.ILR.ReferenceDataService.Model.LARS;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Funding.Apprenticeship.NonContractedAppsActivity;
using ESFA.DC.ILR.ReportService.Reports.Funding.Apprenticeship.NonContractedAppsActivity.Model;
using ESFA.DC.ILR.ReportService.Reports.Model.Interface;
using ESFA.DC.ILR.ReportService.Reports.Service;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;
using ESFA.DC.ILR.Tests.Model;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding.NonContractedAppsActivity
{
    public class NonContractedAppsActivityReportModelBuilderTests
    {
        [Fact]
        public void BuildLearnerDictionary()
        {
            var message = new TestMessage
            {
                Learners = new List<TestLearner>
                {
                    new TestLearner { LearnRefNumber = "Learner1" },
                    new TestLearner { LearnRefNumber = "Learner2" },
                    new TestLearner { LearnRefNumber = "Learner3" },
                    new TestLearner { LearnRefNumber = "Learner4" },
                    new TestLearner { LearnRefNumber = "Learner5" },
                    new TestLearner { LearnRefNumber = "Learner6" },
                }
            };

            var expectedDictionary = new Dictionary<string, ILearner>
            {
                { "Learner1", new TestLearner { LearnRefNumber = "Learner1" }},
                { "Learner2", new TestLearner { LearnRefNumber = "Learner2" }},
                { "Learner3", new TestLearner { LearnRefNumber = "Learner3" }},
                { "Learner4", new TestLearner { LearnRefNumber = "Learner4" }},
                { "Learner5", new TestLearner { LearnRefNumber = "Learner5" }},
                { "Learner6", new TestLearner { LearnRefNumber = "Learner6" }},
            };

            NewBuilder().BuildLearnerDictionary(message).Should().BeEquivalentTo(expectedDictionary);
        }

        [Fact]
        public void BuildFm36LearningDeliveryDictionary()
        {
            var message = new TestMessage
            {
                Learners = new List<TestLearner>
                {
                    new TestLearner { LearnRefNumber = "Learner1", LearningDeliveries = new List<TestLearningDelivery> { new TestLearningDelivery { AimSeqNumber = 1, FundModel = 36 } } },
                    new TestLearner { LearnRefNumber = "Learner2", LearningDeliveries = new List<TestLearningDelivery> { new TestLearningDelivery { AimSeqNumber = 1, FundModel = 70 } } },
                    new TestLearner { LearnRefNumber = "Learner3", LearningDeliveries = new List<TestLearningDelivery> { new TestLearningDelivery { AimSeqNumber = 1, FundModel = 36 }, new TestLearningDelivery { AimSeqNumber = 2, FundModel = 36 } } },
                }
            };

            var expectedDictionary = new Dictionary<string, Dictionary<int, ILearningDelivery>>
            {
                 { "Learner1", new Dictionary<int, ILearningDelivery> {{ 1, new TestLearningDelivery { AimSeqNumber = 1, FundModel = 36 } }} },
                 { "Learner2", new Dictionary<int, ILearningDelivery>() },
                 { "Learner3", new Dictionary<int, ILearningDelivery> {{ 1, new TestLearningDelivery { AimSeqNumber = 1, FundModel = 36 } }, { 2, new TestLearningDelivery { AimSeqNumber = 2, FundModel = 36 } } } },
            };

            NewBuilder().BuildFm36LearningDeliveryDictionary(message).Should().BeEquivalentTo(expectedDictionary);
        }

        [Fact]
        public void BuildFcsFundingStreamPeriodCodes()
        {
            var contractAllocations = new List<FcsContractAllocation>
            {
                new FcsContractAllocation { ContractAllocationNumber = "ConRef1", FundingStreamPeriodCode = "FSPC1" },
                new FcsContractAllocation { ContractAllocationNumber = "ConRef2", FundingStreamPeriodCode = "FSPC1" },
                new FcsContractAllocation { ContractAllocationNumber = "ConRef3", FundingStreamPeriodCode = "FSPC2" },
                new FcsContractAllocation { ContractAllocationNumber = "ConRef4", FundingStreamPeriodCode = "FSPC3" },
            };

            var expectedList = new List<string>
            {
                "FSPC1",
                "FSPC2",
                "FSPC3",
            };

            NewBuilder().BuildFcsFundingStreamPeriodCodes(contractAllocations).Should().BeEquivalentTo(expectedList);
        }

        [Fact]
        public void ValidContractMappings()
        {
            var mappings = new List<KeyValuePair<string, string[]>>
            {
                new KeyValuePair<string, string[]>(FundLineConstants.ApprenticeshipEmployerOnAppService1618, new string[] { ContractsConstants.Levy1799, ContractsConstants.NonLevy1799 }),
                new KeyValuePair<string, string[]>(FundLineConstants.ApprenticeshipEmployerOnAppService19Plus, new string[] { ContractsConstants.Levy1799, ContractsConstants.NonLevy1799 }),
                new KeyValuePair<string, string[]>(FundLineConstants.NonLevyApprenticeship1618NonProcured, new string[] { ContractsConstants.Apps1920 }),
                new KeyValuePair<string, string[]>(FundLineConstants.NonLevyApprenticeship1618Procured, new string[] { ContractsConstants.C1618nlap2018 }),
                new KeyValuePair<string, string[]>(FundLineConstants.NonLevyApprenticeship19PlusNonProcured, new string[] { ContractsConstants.Apps1920 }),
                new KeyValuePair<string, string[]>(FundLineConstants.NonLevyApprenticeship19PlusProcured, new string[] { ContractsConstants.Anlap2018 })
            };

            NewBuilder().ValidContractMappings.Should().BeEquivalentTo(mappings);
        }

        [Fact]
        public void BuildLarsLearningDeliveryDictionary()
        {
            var larsLearningDeliveries = new List<LARSLearningDelivery>
            {
                new LARSLearningDelivery
                {
                    LearnAimRef = "LearnAimRef1",
                    LearnAimRefTitle = "Title1"
                },
                new LARSLearningDelivery
                {
                    LearnAimRef = "LearnAimRef2",
                    LearnAimRefTitle = "Title2"
                }
            };

            var reportBuilder = NewBuilder().BuildLARSDictionary(larsLearningDeliveries);

            reportBuilder.Should().HaveCount(2);
            reportBuilder.Should().ContainKeys(new string[] { "LearnAimRef1", "LearnAimRef2" });
        }

        [Fact]
        public void BuildValidContractMapping()
        {
            var contractsDictionary = new Dictionary<string, string[]>
            {
                { FundLineConstants.ApprenticeshipEmployerOnAppService1618, new string[] { ContractsConstants.Levy1799, ContractsConstants.NonLevy1799 } },
                { FundLineConstants.ApprenticeshipEmployerOnAppService19Plus, new string[] { ContractsConstants.Levy1799, ContractsConstants.NonLevy1799 } },
                { FundLineConstants.NonLevyApprenticeship1618NonProcured, new string[] { ContractsConstants.Apps1920 } },
                { FundLineConstants.NonLevyApprenticeship1618Procured, new string[] { ContractsConstants.C1618nlap2018 } },
                { FundLineConstants.NonLevyApprenticeship19PlusNonProcured, new string[] { ContractsConstants.Apps1920 } },
                { FundLineConstants.NonLevyApprenticeship19PlusProcured, new string[] { ContractsConstants.Anlap2018 } }
            };

            NewBuilder().BuildValidContractMapping().Should().BeEquivalentTo(contractsDictionary);
        }

        [Fact]
        public void SumPeriods()
        {
            var periodisedValues = new decimal?[][]
            {
                new decimal?[] { 1m, 2m, 3m, 4m, 4m, 5m, 6m, 7m, 8m, 8m, 0m, null },
                new decimal?[] { 1m, 2m, 3m, 0m, 4m, 5m, 6m, 7m, 8m, 8m, 0m, null },
                new decimal?[] { 1m, 2m, 3m, 4m, 4m, 5m, 4m, 7m, 8m, 8m, 0m, null },
                new decimal?[] { 1m, 2m, 3m, 0m, 4m, 5m, 6m, 7m, 8m, 8m, 0m, null, },
                new decimal?[] { 0m, 2m, 3m, 4m, 4m, 5m, 6m, 7m, 8m, 8m, 0m, null },
                new decimal?[] { 1m, 1m, 3m, 4m, 4m, 5m, 6m, 7m, 8m, 8m, 0m, null },
                new decimal?[] { 0m, 2m, 0m, 4m, 6m, 5m, 6m, 7m, 8m, 8m, 0m, null },
                new decimal?[] { 1m, 1m, 3m, 4m, 4m, 5m, 6m, 7m, 8m, 8m, 0m, null },
                new decimal?[] { 1m, 1m, 3m, 4m, 4m, 5m, 6m, 7m, 8m, 8m, null, null },
                new decimal?[] { 1m, 2m, 0m, 4m, 4m, 5m, 6m, 7m, 8m, 8m, 0m, null },
                new decimal?[] { 1m, 2m, 3m, 4m, 4m, 5m, 6m, 6m, 8m, 10m, 0m, null },
                new decimal?[] { 1m, 1m, 3m, 4m, 4m, 5m, 6m, 4m, 2m, 10m, 0m, null }
            };

            var expectedResult = new ReportTotals
            {
                LearnRefNumber = "Learner1",
                AimSeqNumber = 1,
                AugustTotal = 10m,
                SeptemberTotal = 20m,
                OctoberTotal = 30m,
                NovemberTotal = 40m,
                DecemberTotal = 50m,
                JanuaryTotal = 60m,
                FebruaryTotal = 70m,
                MarchTotal = 80m,
                AprilTotal = 90m,
                MayTotal = 100m,
                JuneTotal = 0m,
                JulyTotal = 0m
            };

            NewBuilder().SumPeriods(periodisedValues, "Learner1", 1).Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void BuildLearningDeliveryReportTotals()
        {
            var learningDeliveryPeriodisedValues = new List<LearningDeliveryPeriodisedValues>
            {
                new LearningDeliveryPeriodisedValues
                {
                    AttributeName =  AttributeConstants.Fm36MathEngOnProgPayment,
                    Period1 = 1m,
                    Period2 = 1m,
                    Period3 = 1m,
                    Period4 = 1m,
                    Period5 = 1m,
                    Period6 = 1m,
                    Period7 = 1m,
                    Period8 = 1m,
                    Period9 = 1m,
                    Period10 = 1m,
                    Period11 = 1m,
                    Period12 = 1m,
                },
                new LearningDeliveryPeriodisedValues
                {
                    AttributeName =  AttributeConstants.Fm36MathEngBalPayment,
                    Period1 = 1m,
                    Period2 = 1m,
                    Period3 = 1m,
                    Period4 = 1m,
                    Period5 = 1m,
                    Period6 = 1m,
                    Period7 = 1m,
                    Period8 = 1m,
                    Period9 = 1m,
                    Period10 = 1m,
                    Period11 = 1m,
                    Period12 = 1m,
                },
                new LearningDeliveryPeriodisedValues
                {
                    AttributeName =  AttributeConstants.Fm36ProgrammeAimOnProgPayment,
                    Period1 = 1m,
                    Period2 = 1m,
                    Period3 = 1m,
                    Period4 = 1m,
                    Period5 = 1m,
                    Period6 = 1m,
                    Period7 = 1m,
                    Period8 = 1m,
                    Period9 = 1m,
                    Period10 = 1m,
                    Period11 = 1m,
                    Period12 = 1m,
                },
            };

            var expectedResult = new ReportTotals
            {
                LearnRefNumber = "Learner1",
                AimSeqNumber = 1,
                AugustTotal = 2m,
                SeptemberTotal = 2m,
                OctoberTotal = 2m,
                NovemberTotal = 2m,
                DecemberTotal = 2m,
                JanuaryTotal = 2m,
                FebruaryTotal = 2m,
                MarchTotal = 2m,
                AprilTotal = 2m,
                MayTotal = 2m,
                JuneTotal = 2m,
                JulyTotal = 2m
            };

            NewBuilder().BuildLearningDeliveryReportTotals(learningDeliveryPeriodisedValues, "Learner1", 1).Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void BuildPriceEpisodeReportTotals()
        {
            var prceEpisodePeriodisedValues = new List<PriceEpisodePeriodisedValues>
            {
                new PriceEpisodePeriodisedValues
                {
                    AttributeName = AttributeConstants.Fm36PriceEpisodeOnProgPaymentAttributeName,
                    Period1 = 1m,
                    Period2 = 1m,
                    Period3 = 1m,
                    Period4 = 1m,
                    Period5 = 1m,
                    Period6 = 1m,
                    Period7 = 1m,
                    Period8 = 1m,
                    Period9 = 1m,
                    Period10 = 1m,
                    Period11 = 1m,
                    Period12 = 1m,
                },
                new PriceEpisodePeriodisedValues
                {
                    AttributeName = AttributeConstants.Fm3PriceEpisodeBalancePaymentAttributeName,
                    Period1 = 1m,
                    Period2 = 1m,
                    Period3 = 1m,
                    Period4 = 1m,
                    Period5 = 1m,
                    Period6 = 1m,
                    Period7 = 1m,
                    Period8 = 1m,
                    Period9 = 1m,
                    Period10 = 1m,
                    Period11 = 1m,
                    Period12 = 1m,
                },
                new PriceEpisodePeriodisedValues
                {
                    AttributeName = AttributeConstants.Fm36ProgrammeAimOnProgPayment,
                    Period1 = 1m,
                    Period2 = 1m,
                    Period3 = 1m,
                    Period4 = 1m,
                    Period5 = 1m,
                    Period6 = 1m,
                    Period7 = 1m,
                    Period8 = 1m,
                    Period9 = 1m,
                    Period10 = 1m,
                    Period11 = 1m,
                    Period12 = 1m,
                },
            };

            var expectedResult = new ReportTotals
            {
                LearnRefNumber = "Learner1",
                AimSeqNumber = 1,
                AugustTotal = 2m,
                SeptemberTotal = 2m,
                OctoberTotal = 2m,
                NovemberTotal = 2m,
                DecemberTotal = 2m,
                JanuaryTotal = 2m,
                FebruaryTotal = 2m,
                MarchTotal = 2m,
                AprilTotal = 2m,
                MayTotal = 2m,
                JuneTotal = 2m,
                JulyTotal = 2m
            };

            NewBuilder().BuildPriceEpisodeReportTotals(prceEpisodePeriodisedValues, "Learner1", 1).Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void GetNonContractedFundLine_NoContract()
        {
            var contractsDictionary = new Dictionary<string, string[]>
            {
                { FundLineConstants.ApprenticeshipEmployerOnAppService1618, new string[] { ContractsConstants.Levy1799, ContractsConstants.NonLevy1799 } },
                { FundLineConstants.ApprenticeshipEmployerOnAppService19Plus, new string[] { ContractsConstants.Levy1799, ContractsConstants.NonLevy1799 } },
                { FundLineConstants.NonLevyApprenticeship1618NonProcured, new string[] { ContractsConstants.Apps1920 } },
                { FundLineConstants.NonLevyApprenticeship1618Procured, new string[] { ContractsConstants.C1618nlap2018 } },
                { FundLineConstants.NonLevyApprenticeship19PlusNonProcured, new string[] { ContractsConstants.Apps1920 } },
                { FundLineConstants.NonLevyApprenticeship19PlusProcured, new string[] { ContractsConstants.Anlap2018 } }
            };

            var fspCodes = new List<string>
            {
                "FSOC1",
            };

            var expectedFundLine =

            NewBuilder().GetNonContractedFundLine(FundLineConstants.ApprenticeshipEmployerOnAppService1618, fspCodes, contractsDictionary).Should().Be(FundLineConstants.ApprenticeshipEmployerOnAppService1618);
        }

        [Fact]
        public void GetNonContractedFundLine_HasContract()
        {
            var contractsDictionary = new Dictionary<string, string[]>
            {
                { FundLineConstants.ApprenticeshipEmployerOnAppService1618, new string[] { ContractsConstants.Levy1799, ContractsConstants.NonLevy1799 } },
                { FundLineConstants.ApprenticeshipEmployerOnAppService19Plus, new string[] { ContractsConstants.Levy1799, ContractsConstants.NonLevy1799 } },
                { FundLineConstants.NonLevyApprenticeship1618NonProcured, new string[] { ContractsConstants.Apps1920 } },
                { FundLineConstants.NonLevyApprenticeship1618Procured, new string[] { ContractsConstants.C1618nlap2018 } },
                { FundLineConstants.NonLevyApprenticeship19PlusNonProcured, new string[] { ContractsConstants.Apps1920 } },
                { FundLineConstants.NonLevyApprenticeship19PlusProcured, new string[] { ContractsConstants.Anlap2018 } }
            };

            var fspCodes = new List<string>
            {
                "NONLEVY2019",
            };

            var expectedFundLine =

            NewBuilder().GetNonContractedFundLine(FundLineConstants.ApprenticeshipEmployerOnAppService1618, fspCodes, contractsDictionary).Should().BeNullOrEmpty();
        }

        [Fact]
        public void BuildNonContractedFundLinesDictionary_NoContract()
        {
            var contractsDictionary = new Dictionary<string, string[]>
            {
                { FundLineConstants.ApprenticeshipEmployerOnAppService1618, new string[] { ContractsConstants.Levy1799, ContractsConstants.NonLevy1799 } },
                { FundLineConstants.ApprenticeshipEmployerOnAppService19Plus, new string[] { ContractsConstants.Levy1799, ContractsConstants.NonLevy1799 } },
                { FundLineConstants.NonLevyApprenticeship1618NonProcured, new string[] { ContractsConstants.Apps1920 } },
                { FundLineConstants.NonLevyApprenticeship1618Procured, new string[] { ContractsConstants.C1618nlap2018 } },
                { FundLineConstants.NonLevyApprenticeship19PlusNonProcured, new string[] { ContractsConstants.Apps1920 } },
                { FundLineConstants.NonLevyApprenticeship19PlusProcured, new string[] { ContractsConstants.Anlap2018 } }
            };

            var fspCodes = new List<string>();

            var periodisedTextValues = new List<LearningDeliveryPeriodisedTextValues>()
            {
                new LearningDeliveryPeriodisedTextValues
                {
                     AttributeName =  AttributeConstants.Fm36FundLineType,
                     Period1 = FundLineConstants.ApprenticeshipEmployerOnAppService1618,
                     Period2 = FundLineConstants.ApprenticeshipEmployerOnAppService1618,
                     Period3 = FundLineConstants.NonLevyApprenticeship1618NonProcured,
                     Period4 = FundLineConstants.NonLevyApprenticeship1618NonProcured,
                     Period5 = FundLineConstants.NonLevyApprenticeship1618NonProcured,
                     Period6 = FundLineConstants.NonLevyApprenticeship19PlusNonProcured,
                     Period7 = FundLineConstants.NonLevyApprenticeship19PlusNonProcured,
                     Period8 = FundLineConstants.NonLevyApprenticeship19PlusNonProcured,
                     Period9 = null,
                     Period10 = null,
                     Period11 = null,
                     Period12 = null,
                },
                new LearningDeliveryPeriodisedTextValues
                {
                     AttributeName =  AttributeConstants.Fm36LDApplic1618FrameworkUpliftBalancingPayment,
                     Period1 = FundLineConstants.ApprenticeshipEmployerOnAppService1618,
                     Period2 = FundLineConstants.ApprenticeshipEmployerOnAppService1618,
                     Period3 = FundLineConstants.ApprenticeshipEmployerOnAppService1618,
                     Period4 = FundLineConstants.ApprenticeshipEmployerOnAppService1618,
                     Period5 = FundLineConstants.ApprenticeshipEmployerOnAppService1618,
                     Period6 = null,
                     Period7 = null,
                     Period8 = null,
                     Period9 = null,
                     Period10 = null,
                     Period11 = null,
                     Period12 = null,
                }
            };


            var expectedDictionary = new Dictionary<string, FundLines>
            {
                {
                    FundLineConstants.ApprenticeshipEmployerOnAppService1618, new FundLines
                    {
                        LearnRefNumber = "Learner1",
                        AimSeqNumber = 1,
                        AugustFundLine = FundLineConstants.ApprenticeshipEmployerOnAppService1618,
                        SeptemberFundLine = FundLineConstants.ApprenticeshipEmployerOnAppService1618,
                    }
                },
                {
                    FundLineConstants.NonLevyApprenticeship1618NonProcured, new FundLines
                    {
                        LearnRefNumber = "Learner1",
                        AimSeqNumber = 1,
                        OctoberFundLine = FundLineConstants.NonLevyApprenticeship1618NonProcured,
                        NovemberFundLine = FundLineConstants.NonLevyApprenticeship1618NonProcured,
                        DecemberFundLine = FundLineConstants.NonLevyApprenticeship1618NonProcured,
                    }
                },
                {
                    FundLineConstants.NonLevyApprenticeship19PlusNonProcured, new FundLines
                    {
                        LearnRefNumber = "Learner1",
                        AimSeqNumber = 1,
                        JanuaryFundLine = FundLineConstants.NonLevyApprenticeship19PlusNonProcured,
                        FebruaryFundLine = FundLineConstants.NonLevyApprenticeship19PlusNonProcured,
                        MarchFundLine = FundLineConstants.NonLevyApprenticeship19PlusNonProcured,
                    }
                }
            };

            NewBuilder().BuildNonContractedFundLinesDictionary(periodisedTextValues, "Learner1", 1, fspCodes, contractsDictionary).Should().BeEquivalentTo(expectedDictionary);
        }

        [Fact]
        public void BuildNonContractedFundLinesDictionary_HasContract()
        {
            var contractsDictionary = new Dictionary<string, string[]>
            {
                { FundLineConstants.ApprenticeshipEmployerOnAppService1618, new string[] { ContractsConstants.Levy1799, ContractsConstants.NonLevy1799 } },
                { FundLineConstants.ApprenticeshipEmployerOnAppService19Plus, new string[] { ContractsConstants.Levy1799, ContractsConstants.NonLevy1799 } },
                { FundLineConstants.NonLevyApprenticeship1618NonProcured, new string[] { ContractsConstants.Apps1920 } },
                { FundLineConstants.NonLevyApprenticeship1618Procured, new string[] { ContractsConstants.C1618nlap2018 } },
                { FundLineConstants.NonLevyApprenticeship19PlusNonProcured, new string[] { ContractsConstants.Apps1920 } },
                { FundLineConstants.NonLevyApprenticeship19PlusProcured, new string[] { ContractsConstants.Anlap2018 } }
            };

            var fspCodes = new List<string>
            {
                ContractsConstants.Levy1799
            };

            var periodisedTextValues = new List<LearningDeliveryPeriodisedTextValues>()
            {
                new LearningDeliveryPeriodisedTextValues
                {
                     AttributeName =  AttributeConstants.Fm36FundLineType,
                     Period1 = FundLineConstants.ApprenticeshipEmployerOnAppService1618,
                     Period2 = FundLineConstants.ApprenticeshipEmployerOnAppService1618,
                     Period3 = FundLineConstants.NonLevyApprenticeship1618NonProcured,
                     Period4 = FundLineConstants.NonLevyApprenticeship1618NonProcured,
                     Period5 = FundLineConstants.NonLevyApprenticeship1618NonProcured,
                     Period6 = FundLineConstants.NonLevyApprenticeship19PlusNonProcured,
                     Period7 = FundLineConstants.NonLevyApprenticeship19PlusNonProcured,
                     Period8 = FundLineConstants.NonLevyApprenticeship19PlusNonProcured,
                     Period9 = null,
                     Period10 = null,
                     Period11 = null,
                     Period12 = null,
                },
                new LearningDeliveryPeriodisedTextValues
                {
                     AttributeName =  AttributeConstants.Fm36LDApplic1618FrameworkUpliftBalancingPayment,
                     Period1 = FundLineConstants.ApprenticeshipEmployerOnAppService1618,
                     Period2 = FundLineConstants.ApprenticeshipEmployerOnAppService1618,
                     Period3 = FundLineConstants.ApprenticeshipEmployerOnAppService1618,
                     Period4 = FundLineConstants.ApprenticeshipEmployerOnAppService1618,
                     Period5 = FundLineConstants.ApprenticeshipEmployerOnAppService1618,
                     Period6 = null,
                     Period7 = null,
                     Period8 = null,
                     Period9 = null,
                     Period10 = null,
                     Period11 = null,
                     Period12 = null,
                }
            };

            var expectedDictionary = new Dictionary<string, FundLines>
            {
                {
                    FundLineConstants.NonLevyApprenticeship1618NonProcured, new FundLines
                    {
                        LearnRefNumber = "Learner1",
                        AimSeqNumber = 1,
                        OctoberFundLine = FundLineConstants.NonLevyApprenticeship1618NonProcured,
                        NovemberFundLine = FundLineConstants.NonLevyApprenticeship1618NonProcured,
                        DecemberFundLine = FundLineConstants.NonLevyApprenticeship1618NonProcured,
                    }
                },
                {
                    FundLineConstants.NonLevyApprenticeship19PlusNonProcured, new FundLines
                    {
                        LearnRefNumber = "Learner1",
                        AimSeqNumber = 1,
                        JanuaryFundLine = FundLineConstants.NonLevyApprenticeship19PlusNonProcured,
                        FebruaryFundLine = FundLineConstants.NonLevyApprenticeship19PlusNonProcured,
                        MarchFundLine = FundLineConstants.NonLevyApprenticeship19PlusNonProcured,
                    }
                }
            };

            NewBuilder().BuildNonContractedFundLinesDictionary(periodisedTextValues, "Learner1", 1, fspCodes, contractsDictionary).Should().BeEquivalentTo(expectedDictionary);
        }

        [Fact]
        public void BuildNonContractedLearningDelivery()
        {
            var contractsDictionary = new Dictionary<string, string[]>
            {
                { FundLineConstants.ApprenticeshipEmployerOnAppService1618, new string[] { ContractsConstants.Levy1799, ContractsConstants.NonLevy1799 } },
                { FundLineConstants.ApprenticeshipEmployerOnAppService19Plus, new string[] { ContractsConstants.Levy1799, ContractsConstants.NonLevy1799 } },
                { FundLineConstants.NonLevyApprenticeship1618NonProcured, new string[] { ContractsConstants.Apps1920 } },
                { FundLineConstants.NonLevyApprenticeship1618Procured, new string[] { ContractsConstants.C1618nlap2018 } },
                { FundLineConstants.NonLevyApprenticeship19PlusNonProcured, new string[] { ContractsConstants.Apps1920 } },
                { FundLineConstants.NonLevyApprenticeship19PlusProcured, new string[] { ContractsConstants.Anlap2018 } }
            };

            var fspCodes = new List<string>
            {
                ContractsConstants.Levy1799
            };

            var learningDelivery = new LearningDelivery
            {
                AimSeqNumber = 1,
                LearningDeliveryPeriodisedTextValues = new List<LearningDeliveryPeriodisedTextValues>()
                {
                    new LearningDeliveryPeriodisedTextValues
                    {
                         AttributeName =  AttributeConstants.Fm36FundLineType,
                         Period1 = FundLineConstants.ApprenticeshipEmployerOnAppService1618,
                         Period2 = FundLineConstants.ApprenticeshipEmployerOnAppService1618,
                         Period3 = FundLineConstants.NonLevyApprenticeship1618NonProcured,
                         Period4 = FundLineConstants.NonLevyApprenticeship1618NonProcured,
                         Period5 = FundLineConstants.NonLevyApprenticeship1618NonProcured,
                         Period6 = FundLineConstants.NonLevyApprenticeship19PlusNonProcured,
                         Period7 = FundLineConstants.NonLevyApprenticeship19PlusNonProcured,
                         Period8 = FundLineConstants.NonLevyApprenticeship19PlusNonProcured,
                         Period9 = null,
                         Period10 = null,
                         Period11 = null,
                         Period12 = null,
                    },
                },
                LearningDeliveryPeriodisedValues = new List<LearningDeliveryPeriodisedValues>
                {
                    new LearningDeliveryPeriodisedValues
                    {
                        AttributeName = AttributeConstants.Fm36MathEngOnProgPayment,
                        Period1 = 1m,
                        Period2 = 1m,
                        Period3 = 1m,
                        Period4 = 1m,
                        Period5 = 1m,
                        Period6 = 1m,
                        Period7 = 1m,
                        Period8 = 1m,
                        Period9 = 1m,
                        Period10 = 1m,
                        Period11 = 1m,
                        Period12 = 1m,
                    },
                    new LearningDeliveryPeriodisedValues
                    {
                        AttributeName = AttributeConstants.Fm36MathEngBalPayment,
                        Period1 = 1m,
                        Period2 = 1m,
                        Period3 = 1m,
                        Period4 = 1m,
                        Period5 = 1m,
                        Period6 = 1m,
                        Period7 = 1m,
                        Period8 = 1m,
                        Period9 = 1m,
                        Period10 = 1m,
                        Period11 = 1m,
                        Period12 = 1m,
                    },
                    new LearningDeliveryPeriodisedValues
                    {
                        AttributeName = AttributeConstants.Fm36ProgrammeAimOnProgPayment,
                        Period1 = 1m,
                        Period2 = 1m,
                        Period3 = 1m,
                        Period4 = 1m,
                        Period5 = 1m,
                        Period6 = 1m,
                        Period7 = 1m,
                        Period8 = 1m,
                        Period9 = 1m,
                        Period10 = 1m,
                        Period11 = 1m,
                        Period12 = 1m,
                    },
                },
                LearningDeliveryValues = new LearningDeliveryValues
                {
                    LearnDelMathEng = true
                }
            };

            var expectedFM36LearningDelivery = new FM36LearningDeliveryValue
            {
                LearningDeliveryValues = new LearningDeliveryValues
                {
                    LearnDelMathEng = true
                },
                FundLineValues = new List<FundLineValue>
                {
                    new FundLineValue
                    {
                        FundLineType = FundLineConstants.NonLevyApprenticeship1618NonProcured,
                        ReportTotals = new ReportTotals
                        {
                            LearnRefNumber = "Learner1",
                            AimSeqNumber = 1,
                            AugustTotal = 0m,
                            SeptemberTotal = 0m,
                            OctoberTotal = 2m,
                            NovemberTotal = 2m,
                            DecemberTotal = 2m,
                            JanuaryTotal = 0m,
                            FebruaryTotal = 0m,
                            MarchTotal = 0m,
                            AprilTotal = 0m,
                            MayTotal = 0m,
                            JuneTotal = 0m,
                            JulyTotal = 0m
                        }
                    },
                    new FundLineValue
                    {
                        FundLineType = FundLineConstants.NonLevyApprenticeship19PlusNonProcured,
                        ReportTotals = new ReportTotals
                        {
                            LearnRefNumber = "Learner1",
                            AimSeqNumber = 1,
                            AugustTotal = 0m,
                            SeptemberTotal = 0m,
                            OctoberTotal = 0m,
                            NovemberTotal = 0m,
                            DecemberTotal = 0m,
                            JanuaryTotal = 2m,
                            FebruaryTotal = 2m,
                            MarchTotal = 2m,
                            AprilTotal = 0m,
                            MayTotal = 0m,
                            JuneTotal = 0m,
                            JulyTotal = 0m
                        }
                    }
                }
            };

            NewBuilder().BuildNonContractedLearningDelivery(learningDelivery, "Learner1", fspCodes, contractsDictionary).Should().BeEquivalentTo(expectedFM36LearningDelivery);
        }

        [Fact]
        public void BuildNonContractedPriceEpisode()
        {
            var contractsDictionary = new Dictionary<string, string[]>
            {
                { FundLineConstants.ApprenticeshipEmployerOnAppService1618, new string[] { ContractsConstants.Levy1799, ContractsConstants.NonLevy1799 } },
                { FundLineConstants.ApprenticeshipEmployerOnAppService19Plus, new string[] { ContractsConstants.Levy1799, ContractsConstants.NonLevy1799 } },
                { FundLineConstants.NonLevyApprenticeship1618NonProcured, new string[] { ContractsConstants.Apps1920 } },
                { FundLineConstants.NonLevyApprenticeship1618Procured, new string[] { ContractsConstants.C1618nlap2018 } },
                { FundLineConstants.NonLevyApprenticeship19PlusNonProcured, new string[] { ContractsConstants.Apps1920 } },
                { FundLineConstants.NonLevyApprenticeship19PlusProcured, new string[] { ContractsConstants.Anlap2018 } }
            };

            var fspCodes = new List<string>
            {
                ContractsConstants.Levy1799
            };

            var priceEpisode = new PriceEpisode
            {
                PriceEpisodeValues = new PriceEpisodeValues
                {
                    PriceEpisodeFundLineType = FundLineConstants.NonLevyApprenticeship19PlusNonProcured,
                    PriceEpisodeAimSeqNumber = 1
                },
                PriceEpisodePeriodisedValues = new List<PriceEpisodePeriodisedValues>()
                {
                    new PriceEpisodePeriodisedValues
                    {
                         AttributeName =  AttributeConstants.Fm36PriceEpisodeOnProgPaymentAttributeName,
                         Period1 = 1m,
                         Period2 = 1m,
                         Period3 = 1m,
                         Period4 = 1m,
                         Period5 = 1m,
                         Period6 = 1m,
                         Period7 = 1m,
                         Period8 = 1m,
                         Period9 = null,
                         Period10 = null,
                         Period11 = null,
                         Period12 = null,
                    },
                }
            };

            var expectedFM36PriceEpisode = new FM36PriceEpisodeValue
            {
                PriceEpisodeValue = new PriceEpisodeValues
                {
                    PriceEpisodeFundLineType = FundLineConstants.NonLevyApprenticeship19PlusNonProcured,
                    PriceEpisodeAimSeqNumber = 1
                },
                FundLineValues = new FundLineValue
                {
                    FundLineType = FundLineConstants.NonLevyApprenticeship19PlusNonProcured,
                    ReportTotals = new ReportTotals
                    {
                        LearnRefNumber = "Learner1",
                        AimSeqNumber = 1,
                        AugustTotal = 1m,
                        SeptemberTotal = 1m,
                        OctoberTotal = 1m,
                        NovemberTotal = 1m,
                        DecemberTotal = 1m,
                        JanuaryTotal = 1m,
                        FebruaryTotal = 1m,
                        MarchTotal = 1m,
                        AprilTotal = 0m,
                        MayTotal = 0m,
                        JuneTotal = 0m,
                        JulyTotal = 0m
                    }
                }
            };

            NewBuilder().BuildNonContractedPriceEpisode(priceEpisode, "Learner1", fspCodes, contractsDictionary).Should().BeEquivalentTo(expectedFM36PriceEpisode);
        }

        [Theory]
        [InlineData(2019, 09, 1)]
        [InlineData(2019, 10, 1)]
        [InlineData(2020, 07, 1)]
        public void PriceEpisodeFilter_True(int year, int month, int aimSeqNumber)
        {
            var academicYearService = new AcademicYearService();

            var priceEpisode = new PriceEpisodeValues
            {
                PriceEpisodeAimSeqNumber = aimSeqNumber,
                EpisodeStartDate = new DateTime(year, month, 01)
            };

            NewBuilder(academicYearService).PriceEpisodeFilter(priceEpisode, 1).Should().BeTrue();
        }

        [Theory]
        [InlineData(2019, 09, 2)]
        [InlineData(2020, 10, 1)]
        [InlineData(2019, 03, 1)]
        public void PriceEpisodeFilter_False(int year, int month, int aimSeqNumber)
        {
            var academicYearService = new AcademicYearService();

            var priceEpisode = new PriceEpisodeValues
            {
                PriceEpisodeAimSeqNumber = aimSeqNumber,
                EpisodeStartDate = new DateTime(year, month, 01)
            };

            NewBuilder(academicYearService).PriceEpisodeFilter(priceEpisode, 1).Should().BeFalse();
        }

        private NonContractedAppsActivityReportModelBuilder NewBuilder(IAcademicYearService academicYearService = null, IIlrModelMapper ilrModelMapper = null)
        {
            return new NonContractedAppsActivityReportModelBuilder(academicYearService, ilrModelMapper);
        }
    }
}
