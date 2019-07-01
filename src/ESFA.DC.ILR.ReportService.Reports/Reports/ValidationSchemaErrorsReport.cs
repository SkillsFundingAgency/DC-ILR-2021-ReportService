using Aspose.Cells;
using CsvHelper;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Service.Interface.Builders;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;

namespace ESFA.DC.ILR.ReportService.Reports.Reports
{
    public sealed class ValidationSchemaErrorsReport : AbstractReport, IReport
    {
        private readonly ILogger _logger;
        private readonly IFileProviderService<List<ValidationError>> _ilrValidationErrorsProvider;
        private readonly IValidationSchemaErrorsReportBuilder _validationSchemaErrorsReportBuilder;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ICsvService _csvService;

        private FileValidationResult _ilrValidationResult;

        public ValidationSchemaErrorsReport(
            ILogger logger,
            IFileProviderService<List<ValidationError>> ilrValidationErrorsProvider,
            IValidationSchemaErrorsReportBuilder validationSchemaErrorsReportBuilder,
            IDateTimeProvider dateTimeProvider,
            ICsvService csvService,
            IValueProvider valueProvider) :
            base(valueProvider)
        {
            _logger = logger;
            _ilrValidationErrorsProvider = ilrValidationErrorsProvider;
            _validationSchemaErrorsReportBuilder = validationSchemaErrorsReportBuilder;
            _dateTimeProvider = dateTimeProvider;
            _csvService = csvService;
        }

        public string ReportFileName => "Rule Violation Report";

        public string ReportTaskName => ReportTaskNameConstants.ValidationSchemaErrorReport;

        public async Task<IEnumerable<string>> GenerateReportAsync(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            reportServiceContext.Ukprn = GetUkPrn(reportServiceContext.Filename);

            var externalFileName = GetFilename(reportServiceContext);

            var ilrValidationErrors = await _ilrValidationErrorsProvider.ProvideAsync(reportServiceContext, cancellationToken);

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
            var ukPrn = 124;
            DateTime dateTime = _dateTimeProvider.ConvertUtcToUk(reportServiceContext.SubmissionDateTimeUtc);
            return $"{ukPrn}_{reportServiceContext.JobId}_{ReportFileName} {dateTime:yyyyMMdd-HHmmss}";
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
