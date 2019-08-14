using System.Collections.Generic;
using ESFA.DC.ILR.FundingService.FM81.FundingOutput.Model.Output;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Funding.Trailblazer.EmployerIncentive;
using ESFA.DC.ILR.ReportService.Reports.Funding.Trailblazer.EmployerIncentive.Model;
using ESFA.DC.ILR.ReportService.Service.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding.TrailblazerEmployerIncentives
{
    public class TrailblazerEmployerIncentivesReportModelBuilderTests
    {

        [Fact]
        public void CalculatePaymentValue_NoMatch_ReturnZero()
        {
            var periodisedValues = new List<TrailblazerLearningDeliveryPeriodisedValues>
            {
                new TrailblazerLearningDeliveryPeriodisedValues
                {
                    EmployerIds = new Dictionary<string, int?> {{"Attribute1", 123}},
                    AttributeName = "Attribute1",
                    ValuesDictionary = new Dictionary<string, decimal?> {{"Period1", 10.1m}, {"Period2", 22.1m}}
                }
            };

            var periodValue = NewBuilder().CalculatePaymentValue(periodisedValues, "Attribute1", "Period1", 111);

            periodValue.Should().Be(0.0m);
        }

        [Fact]
        public void CalculatePaymentValue_MatchEmpNoMatchAttribute_ReturnZero()
        {
            var periodisedValues = new List<TrailblazerLearningDeliveryPeriodisedValues>
            {
                new TrailblazerLearningDeliveryPeriodisedValues
                {
                    EmployerIds = new Dictionary<string, int?> {{"Attribute1", 111}},
                    AttributeName = "Attribute2",
                    ValuesDictionary = new Dictionary<string, decimal?> {{"Period1", 10.1m}, {"Period2", 22.1m}}
                }
            };

            var periodValue = NewBuilder().CalculatePaymentValue(periodisedValues, "Attribute1", "Period1", 111);

            periodValue.Should().Be(0.0m);
        }

        [Fact]
        public void CalculatePaymentValue_MatchAttributeAndEmployer_ReturnCorrectValue()
        {
            var periodisedValues = new List<TrailblazerLearningDeliveryPeriodisedValues>
            {
                new TrailblazerLearningDeliveryPeriodisedValues
                {
                    EmployerIds = new Dictionary<string, int?> {{"Attribute1", 111}},
                    AttributeName = "Attribute1",
                    ValuesDictionary = new Dictionary<string, decimal?> {{"Period1", 10.1m}, {"Period2", 22.1m}}
                }
            };

            var periodValue = NewBuilder().CalculatePaymentValue(periodisedValues, "Attribute1", "Period1", 111);

            periodValue.Should().Be(10.1m);
        }

        [Fact]
        public void CalculatePaymentValue_MultipleMatchAttributeAndEmployer_ReturnCorrectValue()
        {
            var periodisedValues = new List<TrailblazerLearningDeliveryPeriodisedValues>
            {
                new TrailblazerLearningDeliveryPeriodisedValues
                {
                    EmployerIds = new Dictionary<string, int?> {{"Attribute1", 111}},
                    AttributeName = "Attribute1",
                    ValuesDictionary = new Dictionary<string, decimal?> {{"Period1", 10.1m}, {"Period2", 22.1m}}
                },
                new TrailblazerLearningDeliveryPeriodisedValues
                {
                    EmployerIds = new Dictionary<string, int?> {{"Attribute1", 111}},
                    AttributeName = "Attribute1",
                    ValuesDictionary = new Dictionary<string, decimal?> {{"Period1", 20.1m}, {"Period2", 22.1m}}
                }
            };

            var periodValue = NewBuilder().CalculatePaymentValue(periodisedValues, "Attribute1", "Period1", 111);

            periodValue.Should().Be(30.2m);
        }

        [Fact]
        public void Build_ReturnCorrectOrder()
        {
            var reportServiceContextMock = new Mock<IReportServiceContext>();
            var reportServiceDependantDataMock = new Mock<IReportServiceDependentData>();

            reportServiceDependantDataMock.Setup(x => x.Get<FM81Global>()).Returns(BuildGlobal());

            var models = NewBuilder().Build(reportServiceContextMock.Object, reportServiceDependantDataMock.Object);

            models.Should().BeInAscendingOrder(x => x.EmployerIdentifier);
        }

        private TrailblazerEmployerIncentiveReportModelBuilder NewBuilder()
        {
            return new TrailblazerEmployerIncentiveReportModelBuilder();
        }

        private FM81Global BuildGlobal()
        {
            return new FM81Global
            {
                Learners = new List<FM81Learner>
                {
                    new FM81Learner
                    {
                        LearnRefNumber = "Learner1",
                        LearningDeliveries = new List<LearningDelivery>
                        {
                            new LearningDelivery
                            {
                                LearningDeliveryValues = new LearningDeliveryValue
                                {
                                    EmpIdAchDate = 2,
                                    EmpIdFirstYoungAppDate = 1,
                                    EmpIdSecondYoungAppDate = 3,
                                    EmpIdSmallBusDate = 8
                                },
                                LearningDeliveryPeriodisedValues = new List<LearningDeliveryPeriodisedValue>
                                {
                                    new LearningDeliveryPeriodisedValue
                                    {
                                        AttributeName = AttributeConstants.Fm81AchPayment
                                    },
                                    new LearningDeliveryPeriodisedValue
                                    {
                                        AttributeName = AttributeConstants.Fm81YoungAppFirstPayment,
                                    },
                                    new LearningDeliveryPeriodisedValue
                                    {
                                        AttributeName = AttributeConstants.Fm81YoungAppSecondPayment
                                    },
                                    new LearningDeliveryPeriodisedValue
                                    {
                                        AttributeName = AttributeConstants.Fm81SmallBusPayment
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}
