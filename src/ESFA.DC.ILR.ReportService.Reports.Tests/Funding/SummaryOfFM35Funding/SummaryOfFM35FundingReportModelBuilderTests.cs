using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output;
using ESFA.DC.ILR.ReportService.Model.ILR;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Funding.SummaryOfFM35Funding;
using ESFA.DC.ILR.ReportService.Service.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding.SummaryOfFM35Funding
{
    public class SummaryOfFM35FundingReportModelBuilderTests
    {
        [Fact]
        public void GetFM35LearningDeliveryPeriodisedValues()
        {
            var learner1 = new FM35Learner
            {
                LearnRefNumber = "Learner1",
                LearningDeliveries = new List<LearningDelivery>
                {
                    new LearningDelivery
                    {
                        AimSeqNumber = 1,
                        LearningDeliveryValue = new LearningDeliveryValue
                        {
                            FundLine = FundLineConstants.Apprenticeship1618
                        },
                        LearningDeliveryPeriodisedValues = new List<LearningDeliveryPeriodisedValue>
                        {
                            BuildPeriodisedValuesForAttribute(AttributeConstants.Fm35BalancePayment),
                            BuildPeriodisedValuesForAttribute(AttributeConstants.Fm35LearnSuppFundCash),
                        }
                    },
                    new LearningDelivery
                    {
                        AimSeqNumber = 2,
                        LearningDeliveryValue = new LearningDeliveryValue
                        {
                            FundLine = FundLineConstants.Apprenticeship1923
                        },
                        LearningDeliveryPeriodisedValues = new List<LearningDeliveryPeriodisedValue>
                        {
                            BuildPeriodisedValuesForAttribute(AttributeConstants.Fm35EmpOutcomePay),
                            BuildPeriodisedValuesForAttribute(AttributeConstants.Fm35OnProgPayment),
                        }
                    },
                }
            };

            var learner2 = new FM35Learner
            {
                LearnRefNumber = "Learner2",
                LearningDeliveries = new List<LearningDelivery>
                {
                    new LearningDelivery
                    {
                        AimSeqNumber = 1,
                        LearningDeliveryValue = new LearningDeliveryValue
                        {
                            FundLine = FundLineConstants.Apprenticeship1618
                        },
                        LearningDeliveryPeriodisedValues = new List<LearningDeliveryPeriodisedValue>
                        {
                            BuildPeriodisedValuesForAttribute(AttributeConstants.Fm35BalancePayment),
                            BuildPeriodisedValuesForAttribute(AttributeConstants.Fm35LearnSuppFundCash),
                        }
                    },
                    new LearningDelivery
                    {
                        AimSeqNumber = 2,
                        LearningDeliveryValue = new LearningDeliveryValue
                        {
                            FundLine = FundLineConstants.Apprenticeship1923
                        },
                        LearningDeliveryPeriodisedValues = new List<LearningDeliveryPeriodisedValue>
                        {
                            BuildPeriodisedValuesForAttribute(AttributeConstants.Fm35EmpOutcomePay),
                            BuildPeriodisedValuesForAttribute(AttributeConstants.Fm35OnProgPayment),
                        }
                    },
                }
            };

            var global = new FM35Global
            {
                Learners = new List<FM35Learner>
                {
                    learner1,
                    learner2
                }
            };

            var expectedModel = new List<FM35LearningDeliveryValues>
            {
                BuildFM35PeriodisedValuesForAttribute(1, "Learner1", 1, FundLineConstants.Apprenticeship1618, AttributeConstants.Fm35BalancePayment),
                BuildFM35PeriodisedValuesForAttribute(1, "Learner1", 1, FundLineConstants.Apprenticeship1618, AttributeConstants.Fm35LearnSuppFundCash),
                BuildFM35PeriodisedValuesForAttribute(1, "Learner1", 2, FundLineConstants.Apprenticeship1923, AttributeConstants.Fm35EmpOutcomePay),
                BuildFM35PeriodisedValuesForAttribute(1, "Learner1", 2, FundLineConstants.Apprenticeship1923, AttributeConstants.Fm35OnProgPayment),
                BuildFM35PeriodisedValuesForAttribute(1, "Learner2", 1, FundLineConstants.Apprenticeship1618, AttributeConstants.Fm35BalancePayment),
                BuildFM35PeriodisedValuesForAttribute(1, "Learner2", 1, FundLineConstants.Apprenticeship1618, AttributeConstants.Fm35LearnSuppFundCash),
                BuildFM35PeriodisedValuesForAttribute(1, "Learner2", 2, FundLineConstants.Apprenticeship1923, AttributeConstants.Fm35EmpOutcomePay),
                BuildFM35PeriodisedValuesForAttribute(1, "Learner2", 2, FundLineConstants.Apprenticeship1923, AttributeConstants.Fm35OnProgPayment),
            };

             NewBuilder().GetFM35LearningDeliveryPeriodisedValues(global, 1).Should().BeEquivalentTo(expectedModel);
        }

        [Fact]
        public void BuildFm35LearningDeliveryDictionary()
        {
            var learningDeliveries = new List<FM35LearningDeliveryValues>
            {
                BuildFM35PeriodisedValuesForAttribute(1, "Learner1", 1, FundLineConstants.Apprenticeship1618, AttributeConstants.Fm35BalancePayment),
                BuildFM35PeriodisedValuesForAttribute(1, "Learner1", 1, FundLineConstants.Apprenticeship1618, AttributeConstants.Fm35LearnSuppFundCash),
                BuildFM35PeriodisedValuesForAttribute(1, "Learner1", 2, FundLineConstants.Apprenticeship1923, AttributeConstants.Fm35EmpOutcomePay),
                BuildFM35PeriodisedValuesForAttribute(1, "Learner1", 2, FundLineConstants.Apprenticeship1923, AttributeConstants.Fm35OnProgPayment),
                BuildFM35PeriodisedValuesForAttribute(1, "Learner2", 1, FundLineConstants.Apprenticeship1618, AttributeConstants.Fm35BalancePayment),
                BuildFM35PeriodisedValuesForAttribute(1, "Learner2", 1, FundLineConstants.Apprenticeship1618, AttributeConstants.Fm35LearnSuppFundCash),
                BuildFM35PeriodisedValuesForAttribute(1, "Learner2", 2, FundLineConstants.Apprenticeship1923, AttributeConstants.Fm35EmpOutcomePay),
                BuildFM35PeriodisedValuesForAttribute(1, "Learner2", 2, FundLineConstants.Apprenticeship1923, AttributeConstants.Fm35OnProgPayment),
            };

            var pvArray = new decimal?[]
            {
                1.111m,
                2.222m,
                3.333m,
                4.444m,
                5.555m,
                6.666m,
                7.777m,
                8.888m,
                9.999m,
                10.1010m,
                11.1111m,
                12.1212m
            };

            var expectedDictionary = new Dictionary<string, Dictionary<string, decimal?[][]>>
            {
                {
                    FundLineConstants.Apprenticeship1618,  new Dictionary<string, decimal?[][]>
                    {
                        {
                            AttributeConstants.Fm35BalancePayment, new decimal?[][]
                            {
                                pvArray,
                                pvArray
                            }
                        },
                        {
                            AttributeConstants.Fm35LearnSuppFundCash, new decimal?[][]
                            {
                                pvArray,
                                pvArray
                            }
                        }
                    }
                },
                {
                    FundLineConstants.Apprenticeship1923,  new Dictionary<string, decimal?[][]>
                    {
                        {
                            AttributeConstants.Fm35EmpOutcomePay, new decimal?[][]
                            {
                                pvArray,
                                pvArray
                            }
                        },
                        {
                            AttributeConstants.Fm35OnProgPayment, new decimal?[][]
                            {
                                pvArray,
                                pvArray
                            }
                        }
                    }
                }
            };

            NewBuilder().BuildFm35LearningDeliveryDictionary(learningDeliveries).Should().BeEquivalentTo(expectedDictionary);
        }

        [Fact]
        public void Build()
        {
            var learningDeliveryPeriodisedValues = new List<LearningDeliveryPeriodisedValue>
            {
                BuildPeriodisedValuesForAttribute(AttributeConstants.Fm35OnProgPayment),
                BuildPeriodisedValuesForAttribute(AttributeConstants.Fm35BalancePayment),
                BuildPeriodisedValuesForAttribute(AttributeConstants.Fm35EmpOutcomePay),
                BuildPeriodisedValuesForAttribute(AttributeConstants.Fm35AchievePayment),
                BuildPeriodisedValuesForAttribute(AttributeConstants.Fm35LearnSuppFundCash),
            };

            var learner1 = new FM35Learner
            {
                LearnRefNumber = "Learner1",
                LearningDeliveries = new List<LearningDelivery>
                {
                    new LearningDelivery
                    {
                        AimSeqNumber = 1,
                        LearningDeliveryValue = new LearningDeliveryValue
                        {
                            FundLine = FundLineConstants.Apprenticeship1618
                        },
                        LearningDeliveryPeriodisedValues = learningDeliveryPeriodisedValues
                    },
                    new LearningDelivery
                    {
                        AimSeqNumber = 2,
                        LearningDeliveryValue = new LearningDeliveryValue
                        {
                            FundLine = FundLineConstants.Apprenticeship1923
                        },
                        LearningDeliveryPeriodisedValues = learningDeliveryPeriodisedValues
                    },
                }
            };

            var learner2 = new FM35Learner
            {
                LearnRefNumber = "Learner2",
                LearningDeliveries = new List<LearningDelivery>
                {
                    new LearningDelivery
                    {
                        AimSeqNumber = 1,
                        LearningDeliveryValue = new LearningDeliveryValue
                        {
                            FundLine = FundLineConstants.Apprenticeship1618
                        },
                        LearningDeliveryPeriodisedValues = learningDeliveryPeriodisedValues
                    },
                    new LearningDelivery
                    {
                        AimSeqNumber = 2,
                        LearningDeliveryValue = new LearningDeliveryValue
                        {
                            FundLine = FundLineConstants.Apprenticeship1923
                        },
                        LearningDeliveryPeriodisedValues = learningDeliveryPeriodisedValues
                    },
                }
            };

            var global = new FM35Global
            {
                Learners = new List<FM35Learner>
                {
                    learner1,
                    learner2
                }
            };

            var context = new Mock<IReportServiceContext>();
            var reportServiceDependentData = new Mock<IReportServiceDependentData>();

            context.Setup(c => c.Ukprn).Returns(1);

            reportServiceDependentData.Setup(r => r.Get<FM35Global>()).Returns(global);

            var result = NewBuilder().Build(context.Object, reportServiceDependentData.Object);

            result.Should().HaveCount(84);
            result.Select(f => f.FundingLineType).Distinct().Should().HaveCount(7);
            result.Where(f => f.FundingLineType == FundLineConstants.Apprenticeship1618 && f.Period == 1).FirstOrDefault().Total.Should().Be(11.11m);
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

        private FM35LearningDeliveryValues BuildFM35PeriodisedValuesForAttribute(int ukprn, string learnRefNumber, int aimSeqNumber, string fundline, string attribute)
        {
            return new FM35LearningDeliveryValues()
            {
                UKPRN = ukprn,
                LearnRefNumber = learnRefNumber,
                AimSeqNumber = aimSeqNumber,
                FundLine = fundline,
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

        private SummaryOfFM35FundingReportModelBuilder NewBuilder()
        {
            return new SummaryOfFM35FundingReportModelBuilder();
        } 
    }
}
