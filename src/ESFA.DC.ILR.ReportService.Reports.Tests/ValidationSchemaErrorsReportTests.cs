using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Reports.Validation;
using ESFA.DC.ILR.ReportService.Reports.Validation.Interface;
using ESFA.DC.ILR.ReportService.Reports.Validation.Model;
using ESFA.DC.ILR.ReportService.Reports.Validation.Schema;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;
using ESFA.DC.ILR.ValidationErrors.Interface.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests
{
    public class ValidationSchemaErrorsReportTests
    {
        [Fact]
        public void DependsOn()
        {
            var dependsOn = NewReport().DependsOn.ToList();

            dependsOn.Should().HaveCount(1);
            dependsOn.Should().Contain(DependentDataCatalog.ValidationErrors);
        }

        [Fact]
        public async Task GenerateAsync()
        {
            var container = "container";

            var reportServiceContext = new Mock<IReportServiceContext>();
            reportServiceContext.SetupGet(c => c.Container).Returns(container);

            var cancellationToken = CancellationToken.None;
            
            var validationErrors = Mock.Of<List<ValidationError>>();

            var dependentData = new Mock<IReportServiceDependentData>();

            dependentData.Setup(d => d.Get<List<ValidationError>>()).Returns(validationErrors);

            var fileName = "FileName";

            var fileNameServiceMock = new Mock<IFileNameService>();

            fileNameServiceMock.Setup(s => s.GetFilename(reportServiceContext.Object, "Rule Violation Report", OutputTypes.Csv, true)).Returns(fileName);

            var validationErrorBuilder = new Mock<IValidationSchemaErrorsReportBuilder>();

            var validationErrorRows = Mock.Of<IEnumerable<ValidationErrorRow>>();

            validationErrorBuilder.Setup(b => b.Build(validationErrors)).Returns(validationErrorRows);

            var csvServiceMock = new Mock<ICsvService>();
            
            var frontEndValidationReportMock = new Mock<IFrontEndValidationReport>();
            
            var report = NewReport(validationErrorBuilder.Object, csvServiceMock.Object, fileNameServiceMock.Object, frontEndValidationReportMock.Object);

            var result = await report.GenerateAsync(reportServiceContext.Object, dependentData.Object, cancellationToken);

            result.Should().HaveCount(1);
            result.First().Should().Be(fileName);
            
            csvServiceMock.Verify(s => s.WriteAsync<ValidationErrorRow, ValidationErrorMapper>(validationErrorRows, fileName, container, cancellationToken));

            frontEndValidationReportMock.Verify(r => r.GenerateAsync(reportServiceContext.Object, validationErrorRows, cancellationToken));

        }

        [Fact]
        public void FileName()
        {
            NewReport().ReportName.Should().Be("Rule Violation Report");
        }

        [Fact]
        public void TaskName()
        {
            NewReport().TaskName.Should().Be("TaskGenerateValidationSchemaErrorsReport");
        }

        private ValidationSchemaErrorsReport NewReport(
            IValidationSchemaErrorsReportBuilder validationSchemaErrorsReportBuilder = null,
            ICsvService csvService = null,
            IFileNameService fileNameService = null,
            IFrontEndValidationReport frontEndValidationReport = null)
        {
            return new ValidationSchemaErrorsReport(validationSchemaErrorsReportBuilder, csvService, fileNameService, frontEndValidationReport);
        }
    }
}
