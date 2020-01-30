using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ReportService.Reports.Validation;
using ESFA.DC.ILR.ReportService.Reports.Validation.Schema;
using ESFA.DC.ILR.ValidationErrors.Interface.Models;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Builders
{
    public class ValidationSchemaErrorsReportBuilderTests
    {
        [Fact]
        public void TestValidationSchemaErrorReportBuilder()
        {
            var sut = new ValidationSchemaErrorsReportBuilder();
            var buildIlrValidationErrors = BuildIlrValidationErrors();
            
            var result = sut.Build(buildIlrValidationErrors);

            result.Should().NotBeNullOrEmpty();
            result.Count().Should().Be(1);
            result.First().AimSequenceNumber.Should().Be(1);
            result.First().FieldValues.Should().Be("Line=1|Property=23|Message=InvalidCharacter|");
            result.First().LearnerReferenceNumber.Should().Be("0AchDt03");
            result.First().RuleName.Should().Be("Schema");
            result.First().ErrorMessage.Should().Be("The XML is not well formed.");
            result.First().Severity.Should().Be("E");
        }

        private List<ValidationError> BuildIlrValidationErrors()
        {
            List<ValidationError> ilrValidationErrors = new List<ValidationError>()
            {
                new ValidationError()
                {
                    RuleName = "Schema",
                    Severity = "E",
                    LearnerReferenceNumber = "0AchDt03",
                    AimSequenceNumber = 1,
                    ValidationErrorParameters = new List<ValidationErrorParameter>()
                    {
                        new ValidationErrorParameter()
                        {
                            PropertyName = "Line",
                            Value = "1"
                        },
                        new ValidationErrorParameter()
                        {
                            PropertyName = "Property",
                            Value = "23"
                        },
                        new ValidationErrorParameter()
                        {
                            PropertyName = "Message",
                            Value = "InvalidCharacter"
                        }
                    }
                }
            };

            return ilrValidationErrors;
        }
    }
}
