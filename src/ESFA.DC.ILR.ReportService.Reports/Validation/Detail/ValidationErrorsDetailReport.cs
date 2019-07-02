using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReportService.Reports.Interface;
using ESFA.DC.ILR.ReportService.Reports.Validation.Interface;
using ESFA.DC.ILR.ReportService.Reports.Validation.Model;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;
using ESFA.DC.ILR.ValidationErrors.Interface.Models;

namespace ESFA.DC.ILR.ReportService.Reports.Validation.Detail
{
    public sealed class ValidationErrorsDetailReport : IReport
    {
        private readonly IValidationErrorsReportBuilder _validationErrorsReportBuilder;
        private readonly ICsvService _csvService;
        private readonly IFrontEndValidationReport _frontEndValidationReport;
        private readonly IFileNameService _fileNameService;

        public ValidationErrorsDetailReport(
            IValidationErrorsReportBuilder validationErrorsReportBuilder,
            ICsvService csvService,
            IFrontEndValidationReport frontEndValidationReport,
            IFileNameService fileNameService)
        {
            _validationErrorsReportBuilder = validationErrorsReportBuilder;
            _csvService = csvService;
            _frontEndValidationReport = frontEndValidationReport;
            _fileNameService = fileNameService;
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
            
            var fileName = _fileNameService.GetFilename(reportServiceContext, ReportFileName, OutputTypes.Csv);
            
            var validationErrorRows = _validationErrorsReportBuilder.Build(ilrValidationErrors, ilrMessage, ilrReferenceData.MetaDatas.ValidationErrors);

            await _csvService.WriteAsync<ValidationErrorRow, ValidationErrorMapper>(validationErrorRows, fileName, reportServiceContext.Container, cancellationToken);
            
            await _frontEndValidationReport.GenerateAsync(reportServiceContext, validationErrorRows, cancellationToken);

            return new[] { fileName };
        }
    }
}
