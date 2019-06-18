using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Desktop.Interface;
using ESFA.DC.ILR.ReportService.Desktop.Context;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Desktop.Tests
{
    public class ReportServiceJobContextDesktopContextTest
    {
        [Fact]
        public void Populates_CorrectValues_From_DeskTopContext()
        {
            Mock<IDesktopContext> mockDesktopContext = new Mock<IDesktopContext>();

            var keyValuePairs = new Dictionary<string, object>
            {
                { "Filename", "someilr.xml" },
                { "OriginalFilename", "mtheoriginal.xml"},
                { "UkPrn", 12345678},
                { "FileSizeInBytes", 128},
                { "Container", "ilr-files"},
                { "ValidationErrors", "ValidationErrors"},
                { "ValidLearnRefNumbersCount", 1},
                { "InvalidLearnRefNumbersCount", 2},
                { "ValidationTotalErrorCount", 3},
                { "ValidationTotalWarningCount", 4},
                { "ValidationErrorLookups", "ValidationErrorLookups"},
                { "FundingFm81Output", "FundingFm81Output"},
                { "FundingFm70Output", "FundingFm70Output"},
                { "FundingFm36Output", "FundingFm36Output"},
                { "FundingFm35Output", "FundingFm35Output"},
                { "FundingFm25Output", "FundingFm25Output"},
                { "FundingAlbOutput", "FundingAlbOutput"},
                { "ValidLearnRefNumbers", "ValidLearnRefNumbers"},
                { "ReturnPeriod", 8},
                { "Tasks", "TaskGenerateValidationReport|TaskGenerateFundingSummaryReport|TaskGenerateAdultFundingClaimReport"}
            };
            mockDesktopContext.Setup(x => x.DateTimeUtc).Returns(new DateTime(2019, 10, 10));
            mockDesktopContext.SetupGet(x => x.KeyValuePairs).Returns(keyValuePairs);

            var context = NewContext(mockDesktopContext.Object);

            context.Filename.Should().Be("someilr.xml");
            context.OriginalFilename.Should().Be("mtheoriginal.xml");
            context.Ukprn.Should().Be(12345678);
            context.FileSizeInBytes.Should().Be(128);
            context.Container.Should().Be("ilr-files");
            context.ValidationErrorsKey.Should().Be("ValidationErrors");
            context.ValidLearnRefNumbersCount.Should().Be(1);
            context.InvalidLearnRefNumbersCount.Should().Be(2);
            context.ValidationTotalErrorCount.Should().Be(3);
            context.ValidationTotalWarningCount.Should().Be(4);
            context.ValidationErrorsLookupsKey.Should().Be("ValidationErrorLookups");
            context.FundingFM81OutputKey.Should().Be("FundingFm81Output");
            context.FundingFM70OutputKey.Should().Be("FundingFm70Output");
            context.FundingFM36OutputKey.Should().Be("FundingFm36Output");
            context.FundingFM36OutputKey.Should().Be("FundingFm36Output");
            context.FundingFM35OutputKey.Should().Be("FundingFm35Output");
            context.FundingFM25OutputKey.Should().Be("FundingFm25Output");
            context.FundingALBOutputKey.Should().Be("FundingAlbOutput");
            context.ValidLearnRefNumbersKey.Should().Be("ValidLearnRefNumbers");
            context.ReturnPeriod.Should().Be(8);
            context.SubmissionDateTimeUtc.Should().Be(new DateTime(2019, 10, 10));
            context.Tasks.Count().Should().Be(3);
            context.Tasks.ElementAt(0).Should().Be("TaskGenerateValidationReport");
            context.Tasks.ElementAt(1).Should().Be("TaskGenerateFundingSummaryReport");
            context.Tasks.ElementAt(2).Should().Be("TaskGenerateAdultFundingClaimReport");
        }

        private ReportServiceJobContextDesktopContext NewContext(IDesktopContext desktopContext)
        {
            return new ReportServiceJobContextDesktopContext(desktopContext);
        }
    }
}
