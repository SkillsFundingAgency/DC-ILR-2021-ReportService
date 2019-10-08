using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReferenceDataService.Model.FCS;
using ESFA.DC.ILR.ReferenceDataService.Model.LARS;
using ESFA.DC.ILR.ReferenceDataService.Model.MetaData.CollectionDates;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Funding.Apprenticeship.NonContractedAppsActivity;
using ESFA.DC.ILR.ReportService.Reports.Funding.Apprenticeship.NonContractedAppsActivity.Model;
using ESFA.DC.ILR.ReportService.Reports.Model;
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
            var mappings = new Dictionary<string, string[]>
            {
                { FundLineConstants.ApprenticeshipEmployerOnAppService1618, new string[] { ContractsConstants.Levy1799, ContractsConstants.NonLevy1799 } },
                { FundLineConstants.ApprenticeshipEmployerOnAppService19Plus, new string[] { ContractsConstants.Levy1799, ContractsConstants.NonLevy1799 } },
                { FundLineConstants.NonLevyApprenticeship1618NonProcured, new string[] { ContractsConstants.Apps1920 } },
                { FundLineConstants.NonLevyApprenticeship1618Procured, new string[] { ContractsConstants.C1618nlap2018 } },
                { FundLineConstants.NonLevyApprenticeship19PlusNonProcured, new string[] { ContractsConstants.Apps1920 } },
                { FundLineConstants.NonLevyApprenticeship19PlusProcured, new string[] { ContractsConstants.Anlap2018 } }
            };

            NewBuilder().ValidContractsDictionary.Should().BeEquivalentTo(mappings);
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
            var fspCodes = new List<string>
            {
                "FSOC1",
            };

            var expectedFundLine =

            NewBuilder().GetNonContractedFundLine(FundLineConstants.ApprenticeshipEmployerOnAppService1618, fspCodes).Should().Be(FundLineConstants.ApprenticeshipEmployerOnAppService1618);
        }

        [Fact]
        public void GetNonContractedFundLine_HasContract()
        {
            var fspCodes = new List<string>
            {
                "NONLEVY2019",
            };

            var expectedFundLine =

            NewBuilder().GetNonContractedFundLine(FundLineConstants.ApprenticeshipEmployerOnAppService1618, fspCodes).Should().BeNullOrEmpty();
        }

        [Fact]
        public void BuildNonContractedFundLinesDictionary_NoContract()
        {
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

            NewBuilder().BuildNonContractedFundLinesDictionary(periodisedTextValues, "Learner1", 1, fspCodes).Should().BeEquivalentTo(expectedDictionary);
        }

        [Fact]
        public void BuildNonContractedFundLinesDictionary_HasContract()
        {
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

            NewBuilder().BuildNonContractedFundLinesDictionary(periodisedTextValues, "Learner1", 1, fspCodes).Should().BeEquivalentTo(expectedDictionary);
        }

        [Fact]
        public void BuildNonContractedLearningDelivery()
        {
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

            NewBuilder().BuildNonContractedLearningDelivery(learningDelivery, "Learner1", fspCodes).Should().BeEquivalentTo(expectedFM36LearningDelivery);
        }

        [Fact]
        public void BuildNonContractedPriceEpisode()
        {
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

            NewBuilder().BuildNonContractedPriceEpisode(priceEpisode, "Learner1", fspCodes).Should().BeEquivalentTo(expectedFM36PriceEpisode);
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

        [Fact]
        public void BuildFm36Learners()
        {
            var fspCodes = new List<string>
            {
                "Code1"
            };

            ILearningDelivery learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = "LearnAimRef1",
                AimSeqNumber = 1,
                FundModel = 36,
                LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                {
                    new TestLearningDeliveryFAM
                    {
                        LearnDelFAMType = "SOF",
                        LearnDelFAMCode = "105",
                        LearnDelFAMDateFromNullable = new DateTime(2019, 8, 1)
                    },
                    new TestLearningDeliveryFAM
                    {
                        LearnDelFAMType = "ACT",
                        LearnDelFAMCode = "1",
                        LearnDelFAMDateFromNullable = new DateTime(2019, 9, 1)
                    }
                }
            };

            ILearner learner1 = new TestLearner
            {
                LearnRefNumber = "Learner1",
                LearningDeliveries = new []
                {
                    learningDelivery
                }
            };

            ILearner learner2 = new TestLearner
            {
                LearnRefNumber = "Learner2",
                LearningDeliveries = new []
                {
                    learningDelivery
                }
            };

            ILearner learner3 = new TestLearner
            {
                LearnRefNumber = "Learner3",
                LearningDeliveries = new []
                {
                   learningDelivery
                }
            };

            IMessage message = new TestMessage
            {
                Learners = new []
                {
                    learner1,
                    learner2,
                    learner3
                }
            };

            var global = new FM36Global
            {
                Learners = new List<FM36Learner>
                {
                    new FM36Learner
                    {
                        LearnRefNumber = "Learner1",
                        LearningDeliveries = new List<LearningDelivery>
                        {
                            new LearningDelivery
                            {
                                AimSeqNumber = 1,
                                LearningDeliveryValues =  new LearningDeliveryValues
                                {
                                    LearnDelMathEng = true
                                },
                                LearningDeliveryPeriodisedTextValues = new List<LearningDeliveryPeriodisedTextValues>
                                {
                                    new LearningDeliveryPeriodisedTextValues
                                    {
                                        AttributeName = AttributeConstants.Fm36FundLineType,
                                        Period1 = FundLineConstants.ApprenticeshipEmployerOnAppService1618,
                                        Period2 = FundLineConstants.ApprenticeshipEmployerOnAppService1618,
                                        Period3 = FundLineConstants.ApprenticeshipEmployerOnAppService1618
                                    }
                                },
                                LearningDeliveryPeriodisedValues =  new List<LearningDeliveryPeriodisedValues>
                                {
                                    new LearningDeliveryPeriodisedValues
                                    {
                                        AttributeName = AttributeConstants.Fm36MathEngOnProgPayment,
                                        Period1 = 1m,
                                        Period2 = 2m,
                                        Period3 = 3m
                                    }
                                },
                            }
                        },
                        PriceEpisodes = new List<PriceEpisode>
                        {
                            new PriceEpisode
                            {
                                PriceEpisodeValues = new PriceEpisodeValues
                                {
                                    PriceEpisodeAimSeqNumber = 1,
                                    EpisodeStartDate = new DateTime(2019, 09, 01),
                                    PriceEpisodeFundLineType = FundLineConstants.ApprenticeshipEmployerOnAppService1618,
                                },
                                PriceEpisodePeriodisedValues = new List<PriceEpisodePeriodisedValues>
                                {
                                    new PriceEpisodePeriodisedValues
                                    {
                                        AttributeName = AttributeConstants.Fm36PriceEpisodeOnProgPaymentAttributeName,
                                        Period1 = 1m,
                                        Period2 = 2m,
                                        Period3 = 3m
                                    }
                                }
                            }
                        }
                    },
                    new FM36Learner
                    {
                        LearnRefNumber = "Learner2",
                        LearningDeliveries = new List<LearningDelivery>
                        {
                            new LearningDelivery
                            {
                                AimSeqNumber = 1,
                                LearningDeliveryValues = new LearningDeliveryValues
                                {
                                    LearnDelMathEng = false
                                },
                                LearningDeliveryPeriodisedTextValues = new List<LearningDeliveryPeriodisedTextValues>
                                {
                                    new LearningDeliveryPeriodisedTextValues
                                    {
                                        AttributeName = AttributeConstants.Fm36FundLineType,
                                        Period1 = FundLineConstants.ApprenticeshipEmployerOnAppService1618,
                                        Period2 = FundLineConstants.ApprenticeshipEmployerOnAppService1618,
                                        Period3 = FundLineConstants.ApprenticeshipEmployerOnAppService1618
                                    }
                                },
                                LearningDeliveryPeriodisedValues =  new List<LearningDeliveryPeriodisedValues>
                                {
                                    new LearningDeliveryPeriodisedValues
                                    {
                                        AttributeName = AttributeConstants.Fm36MathEngOnProgPayment,
                                        Period1 = 1m,
                                        Period2 = 2m,
                                        Period3 = 3m
                                    }
                                },
                            },
                        },
                        PriceEpisodes = new List<PriceEpisode>
                        {
                            new PriceEpisode
                            {
                                PriceEpisodeValues = new PriceEpisodeValues
                                {
                                    PriceEpisodeAimSeqNumber = 1,
                                    EpisodeStartDate = new DateTime(2019, 09, 01),
                                    PriceEpisodeFundLineType = FundLineConstants.ApprenticeshipEmployerOnAppService1618,
                                },
                                PriceEpisodePeriodisedValues = new List<PriceEpisodePeriodisedValues>
                                {
                                    new PriceEpisodePeriodisedValues
                                    {
                                        AttributeName = AttributeConstants.Fm36PriceEpisodeOnProgPaymentAttributeName,
                                        Period1 = 1m,
                                        Period2 = 2m,
                                        Period3 = 3m
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var expectedModel = new FM36LearnerData[]
            {
                new FM36LearnerData
                {
                    Learner = learner1,
                    ProviderSpecLearnerMonitoringModels = new ProviderSpecLearnerMonitoringModel(),
                    LearningDeliveries = new FM36LearningDeliveryData[]
                    {
                        new FM36LearningDeliveryData
                        {
                            ProviderSpecDeliveryMonitoringModels = new ProviderSpecDeliveryMonitoringModel(),
                            LearningDeliveryFAMsModels = new LearningDeliveryFAMsModel
                            {
                                SOF = "105",
                            },
                            LearningDelivery = learningDelivery,
                            FM36LearningDelivery = new FM36LearningDeliveryValue
                            {
                                LearningDeliveryValues = new LearningDeliveryValues
                                {
                                    LearnDelMathEng = true
                                },
                                FundLineValues = new []
                                {
                                    new FundLineValue
                                    {
                                         FundLineType = FundLineConstants.ApprenticeshipEmployerOnAppService1618,
                                         ReportTotals = new ReportTotals
                                         {
                                             LearnRefNumber = "Learner1",
                                             AimSeqNumber = 1,
                                             AugustTotal = 1m,
                                             SeptemberTotal = 2m,
                                             OctoberTotal = 3m,
                                             NovemberTotal = 0m,
                                             DecemberTotal = 0m,
                                             JanuaryTotal = 0m,
                                             FebruaryTotal = 0m,
                                             MarchTotal = 0m,
                                             AprilTotal= 0m,
                                             MayTotal = 0m,
                                             JuneTotal = 0m,
                                             JulyTotal = 0m,
                                         }
                                    }
                                }
                            },
                            LearningDeliveryFAMs_ACT = new []
                            {
                                new TestLearningDeliveryFAM
                                {
                                    LearnDelFAMType = "ACT",
                                    LearnDelFAMCode = "1",
                                    LearnDelFAMDateFromNullable = new DateTime(2019, 9, 1)
                                }
                            },
                            FM36PriceEpisodes = new FM36PriceEpisodeValue[]
                            {
                                new FM36PriceEpisodeValue
                                {
                                    FundLineValues = new FundLineValue
                                    {
                                         FundLineType = FundLineConstants.ApprenticeshipEmployerOnAppService1618,
                                         ReportTotals = new ReportTotals
                                         {
                                             LearnRefNumber = "Learner1",
                                             AimSeqNumber = 1,
                                             AugustTotal = 1m,
                                             SeptemberTotal = 2m,
                                             OctoberTotal = 3m,
                                             NovemberTotal = 0m,
                                             DecemberTotal = 0m,
                                             JanuaryTotal = 0m,
                                             FebruaryTotal = 0m,
                                             MarchTotal = 0m,
                                             AprilTotal= 0m,
                                             MayTotal = 0m,
                                             JuneTotal = 0m,
                                             JulyTotal = 0m,
                                         }
                                    },
                                    PriceEpisodeValue = new PriceEpisodeValues
                                    {
                                        PriceEpisodeAimSeqNumber = 1,
                                        EpisodeStartDate = new DateTime(2019, 9, 1),
                                        PriceEpisodeFundLineType = FundLineConstants.ApprenticeshipEmployerOnAppService1618
                                    }
                                }
                            }
                        }
                    }
                },
                new FM36LearnerData
                {
                    Learner = learner2,
                    ProviderSpecLearnerMonitoringModels = new ProviderSpecLearnerMonitoringModel(),
                    LearningDeliveries = new FM36LearningDeliveryData[]
                    {
                        new FM36LearningDeliveryData
                        {
                            ProviderSpecDeliveryMonitoringModels = new ProviderSpecDeliveryMonitoringModel(),
                            LearningDeliveryFAMsModels = new LearningDeliveryFAMsModel
                            {
                                SOF = "105",
                            },
                            LearningDelivery = learningDelivery,
                            FM36LearningDelivery = new FM36LearningDeliveryValue
                            {
                                LearningDeliveryValues = new LearningDeliveryValues
                                {
                                    LearnDelMathEng = false
                                },
                                FundLineValues = new []
                                {
                                    new FundLineValue
                                    {
                                         FundLineType = FundLineConstants.ApprenticeshipEmployerOnAppService1618,
                                         ReportTotals = new ReportTotals
                                         {
                                             LearnRefNumber = "Learner2",
                                             AimSeqNumber = 1,
                                             AugustTotal = 1m,
                                             SeptemberTotal = 2m,
                                             OctoberTotal = 3m,
                                             NovemberTotal = 0m,
                                             DecemberTotal = 0m,
                                             JanuaryTotal = 0m,
                                             FebruaryTotal = 0m,
                                             MarchTotal = 0m,
                                             AprilTotal= 0m,
                                             MayTotal = 0m,
                                             JuneTotal = 0m,
                                             JulyTotal = 0m,
                                         }
                                    }
                                }
                            },
                            LearningDeliveryFAMs_ACT = new []
                            {
                                new TestLearningDeliveryFAM
                                {
                                    LearnDelFAMType = "ACT",
                                    LearnDelFAMCode = "1",
                                    LearnDelFAMDateFromNullable = new DateTime(2019, 9, 1)
                                }
                            },
                            FM36PriceEpisodes = new FM36PriceEpisodeValue[]
                            {
                                new FM36PriceEpisodeValue
                                {
                                    FundLineValues = new FundLineValue
                                    {
                                         FundLineType = FundLineConstants.ApprenticeshipEmployerOnAppService1618,
                                         ReportTotals = new ReportTotals
                                         {
                                             LearnRefNumber = "Learner2",
                                             AimSeqNumber = 1,
                                             AugustTotal = 1m,
                                             SeptemberTotal = 2m,
                                             OctoberTotal = 3m,
                                             NovemberTotal = 0m,
                                             DecemberTotal = 0m,
                                             JanuaryTotal = 0m,
                                             FebruaryTotal = 0m,
                                             MarchTotal = 0m,
                                             AprilTotal= 0m,
                                             MayTotal = 0m,
                                             JuneTotal = 0m,
                                             JulyTotal = 0m,
                                         }
                                    },
                                    PriceEpisodeValue = new PriceEpisodeValues
                                    {
                                        PriceEpisodeAimSeqNumber = 1,
                                        EpisodeStartDate = new DateTime(2019, 9, 1),
                                        PriceEpisodeFundLineType = FundLineConstants.ApprenticeshipEmployerOnAppService1618
                                    }
                                }
                            }
                        }
                    }
                }
            };

            NewBuilder(new AcademicYearService(), new IlrModelMapper()).BuildFm36Learners(message, global, fspCodes).Should().BeEquivalentTo(expectedModel);
        }

        [Fact]
        public void BuildFm36Learners_NoLearners()
        {
            var fspCodes = new List<string>
            {
                "Code1"
            };

            IMessage message = new TestMessage();

            var expectedModel = new FM36LearnerData[]{};

            NewBuilder(new AcademicYearService(), new IlrModelMapper()).BuildFm36Learners(message, new FM36Global(), fspCodes).Should().BeEquivalentTo(expectedModel);
        }

        [Fact]
        public void BuildLearningDeliveryACTValues_LearnActEndDate()
        {
            var learnActEndDate = new DateTime(2020, 01, 01);
            var learningDeliveryFams = new List<LearningDeliveryACT>
            {
                new LearningDeliveryACT
                {
                    LearnDelFAMType = LearnerFAMTypeConstants.ACT,
                    LearnDelFAMCode = "1",
                    LearnDelFAMDateFromNullable = new DateTime(2019, 09, 01),
                    LearnDelFAMDateToNullable = new DateTime(2019, 11, 01)
                },
                new LearningDeliveryACT
                {
                    LearnDelFAMType = LearnerFAMTypeConstants.ACT,
                    LearnDelFAMCode = "2",
                    LearnDelFAMDateFromNullable = new DateTime(2019, 11, 02),
                    LearnDelFAMDateToNullable = new DateTime(2020, 01, 01)
                }
            } as ICollection<ILearningDeliveryFAM>;

            var fundlineValues =  new FundLineValue
            {
                ReportTotals = new ReportTotals
                {
                    LearnRefNumber = "Learner1",
                    AimSeqNumber = 1,
                    AugustTotal = 1,
                    SeptemberTotal = 2m
                }
            };

            var censusEndDates = CensusDates().ToDictionary(p => p.Period, e => (DateTime?)e.End);

            var expectedLearningDeliveryFams = new List<LearningDeliveryACT>
            {
                new LearningDeliveryACT
                {
                    LearnDelFAMType = LearnerFAMTypeConstants.ACT,
                    LearnDelFAMCode = "2",
                    LearnDelFAMDateFromNullable = new DateTime(2019, 11, 02),
                    LearnDelFAMDateToNullable = new DateTime(2020, 01, 01)
                }
            } as ICollection<ILearningDeliveryFAM>;

            NewBuilder().BuildLearningDeliveryACTValues(
                learnActEndDate,
                learningDeliveryFams,
                fundlineValues,
                censusEndDates,
                "2").Should().BeEquivalentTo(expectedLearningDeliveryFams);
        }

        [Fact]
        public void BuildLearningDeliveryACTValues_NoLearnActEndDate()
        {
            ICollection<ILearningDeliveryFAM> learningDeliveryFams = new LearningDeliveryACT[]
            {
                new LearningDeliveryACT
                {
                    LearnDelFAMType = LearnerFAMTypeConstants.ACT,
                    LearnDelFAMCode = "1",
                    LearnDelFAMDateFromNullable = new DateTime(2019, 09, 01),
                    LearnDelFAMDateToNullable = new DateTime(2019, 11, 01)
                },
                new LearningDeliveryACT
                {
                    LearnDelFAMType = LearnerFAMTypeConstants.ACT,
                    LearnDelFAMCode = "2",
                    LearnDelFAMDateFromNullable = new DateTime(2019, 11, 02),
                    LearnDelFAMDateToNullable = new DateTime(2020, 01, 01)
                }
            };

            var fundlineValue = new FundLineValue
            {
                ReportTotals = new ReportTotals
                {
                    LearnRefNumber = "Learner1",
                    AimSeqNumber = 1,
                    AugustTotal = 1,
                    SeptemberTotal = 2m
                }
            };

            var censusEndDates = CensusDates().ToDictionary(p => p.Period, e => (DateTime?)e.End);

             ICollection<ILearningDeliveryFAM> expectedLearningDeliveryFams = new LearningDeliveryACT[]
             {
                new LearningDeliveryACT
                {
                    LearnDelFAMType = LearnerFAMTypeConstants.ACT,
                    LearnDelFAMCode = "1",
                    LearnDelFAMDateFromNullable = new DateTime(2019, 09, 01),
                    LearnDelFAMDateToNullable = new DateTime(2019, 11, 01)
                }
            };

            NewBuilder().BuildLearningDeliveryACTValues(null, learningDeliveryFams, fundlineValue, censusEndDates, "1").Should().BeEquivalentTo(expectedLearningDeliveryFams);
        }

        [Fact]
        public void BuildPriceEpisodeACTValues()
        {
            ICollection<ILearningDeliveryFAM> learningDeliveryFams = new LearningDeliveryACT[]
            {
                new LearningDeliveryACT
                {
                    LearnDelFAMType = LearnerFAMTypeConstants.ACT,
                    LearnDelFAMCode = "1",
                    LearnDelFAMDateFromNullable = new DateTime(2019, 09, 01),
                    LearnDelFAMDateToNullable = new DateTime(2019, 11, 01)
                },
                new LearningDeliveryACT
                {
                    LearnDelFAMType = LearnerFAMTypeConstants.ACT,
                    LearnDelFAMCode = "2",
                    LearnDelFAMDateFromNullable = new DateTime(2019, 11, 02),
                    LearnDelFAMDateToNullable = new DateTime(2020, 01, 01)
                },
                new LearningDeliveryACT
                {
                    LearnDelFAMType = LearnerFAMTypeConstants.ACT,
                    LearnDelFAMCode = "1",
                    LearnDelFAMDateFromNullable = new DateTime(2020, 01, 02)
                }
            };

            var expectedFam = new LearningDeliveryACT
            {
                LearnDelFAMType = LearnerFAMTypeConstants.ACT,
                LearnDelFAMCode = "1",
                LearnDelFAMDateFromNullable = new DateTime(2019, 09, 01),
                LearnDelFAMDateToNullable = new DateTime(2019, 11, 01)
            };

            NewBuilder().BuildPriceEpisodeACTValues(new DateTime(2019, 09, 1), learningDeliveryFams).Should().BeEquivalentTo(expectedFam);
        }

        [Fact]
        public void ReportRowTotalApplicable_True()
        {
            var learningDeliveryFam = new LearningDeliveryACT
            {
                LearnDelFAMType = LearnerFAMTypeConstants.ACT,
                LearnDelFAMCode = "1",
                LearnDelFAMDateFromNullable = new DateTime(2019, 09, 01),
                LearnDelFAMDateToNullable = new DateTime(2019, 11, 01)
            };

            NewBuilder().ReportRowTotalApplicable(learningDeliveryFam, new DateTime(2019, 10, 1)).Should().BeTrue();
        }

        [Fact]
        public void ReportRowTotalApplicable_False()
        {
            var learningDeliveryFam = new LearningDeliveryACT
            {
                LearnDelFAMType = LearnerFAMTypeConstants.ACT,
                LearnDelFAMCode = "1",
                LearnDelFAMDateFromNullable = new DateTime(2019, 09, 01),
                LearnDelFAMDateToNullable = new DateTime(2019, 11, 01)
            };

            NewBuilder().ReportRowTotalApplicable(learningDeliveryFam, new DateTime(2019, 08, 1)).Should().BeFalse();
        }

        [Fact]
        public void ReportRowTotalApplicable_False_Null()
        {
            var learningDeliveryFam = new LearningDeliveryACT
            {
                LearnDelFAMType = LearnerFAMTypeConstants.ACT,
                LearnDelFAMCode = "1",
            };

            NewBuilder().ReportRowTotalApplicable(learningDeliveryFam, new DateTime(2019, 08, 1)).Should().BeFalse();
        }

        [Fact]
        public void ApplySort()
        {
            var models = new List<NonContractedAppsActivityReportModel>
            {
                new NonContractedAppsActivityReportModel
                {
                    Learner = new MessageLearner { LearnRefNumber = "Learner1" },
                    LearningDelivery = new MessageLearnerLearningDelivery { AimSeqNumber = 1 },
                    LearningDeliveryFAM_ACTs = new LearningDeliveryACT()
                },
                new NonContractedAppsActivityReportModel
                {
                    Learner = new MessageLearner { LearnRefNumber = "Learner3" },
                    LearningDelivery = new MessageLearnerLearningDelivery { AimSeqNumber = 2 },
                    LearningDeliveryFAM_ACTs = new LearningDeliveryACT()
                },
                new NonContractedAppsActivityReportModel
                {
                    Learner = new MessageLearner { LearnRefNumber = "Learner3" },
                    LearningDelivery = new MessageLearnerLearningDelivery { AimSeqNumber = 1 },
                    LearningDeliveryFAM_ACTs = new LearningDeliveryACT()
                },
                new NonContractedAppsActivityReportModel
                {
                    Learner = new MessageLearner { LearnRefNumber = "Learner2" },
                    LearningDelivery = new MessageLearnerLearningDelivery { AimSeqNumber = 1 },
                    LearningDeliveryFAM_ACTs = new LearningDeliveryACT()
                },
            };

            var expectedModels = new List<NonContractedAppsActivityReportModel>
            {
                new NonContractedAppsActivityReportModel
                {
                    Learner = new MessageLearner { LearnRefNumber = "Learner1" },
                    LearningDelivery = new MessageLearnerLearningDelivery { AimSeqNumber = 1 },
                    LearningDeliveryFAM_ACTs = new LearningDeliveryACT()
                },
                new NonContractedAppsActivityReportModel
                {
                    Learner = new MessageLearner { LearnRefNumber = "Learner2" },
                    LearningDelivery = new MessageLearnerLearningDelivery { AimSeqNumber = 1 },
                    LearningDeliveryFAM_ACTs = new LearningDeliveryACT()
                },
                new NonContractedAppsActivityReportModel
                {
                    Learner = new MessageLearner { LearnRefNumber = "Learner3" },
                    LearningDelivery = new MessageLearnerLearningDelivery { AimSeqNumber = 1 },
                    LearningDeliveryFAM_ACTs = new LearningDeliveryACT()
                },
                new NonContractedAppsActivityReportModel
                {
                    Learner = new MessageLearner { LearnRefNumber = "Learner3" },
                    LearningDelivery = new MessageLearnerLearningDelivery { AimSeqNumber = 2 },
                    LearningDeliveryFAM_ACTs = new LearningDeliveryACT()
                },
            };

            NewBuilder().ApplySort(models).Should().BeEquivalentTo(expectedModels);
        }

        [Fact]
        public void BuildReportRows_NoRows()
        {
            var learnerData = new FM36LearnerData[]
            {
                new FM36LearnerData
                {
                    Learner = new MessageLearner
                    {
                        LearnRefNumber = "Learner1",
                    },
                    LearningDeliveries = new FM36LearningDeliveryData[]
                    {
                        new FM36LearningDeliveryData
                        {
                            LearningDelivery = new MessageLearnerLearningDelivery
                            {
                                LearnAimRef = "LearnAimRef1",
                                AimSeqNumber = 1,
                                FundModel = 36,
                            },
                            FM36LearningDelivery = new FM36LearningDeliveryValue
                            {
                                FundLineValues = new FundLineValue[]
                                {
                                    new FundLineValue
                                    {
                                        FundLineType = FundLineConstants.ApprenticeshipEmployerOnAppService1618,
                                        ReportTotals = new ReportTotals
                                        {
                                        }
                                    }
                                },
                                LearningDeliveryValues = new LearningDeliveryValues
                                {
                                    LearnDelMathEng = true
                                }
                            },
                            FM36PriceEpisodes = new FM36PriceEpisodeValue[]
                            {
                                new FM36PriceEpisodeValue
                                {
                                    FundLineValues = new FundLineValue(),
                                    PriceEpisodeValue = new PriceEpisodeValues
                                    {

                                    }
                                },
                            }
                        },
                    },
                }
            };

            var larsDictionary = new Dictionary<string, LARSLearningDelivery>
            {
                { "LearnAimRef1", new LARSLearningDelivery { LearnAimRef = "LearnAimRef1", LearnAimRefTitle = "Title1" } },
                { "LearnAimRef2", new LARSLearningDelivery { LearnAimRef = "LearnAimRef2", LearnAimRefTitle = "Title2" } }
            };

            var censusEndDates = CensusDates().ToDictionary(p => p.Period, e => (DateTime?)e.End);

            var expectedModels = new List<NonContractedAppsActivityReportModel>
            {
                new NonContractedAppsActivityReportModel
                {
                    Learner = new MessageLearner { LearnRefNumber = "Learner1" },
                    LearningDelivery = new MessageLearnerLearningDelivery { AimSeqNumber = 1 }
                },
                new NonContractedAppsActivityReportModel
                {
                    Learner = new MessageLearner { LearnRefNumber = "Learner2" },
                    LearningDelivery = new MessageLearnerLearningDelivery { AimSeqNumber = 1 }
                },
            };

            NewBuilder().BuildReportRows(learnerData, larsDictionary, censusEndDates).Should().BeNullOrEmpty();
        }

        [Fact]
        public void BuildReportRows()
        {
            var learnerData = new FM36LearnerData[]
            {
                new FM36LearnerData
                {
                    Learner = new MessageLearner
                    {
                        LearnRefNumber = "Learner1",
                    },
                    LearningDeliveries = new FM36LearningDeliveryData[]
                    {
                        new FM36LearningDeliveryData
                        {
                            LearningDelivery = new MessageLearnerLearningDelivery
                            {
                                LearnAimRef = "LearnAimRef1",
                                AimSeqNumber = 1,
                                FundModel = 36,
                                LearnStartDate = new DateTime(2019, 09, 01),
                            },
                            LearningDeliveryFAMs_ACT = new LearningDeliveryACT[]
                            {
                                new LearningDeliveryACT
                                {
                                    LearnDelFAMType = LearnerFAMTypeConstants.ACT,
                                    LearnDelFAMCode = "1",
                                    LearnDelFAMDateFromNullable = new DateTime(2019, 09, 01),
                                }
                            },
                            FM36LearningDelivery = new FM36LearningDeliveryValue
                            {
                                FundLineValues = new FundLineValue[]
                                {
                                    new FundLineValue
                                    {
                                        FundLineType = FundLineConstants.ApprenticeshipEmployerOnAppService1618,
                                        ReportTotals = new ReportTotals
                                        {
                                            LearnRefNumber = "Learner1",
                                            AimSeqNumber = 1,
                                            AugustTotal = 1m,
                                            SeptemberTotal = 2m,
                                            OctoberTotal = 0m,
                                            NovemberTotal = 0m,
                                            DecemberTotal = 0m,
                                            JanuaryTotal = 0m,
                                            FebruaryTotal = 0m,
                                            MarchTotal = 0m,
                                            AprilTotal= 0m,
                                            MayTotal = 0m,
                                            JuneTotal = 0m,
                                            JulyTotal = 0m,
                                        }
                                    },
                                    new FundLineValue
                                    {
                                        FundLineType = FundLineConstants.ApprenticeshipEmployerOnAppService19Plus,
                                        ReportTotals = new ReportTotals
                                        {
                                            LearnRefNumber = "Learner1",
                                            AimSeqNumber = 1,
                                            AugustTotal = 0m,
                                            SeptemberTotal = 0m,
                                            OctoberTotal = 1m,
                                            NovemberTotal = 2m,
                                            DecemberTotal = 0m,
                                            JanuaryTotal = 0m,
                                            FebruaryTotal = 0m,
                                            MarchTotal = 0m,
                                            AprilTotal= 0m,
                                            MayTotal = 0m,
                                            JuneTotal = 0m,
                                            JulyTotal = 0m,
                                        }
                                    }
                                },
                                LearningDeliveryValues = new LearningDeliveryValues
                                {
                                    LearnDelMathEng = true
                                }
                            },
                        },
                    },
                },
                new FM36LearnerData
                {
                    Learner = new MessageLearner
                    {
                        LearnRefNumber = "Learner2",
                    },
                    LearningDeliveries = new FM36LearningDeliveryData[]
                    {
                        new FM36LearningDeliveryData
                        {
                            LearningDelivery = new MessageLearnerLearningDelivery
                            {
                                LearnAimRef = "LearnAimRef2",
                                AimSeqNumber = 1,
                                FundModel = 36,
                                LearnStartDate = new DateTime(2019, 09, 01),
                            },
                            LearningDeliveryFAMs_ACT = new LearningDeliveryACT[]
                            {
                                new LearningDeliveryACT
                                {
                                    LearnDelFAMType = LearnerFAMTypeConstants.ACT,
                                    LearnDelFAMCode = "1",
                                    LearnDelFAMDateFromNullable = new DateTime(2019, 09, 01),
                                }
                            },
                            FM36LearningDelivery = new FM36LearningDeliveryValue
                            {
                                FundLineValues = new FundLineValue[]
                                {
                                    new FundLineValue
                                    {
                                        FundLineType = FundLineConstants.ApprenticeshipEmployerOnAppService1618,
                                        ReportTotals = new ReportTotals
                                        {
                                            LearnRefNumber = "Learner2",
                                            AimSeqNumber = 1,
                                            AugustTotal = 1m,
                                            SeptemberTotal = 2m
                                        }
                                    }
                                },
                                LearningDeliveryValues = new LearningDeliveryValues
                                {
                                    LearnDelMathEng = false
                                }
                            },
                            FM36PriceEpisodes = new FM36PriceEpisodeValue[]
                            {
                                new FM36PriceEpisodeValue
                                {
                                    PriceEpisodeValue = new PriceEpisodeValues
                                    {
                                        EpisodeStartDate = new DateTime(2019, 09, 01),
                                        PriceEpisodeFundLineType = FundLineConstants.ApprenticeshipEmployerOnAppService1618,
                                    },
                                    FundLineValues = new FundLineValue
                                    {
                                        FundLineType = FundLineConstants.ApprenticeshipEmployerOnAppService1618,
                                        ReportTotals = new ReportTotals
                                        {
                                            LearnRefNumber = "Learner2",
                                            AimSeqNumber = 1,
                                            AugustTotal = 1m,
                                            SeptemberTotal = 2m,
                                            OctoberTotal = 0m,
                                            NovemberTotal = 0m,
                                            DecemberTotal = 0m,
                                            JanuaryTotal = 0m,
                                            FebruaryTotal = 0m,
                                            MarchTotal = 0m,
                                            AprilTotal= 0m,
                                            MayTotal = 0m,
                                            JuneTotal = 0m,
                                            JulyTotal = 0m,
                                        }
                                    },
                                },
                            }
                        },
                    },
                 }
            };

            var larsDictionary = new Dictionary<string, LARSLearningDelivery>
            {
                { "LearnAimRef1", new LARSLearningDelivery { LearnAimRef = "LearnAimRef1", LearnAimRefTitle = "Title1" } },
                { "LearnAimRef2", new LARSLearningDelivery { LearnAimRef = "LearnAimRef2", LearnAimRefTitle = "Title2" } }
            };

            var censusEndDates = CensusDates().ToDictionary(p => p.Period, e => (DateTime?)e.End);

            var expectedModels = new List<NonContractedAppsActivityReportModel>
            {
                new NonContractedAppsActivityReportModel
                {
                    Learner = new MessageLearner { LearnRefNumber = "Learner1" },
                    LearningDelivery = new MessageLearnerLearningDelivery { AimSeqNumber = 1, FundModel = 36, LearnStartDate = new DateTime(2019, 09, 01), LearnAimRef = "LearnAimRef1" },
                    LarsLearningDelivery = larsDictionary["LearnAimRef1"],
                    FundingLineType = FundLineConstants.ApprenticeshipEmployerOnAppService1618,
                    Fm36LearningDelivery = new LearningDeliveryValues
                    {
                        LearnDelMathEng = true
                    },
                    LearningDeliveryFAM_ACTs = new LearningDeliveryACT
                    {
                        LearnDelFAMType = LearnerFAMTypeConstants.ACT,
                        LearnDelFAMCode = "1",
                        LearnDelFAMDateFromNullable = new DateTime(2019, 09, 01),
                    },
                    AugustTotal = 0m,
                    SeptemberTotal = 2m,
                    OctoberTotal = 0m,
                    NovemberTotal = 0m,
                    DecemberTotal = 0m,
                    JanuaryTotal = 0m,
                    FebruaryTotal = 0m,
                    MarchTotal = 0m,
                    AprilTotal= 0m,
                    MayTotal = 0m,
                    JuneTotal = 0m,
                    JulyTotal = 0m,
                },
                new NonContractedAppsActivityReportModel
                {
                    Learner = new MessageLearner { LearnRefNumber = "Learner1" },
                    LearningDelivery = new MessageLearnerLearningDelivery { AimSeqNumber = 1, FundModel = 36, LearnStartDate = new DateTime(2019, 09, 01), LearnAimRef = "LearnAimRef1" },
                    LarsLearningDelivery = larsDictionary["LearnAimRef1"],
                    FundingLineType = FundLineConstants.ApprenticeshipEmployerOnAppService19Plus,
                    Fm36LearningDelivery = new LearningDeliveryValues
                    {
                        LearnDelMathEng = true
                    },
                    LearningDeliveryFAM_ACTs = new LearningDeliveryACT
                    {
                        LearnDelFAMType = LearnerFAMTypeConstants.ACT,
                        LearnDelFAMCode = "1",
                        LearnDelFAMDateFromNullable = new DateTime(2019, 09, 01),
                    },
                    AugustTotal = 0m,
                    SeptemberTotal = 0m,
                    OctoberTotal = 1m,
                    NovemberTotal = 2m,
                    DecemberTotal = 0m,
                    JanuaryTotal = 0m,
                    FebruaryTotal = 0m,
                    MarchTotal = 0m,
                    AprilTotal= 0m,
                    MayTotal = 0m,
                    JuneTotal = 0m,
                    JulyTotal = 0m,
                },
                new NonContractedAppsActivityReportModel
                {
                    Learner = new MessageLearner { LearnRefNumber = "Learner2" },
                    LearningDelivery = new MessageLearnerLearningDelivery { AimSeqNumber = 1, FundModel = 36, LearnStartDate = new DateTime(2019, 09, 01), LearnAimRef = "LearnAimRef2"},
                    LarsLearningDelivery = larsDictionary["LearnAimRef2"],
                    FundingLineType = FundLineConstants.ApprenticeshipEmployerOnAppService1618,
                    Fm36LearningDelivery = new LearningDeliveryValues
                    {
                        LearnDelMathEng = false
                    },
                    PriceEpisodeStartDate = new DateTime(2019, 09, 01),
                    PriceEpisodeValues = new PriceEpisodeValues
                    {
                        EpisodeStartDate = new DateTime(2019, 09, 01),
                        PriceEpisodeFundLineType = FundLineConstants.ApprenticeshipEmployerOnAppService1618,
                    },
                    LearningDeliveryFAM_ACTs = new LearningDeliveryACT
                    {
                        LearnDelFAMType = LearnerFAMTypeConstants.ACT,
                        LearnDelFAMCode = "1",
                        LearnDelFAMDateFromNullable = new DateTime(2019, 09, 01),
                    },
                    AugustTotal = 1m,
                    SeptemberTotal = 2m,
                    OctoberTotal = 0m,
                    NovemberTotal = 0m,
                    DecemberTotal = 0m,
                    JanuaryTotal = 0m,
                    FebruaryTotal = 0m,
                    MarchTotal = 0m,
                    AprilTotal= 0m,
                    MayTotal = 0m,
                    JuneTotal = 0m,
                    JulyTotal = 0m,
                },
            };

            NewBuilder().BuildReportRows(learnerData, larsDictionary, censusEndDates).Should().BeEquivalentTo(expectedModels);
        }

        [Fact]
        public void Build()
        {
            var contractsDictionary = new List<KeyValuePair<string, string[]>>
            {
                new KeyValuePair<string, string[]>(FundLineConstants.ApprenticeshipEmployerOnAppService1618, new string[] { ContractsConstants.Levy1799, ContractsConstants.NonLevy1799 }),
                new KeyValuePair<string, string[]>(FundLineConstants.ApprenticeshipEmployerOnAppService19Plus, new string[] { ContractsConstants.Levy1799, ContractsConstants.NonLevy1799 }),
                new KeyValuePair<string, string[]>(FundLineConstants.NonLevyApprenticeship1618NonProcured, new string[] { ContractsConstants.Apps1920 }),
                new KeyValuePair<string, string[]>(FundLineConstants.NonLevyApprenticeship1618Procured, new string[] { ContractsConstants.C1618nlap2018 }),
                new KeyValuePair<string, string[]>(FundLineConstants.NonLevyApprenticeship19PlusNonProcured, new string[] { ContractsConstants.Apps1920 }),
                new KeyValuePair<string, string[]>(FundLineConstants.NonLevyApprenticeship19PlusProcured, new string[] { ContractsConstants.Anlap2018 })
            }.ToDictionary(k => k.Key, v => v.Value);

            var fspCodes = new List<string>
            {
                "Code1"
            };

            ILearningDelivery learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = "LearnAimRef1",
                AimSeqNumber = 1,
                FundModel = 36,
                LearnStartDate = new DateTime(2019, 09, 01),
                LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                {
                    new TestLearningDeliveryFAM
                    {
                        LearnDelFAMType = "SOF",
                        LearnDelFAMCode = "105",
                        LearnDelFAMDateFromNullable = new DateTime(2019, 8, 1)
                    },
                    new TestLearningDeliveryFAM
                    {
                        LearnDelFAMType = "ACT",
                        LearnDelFAMCode = "1",
                        LearnDelFAMDateFromNullable = new DateTime(2019, 9, 1)
                    }
                }
            };

            ILearner learner1 = new TestLearner
            {
                LearnRefNumber = "Learner1",
                LearningDeliveries = new[]
                {
                    learningDelivery
                }
            };

            ILearner learner2 = new TestLearner
            {
                LearnRefNumber = "Learner2",
                LearningDeliveries = new[]
                {
                    learningDelivery
                }
            };

            ILearner learner3 = new TestLearner
            {
                LearnRefNumber = "Learner3",
                LearningDeliveries = new[]
                {
                   learningDelivery
                }
            };

            IMessage message = new TestMessage
            {
                Learners = new[]
                {
                    learner1,
                    learner2,
                    learner3
                }
            };

            var global = new FM36Global
            {
                Learners = new List<FM36Learner>
                {
                    new FM36Learner
                    {
                        LearnRefNumber = "Learner1",
                        LearningDeliveries = new List<LearningDelivery>
                        {
                            new LearningDelivery
                            {
                                AimSeqNumber = 1,
                                LearningDeliveryValues = new LearningDeliveryValues
                                {
                                    LearnDelMathEng = true
                                },
                                LearningDeliveryPeriodisedTextValues = new List<LearningDeliveryPeriodisedTextValues>
                                {
                                    new LearningDeliveryPeriodisedTextValues
                                    {
                                        AttributeName = AttributeConstants.Fm36FundLineType,
                                        Period1 = FundLineConstants.ApprenticeshipEmployerOnAppService1618,
                                        Period2 = FundLineConstants.ApprenticeshipEmployerOnAppService1618,
                                        Period3 = FundLineConstants.ApprenticeshipEmployerOnAppService1618
                                    }
                                },
                                LearningDeliveryPeriodisedValues =  new List<LearningDeliveryPeriodisedValues>
                                {
                                    new LearningDeliveryPeriodisedValues
                                    {
                                        AttributeName = AttributeConstants.Fm36MathEngOnProgPayment,
                                        Period1 = 0m,
                                        Period2 = 2m,
                                        Period3 = 3m
                                    }
                                },
                            }
                        },
                        PriceEpisodes = new List<PriceEpisode>
                        {
                            new PriceEpisode
                            {
                                PriceEpisodeValues = new PriceEpisodeValues
                                {
                                    PriceEpisodeAimSeqNumber = 1,
                                    EpisodeStartDate = new DateTime(2019, 09, 01),
                                    PriceEpisodeFundLineType = FundLineConstants.ApprenticeshipEmployerOnAppService1618,
                                },
                                PriceEpisodePeriodisedValues = new List<PriceEpisodePeriodisedValues>
                                {
                                    new PriceEpisodePeriodisedValues
                                    {
                                        AttributeName = AttributeConstants.Fm36PriceEpisodeOnProgPaymentAttributeName,
                                        Period1 = 0m,
                                        Period2 = 2m,
                                        Period3 = 3m
                                    }
                                }
                            }
                        }
                    },
                    new FM36Learner
                    {
                        LearnRefNumber = "Learner2",
                        LearningDeliveries = new List<LearningDelivery>
                        {
                            new LearningDelivery
                            {
                                AimSeqNumber = 1,
                                LearningDeliveryValues = new LearningDeliveryValues
                                {
                                    LearnDelMathEng = false
                                },
                                LearningDeliveryPeriodisedTextValues = new List<LearningDeliveryPeriodisedTextValues>
                                {
                                    new LearningDeliveryPeriodisedTextValues
                                    {
                                        AttributeName = AttributeConstants.Fm36FundLineType,
                                        Period1 = FundLineConstants.ApprenticeshipEmployerOnAppService1618,
                                        Period2 = FundLineConstants.ApprenticeshipEmployerOnAppService1618,
                                        Period3 = FundLineConstants.ApprenticeshipEmployerOnAppService1618
                                    }
                                },
                                LearningDeliveryPeriodisedValues =  new List<LearningDeliveryPeriodisedValues>
                                {
                                    new LearningDeliveryPeriodisedValues
                                    {
                                        AttributeName = AttributeConstants.Fm36MathEngOnProgPayment,
                                        Period1 = 0m,
                                        Period2 = 2m,
                                        Period3 = 3m
                                    }
                                },
                            },
                        },
                        PriceEpisodes = new List<PriceEpisode>
                        {
                            new PriceEpisode
                            {
                                PriceEpisodeValues = new PriceEpisodeValues
                                {
                                    PriceEpisodeAimSeqNumber = 1,
                                    EpisodeStartDate = new DateTime(2019, 09, 01),
                                    PriceEpisodeFundLineType = FundLineConstants.ApprenticeshipEmployerOnAppService1618,
                                },
                                PriceEpisodePeriodisedValues = new List<PriceEpisodePeriodisedValues>
                                {
                                    new PriceEpisodePeriodisedValues
                                    {
                                        AttributeName = AttributeConstants.Fm36PriceEpisodeOnProgPaymentAttributeName,
                                        Period1 = 0m,
                                        Period2 = 2m,
                                        Period3 = 3m
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var referenceDataRoot = new ReferenceDataRoot
            {
                MetaDatas = new ReferenceDataService.Model.MetaData.MetaData
                {
                    CollectionDates = new IlrCollectionDates
                    {
                        CensusDates = CensusDates()
                    },
                },
                LARSLearningDeliveries = new List<LARSLearningDelivery>
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
                },
                FCSContractAllocations = new List<FcsContractAllocation>
                {
                    new FcsContractAllocation { ContractAllocationNumber = "ConRef1", FundingStreamPeriodCode = "FSPC1" },
                    new FcsContractAllocation { ContractAllocationNumber = "ConRef2", FundingStreamPeriodCode = "FSPC1" },
                    new FcsContractAllocation { ContractAllocationNumber = "ConRef3", FundingStreamPeriodCode = "FSPC2" },
                    new FcsContractAllocation { ContractAllocationNumber = "ConRef4", FundingStreamPeriodCode = "FSPC3" },
                }
            };

            var expectedModels = new List<NonContractedAppsActivityReportModel>
            {
                new NonContractedAppsActivityReportModel
                {
                    Learner = learner1,
                    LearningDelivery = learner1.LearningDeliveries.First(),
                    LearningDeliveryFAMs = new LearningDeliveryFAMsModel { SOF = "105" },
                    LarsLearningDelivery = new LARSLearningDelivery
                    {
                        LearnAimRef = "LearnAimRef1",
                        LearnAimRefTitle = "Title1"
                    },
                    FundingLineType = FundLineConstants.ApprenticeshipEmployerOnAppService1618,
                    Fm36LearningDelivery = new LearningDeliveryValues
                    {
                        LearnDelMathEng = true
                    },
                    LearningDeliveryFAM_ACTs = new LearningDeliveryACT
                    {
                        LearnDelFAMType = LearnerFAMTypeConstants.ACT,
                        LearnDelFAMCode = "1",
                        LearnDelFAMDateFromNullable = new DateTime(2019, 09, 01),
                    },
                    ProviderSpecDeliveryMonitoring = new ProviderSpecDeliveryMonitoringModel(),
                    ProviderSpecLearnerMonitoring = new ProviderSpecLearnerMonitoringModel(),
                    AugustTotal = 0m,
                    SeptemberTotal = 2m,
                    OctoberTotal = 3m,
                    NovemberTotal = 0m,
                    DecemberTotal = 0m,
                    JanuaryTotal = 0m,
                    FebruaryTotal = 0m,
                    MarchTotal = 0m,
                    AprilTotal= 0m,
                    MayTotal = 0m,
                    JuneTotal = 0m,
                    JulyTotal = 0m,
                },
                new NonContractedAppsActivityReportModel
                {
                    Learner = learner2,
                    LearningDelivery = learner2.LearningDeliveries.First(),
                    LearningDeliveryFAMs = new LearningDeliveryFAMsModel { SOF = "105" },
                    LarsLearningDelivery = new LARSLearningDelivery
                    {
                        LearnAimRef = "LearnAimRef1",
                        LearnAimRefTitle = "Title1"
                    },
                    FundingLineType = FundLineConstants.ApprenticeshipEmployerOnAppService1618,
                     Fm36LearningDelivery = new LearningDeliveryValues
                    {
                        LearnDelMathEng = false
                    },
                    PriceEpisodeStartDate = new DateTime(2019, 09, 01),
                    PriceEpisodeValues = new PriceEpisodeValues
                    {
                        EpisodeStartDate = new DateTime(2019, 09, 01),
                        PriceEpisodeAimSeqNumber = 1,
                        PriceEpisodeFundLineType = FundLineConstants.ApprenticeshipEmployerOnAppService1618,
                    },
                    LearningDeliveryFAM_ACTs = new LearningDeliveryACT
                    {
                        LearnDelFAMType = LearnerFAMTypeConstants.ACT,
                        LearnDelFAMCode = "1",
                        LearnDelFAMDateFromNullable = new DateTime(2019, 09, 01),
                    },
                    ProviderSpecDeliveryMonitoring = new ProviderSpecDeliveryMonitoringModel(),
                    ProviderSpecLearnerMonitoring = new ProviderSpecLearnerMonitoringModel(),
                    AugustTotal = 0m,
                    SeptemberTotal = 2m,
                    OctoberTotal = 3m,
                    NovemberTotal = 0m,
                    DecemberTotal = 0m,
                    JanuaryTotal = 0m,
                    FebruaryTotal = 0m,
                    MarchTotal = 0m,
                    AprilTotal= 0m,
                    MayTotal = 0m,
                    JuneTotal = 0m,
                    JulyTotal = 0m,
                },
            };

            var dependentDataMock = new Mock<IReportServiceDependentData>();

            dependentDataMock.Setup(d => d.Get<ReferenceDataRoot>()).Returns(referenceDataRoot);
            dependentDataMock.Setup(d => d.Get<FM36Global>()).Returns(global);
            dependentDataMock.Setup(d => d.Get<IMessage>()).Returns(message);

            NewBuilder(new AcademicYearService(), new IlrModelMapper()).Build(new Mock<IReportServiceContext>().Object, dependentDataMock.Object).Should().BeEquivalentTo(expectedModels);
        }

        private List<CensusDate> CensusDates() => new List<CensusDate>
        {
            new CensusDate { Period = 1, Start = new DateTime(2019, 08, 01), End = new DateTime(2019, 08, 31) },
            new CensusDate { Period = 2, Start = new DateTime(2019, 09, 01), End = new DateTime(2019, 09, 30) },
            new CensusDate { Period = 3, Start = new DateTime(2019, 10, 01), End = new DateTime(2019, 10, 31) },
            new CensusDate { Period = 4, Start = new DateTime(2019, 11, 01), End = new DateTime(2019, 11, 30) },
            new CensusDate { Period = 5, Start = new DateTime(2019, 12, 01), End = new DateTime(2019, 12, 31) },
            new CensusDate { Period = 6, Start = new DateTime(2020, 01, 01), End = new DateTime(2020, 01, 31) },
            new CensusDate { Period = 7, Start = new DateTime(2020, 02, 01), End = new DateTime(2020, 02, 29) },
            new CensusDate { Period = 8, Start = new DateTime(2020, 03, 01), End = new DateTime(2020, 03, 31) },
            new CensusDate { Period = 9, Start = new DateTime(2020, 04, 01), End = new DateTime(2020, 04, 30) },
            new CensusDate { Period = 10, Start = new DateTime(2020, 05, 01), End = new DateTime(2020, 05, 31) },
            new CensusDate { Period = 11, Start = new DateTime(2020, 06, 01), End = new DateTime(2020, 06, 30) },
            new CensusDate { Period = 12, Start = new DateTime(2020, 07, 01), End = new DateTime(2020, 07, 31) }
        };


        private NonContractedAppsActivityReportModelBuilder NewBuilder(IAcademicYearService academicYearService = null, IIlrModelMapper ilrModelMapper = null)
        {
            return new NonContractedAppsActivityReportModelBuilder(academicYearService, ilrModelMapper);
        }
    }
}
