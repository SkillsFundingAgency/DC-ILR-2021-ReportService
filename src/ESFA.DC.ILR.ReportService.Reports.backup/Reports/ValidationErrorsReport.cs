using CsvHelper;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Reports.Comparer;
using ESFA.DC.ILR.ReportService.Reports.Extensions;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Providers;
using ESFA.DC.ILR.ReportService.Service.Model;
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
using Aspose.Cells;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Mapper;

namespace ESFA.DC.ILR.ReportService.Reports.Reports
{
    public sealed class ValidationErrorsReport : AbstractReport, IReport
    {
        private static readonly ValidationErrorsModelComparer ValidationErrorsModelComparer = new ValidationErrorsModelComparer();

        private readonly ILogger _logger;
        private readonly IFileService _fileService;
        private readonly IJsonSerializationService _jsonSerializationService;
        private readonly IIlrProviderService _ilrProviderService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IValueProvider _valueProvider;

        private string _externalFileName;
        private string _fileName;
        private FileValidationResult _ilrValidationResult;

        public ValidationErrorsReport(
            ILogger logger,
            IFileService fileService,
            IJsonSerializationService jsonSerializationService,
            IIlrProviderService ilrProviderService,
            IDateTimeProvider dateTimeProvider,
            IValueProvider valueProvider):
            base(valueProvider)
        {
            _logger = logger;
            _fileService = fileService;
            _jsonSerializationService = jsonSerializationService;
            _ilrProviderService = ilrProviderService;
            _dateTimeProvider = dateTimeProvider;
            _valueProvider = valueProvider;
        }

        public string ReportFileName => "Rule Violation Report";

        public string ReportTaskName => ReportTaskNameConstants.ValidationReport;

        public async Task GenerateReport(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            Task<IMessage> ilrTask = _ilrProviderService.GetIlrFile(reportServiceContext, cancellationToken);
            Task<List<ValidationErrorDto>> validationErrorDtosTask = ReadAndDeserialiseValidationErrorsAsync(reportServiceContext, cancellationToken);
            await Task.WhenAll(ilrTask, validationErrorDtosTask);

            _externalFileName = GetFilename(reportServiceContext);
            _fileName = GetZipFilename(reportServiceContext);

            List<ValidationErrorDto> validationErrorDtos = validationErrorDtosTask.Result;

            List<ValidationErrorModel> validationErrors = ValidationErrorModels(ilrTask.Result, validationErrorDtos);
            GenerateFrontEndValidationReport(reportServiceContext, validationErrorDtos);

            await PersistValuesToStorage(validationErrors, reportServiceContext, cancellationToken);
        }

        private string GetFilename(IReportServiceContext reportServiceContext)
        {
            DateTime dateTime = _dateTimeProvider.ConvertUtcToUk(reportServiceContext.SubmissionDateTimeUtc);
            return $"{reportServiceContext.Ukprn}_{reportServiceContext.JobId}_{ReportFileName} {dateTime:yyyyMMdd-HHmmss}";
        }

        private string GetZipFilename(IReportServiceContext reportServiceContext)
        {
            DateTime dateTime = _dateTimeProvider.ConvertUtcToUk(reportServiceContext.SubmissionDateTimeUtc);
            return $"{ReportFileName} {dateTime:yyyyMMdd-HHmmss}";
        }
        private void GenerateFrontEndValidationReport(
            IReportServiceContext reportServiceContext,
            List<ValidationErrorDto> validationErrorDtos)
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
        }

