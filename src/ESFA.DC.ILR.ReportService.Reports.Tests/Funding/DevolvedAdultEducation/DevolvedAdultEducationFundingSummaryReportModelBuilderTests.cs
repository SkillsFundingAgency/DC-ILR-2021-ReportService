using System.Collections.Generic;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.ReportService.Models.ReferenceData;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.EAS;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Devolved;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding.DevolvedAdultEducation
{
    public class DevolvedAdultEducationFundingSummaryReportModelBuilderTests
    {
        [Fact]
        public void BuildEASDictionary()
        {
            var expectedOutput = new Dictionary<string, Dictionary<string, decimal?[][]>>
            {
                {
                    "FundLine1", new Dictionary<string, decimal?[][]>
                    {
                        {
                            "AdjustmentTypeName1", new decimal?[][]
                            {
                                new decimal?[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
                            }
                        },
                        {
                            "AdjustmentTypeName2", new decimal?[][]
                            {
                               new decimal?[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
                            }
                        },
                        {
                            "AdjustmentTypeName3", new decimal?[][]
                            {
                                new decimal?[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
                            }
                        }
                    }
                }
            };

            var referenceDataRoot = new ReferenceDataRoot
            {
                EasFundingLines = new List<EasFundingLine>
                {
                    new EasFundingLine
                    {
                        FundLine = "FundLine1",
                        EasSubmissionValues = new List<EasSubmissionValue>
                        {
                            new EasSubmissionValue
                            {
                                AdjustmentTypeName = "AdjustmentTypeName1",
                                PaymentName = "PaymentName1",
                                Period1 = new List<EasPaymentValue>()
                            },
                            new EasSubmissionValue
                            {
                                AdjustmentTypeName = "AdjustmentTypeName2",
                                PaymentName = "PaymentName2",
                                Period1 = new List<EasPaymentValue>
                                {
                                    new EasPaymentValue(1m, 101)
                                }
                            },
                            new EasSubmissionValue
                            {
                                AdjustmentTypeName = "AdjustmentTypeName3",
                                PaymentName = "PaymentName3",
                               Period1 = new List<EasPaymentValue>
                                {
                                    new EasPaymentValue(1m, 101),
                                    new EasPaymentValue(1m, 105),
                                }
                            }
                        }
                    }
                }
            };

            NewBuilder().BuildEASDictionary(referenceDataRoot, "105").Should().BeEquivalentTo(expectedOutput);
        }

        public DevolvedAdultEducationFundingSummaryReportModelBuilder NewBuilder(IDateTimeProvider dateTimeProvider = null)
        {
            return new DevolvedAdultEducationFundingSummaryReportModelBuilder(dateTimeProvider);
        }
    }
}
