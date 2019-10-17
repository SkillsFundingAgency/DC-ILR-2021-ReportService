using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;

namespace ESFA.DC.ILR.ReportService.Reports.Validation.Summary
{
    public class RuleViolationSummaryReport : AbstractReport, IReport
    {
        private readonly IFileNameService _fileNameService;
        private readonly IModelBuilder<RuleViolationSummaryReportModel> _modelBuilder;
        private readonly IExcelService _excelService;
        private const int FisInfoRow = 8;

        public RuleViolationSummaryReport(
            IFileNameService fileNameService,
            IModelBuilder<RuleViolationSummaryReportModel> modelBuilder,
            IExcelService excelService
        )
            : base(ReportTaskNameConstants.RuleViolationSummaryReport, "Rule Violation Summary Report")
        {
            _fileNameService = fileNameService;
            _modelBuilder = modelBuilder;
            _excelService = excelService;
        }

        public async Task<IEnumerable<string>> GenerateAsync(IReportServiceContext reportServiceContext,
            IReportServiceDependentData reportsDependentData,
            CancellationToken cancellationToken)
        {
            var fileName = _fileNameService.GetFilename(reportServiceContext, ReportName, OutputTypes.Excel);
            var model = _modelBuilder.Build(reportServiceContext, reportsDependentData);
            var workbook = _excelService.BindExcelTemplateToWorkbook(model, ReportTemplateConstants.RuleViolationSummaryReportTemplateName, ReportTemplateConstants.RuleViolationSummaryReportTemplateDataSource);
            await _excelService.SaveWorkbookAsync(workbook, fileName, reportServiceContext.Container, cancellationToken);
            return new[] {fileName};
        }

        public IEnumerable<Type> DependsOn => new List<Type>()
        {
            DependentDataCatalog.InvalidIlr,
            DependentDataCatalog.ReferenceData,
            DependentDataCatalog.ValidationErrors,
        };
    }
}
