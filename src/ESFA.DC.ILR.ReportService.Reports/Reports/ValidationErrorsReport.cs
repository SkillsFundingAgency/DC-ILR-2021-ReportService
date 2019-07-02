using CsvHelper;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Extensions;
using ESFA.DC.ILR.ReportService.Reports.Mapper;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Model.ReportModels;
using ESFA.DC.ILR.ValidationErrors.Interface.Models;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.ReportService.Reports.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;
using ESFA.DC.ILR.ReportService.Service.Model.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Reports
{
    public sealed class ValidationErrorsReport : IReport
    {
        private readonly ILogger _logger;
        private readonly IFileService _fileService;
        private readonly IJsonSerializationService _jsonSerializationService;
        private readonly IValidationErrorsReportBuilder _validationErrorsReportBuilder;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ICsvService _csvService;

        private FileValidationResult _ilrValidationResult;

        public ValidationErrorsReport(
            ILogger logger,
            IFileService fileService,
            IJsonSerializationService jsonSerializationService,
            IValidationErrorsReportBuilder validationErrorsReportBuilder,
            IDateTimeProvider dateTimeProvider,
            ICsvService csvService)
        {
            _logger = logger;
            _fileService = fileService;
            _jsonSerializationService = jsonSerializationService;
            _validationErrorsReportBuilder = validationErrorsReportBuilder;
            _dateTimeProvider = dateTimeProvider;
            _csvService = csvService;
        }

        public string ReportFileName => "Rule Violation Report";

        public string ReportTaskName => ReportTaskNameConstants.ValidationReport;

        public IEnumerable<Type> DependsOn => new List<Type>()
        {
            typeof(IMessage),
            typeof(ReferenceDataRoot),
            typeof(List<ValidationError>)
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

            await GenerateFrontEndValidationReport(reportServiceContext, validationErrorsDto, externalFileName, cancellationToken);
            
            return reportOutputFilenames;
        }

        private async Task<IEnumerable<string>> PersistValidationErrorsReport(IEnumerable<ValidationErrorModel> validationErrors, IReportServiceContext reportServiceContext, string externalFileName, CancellationToken cancellationToken)
        {
            var fileName = $"{externalFileName}.csv";

            await _csvService.WriteAsync<ValidationErrorModel, ValidationErrorMapper>(validationErrors, fileName, reportServiceContext.Container, cancellationToken);

            return new[] {fileName};
        }

        private string GetFilename(IReportServiceContext reportServiceContext)
        {
            DateTime dateTime = _dateTimeProvider.ConvertUtcToUk(reportServiceContext.SubmissionDateTimeUtc);
            return $"{reportServiceContext.Ukprn}_{reportServiceContext.JobId}_{ReportFileName} {dateTime:yyyyMMdd-HHmmss}";
        }

        #region Front End Report(Will be a new report) 

        private async Task<string> GenerateFrontEndValidationReport(
            IReportServiceContext reportServiceContext,
            IEnumerable<ValidationErrorDto> validationErrorDtos,
            string externalFileName,
            CancellationToken cancellationToken)
        {
            var validationErrorDtosList = validationErrorDtos.ToList();

            var errors = validationErrorDtosList.Where(x => string.Equals(x.Severity, "E", StringComparison.OrdinalIgnoreCase) || string.Equals(x.Severity, "F", StringComparison.OrdinalIgnoreCase)).ToArray();
            var warnings = validationErrorDtosList.Where(x => string.Equals(x.Severity, "W", StringComparison.OrdinalIgnoreCase)).ToArray();

            _ilrValidationResult = new FileValidationResult
            {
                TotalLearners = GetNumberOfLearners(reportServiceContext),
                TotalErrors = errors.Length,
                TotalWarnings = warnings.Length,
                TotalWarningLearners = warnings.DistinctByCount(x => x.LearnerReferenceNumber),
                TotalErrorLearners = errors.DistinctByCount(x => x.LearnerReferenceNumber),
                ErrorMessage = validationErrorDtosList.FirstOrDefault(x => string.Equals(x.Severity, "F", StringComparison.OrdinalIgnoreCase))?.ErrorMessage,
                //TotalDataMatchErrors = _validationStageOutputCache.DataMatchProblemCount,
                //TotalDataMatchLearners = _validationStageOutputCache.DataMatchProblemLearnersCount
            };

            var fileName = $"{externalFileName}.json";

            using (Stream fileStream = await _fileService.OpenWriteStreamAsync(fileName, reportServiceContext.Container, cancellationToken))
            {
                _jsonSerializationService.Serialize(_ilrValidationResult, fileStream);
            }

            return fileName;
        }

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

        private int GetNumberOfLearners(IReportServiceContext reportServiceContext)
        {
            int ret = 0;
            try
            {
                ret = reportServiceContext.ValidLearnRefNumbersCount;
                ret = ret + reportServiceContext.InvalidLearnRefNumbersCount;
            }
            catch (Exception ex)
            {
                _logger.LogError("Can't read number of learners", ex);
            }

            return ret;
        }
        #endregion 
    }
}
