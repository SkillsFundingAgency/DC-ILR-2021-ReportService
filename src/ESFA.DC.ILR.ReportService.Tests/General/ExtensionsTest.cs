using System.Collections.Generic;
using ESFA.DC.ILR.ValidationErrors.Interface.Models;
using ESFA.DC.ILR1819.ReportService.Service.Helper;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR1819.ReportService.Tests.General
{
    public sealed class ExtensionsTest
    {
        [Fact]
        public void TestDistinctByCount()
        {
            List<ValidationErrorDto> tokens = new List<ValidationErrorDto>()
            {
                new ValidationErrorDto() { LearnerReferenceNumber = "A" },
                new ValidationErrorDto() { LearnerReferenceNumber = "B" },
                new ValidationErrorDto() { LearnerReferenceNumber = string.Empty },
                new ValidationErrorDto() { LearnerReferenceNumber = "C" },
                new ValidationErrorDto() { LearnerReferenceNumber = "A" },
                new ValidationErrorDto() { LearnerReferenceNumber = "D" }
            };

            int count = tokens.DistinctByCount(x => x.LearnerReferenceNumber);

            count.Should().Be(4);
        }
    }
}
