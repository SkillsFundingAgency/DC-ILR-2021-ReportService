using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output;
using ESFA.DC.ILR.ReportService.Reports.Funding;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding
{
    public class PeriodisedValuesLookupProviderTests
    {
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

        private PeriodisedValuesLookupProvider NewProvider()
        {
            return new PeriodisedValuesLookupProvider();
        }
    }
}
