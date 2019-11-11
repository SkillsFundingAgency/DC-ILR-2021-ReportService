using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Loose.Interface;
using ESFA.DC.ILR.ReportService.Models.ReferenceData;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Validation.Interface;
using ESFA.DC.ILR.ReportService.Reports.Validation.Model;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;
using ESFA.DC.ILR.ValidationErrors.Interface.Models;

namespace ESFA.DC.ILR.ReportService.Reports.Validation.Detail
{
    public sealed class ValidationErrorsDetailReport : AbstractReport, IReport
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
            : base(ReportTaskNameConstants.ValidationReport, "Rule Violation Report")
        {
            _validationErrorsReportBuilder = validationErrorsReportBuilder;
            _csvService = csvService;
            _frontEndValidationReport = frontEndValidationReport;
            _fileNameService = fileNameService;
        }

        public IEnumerable<Type> DependsOn => new List<Type>()
        {
            DependentDataCatalog.InputIlr,
            DependentDataCatalog.ReferenceData,
            DependentDataCatalog.ValidationErrors,
        };

        public async Task<IEnumerable<string>> GenerateAsync(IReportServiceContext reportServiceContext, IReportServiceDependentData reportsDependentData, CancellationToken cancellationToken)
        {
            ILooseMessage ilrMessage =  reportsDependentData.Get<ILooseMessage>();
            ReferenceDataRoot ilrReferenceData = reportsDependentData.Get<ReferenceDataRoot>();
            List<ValidationError> ilrValidationErrors = reportsDependentData.Get<List<ValidationError>>();
            
            var fileName = _fileNameService.GetFilename(reportServiceContext, ReportName, OutputTypes.Csv);
            
            var validationErrorRows = _validationErrorsReportBuilder.Build(ilrValidationErrors, ilrMessage, ilrReferenceData.MetaDatas.ValidationErrors);

            await _csvService.WriteAsync<ValidationErrorRow, ValidationErrorMapper>(validationErrorRows, fileName, reportServiceContext.Container, cancellationToken);
            
            await _frontEndValidationReport.GenerateAsync(reportServiceContext, validationErrorRows,false, cancellationToken);

            return new[] { fileName };
        }
    }
}
