using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReferenceDataService.Model.MetaData;
using ESFA.DC.ILR.ReportService.Reports.Interface;
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

            var message = Mock.Of<IMessage>();

            var validationErrorsMetadata = new List<ReferenceDataService.Model.MetaData.ValidationError>();

            var referenceDataRoot = new ReferenceDataRoot()
            {
                MetaDatas = new MetaData()
                {
                    ValidationErrors = validationErrorsMetadata
                }
            };

            var validationErrors = Mock.Of<List<ValidationError>>();
                
            var dependentData = new Mock<IReportServiceDependentData>();

            dependentData.Setup(d => d.Get<IMessage>()).Returns(message);
            dependentData.Setup(d => d.Get<ReferenceDataRoot>()).Returns(referenceDataRoot);
            dependentData.Setup(d => d.Get<List<ValidationError>>()).Returns(validationErrors);

            var fileName = "FileName";

            var fileNameServiceMock = new Mock<IFileNameService>();

            fileNameServiceMock.Setup(s => s.GetFilename(reportServiceContext.Object, "Rule Violation Report", OutputTypes.Csv)).Returns(fileName);

            var validationErrorBuilder = new Mock<IValidationErrorsReportBuilder>();

            var validationErrorRows = Mock.Of<IEnumerable<ValidationErrorRow>>();

            validationErrorBuilder.Setup(b => b.Build(validationErrors, message, validationErrorsMetadata)).Returns(validationErrorRows);

            var csvServiceMock = new Mock<ICsvService>();

            var frontEndValidationReportMock = new Mock<IFrontEndValidationReport>();

            var report = NewReport(validationErrorBuilder.Object, csvServiceMock.Object, frontEndValidationReportMock.Object, fileNameServiceMock.Object);

            var result = await report.GenerateReportAsync(reportServiceContext.Object, dependentData.Object, cancellationToken);

            result.Should().HaveCount(1);
            result.First().Should().Be(fileName);

            frontEndValidationReportMock.Verify(r => r.GenerateAsync(reportServiceContext.Object, validationErrorRows, cancellationToken));
            csvServiceMock.Verify(s => s.WriteAsync<ValidationErrorRow, ValidationErrorMapper>(validationErrorRows, fileName, container, cancellationToken));
        }

        private ValidationErrorsDetailReport NewReport(
            IValidationErrorsReportBuilder validationErrorsReportBuilder = null,
            ICsvService csvService = null,
            IFrontEndValidationReport frontEndValidationReport = null,
            IFileNameService fileNameService = null)
        {
            return new ValidationErrorsDetailReport(validationErrorsReportBuilder, csvService, frontEndValidationReport, fileNameService);
        }

        //public async Task<IEnumerable<string>> GenerateReportAsync(IReportServiceContext reportServiceContext, IReportServiceDependentData reportsDependentData, CancellationToken cancellationToken)
        //{
        //    IMessage ilrMessage = reportsDependentData.Get<IMessage>();
        //    ReferenceDataRoot ilrReferenceData = reportsDependentData.Get<ReferenceDataRoot>();
        //    List<ValidationError> ilrValidationErrors = reportsDependentData.Get<List<ValidationError>>();

        //    var fileName = _fileNameService.GetFilename(reportServiceContext, ReportFileName, OutputTypes.Csv);

        //    var validationErrorRows = _validationErrorsReportBuilder.Build(ilrValidationErrors, ilrMessage, ilrReferenceData.MetaDatas.ValidationErrors);

        //    await _csvService.WriteAsync<ValidationErrorRow, ValidationErrorMapper>(validationErrorRows, fileName, reportServiceContext.Container, cancellationToken);

        //    await _frontEndValidationReport.GenerateAsync(reportServiceContext, validationErrorRows, fileName, cancellationToken);

        //    return new[] { fileName };
        //}
    }
}
