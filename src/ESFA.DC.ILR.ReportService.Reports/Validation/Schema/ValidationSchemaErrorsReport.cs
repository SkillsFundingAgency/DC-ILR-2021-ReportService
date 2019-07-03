using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Interface;
using ESFA.DC.ILR.ReportService.Reports.Validation.Model;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;
using ESFA.DC.ILR.ValidationErrors.Interface.Models;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.ILR.ReportService.Reports.Validation.Schema
{
    public sealed class ValidationSchemaErrorsReport : AbstractReport, IReport
    {
        private readonly IValidationSchemaErrorsReportBuilder _validationSchemaErrorsReportBuilder;
        private readonly ICsvService _csvService;
        private readonly IFileNameService _fileNameService;


        public ValidationSchemaErrorsReport(
            IValidationSchemaErrorsReportBuilder validationSchemaErrorsReportBuilder,
            ICsvService csvService,
            IFileNameService fileNameService)
        : base(ReportTaskNameConstants.ValidationSchemaErrorReport, "Rule Violation Report")
        {
            _validationSchemaErrorsReportBuilder = validationSchemaErrorsReportBuilder;
            _csvService = csvService;
            _fileNameService = fileNameService;
        }

        public IEnumerable<Type> DependsOn => new List<Type>()
        {
            DependentDataCatalog.ValidationErrors
        };

        public async Task<IEnumerable<string>> GenerateAsync(IReportServiceContext reportServiceContext, IReportServiceDependentData reportsDependentData, CancellationToken cancellationToken)
        {
            var ilrValidationErrors = reportsDependentData.Get<List<ValidationError>>();

            var fileName = _fileNameService.GetFilename(reportServiceContext, FileName, OutputTypes.Csv);

            var validationErrorRows = _validationSchemaErrorsReportBuilder.Build(ilrValidationErrors);

            await _csvService.WriteAsync<ValidationErrorRow, ValidationErrorMapper>(validationErrorRows, fileName, reportServiceContext.Container, cancellationToken);

            return new[] { fileName };
        }
    }
}
