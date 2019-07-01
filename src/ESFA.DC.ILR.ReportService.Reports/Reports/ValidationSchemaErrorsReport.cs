using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.ReportService.Reports.Mapper;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Model.ReportModels;
using ESFA.DC.ILR.ValidationErrors.Interface.Models;
using ESFA.DC.Logging.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Service.Interface.Builders;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;
using ESFA.DC.ILR.ReportService.Service.Model.Interface;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ReportService.Reports.Reports
{
    public sealed class ValidationSchemaErrorsReport : IReport
    {
        private readonly ILogger _logger;
        private readonly IValidationSchemaErrorsReportBuilder _validationSchemaErrorsReportBuilder;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ICsvService _csvService;


        public ValidationSchemaErrorsReport(
            ILogger logger,
            IFileService fileService,
            IJsonSerializationService jsonSerializationService,
            IValidationSchemaErrorsReportBuilder validationSchemaErrorsReportBuilder,
            IDateTimeProvider dateTimeProvider,
            ICsvService csvService)
        {
            _logger = logger;
            _validationSchemaErrorsReportBuilder = validationSchemaErrorsReportBuilder;
            _dateTimeProvider = dateTimeProvider;
            _csvService = csvService;
        }

        public string ReportFileName => "Rule Violation Report";

        public string ReportTaskName => ReportTaskNameConstants.ValidationSchemaErrorReport;

        public IEnumerable<Type> DependsOn => new List<Type>()
        {
            typeof(List<ValidationError>)
        };

        public async Task<IEnumerable<string>> GenerateReportAsync(IReportServiceContext reportServiceContext, IReportServiceDependentData reportsDependentData, CancellationToken cancellationToken)
        {
            reportServiceContext.Ukprn = GetUkPrn(reportServiceContext.Filename);

            var externalFileName = GetFilename(reportServiceContext);
            var ilrValidationErrors = reportsDependentData.Get<List<ValidationError>>();
            var validationErrorModels = _validationSchemaErrorsReportBuilder.Build(ilrValidationErrors);

            return await PersistValidationErrorsReport(validationErrorModels, reportServiceContext, externalFileName, cancellationToken);
        }
      
        private async Task<IEnumerable<string>> PersistValidationErrorsReport(IEnumerable<ValidationErrorModel> validationErrors, IReportServiceContext reportServiceContext, string externalFileName, CancellationToken cancellationToken)
        {
            var fileName = $"{externalFileName}.csv";

            await _csvService.WriteAsync<ValidationErrorModel, ValidationErrorMapper>(validationErrors, fileName, reportServiceContext.Container, cancellationToken);

            return new []{ fileName };
        }

        private string GetFilename(IReportServiceContext reportServiceContext)
        {
            DateTime dateTime = _dateTimeProvider.ConvertUtcToUk(reportServiceContext.SubmissionDateTimeUtc);
            return $"{reportServiceContext.Ukprn}_{reportServiceContext.JobId}_{ReportFileName} {dateTime:yyyyMMdd-HHmmss}";
        }
        private int GetUkPrn(string fileName)
        {
            var ukPrn = 99999999;
            try
            {
                var fileNameParts = fileName.Substring(0, fileName.IndexOf('.')).Split('-');
                ukPrn = Convert.ToInt32(fileNameParts[1]);
            }
            catch (Exception ex)
            {
                _logger.LogError("ValidationErrorsSchemaReport - Could not parse UkPRN from the filename");
            }

            return ukPrn;
        }

    }
}
