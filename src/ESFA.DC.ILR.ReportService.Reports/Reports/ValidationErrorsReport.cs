using CsvHelper;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Extensions;
using ESFA.DC.ILR.ReportService.Reports.Mapper;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Providers;
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
using ESFA.DC.ILR.ReportService.Service.Interface.Builders;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;

namespace ESFA.DC.ILR.ReportService.Reports.Reports
{
    public sealed class ValidationErrorsReport : AbstractReport, IReport
    {
        private readonly ILogger _logger;
        private readonly IFileService _fileService;
        private readonly IJsonSerializationService _jsonSerializationService;
        private readonly IFileProviderService<IMessage> _ilrProviderService;
        private readonly IFileProviderService<ReferenceDataRoot> _ilrReferenceDataProviderService;
        private readonly IFileProviderService<List<ValidationError>> _ilrValidationErrorsProvider;
        private readonly IValidationErrorsReportBuilder _validationErrorsReportBuilder;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ICsvService _csvService;

        private FileValidationResult _ilrValidationResult;

        public ValidationErrorsReport(
            ILogger logger,
            IFileService fileService,
            IJsonSerializationService jsonSerializationService,
            IFileProviderService<IMessage> ilrProviderService,
            IFileProviderService<ReferenceDataRoot> ilrReferenceDataProviderService,
            IFileProviderService<List<ValidationError>> ilrValidationErrorsProvider,
            IValidationErrorsReportBuilder validationErrorsReportBuilder,
            IDateTimeProvider dateTimeProvider,
            ICsvService csvService,
            IValueProvider valueProvider) 
            : base(valueProvider)
        {
            _logger = logger;
            _fileService = fileService;
            _jsonSerializationService = jsonSerializationService;
            _ilrProviderService = ilrProviderService;
            _ilrReferenceDataProviderService = ilrReferenceDataProviderService;
            _ilrValidationErrorsProvider = ilrValidationErrorsProvider;
            _validationErrorsReportBuilder = validationErrorsReportBuilder;
            _dateTimeProvider = dateTimeProvider;
            _csvService = csvService;
        }

        public string ReportFileName => "Rule Violation Report";

        public string ReportTaskName => ReportTaskNameConstants.ValidationReport;

        public async Task<IEnumerable<string>> GenerateReportAsync(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            List<string> reportOutputFilenames = new List<string>();

            IMessage ilrMessage = await _ilrProviderService.ProvideAsync(reportServiceContext, cancellationToken);
            ReferenceDataRoot ilrReferenceData = await _ilrReferenceDataProviderService.ProvideAsync(reportServiceContext, cancellationToken);
            List<ValidationError> ilrValidationErrors = await _ilrValidationErrorsProvider.ProvideAsync(reportServiceContext, cancellationToken);

            reportServiceContext.Ukprn = ilrMessage.HeaderEntity.SourceEntity.UKPRN;
            var externalFileName = GetFilename(reportServiceContext);
            var validationErrorModels = _validationErrorsReportBuilder.Build(ilrValidationErrors, ilrMessage, ilrReferenceData.MetaDatas.ValidationErrors);
            var list = await PersistValidationErrorsReport(validationErrorModels, reportServiceContext, externalFileName, cancellationToken);
            reportOutputFilenames.AddRange(list);

            var validationErrorsDto = BuildValidationErrors(ilrValidationErrors, ilrReferenceData.MetaDatas.ValidationErrors);

            await GenerateFrontEndValidationReport(reportServiceContext, validationErrorsDto, externalFileName, cancellationToken);
            
            return reportOutputFilenames;
        }

        private async Task<List<string>> PersistValidationErrorsReport(List<ValidationErrorModel> validationErrors, IReportServiceContext reportServiceContext, string externalFileName, CancellationToken cancellationToken)
        {
            List<string> filesGenerated = new List<string>();

            var fileName = $"{externalFileName}.csv";

            await _csvService.WriteAsync<ValidationErrorModel, ValidationErrorMapper>(validationErrors, fileName, reportServiceContext.Container, cancellationToken);
            
            filesGenerated.Add(fileName);

            return filesGenerated;
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
