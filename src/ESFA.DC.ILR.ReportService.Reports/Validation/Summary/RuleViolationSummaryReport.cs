using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ExcelService.Interface;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Validation.Summary
{
    public class RuleViolationSummaryReport : AbstractExcelReport<RuleViolationSummaryReportModel>, IReport
    {
        public RuleViolationSummaryReport(
            IFileNameService fileNameService,
            IModelBuilder<RuleViolationSummaryReportModel> modelBuilder,
            IExcelFileService excelService
        )
            : base(
                  fileNameService,
                  modelBuilder,
                  excelService,
                  ReportTemplateConstants.RuleViolationSummaryReportTemplateName,
                  ReportTemplateConstants.RuleViolationSummaryReportTemplateDataSource,
                  ReportTaskNameConstants.RuleViolationSummaryReport,
                  ReportNameConstants.RuleViolationSummary)
        {
        }

        public async Task<IEnumerable<string>> GenerateAsync(IReportServiceContext reportServiceContext, IReportServiceDependentData reportsDependentData, CancellationToken cancellationToken)
        {
            var filenames = await GenerateExcelAsync(reportServiceContext, reportsDependentData, cancellationToken);

            return filenames;
        }

        public IEnumerable<Type> DependsOn => new List<Type>()
        {
            DependentDataCatalog.InputIlr,
            DependentDataCatalog.ReferenceData,
            DependentDataCatalog.ValidationErrors,
        };
    }
}
