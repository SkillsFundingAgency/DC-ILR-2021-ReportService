using Aspose.Cells;
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
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Service.Interface.Builders;

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
            IValueProvider valueProvider) :
            base(valueProvider)
        {
            _logger = logger;
            _fileService = fileService;
            _jsonSerializationService = jsonSerializationService;
            _ilrProviderService = ilrProviderService;
            _ilrReferenceDataProviderService = ilrReferenceDataProviderService;
            _ilrValidationErrorsProvider = ilrValidationErrorsProvider;
            _validationErrorsReportBuilder = validationErrorsReportBuilder;
            _dateTimeProvider = dateTimeProvider;
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

            List<ValidationErrorDto> validationErrorsDto = BuildValidationErrors(ilrValidationErrors, ilrReferenceData.MetaDatas.ValidationErrors);
            await GenerateFrontEndValidationReport(reportServiceContext, validationErrorsDto, externalFileName, cancellationToken);
            
            return reportOutputFilenames;
        }

        private async Task<List<string>> PersistValidationErrorsReport(List<ValidationErrorModel> validationErrors, IReportServiceContext reportServiceContext, string externalFileName, CancellationToken cancellationToken)
        {
            List<string> filesGenerated = new List<string>();

            using (Stream stream = await _fileService.OpenWriteStreamAsync($"{externalFileName}.csv", reportServiceContext.Container, cancellationToken))
            {
                UTF8Encoding utF8Encoding = new UTF8Encoding(false, true);
                using (TextWriter textWriter = new StreamWriter(stream, utF8Encoding))
                {
                    using (CsvWriter csvWriter = new CsvWriter(textWriter))
                    {
                        WriteCsvRecords<ValidationErrorMapper, ValidationErrorModel>(csvWriter, validationErrors);
                        csvWriter.Flush();
                        textWriter.Flush();
                    }
                }
            }
            filesGenerated.Add($"{externalFileName}.csv");

            Workbook workbook = new Workbook();
            Worksheet sheet = workbook.Worksheets[0];
            WriteExcelRecords(sheet, new ValidationErrorMapper(), validationErrors, null, null);

            using (Stream ms = await _fileService.OpenWriteStreamAsync($"{externalFileName}.xlsx", reportServiceContext.Container, cancellationToken))
            {
                workbook.Save(ms, SaveFormat.Xlsx);
            }
            filesGenerated.Add($"{externalFileName}.xlsx");
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
            List<ValidationErrorDto> validationErrorDtos,
            string externalFileName,
            CancellationToken cancellationToken)
        {
            var errors = validationErrorDtos.Where(x => string.Equals(x.Severity, "E", StringComparison.OrdinalIgnoreCase) || string.Equals(x.Severity, "F", StringComparison.OrdinalIgnoreCase)).ToArray();
            var warnings = validationErrorDtos.Where(x => string.Equals(x.Severity, "W", StringComparison.OrdinalIgnoreCase)).ToArray();

            _ilrValidationResult = new FileValidationResult
            {
                TotalLearners = GetNumberOfLearners(reportServiceContext),
                TotalErrors = errors.Length,
                TotalWarnings = warnings.Length,
                TotalWarningLearners = warnings.DistinctByCount(x => x.LearnerReferenceNumber),
                TotalErrorLearners = errors.DistinctByCount(x => x.LearnerReferenceNumber),
                ErrorMessage = validationErrorDtos.FirstOrDefault(x => string.Equals(x.Severity, "F", StringComparison.OrdinalIgnoreCase))?.ErrorMessage,
                //TotalDataMatchErrors = _validationStageOutputCache.DataMatchProblemCount,
                //TotalDataMatchLearners = _validationStageOutputCache.DataMatchProblemLearnersCount
            };

            using (Stream fileStream = await _fileService.OpenWriteStreamAsync($"{externalFileName}.json", reportServiceContext.Container, cancellationToken))
            {
                _jsonSerializationService.Serialize(_ilrValidationResult, fileStream);
            }

            return $"{externalFileName}.json";
        }

        private List<ValidationErrorDto> BuildValidationErrors(List<ValidationError> ilrValidationErrors, IReadOnlyCollection<ReferenceDataService.Model.MetaData.ValidationError> validationErrorsMetadata)
        {
            List<ValidationErrorDto> result = new List<ValidationErrorDto>();
            try
            {
                foreach (ValidationError validationError in ilrValidationErrors)
                {
                    result.Add(new ValidationErrorDto
                    {
                        AimSequenceNumber = validationError.AimSequenceNumber,
                        LearnerReferenceNumber = validationError.LearnerReferenceNumber,
                        RuleName = validationError.RuleName,
                        Severity = validationError.Severity,
                        ErrorMessage = validationErrorsMetadata.FirstOrDefault(x => string.Equals(x.RuleName, validationError.RuleName, StringComparison.OrdinalIgnoreCase))?.Message,
                        FieldValues = validationError.ValidationErrorParameters == null
                             ? string.Empty
                             : GetValidationErrorParameters(validationError.ValidationErrorParameters.ToList()),
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to merge validation error messages", ex);
            }


            return result;
        }

        private string GetValidationErrorParameters(List<ValidationErrorParameter> validationErrorParameters)
        {
            StringBuilder result = new StringBuilder();
            validationErrorParameters.ForEach(x =>
            {
                result.Append($"{x.PropertyName}={x.Value}|");
            });

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
