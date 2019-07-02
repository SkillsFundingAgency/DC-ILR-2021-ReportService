using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReportService.Reports.Extensions;
using ESFA.DC.ILR.ReportService.Reports.Interface;
using ESFA.DC.ILR.ReportService.Reports.Validation.Interface;
using ESFA.DC.ILR.ReportService.Reports.Validation.Model;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;
using ESFA.DC.ILR.ValidationErrors.Interface.Models;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ReportService.Reports.Validation.Detail
{
    public sealed class ValidationErrorsDetailReport : IReport
    {
        private readonly ILogger _logger;
        private readonly IValidationErrorsReportBuilder _validationErrorsReportBuilder;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ICsvService _csvService;
        private readonly IFrontEndValidationReport _frontEndValidationReport;
        
        public ValidationErrorsDetailReport(
            ILogger logger,
            IValidationErrorsReportBuilder validationErrorsReportBuilder,
            IDateTimeProvider dateTimeProvider,
            ICsvService csvService,
            IFrontEndValidationReport frontEndValidationReport)
        {
            _logger = logger;
            _validationErrorsReportBuilder = validationErrorsReportBuilder;
            _dateTimeProvider = dateTimeProvider;
            _csvService = csvService;
            _frontEndValidationReport = frontEndValidationReport;
        }

        public string ReportFileName => "Rule Violation Report";

        public string ReportTaskName => ReportTaskNameConstants.ValidationReport;

        public IEnumerable<Type> DependsOn => new List<Type>()
        {
            DependentDataCatalog.Ilr,
            DependentDataCatalog.ReferenceData,
            DependentDataCatalog.ValidationErrors,
        };

        public async Task<IEnumerable<string>> GenerateReportAsync(IReportServiceContext reportServiceContext, IReportServiceDependentData reportsDependentData, CancellationToken cancellationToken)
        {
            IMessage ilrMessage =  reportsDependentData.Get<IMessage>();
            ReferenceDataRoot ilrReferenceData = reportsDependentData.Get<ReferenceDataRoot>();
            List<ValidationError> ilrValidationErrors = reportsDependentData.Get<List<ValidationError>>();

            reportServiceContext.Ukprn = ilrMessage.HeaderEntity.SourceEntity.UKPRN;
            var externalFileName = GetFilename(reportServiceContext);

            var validationErrorModels = _validationErrorsReportBuilder.Build(ilrValidationErrors, ilrMessage, ilrReferenceData.MetaDatas.ValidationErrors);
            var reportOutputFilenames = await PersistValidationErrorsReport(validationErrorModels, reportServiceContext, externalFileName, cancellationToken);

            var validationErrorsDto = BuildValidationErrors(ilrValidationErrors, ilrReferenceData.MetaDatas.ValidationErrors);

            await _frontEndValidationReport.GenerateAsync(reportServiceContext, validationErrorsDto, externalFileName, cancellationToken);
            
            return reportOutputFilenames;
        }

        private async Task<IEnumerable<string>> PersistValidationErrorsReport(IEnumerable<ValidationErrorRow> validationErrors, IReportServiceContext reportServiceContext, string externalFileName, CancellationToken cancellationToken)
        {
            var fileName = $"{externalFileName}.csv";

            await _csvService.WriteAsync<ValidationErrorRow, ValidationErrorMapper>(validationErrors, fileName, reportServiceContext.Container, cancellationToken);

            return new[] {fileName};
        }

        private string GetFilename(IReportServiceContext reportServiceContext)
        {
            DateTime dateTime = _dateTimeProvider.ConvertUtcToUk(reportServiceContext.SubmissionDateTimeUtc);
            return $"{reportServiceContext.Ukprn}_{reportServiceContext.JobId}_{ReportFileName} {dateTime:yyyyMMdd-HHmmss}";
        }

        #region Front End Report(Will be a new report) 

        private IEnumerable<ValidationErrorDto> BuildValidationErrors(List<ValidationError> ilrValidationErrors, IReadOnlyCollection<ReferenceDataService.Model.MetaData.ValidationError> validationErrorsMetadata)
        {
            try
            {
                return ilrValidationErrors.Select(e =>
                    new ValidationErrorDto
                    {
                        AimSequenceNumber = e.AimSequenceNumber,
                        LearnerReferenceNumber = e.LearnerReferenceNumber,
                        RuleName = e.RuleName,
                        Severity = e.Severity,
                        ErrorMessage = validationErrorsMetadata.FirstOrDefault(x => string.Equals(x.RuleName, e.RuleName, StringComparison.OrdinalIgnoreCase))?.Message,
                        FieldValues = e.ValidationErrorParameters == null
                            ? string.Empty
                            : GetValidationErrorParameters(e.ValidationErrorParameters)
                    });
              
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to merge validation error messages", ex);

                throw;
            }
        }

        private string GetValidationErrorParameters(IEnumerable<ValidationErrorParameter> validationErrorParameters)
        {
            StringBuilder result = new StringBuilder();

            foreach (var parameter in validationErrorParameters)
            {
                result.Append($"{parameter.PropertyName}={parameter.Value}|");
            }

            return result.ToString();
        }

        #endregion 
    }
}
