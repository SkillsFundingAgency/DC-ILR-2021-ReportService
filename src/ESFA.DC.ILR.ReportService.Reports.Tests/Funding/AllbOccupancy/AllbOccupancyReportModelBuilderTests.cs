using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.FundingService.ALB.FundingOutput.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReferenceDataService.Model.LARS;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.ALLB;
using ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Devolved;
using ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Main;
using ESFA.DC.ILR.ReportService.Reports.Model;
using ESFA.DC.ILR.ReportService.Reports.Model.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.Tests.Model;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding.DevolvedOccupancy
{
    public class AllbOccupancyReportModelBuilderTests
    {
        [Fact]
        public void LearningDeliveryFilter_False_NullLearningDelivery()
        {
            ILearningDelivery learningDelivery = null;
            
            var albLearningDelivery = new LearningDelivery()
            {
                LearningDeliveryValue = new LearningDeliveryValue()
            };

            NewBuilder().Filter(learningDelivery, albLearningDelivery).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFilter_False_NullAlbLearningDelivery()
        {
            var learningDeliveryMock = new Mock<ILearningDelivery>();

            learningDeliveryMock.SetupGet(ld => ld.LearningDeliveryFAMs).Returns(new List<ILearningDeliveryFAM>());

            LearningDelivery albLearningDelivery = null;

            NewBuilder().Filter(learningDeliveryMock.Object, albLearningDelivery).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFilter_False_NullLearningDeliveryFams()
        {
            var learningDeliveryMock = new Mock<ILearningDelivery>();

            learningDeliveryMock.SetupGet(ld => ld.LearningDeliveryFAMs).Returns(null as IReadOnlyCollection<ILearningDeliveryFAM>);

            var albLearningDelivery = new LearningDelivery()
            {
                LearningDeliveryValue = new LearningDeliveryValue()
            };

            NewBuilder().Filter(learningDeliveryMock.Object, albLearningDelivery).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFilter_False_NullAlbLearningDeliveryValue()
        {
            var learningDeliveryMock = new Mock<ILearningDelivery>();

            learningDeliveryMock.SetupGet(ld => ld.LearningDeliveryFAMs).Returns(new List<ILearningDeliveryFAM>());

            var albLearningDelivery = new LearningDelivery()
            {
                LearningDeliveryValue = null
            };

            NewBuilder().Filter(learningDeliveryMock.Object, albLearningDelivery).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFilter_False_Empty()
        {
            var learningDeliveryMock = new Mock<ILearningDelivery>();

            learningDeliveryMock.SetupGet(ld => ld.LearningDeliveryFAMs).Returns(new List<ILearningDeliveryFAM>());

            var albLearningDelivery = new LearningDelivery()
            {
                LearningDeliveryValue = new LearningDeliveryValue()
            };

            NewBuilder().Filter(learningDeliveryMock.Object, albLearningDelivery).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFilter_False_FundModel()
        {
            var learningDeliveryMock = new Mock<ILearningDelivery>();
            var learningDeliveryFAM = new Mock<ILearningDeliveryFAM>();

            learningDeliveryMock.SetupGet(ld => ld.FundModel).Returns(35);

            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>()
            {
                learningDeliveryFAM.Object
            };

            var albLearningDelivery = new LearningDelivery()
            {
                LearningDeliveryValue = new LearningDeliveryValue()
            };

            learningDeliveryMock.SetupGet(ld => ld.LearningDeliveryFAMs).Returns(learningDeliveryFAMs);

            NewBuilder().Filter(learningDeliveryMock.Object, albLearningDelivery).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFilter_False_Type()
        {
            var learningDeliveryMock = new Mock<ILearningDelivery>();
            var learningDeliveryFAM = new Mock<ILearningDeliveryFAM>();

            learningDeliveryMock.SetupGet(ld => ld.FundModel).Returns(99);

            learningDeliveryFAM.SetupGet(fam => fam.LearnDelFAMType).Returns("NotADL");

            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>()
            {
                learningDeliveryFAM.Object
            };

            var albLearningDelivery = new LearningDelivery()
            {
                LearningDeliveryValue = new LearningDeliveryValue()
            };

            learningDeliveryMock.SetupGet(ld => ld.LearningDeliveryFAMs).Returns(learningDeliveryFAMs);

            NewBuilder().Filter(learningDeliveryMock.Object, albLearningDelivery).Should().BeFalse();
        }
        
        [Fact]
        public void LearningDeliveryFilter_False_Code()
        {
            var learningDeliveryMock = new Mock<ILearningDelivery>();
            var learningDeliveryFAM = new Mock<ILearningDeliveryFAM>();

            learningDeliveryMock.SetupGet(ld => ld.FundModel).Returns(99);

            learningDeliveryFAM.SetupGet(fam => fam.LearnDelFAMType).Returns("ADL");
            learningDeliveryFAM.SetupGet(fam => fam.LearnDelFAMCode).Returns("999");

            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>()
            {
                learningDeliveryFAM.Object
            };

            var albLearningDelivery = new LearningDelivery()
            {
                LearningDeliveryValue = new LearningDeliveryValue()
            };

            learningDeliveryMock.SetupGet(ld => ld.LearningDeliveryFAMs).Returns(learningDeliveryFAMs);

            NewBuilder().Filter(learningDeliveryMock.Object, albLearningDelivery).Should().BeFalse();
        }
    
        [Fact]
        public void LearningDeliveryFilter_TrueAreaCostFactAdj()
        {
            var learningDeliveryMock = new Mock<ILearningDelivery>();
            var learningDeliveryFAM = new Mock<ILearningDeliveryFAM>();

            learningDeliveryMock.SetupGet(ld => ld.FundModel).Returns(99);

            learningDeliveryFAM.SetupGet(fam => fam.LearnDelFAMType).Returns("ADL");
            learningDeliveryFAM.SetupGet(fam => fam.LearnDelFAMCode).Returns("1");

            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>()
            {
                learningDeliveryFAM.Object
            };

            var albLearningDelivery = new LearningDelivery()
            {
                LearningDeliveryValue = new LearningDeliveryValue()
                {
                    AreaCostFactAdj = 10
                }
            };

            learningDeliveryMock.SetupGet(ld => ld.LearningDeliveryFAMs).Returns(learningDeliveryFAMs);

            NewBuilder().Filter(learningDeliveryMock.Object, albLearningDelivery).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryFilter_TrueALBFam()
        {
            var learningDeliveryMock = new Mock<ILearningDelivery>();
            var learningDeliveryFAM = new Mock<ILearningDeliveryFAM>();
            var learningDeliveryFAMALB = new Mock<ILearningDeliveryFAM>();

            learningDeliveryMock.SetupGet(ld => ld.FundModel).Returns(99);

            learningDeliveryFAM.SetupGet(fam => fam.LearnDelFAMType).Returns("ADL");
            learningDeliveryFAM.SetupGet(fam => fam.LearnDelFAMCode).Returns("1");

            learningDeliveryFAMALB.SetupGet(fam => fam.LearnDelFAMType).Returns("ALB");

            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>()
            {
                learningDeliveryFAM.Object,
                learningDeliveryFAMALB.Object,
            };

            var albLearningDelivery = new LearningDelivery()
            {
                LearningDeliveryValue = new LearningDeliveryValue()
            };

            learningDeliveryMock.SetupGet(ld => ld.LearningDeliveryFAMs).Returns(learningDeliveryFAMs);

            NewBuilder().Filter(learningDeliveryMock.Object, albLearningDelivery).Should().BeTrue();
        }

        [Fact]
        public void OrderBy()
        {
            var six = new AllbOccupancyReportModel() { Learner = new TestLearner() { LearnRefNumber = "D" }, LearningDelivery = new TestLearningDelivery() { AimSeqNumber = 1 } };
            var five = new AllbOccupancyReportModel() { Learner = new TestLearner() { LearnRefNumber = "C" }, LearningDelivery = new TestLearningDelivery() { AimSeqNumber = 1 } };
            var two = new AllbOccupancyReportModel() { Learner = new TestLearner() { LearnRefNumber = "A" }, LearningDelivery = new TestLearningDelivery() { AimSeqNumber = 2 } };
            var four = new AllbOccupancyReportModel() { Learner = new TestLearner() { LearnRefNumber = "B" }, LearningDelivery = new TestLearningDelivery() { AimSeqNumber = 1 } };
            var three = new AllbOccupancyReportModel() { Learner = new TestLearner() { LearnRefNumber = "A" }, LearningDelivery = new TestLearningDelivery() { AimSeqNumber = 3 } };
            var one = new AllbOccupancyReportModel() { Learner = new TestLearner() { LearnRefNumber = "A" }, LearningDelivery = new TestLearningDelivery() { AimSeqNumber = 1 } };

            var rows = new List<AllbOccupancyReportModel>()
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

            var adlLearningDeliveryFam = new Mock<ILearningDeliveryFAM>();

            adlLearningDeliveryFam.SetupGet(fam => fam.LearnDelFAMType).Returns("ADL");
            adlLearningDeliveryFam.SetupGet(fam => fam.LearnDelFAMCode).Returns("1");

            var albLearningDeliveryFam = new Mock<ILearningDeliveryFAM>();

            albLearningDeliveryFam.SetupGet(fam => fam.LearnDelFAMType).Returns("ALB");


            var learningDeliveryFams = new List<ILearningDeliveryFAM>()
            {
                albLearningDeliveryFam.Object,
                adlLearningDeliveryFam.Object,
            };

            var providerSpecDeliveryMonitorings = new List<IProviderSpecDeliveryMonitoring>()
            {
                new TestProviderSpecDeliveryMonitoring(),
            };

            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = 99,
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

            var albLearningDeliveryValue = new LearningDeliveryValue();

            var albLearningDelivery = new LearningDelivery()
            {
                AimSeqNumber = 1,
                LearningDeliveryValue = albLearningDeliveryValue,
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

            var fm35Global = new ALBGlobal()
            {
                Learners = new List<ALBLearner>()
                {
                    new ALBLearner()
                    {
                        LearnRefNumber = "LearnRefNumber",
                        LearningDeliveries = new List<LearningDelivery>()
                        {
                            albLearningDelivery,
                        }
                    }
                }
            };

            dependentDataMock.Setup(d => d.Get<IMessage>()).Returns(message);
            dependentDataMock.Setup(d => d.Get<ReferenceDataRoot>()).Returns(referenceDataRoot);
            dependentDataMock.Setup(d => d.Get<ALBGlobal>()).Returns(fm35Global);

            var providerSpecDeliveryMonitoringModel = new ProviderSpecDeliveryMonitoringModel();
            var providerSpecLearnerMonitoringModel = new ProviderSpecLearnerMonitoringModel();
            var learningDeliveryFamsModel = new LearningDeliveryFAMsModel();

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
            row.Fm99LearningDelivery.Should().Be(albLearningDeliveryValue);

            row.ProviderSpecDeliveryMonitoring.Should().Be(providerSpecDeliveryMonitoringModel);
            row.ProviderSpecLearnerMonitoring.Should().Be(providerSpecLearnerMonitoringModel);
            row.PeriodisedValues.Should().NotBeNull();
            row.LearningDeliveryFAMs.Should().Be(learningDeliveryFamsModel);
        }

        //public bool Filter(ILearningDelivery learningDelivery, LearningDelivery albLearningDelivery)
        //{
        //    if (learningDelivery?.LearningDeliveryFAMs != null && albLearningDelivery?.LearningDeliveryValue != null)
        //    {
        //        return learningDelivery.FundModel == FundModelConstants.FM99
        //               && (learningDelivery.LearningDeliveryFAMs.Any(fam =>
        //                       fam.LearnDelFAMType.CaseInsensitiveEquals(LearningDeliveryFAMTypeConstants.ADL) &&
        //                       fam.LearnDelFAMCode == "1") == true)
        //               && (albLearningDelivery.LearningDeliveryValue.AreaCostFactAdj > 0
        //                   || learningDelivery.LearningDeliveryFAMs.Any(fam =>
        //                       fam.LearnDelFAMType.CaseInsensitiveEquals(LearningDeliveryFAMTypeConstants.ALB)) == true);
        //    }

        //    return false;
        //}

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
                                    FundModel = 99,
                                    LearnAimRef = "learnAimRef" + l,
                                    AimSeqNumber = 1,
                                    LearningDeliveryFAMs = new List<ILearningDeliveryFAM>()
                                    {
                                        new TestLearningDeliveryFAM()
                                        {
                                            LearnDelFAMType = "ADL",
                                            LearnDelFAMCode = "1",
                                        },
                                        new TestLearningDeliveryFAM()
                                        {
                                            LearnDelFAMType = "ALB",
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
            
            var albGlobal = new ALBGlobal()
            {
                Learners = Enumerable.Range(0, learnerCount)
                    .Select(
                        l =>
                        new ALBLearner()
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
            dependentDataMock.Setup(d => d.Get<ALBGlobal>()).Returns(albGlobal);

            var result = NewBuilder(ilrModelMapperMock.Object).Build(reportServiceContext, dependentDataMock.Object).ToList();

            result.Should().HaveCount(learnerCount);
        }

        private AllbOccupancyReportModelBuilder NewBuilder(IIlrModelMapper ilrModelMapper = null)
        {
            return new AllbOccupancyReportModelBuilder(ilrModelMapper);
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
