﻿using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReferenceDataService.Model.LARS;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Devolved;
using ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Devolved.Model;
using ESFA.DC.ILR.ReportService.Reports.Model;
using ESFA.DC.ILR.ReportService.Reports.Model.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.Tests.Model;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding.DevolvedOccupancy
{
    public class DevolvedAdultEducationOccupancyReportModelBuilderTests
    {
        [Fact]
        public void LearningDeliveryFilter_False_Null()
        {
            var learningDeliveryMock = new Mock<ILearningDelivery>();

            learningDeliveryMock.SetupGet(ld => ld.LearningDeliveryFAMs).Returns(null as IReadOnlyCollection<ILearningDeliveryFAM>);

            NewBuilder().LearningDeliveryReportFilter(learningDeliveryMock.Object).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFilter_False_Empty()
        {
            var learningDeliveryMock = new Mock<ILearningDelivery>();

            learningDeliveryMock.SetupGet(ld => ld.LearningDeliveryFAMs).Returns(new List<ILearningDeliveryFAM>());

            NewBuilder().LearningDeliveryReportFilter(learningDeliveryMock.Object).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFilter_False_Type()
        {
            var learningDeliveryMock = new Mock<ILearningDelivery>();
            var learningDeliveryFAM = new Mock<ILearningDeliveryFAM>();

            learningDeliveryFAM.SetupGet(fam => fam.LearnDelFAMType).Returns("NotSOF");

            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>()
            {
                learningDeliveryFAM.Object
            };
            
            learningDeliveryMock.SetupGet(ld => ld.LearningDeliveryFAMs).Returns(learningDeliveryFAMs);

            NewBuilder().LearningDeliveryReportFilter(learningDeliveryMock.Object).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFilter_False_Code()
        {
            var learningDeliveryMock = new Mock<ILearningDelivery>();
            var learningDeliveryFAM = new Mock<ILearningDeliveryFAM>();

            learningDeliveryFAM.SetupGet(fam => fam.LearnDelFAMType).Returns("SOF");
            learningDeliveryFAM.SetupGet(fam => fam.LearnDelFAMCode).Returns("999");

            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>()
            {
                learningDeliveryFAM.Object
            };

            learningDeliveryMock.SetupGet(ld => ld.LearningDeliveryFAMs).Returns(learningDeliveryFAMs);

            NewBuilder().LearningDeliveryReportFilter(learningDeliveryMock.Object).Should().BeFalse();
        }

        [InlineData("110")]
        [InlineData("111")]
        [InlineData("112")]
        [InlineData("113")]
        [InlineData("114")]
        [InlineData("115")]
        [InlineData("116")]
        [Theory]
        public void LearningDeliveryFilter_True(string code)
        {
            var learningDeliveryMock = new Mock<ILearningDelivery>();
            var learningDeliveryFAM = new Mock<ILearningDeliveryFAM>();

            learningDeliveryFAM.SetupGet(fam => fam.LearnDelFAMType).Returns("SOF");
            learningDeliveryFAM.SetupGet(fam => fam.LearnDelFAMCode).Returns(code);

            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>()
            {
                learningDeliveryFAM.Object
            };

            learningDeliveryMock.SetupGet(ld => ld.LearningDeliveryFAMs).Returns(learningDeliveryFAMs);

            NewBuilder().LearningDeliveryReportFilter(learningDeliveryMock.Object).Should().BeTrue();
        }

        

        [Fact]
        public void OrderBy()
        {
            var six = new DevolvedAdultEducationOccupancyReportModel() { Learner = new TestLearner() { LearnRefNumber = "D" }, LearningDelivery = new TestLearningDelivery() { AimSeqNumber = 1 } };
            var five = new DevolvedAdultEducationOccupancyReportModel() { Learner = new TestLearner() { LearnRefNumber = "C" }, LearningDelivery = new TestLearningDelivery() { AimSeqNumber = 1 } };
            var two = new DevolvedAdultEducationOccupancyReportModel() { Learner = new TestLearner() { LearnRefNumber = "A" }, LearningDelivery = new TestLearningDelivery() { AimSeqNumber = 2 } };
            var four = new DevolvedAdultEducationOccupancyReportModel() { Learner = new TestLearner() { LearnRefNumber = "B" }, LearningDelivery = new TestLearningDelivery() { AimSeqNumber = 1 } };
            var three = new DevolvedAdultEducationOccupancyReportModel() { Learner = new TestLearner() { LearnRefNumber = "A" }, LearningDelivery = new TestLearningDelivery() { AimSeqNumber = 3 } };
            var one = new DevolvedAdultEducationOccupancyReportModel() { Learner = new TestLearner() { LearnRefNumber = "A" }, LearningDelivery = new TestLearningDelivery() { AimSeqNumber = 1 } };

            var rows = new List<DevolvedAdultEducationOccupancyReportModel>()
            {
                six, five, two, four, three, one
            };

            var result = NewBuilder().Order(rows).ToList();

            result.Should().ContainInOrder(one, two, three, four, five, six);
        }

        [Fact]
        public void Build_SingleRow()
        {
            var reportServiceContext = Mock.Of<IReportServiceContext>();
            var dependentDataMock = new Mock<IReportServiceDependentData>();

            var learningDeliveryFams = new List<ILearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = "SOF",
                    LearnDelFAMCode = "110",
                }
            };

            var providerSpecDeliveryMonitorings = new List<IProviderSpecDeliveryMonitoring>()
            {
                new TestProviderSpecDeliveryMonitoring(),
            };

            var learningDelivery = new TestLearningDelivery()
            {
                LearnAimRef = "learnAimRef",
                AimSeqNumber = 1,
                LearningDeliveryFAMs = learningDeliveryFams,
                ProviderSpecDeliveryMonitorings = providerSpecDeliveryMonitorings
            };

            var providerSpecLearnerMonitorings = new List<IProviderSpecLearnerMonitoring>()
            {
                new TestProviderSpecLearnerMonitoring(),
            };

            var learner = new TestLearner()
            {
                LearnRefNumber = "LearnRefNumber",
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    learningDelivery
                },
                ProviderSpecLearnerMonitorings = providerSpecLearnerMonitorings,
            };

            var message = new TestMessage()
            {
                Learners = new List<ILearner>()
                {
                    learner
                }
            };

            var larsLearningDelivery = new LARSLearningDelivery()
            {
                LearnAimRef = "learnAimRef"
            };

            var referenceDataRoot = new ReferenceDataRoot()
            {
                LARSLearningDeliveries = new List<LARSLearningDelivery>()
                {
                    larsLearningDelivery
                }
            };

            var fm35LearningDeliveryValue = new LearningDeliveryValue();

            var fm35LearningDelivery = new LearningDelivery()
            {
                AimSeqNumber = 1,
                LearningDeliveryValue = fm35LearningDeliveryValue,
                LearningDeliveryPeriodisedValues = new List<LearningDeliveryPeriodisedValue>()
                {
                    BuildPeriodisedValuesForAttribute(AttributeConstants.Fm35OnProgPayment),
                    BuildPeriodisedValuesForAttribute(AttributeConstants.Fm35AchievePayPct),
                    BuildPeriodisedValuesForAttribute(AttributeConstants.Fm35AchievePayment),
                    BuildPeriodisedValuesForAttribute(AttributeConstants.Fm35EmpOutcomePay),
                    BuildPeriodisedValuesForAttribute(AttributeConstants.Fm35BalancePayment),
                    BuildPeriodisedValuesForAttribute(AttributeConstants.Fm35LearnSuppFundCash),
                }
            };

            var fm35Global = new FM35Global()
            {
                Learners = new List<FM35Learner>()
                {
                    new FM35Learner()
                    {
                        LearnRefNumber = "LearnRefNumber",
                        LearningDeliveries = new List<LearningDelivery>()
                        {
                            fm35LearningDelivery,
                        }
                    }
                }
            };

            dependentDataMock.Setup(d => d.Get<IMessage>()).Returns(message);
            dependentDataMock.Setup(d => d.Get<ReferenceDataRoot>()).Returns(referenceDataRoot);
            dependentDataMock.Setup(d => d.Get<FM35Global>()).Returns(fm35Global);

            var providerSpecDeliveryMonitoringModel = new ProviderSpecDeliveryMonitoringModel();
            var providerSpecLearnerMonitoringModel = new ProviderSpecLearnerMonitoringModel();
            var learningDeliveryFamsModel = new LearningDeliveryFAMsModel()
            {
                SOF = "110"
            };

            var ilrModelMapperMock = new Mock<IIlrModelMapper>();

            ilrModelMapperMock.Setup(m => m.MapLearningDeliveryFAMs(learningDeliveryFams)).Returns(learningDeliveryFamsModel);
            ilrModelMapperMock.Setup(m => m.MapProviderSpecDeliveryMonitorings(providerSpecDeliveryMonitorings)).Returns(providerSpecDeliveryMonitoringModel);
            ilrModelMapperMock.Setup(m => m.MapProviderSpecLearnerMonitorings(providerSpecLearnerMonitorings)).Returns(providerSpecLearnerMonitoringModel);

            var result = NewBuilder(ilrModelMapperMock.Object).Build(reportServiceContext, dependentDataMock.Object).ToList();   

            result.Should().HaveCount(1);

            var row = result[0];

            row.Learner.Should().Be(learner);
            row.LearningDelivery.Should().Be(learningDelivery);
            row.LarsLearningDelivery.Should().Be(larsLearningDelivery);
            row.Fm35LearningDelivery.Should().Be(fm35LearningDeliveryValue);

            row.ProviderSpecDeliveryMonitoring.Should().Be(providerSpecDeliveryMonitoringModel);
            row.ProviderSpecLearnerMonitoring.Should().Be(providerSpecLearnerMonitoringModel);
            row.PeriodisedValues.Should().NotBeNull();
            row.LearningDeliveryFAMs.Should().Be(learningDeliveryFamsModel);

            row.McaGlaShortCode.Should().Be("GMCA");
        }

        [Fact]
        public void Build_FiftyThousandLearners()
        {
            var reportServiceContext = Mock.Of<IReportServiceContext>();
            var dependentDataMock = new Mock<IReportServiceDependentData>();

            var ilrModelMapperMock = new Mock<IIlrModelMapper>();

            var providerSpecDeliveryMonitoring = new ProviderSpecDeliveryMonitoringModel();
            var providerSpecLearnMonitoring = new ProviderSpecLearnerMonitoringModel();
            var famModel = new LearningDeliveryFAMsModel();

            ilrModelMapperMock.Setup(m => m.MapProviderSpecDeliveryMonitorings(It.IsAny<IEnumerable<IProviderSpecDeliveryMonitoring>>())).Returns(providerSpecDeliveryMonitoring);
            ilrModelMapperMock.Setup(m => m.MapProviderSpecLearnerMonitorings(It.IsAny<IEnumerable<IProviderSpecLearnerMonitoring>>())).Returns(providerSpecLearnMonitoring);
            ilrModelMapperMock.Setup(m => m.MapLearningDeliveryFAMs(It.IsAny<IEnumerable<ILearningDeliveryFAM>>())).Returns(famModel);

            var learnerCount = 50000;
            
            var message = new TestMessage()
            {
                Learners = 
                    Enumerable.Range(0, learnerCount).Select(
                        l => new TestLearner()
                        {
                            LearnRefNumber = "LearnRefNumber" + l,
                            LearningDeliveries = new List<ILearningDelivery>()
                            {
                                new TestLearningDelivery()
                                {
                                    LearnAimRef = "learnAimRef" + l,
                                    AimSeqNumber = 1,
                                    LearningDeliveryFAMs = new List<ILearningDeliveryFAM>()
                                    {
                                        new TestLearningDeliveryFAM()
                                        {
                                            LearnDelFAMType = "SOF",
                                            LearnDelFAMCode = "110",
                                        }
                                    }
                                }
                            }
                        }).ToList()
            };

            var referenceDataRoot = new ReferenceDataRoot()
            {
                LARSLearningDeliveries = Enumerable.Range(0, learnerCount)
                    .Select(ld => 
                    new LARSLearningDelivery()
                    {
                        LearnAimRef = "learnAimRef" + ld
                    }).ToList()
            };
            
            var fm35Global = new FM35Global()
            {
                Learners = Enumerable.Range(0, learnerCount)
                    .Select(
                        l =>
                        new FM35Learner()
                        {
                            LearnRefNumber = "LearnRefNumber" + l,
                            LearningDeliveries = new List<LearningDelivery>()
                            {
                                new LearningDelivery()
                                {
                                    AimSeqNumber = 1,
                                    LearningDeliveryValue = new LearningDeliveryValue(),
                                    LearningDeliveryPeriodisedValues = new List<LearningDeliveryPeriodisedValue>()
                                    {
                                        BuildPeriodisedValuesForAttribute(AttributeConstants.Fm35OnProgPayment),
                                        BuildPeriodisedValuesForAttribute(AttributeConstants.Fm35AchievePayPct),
                                        BuildPeriodisedValuesForAttribute(AttributeConstants.Fm35AchievePayment),
                                        BuildPeriodisedValuesForAttribute(AttributeConstants.Fm35EmpOutcomePay),
                                        BuildPeriodisedValuesForAttribute(AttributeConstants.Fm35BalancePayment),
                                        BuildPeriodisedValuesForAttribute(AttributeConstants.Fm35LearnSuppFundCash),
                                    }
                                }
                            }
                        }).ToList()
            };

            dependentDataMock.Setup(d => d.Get<IMessage>()).Returns(message);
            dependentDataMock.Setup(d => d.Get<ReferenceDataRoot>()).Returns(referenceDataRoot);
            dependentDataMock.Setup(d => d.Get<FM35Global>()).Returns(fm35Global);

            var result = NewBuilder(ilrModelMapperMock.Object).Build(reportServiceContext, dependentDataMock.Object).ToList();

            result.Should().HaveCount(learnerCount);
        }

        private DevolvedAdultEducationOccupancyReportModelBuilder NewBuilder(IIlrModelMapper ilrModelMapper = null)
        {
            return new DevolvedAdultEducationOccupancyReportModelBuilder(ilrModelMapper);
        } 

        private LearningDeliveryPeriodisedValue BuildPeriodisedValuesForAttribute(string attribute)
        {
            return new LearningDeliveryPeriodisedValue()
            {
                AttributeName = attribute,
                Period1 = 1.111m,
                Period2 = 2.222m,
                Period3 = 3.333m,
                Period4 = 4.444m,
                Period5 = 5.555m,
                Period6 = 6.666m,
                Period7 = 7.777m,
                Period8 = 8.888m,
                Period9 = 9.999m,
                Period10 = 10.1010m,
                Period11 = 11.1111m,
                Period12 =  12.1212m
            };
        }
    }
}
