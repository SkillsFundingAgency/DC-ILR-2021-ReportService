using Aspose.Cells;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.HighNeedsStudentSummary.Model;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.HighNeedsStudentSummary
{
    public class HighNeedsStudentSummaryReport : AbstractReport, IReport
    {
        private readonly IFileNameService _fileNameService;
        private readonly IModelBuilder<HighNeedsStudentSummaryReportModel> _modelBuilder;
        private readonly IExcelService _excelService;

        public HighNeedsStudentSummaryReport(
            IFileNameService fileNameService,
            IModelBuilder<HighNeedsStudentSummaryReportModel> modelBuilder,
            IExcelService excelService
           )
            : base(ReportTaskNameConstants.HNSSummaryReport, "High Needs Students Summary Report")
        {
            _fileNameService = fileNameService;
            _modelBuilder = modelBuilder;
            _excelService = excelService;
        }

        public async Task<IEnumerable<string>> GenerateAsync(IReportServiceContext reportServiceContext, IReportServiceDependentData reportsDependentData,
            CancellationToken cancellationToken)
        {
            var fileName = _fileNameService.GetFilename(reportServiceContext, ReportName, OutputTypes.Excel);
            var model = _modelBuilder.Build(reportServiceContext, reportsDependentData);
            var workbook = _excelService.BindExcelTemplateToWorkbook(model, "HNSSummaryReportTemplate.xlsx", "HNSSummary");
            await _excelService.SaveWorkbookAsync(workbook, fileName, reportServiceContext.Container, cancellationToken);
            return new[] { fileName };
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