        private async Task<List<ValidationErrorDto>> ReadAndDeserialiseValidationErrorsAsync(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            List<ValidationErrorDto> result = new List<ValidationErrorDto>();

            try
            {
                string validationErrorsStr = string.Empty;

                using (var stream = await _fileService.OpenReadStreamAsync(reportServiceContext.ValidationErrorsKey, reportServiceContext.Container, cancellationToken))
                {
                    StreamReader reader = new StreamReader(stream);
                    validationErrorsStr = reader.ReadToEnd();
                }

                try
                {
                    List<ValidationError> validationErrors = _jsonSerializationService.Deserialize<List<ValidationError>>(validationErrorsStr);

                    // Extract the rules names and fetch the details for them.
                    string[] rulesNames = validationErrors.Select(x => x.RuleName).Distinct().ToArray();
                    List<ValidationErrorDetails> validationErrorDetails = rulesNames.Select(x => new ValidationErrorDetails(x)).ToList();

                    foreach (ValidationError validationError in validationErrors)
                    {
                        ValidationErrorDetails validationErrorDetail = validationErrorDetails.SingleOrDefault(x => string.Equals(x.RuleName, validationError.RuleName, StringComparison.OrdinalIgnoreCase));

                        result.Add(new ValidationErrorDto
                        {
                            AimSequenceNumber = validationError.AimSequenceNumber,
                            LearnerReferenceNumber = validationError.LearnerReferenceNumber,
                            RuleName = validationError.RuleName,
                            Severity = validationError.Severity,
                            ErrorMessage = validationErrorDetail?.Message,
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
            }
            catch (Exception ex)
            {
                _logger.LogError("Can't process validation errors", ex);
            }

            return result;
        }

        private async Task PersistValuesToStorage(List<ValidationErrorModel> validationErrors, IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            using (Stream fileStream = await _fileService.OpenWriteStreamAsync($"{_externalFileName}.json", reportServiceContext.Container, cancellationToken))
            {
                _jsonSerializationService.Serialize(_jsonSerializationService.Serialize(_ilrValidationResult), fileStream);
            }

            using (Stream stream = await _fileService.OpenWriteStreamAsync($"{_externalFileName}.csv", reportServiceContext.Container, cancellationToken))
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

            Workbook workbook = new Workbook();
            Worksheet sheet = workbook.Worksheets[0];
            WriteExcelRecords(sheet, new ValidationErrorMapper(), validationErrors, null, null);

            using (Stream ms = await _fileService.OpenWriteStreamAsync($"{_externalFileName}.xlsx", reportServiceContext.Container, cancellationToken))
            {
                workbook.Save(ms, SaveFormat.Xlsx);
            }
        }
       
        private List<ValidationErrorModel> ValidationErrorModels(IMessage message, List<ValidationErrorDto> validationErrorDtos)
        {
            var validationErrors = new List<ValidationErrorModel>();
            foreach (ValidationErrorDto validationErrorDto in validationErrorDtos)
            {
                if (message == null)
                {
                    validationErrors.Add(new ValidationErrorModel(
                        validationErrorDto.Severity,
                        validationErrorDto.LearnerReferenceNumber,
                        validationErrorDto.RuleName,
                        validationErrorDto.FieldValues,
                        validationErrorDto.ErrorMessage,
                        validationErrorDto.AimSequenceNumber));
                }
                else
                {
                    ILearner learner = message.Learners?.FirstOrDefault(x => x.LearnRefNumber == validationErrorDto.LearnerReferenceNumber);
                    ILearningDelivery learningDelivery = learner?.LearningDeliveries?.FirstOrDefault(x => x.AimSeqNumber == validationErrorDto.AimSequenceNumber);

                    validationErrors.Add(new ValidationErrorModel(
                        validationErrorDto.Severity,
                        validationErrorDto.LearnerReferenceNumber,
                        validationErrorDto.RuleName,
                        validationErrorDto.FieldValues,
                        validationErrorDto.ErrorMessage,
                        validationErrorDto.AimSequenceNumber,
                        learningDelivery?.LearnAimRef,
                        learningDelivery?.SWSupAimId,
                        learningDelivery?.FundModel,
                        learningDelivery?.PartnerUKPRNNullable,
                        learner?.ProviderSpecLearnerMonitorings?.FirstOrDefault(x => string.Equals(x.ProvSpecLearnMonOccur, "A", StringComparison.OrdinalIgnoreCase))?.ProvSpecLearnMon,
                        learner?.ProviderSpecLearnerMonitorings?.FirstOrDefault(x => string.Equals(x.ProvSpecLearnMonOccur, "B", StringComparison.OrdinalIgnoreCase))?.ProvSpecLearnMon,
                        learningDelivery?.ProviderSpecDeliveryMonitorings?.FirstOrDefault(x => string.Equals(x.ProvSpecDelMonOccur, "A", StringComparison.OrdinalIgnoreCase))?.ProvSpecDelMon,
                        learningDelivery?.ProviderSpecDeliveryMonitorings?.FirstOrDefault(x => string.Equals(x.ProvSpecDelMonOccur, "B", StringComparison.OrdinalIgnoreCase))?.ProvSpecDelMon,
                        learningDelivery?.ProviderSpecDeliveryMonitorings?.FirstOrDefault(x => string.Equals(x.ProvSpecDelMonOccur, "C", StringComparison.OrdinalIgnoreCase))?.ProvSpecDelMon,
                        learningDelivery?.ProviderSpecDeliveryMonitorings?.FirstOrDefault(x => string.Equals(x.ProvSpecDelMonOccur, "D", StringComparison.OrdinalIgnoreCase))?.ProvSpecDelMon));
                }
            }

            validationErrors.Sort(ValidationErrorsModelComparer);
            return validationErrors;
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
    }
}
