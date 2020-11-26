using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ExcelService.Interface;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.HighNeedsStudentSummary
{
    public class HighNeedsStudentSummaryReport : AbstractExcelReport<HighNeedsStudentSummaryReportModel>, IReport
    {
        public HighNeedsStudentSummaryReport(
            IFileNameService fileNameService,
            IModelBuilder<HighNeedsStudentSummaryReportModel> modelBuilder,
            IExcelFileService excelService
           )
            : base(
                  fileNameService,
                  modelBuilder,
                  excelService,
                  ReportTemplateConstants.HNSSummaryTemplateName,
                  ReportTemplateConstants.HNSSummaryDataSource,
                  ReportTaskNameConstants.HNSSummaryReport,
                  ReportNameConstants.HighNeedsStudentSummary)
        {
        }

        public async Task<IEnumerable<string>> GenerateAsync(IReportServiceContext reportServiceContext, IReportServiceDependentData reportsDependentData, CancellationToken cancellationToken)
        {
            var filenames = await GenerateExcelAsync(reportServiceContext, reportsDependentData, cancellationToken);

            return filenames;
        }

        public virtual IEnumerable<Type> DependsOn
            => new[]
            {
                DependentDataCatalog.Fm25,
                DependentDataCatalog.ValidIlr,
                DependentDataCatalog.ReferenceData
            };
    }
}
