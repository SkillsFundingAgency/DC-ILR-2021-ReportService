﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.CsvService.Interface;
using ESFA.DC.ILR.Model.Loose.Interface;
using ESFA.DC.ILR.ReportService.Models.ReferenceData;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.MetaData;
using ESFA.DC.ILR.ReportService.Reports.Validation;
using ESFA.DC.ILR.ReportService.Reports.Validation.Detail;
using ESFA.DC.ILR.ReportService.Reports.Validation.Interface;
using ESFA.DC.ILR.ReportService.Reports.Validation.Model;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;
using FluentAssertions;
using Moq;
using Xunit;
using ValidationError = ESFA.DC.ILR.ValidationErrors.Interface.Models.ValidationError;

namespace ESFA.DC.ILR.ReportService.Reports.Tests
{
    public class ValidationErrorDetailReportTests
    {
        [Fact]
        public async Task GenerateAsync()
        {
            var container = "container";

            var reportServiceContext = new Mock<IReportServiceContext>();
            reportServiceContext.SetupGet(c => c.Container).Returns(container);
            
            var cancellationToken = CancellationToken.None;

            var message = Mock.Of<ILooseMessage>();

            var validationErrorsMetadata = new List<ESFA.DC.ILR.ReportService.Models.ReferenceData.MetaData.ValidationError>();

            var referenceDataRoot = new ReferenceDataRoot()
            {
                MetaDatas = new MetaData()
                {
                    ValidationErrors = validationErrorsMetadata
                }
            };

            var validationErrors = Mock.Of<List<ValidationError>>();
                
            var dependentData = new Mock<IReportServiceDependentData>();

            dependentData.Setup(d => d.Get<ILooseMessage>()).Returns(message);
            dependentData.Setup(d => d.Get<ReferenceDataRoot>()).Returns(referenceDataRoot);
            dependentData.Setup(d => d.Get<List<ValidationError>>()).Returns(validationErrors);

            var fileName = "FileName";

            var fileNameServiceMock = new Mock<IFileNameService>();

            fileNameServiceMock.Setup(s => s.GetFilename(reportServiceContext.Object, "Rule Violation Report", OutputTypes.Csv, true)).Returns(fileName);

            var validationErrorBuilder = new Mock<IValidationErrorsReportBuilder>();

            var validationErrorRows = Mock.Of<IEnumerable<ValidationErrorRow>>();

            validationErrorBuilder.Setup(b => b.Build(validationErrors, message, validationErrorsMetadata)).Returns(validationErrorRows);

            var csvServiceMock = new Mock<ICsvFileService>();

            var frontEndValidationReportMock = new Mock<IFrontEndValidationReport>();

            var report = NewReport(validationErrorBuilder.Object, csvServiceMock.Object, frontEndValidationReportMock.Object, fileNameServiceMock.Object);

            var result = await report.GenerateAsync(reportServiceContext.Object, dependentData.Object, cancellationToken);

            result.Should().HaveCount(1);
            result.First().Should().Be(fileName);

            frontEndValidationReportMock.Verify(r => r.GenerateAsync(reportServiceContext.Object, validationErrorRows, false, cancellationToken));
            csvServiceMock.Verify(s => s.WriteAsync<ValidationErrorRow, ValidationErrorMapper>(validationErrorRows, fileName, container, cancellationToken, null, null));
        }

        [Fact]
        public void FileName()
        {
            NewReport().ReportName.Should().Be("Rule Violation Report");
        }

        [Fact]
        public void TaskName()
        {
            NewReport().TaskName.Should().Be("TaskGenerateValidationReport");
        }

        private ValidationErrorsDetailReport NewReport(
            IValidationErrorsReportBuilder validationErrorsReportBuilder = null,
            ICsvFileService csvService = null,
            IFrontEndValidationReport frontEndValidationReport = null,
            IFileNameService fileNameService = null)
        {
            return new ValidationErrorsDetailReport(validationErrorsReportBuilder, csvService, frontEndValidationReport, fileNameService);
        }
    }
}
