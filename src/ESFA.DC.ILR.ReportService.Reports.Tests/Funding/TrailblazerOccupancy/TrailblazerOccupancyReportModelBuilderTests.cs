using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.FundingService.FM81.FundingOutput.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReferenceDataService.Model.LARS;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Trailblazer;
using ESFA.DC.ILR.ReportService.Reports.Model;
using ESFA.DC.ILR.ReportService.Reports.Model.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.Tests.Model;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding.TrailblazerOccupancy
{
    public class TrailblazerOccupancyReportModelBuilderTests
    {
        [Fact]
        public void LearningDeliveryFilter_False_NullLearningDelivery()
        {
            ILearningDelivery learningDelivery = null;

            NewBuilder().Filter(learningDelivery).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFilter_False_Empty()
        {
            var learningDeliveryMock = new Mock<ILearningDelivery>();

            learningDeliveryMock.SetupGet(ld => ld.LearningDeliveryFAMs).Returns(new List<ILearningDeliveryFAM>());

            NewBuilder().Filter(learningDeliveryMock.Object).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFilter_False_FundModel()
        {
            var learningDeliveryMock = new Mock<ILearningDelivery>();

            learningDeliveryMock.SetupGet(ld => ld.FundModel).Returns(35);
            learningDeliveryMock.SetupGet(ld => ld.ProgTypeNullable).Returns(25);

            NewBuilder().Filter(learningDeliveryMock.Object).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFilter_False_ProgType()
        {
            var learningDeliveryMock = new Mock<ILearningDelivery>();

            learningDeliveryMock.SetupGet(ld => ld.FundModel).Returns(81);
            learningDeliveryMock.SetupGet(ld => ld.ProgTypeNullable).Returns(1);

            NewBuilder().Filter(learningDeliveryMock.Object).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFilter_True()
        {
            var learningDeliveryMock = new Mock<ILearningDelivery>();

            learningDeliveryMock.SetupGet(ld => ld.FundModel).Returns(81);
            learningDeliveryMock.SetupGet(ld => ld.ProgTypeNullable).Returns(25);

            NewBuilder().Filter(learningDeliveryMock.Object).Should().BeTrue();
        }

        [Fact]
        public void OrderBy()
        {
            var six = new TrailblazerOccupancyReportModel() { Learner = new TestLearner() { LearnRefNumber = "D" }, LearningDelivery = new TestLearningDelivery() { AimSeqNumber = 1 } };
            var five = new TrailblazerOccupancyReportModel() { Learner = new TestLearner() { LearnRefNumber = "C" }, LearningDelivery = new TestLearningDelivery() { AimSeqNumber = 1 } };
            var two = new TrailblazerOccupancyReportModel() { Learner = new TestLearner() { LearnRefNumber = "A" }, LearningDelivery = new TestLearningDelivery() { AimSeqNumber = 2 } };
            var four = new TrailblazerOccupancyReportModel() { Learner = new TestLearner() { LearnRefNumber = "B" }, LearningDelivery = new TestLearningDelivery() { AimSeqNumber = 1 } };
            var three = new TrailblazerOccupancyReportModel() { Learner = new TestLearner() { LearnRefNumber = "A" }, LearningDelivery = new TestLearningDelivery() { AimSeqNumber = 3 } };
            var one = new TrailblazerOccupancyReportModel() { Learner = new TestLearner() { LearnRefNumber = "A" }, LearningDelivery = new TestLearningDelivery() { AimSeqNumber = 1 } };

            var rows = new List<TrailblazerOccupancyReportModel>()
            {
                six, five, two, four, three, one
            };

            var result = NewBuilder().Order(rows).ToList();

            result.Should().ContainInOrder(one, two, three, four, five, six);
        }

        [Fact]
        public void BuildAppFinRecordModel()
        {
            var appFinRecords = new List<IAppFinRecord>()
            {
                new TestAppFinRecord()
                {
                    AFinType = "PMR",
                    AFinCode = 1,
                    AFinDate = new DateTime(2018, 09, 01),
                    AFinAmount = 50
                },
                new TestAppFinRecord()
                {
                    AFinType = "PMR",
                    AFinCode = 2,
                    AFinDate = new DateTime(2018, 10, 01),
                    AFinAmount = 100
                },
                new TestAppFinRecord()
                {
                    AFinType = "PMR",
                    AFinCode = 3,
                    AFinDate = new DateTime(2018, 11, 01),
                    AFinAmount = 100
                },
                new TestAppFinRecord()
                {
                    AFinType = "PMR",
                    AFinCode = 1,
                    AFinDate = new DateTime(2019, 08, 01),
                    AFinAmount = 50
                },
                new TestAppFinRecord()
                {
                    AFinType = "PMR",
                    AFinCode = 2,
                    AFinDate = new DateTime(2019, 08, 01),
                    AFinAmount = 100
                },
                new TestAppFinRecord()
                {
                    AFinType = "PMR",
                    AFinCode = 3,
                    AFinDate = new DateTime(2019, 08, 01),
                    AFinAmount = 50
                },
                new TestAppFinRecord()
                {
                    AFinType = "PMR",
                    AFinCode = 1,
                    AFinDate = new DateTime(2020, 07, 01),
                    AFinAmount = 50
                },
                new TestAppFinRecord()
                {
                    AFinType = "PMR",
                    AFinCode = 2,
                    AFinDate = new DateTime(2020, 07, 01),
                    AFinAmount = 100
                },
                new TestAppFinRecord()
                {
                    AFinType = "PMR",
                    AFinCode = 3,
                    AFinDate = new DateTime(2020, 07, 01),
                    AFinAmount = 50
                },
                new TestAppFinRecord()
                {
                    AFinType = "TNP",
                    AFinCode = 1,
                    AFinDate = new DateTime(2019, 10, 01),
                    AFinAmount = 200
                },
                new TestAppFinRecord()
                {
                    AFinType = "TNP",
                    AFinCode = 1,
                    AFinDate = new DateTime(2019, 11, 01),
                    AFinAmount = 300
                },
                new TestAppFinRecord()
                {
                    AFinType = "TNP",
                    AFinCode = 2,
                    AFinDate = new DateTime(2019, 11, 01),
                    AFinAmount = 400
                },
                new TestAppFinRecord()
                {
                    AFinType = "TNP",
                    AFinCode = 2,
                    AFinDate = new DateTime(2019, 12, 01),
                    AFinAmount = 500
                },
            };

            var learningDelivery = new TestLearningDelivery()
            {
                AppFinRecords = appFinRecords,
                AimType = 1
            };

            var result = NewBuilder().BuildAppFinRecordModel(learningDelivery);

            result.LatestTotalNegotiatedPrice1.Should().Be(300);
            result.LatestTotalNegotiatedPrice2.Should().Be(500);
            result.SumOfPmrsBeforeFundingYear.Should().Be(50);
            result.SumOfAugustPmrs.Should().Be(100);
            result.SumOfJulyPmrs.Should().Be(100);
            result.PmrsTotal.Should().Be(200);
        }

        [Fact]
        public void Build_SingleRow()
        {
            var reportServiceContext = Mock.Of<IReportServiceContext>();
            var dependentDataMock = new Mock<IReportServiceDependentData>();

            var learningDeliveryFams = new List<ILearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
            };

            var providerSpecDeliveryMonitorings = new List<IProviderSpecDeliveryMonitoring>()
            {
                new TestProviderSpecDeliveryMonitoring(),
            };

            var appFinRecords = new List<IAppFinRecord>()
            {
                new TestAppFinRecord(),
            };

            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = 81,
                LearnAimRef = "learnAimRef",
                AimSeqNumber = 1,
                AimType = 1,
                ProgTypeNullable = 25,
                LearningDeliveryFAMs = learningDeliveryFams,
                ProviderSpecDeliveryMonitorings = providerSpecDeliveryMonitorings,
                AppFinRecords = appFinRecords
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

            var fm81LearningDeliveryValue = new LearningDeliveryValue();

            var fm81LearningDelivery = new LearningDelivery()
            {
                AimSeqNumber = 1,
                LearningDeliveryValues = fm81LearningDeliveryValue,
                LearningDeliveryPeriodisedValues = new List<LearningDeliveryPeriodisedValue>()
                {
                    BuildPeriodisedValuesForAttribute(AttributeConstants.Fm81CoreGovContPayment),
                    BuildPeriodisedValuesForAttribute(AttributeConstants.Fm81MathEngOnProgPayment),
                    BuildPeriodisedValuesForAttribute(AttributeConstants.Fm81MathEngBalPayment),
                    BuildPeriodisedValuesForAttribute(AttributeConstants.Fm81LearnSuppFundCash),
                    BuildPeriodisedValuesForAttribute(AttributeConstants.Fm81SmallBusPayment),
                    BuildPeriodisedValuesForAttribute(AttributeConstants.Fm81YoungAppPayment),
                    BuildPeriodisedValuesForAttribute(AttributeConstants.Fm81AchPayment),
                }
            };

            var fm81Global = new FM81Global()
            {
                Learners = new List<FM81Learner>()
                {
                    new FM81Learner()
                    {
                        LearnRefNumber = "LearnRefNumber",
                        LearningDeliveries = new List<LearningDelivery>()
                        {
                            fm81LearningDelivery,
                        }
                    }
                }
            };

            dependentDataMock.Setup(d => d.Get<IMessage>()).Returns(message);
            dependentDataMock.Setup(d => d.Get<ReferenceDataRoot>()).Returns(referenceDataRoot);
            dependentDataMock.Setup(d => d.Get<FM81Global>()).Returns(fm81Global);

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
            row.Fm81LearningDelivery.Should().Be(fm81LearningDeliveryValue);

            row.ProviderSpecDeliveryMonitoring.Should().Be(providerSpecDeliveryMonitoringModel);
            row.ProviderSpecLearnerMonitoring.Should().Be(providerSpecLearnerMonitoringModel);
            row.PeriodisedValues.Should().NotBeNull();
            row.LearningDeliveryFAMs.Should().Be(learningDeliveryFamsModel);
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
                                    FundModel = 81,
                                    LearnAimRef = "learnAimRef" + l,
                                    AimSeqNumber = 1,
                                    ProgTypeNullable = 25,
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

            var albGlobal = new FM81Global()
            {
                Learners = Enumerable.Range(0, learnerCount)
                    .Select(
                        l =>
                        new FM81Learner()
                        {
                            LearnRefNumber = "LearnRefNumber" + l,
                            LearningDeliveries = new List<LearningDelivery>()
                            {
                                new LearningDelivery()
                                {
                                    AimSeqNumber = 1,
                                    LearningDeliveryValues = new LearningDeliveryValue(),
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
            dependentDataMock.Setup(d => d.Get<FM81Global>()).Returns(albGlobal);

            var result = NewBuilder(ilrModelMapperMock.Object).Build(reportServiceContext, dependentDataMock.Object).ToList();

            result.Should().HaveCount(learnerCount);
        }

        private TrailblazerOccupancyReportModelBuilder NewBuilder(IIlrModelMapper ilrModelMapper = null)
        {
            return new TrailblazerOccupancyReportModelBuilder(ilrModelMapper);
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
                Period12 = 12.1212m
            };
        }
    }
}
