using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Reports.Funding.Trailblazer.EmployerIncentive;
using ESFA.DC.ILR.ReportService.Reports.Funding.Trailblazer.EmployerIncentive.Model;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding.MainOccupancy
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

        private TrailblazerEmployerIncentiveReportModelBuilder NewBuilder()
        {
            return new TrailblazerEmployerIncentiveReportModelBuilder();
        }
    }
}
