using ESFA.DC.ExcelService.Interface;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Frm.Model;
using ESFA.DC.ILR.ReportService.Reports.Frm.Model.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ReportService.Reports.Frm
{
    public class FrmReport : AbstractReport, IReport
    {
        private readonly IExcelFileService _excelService;
        private readonly IFileNameService _fileNameService;
        private readonly IList<IFrmWorksheetReport> _frmReports;
        private readonly IModelBuilder<IFrmSummaryReport> _frmSummaryReportModelBuilder;
        private readonly IRenderService<IFrmSummaryReport> _frmSummaryReportRenderService;

        private string SummaryName => "Summary";

        public FrmReport(IList<IFrmWorksheetReport> frmReports, IFileNameService fileNameService, IExcelFileService excelService, IModelBuilder<IFrmSummaryReport> frmSummaryReportModelBuilder, IRenderService<IFrmSummaryReport> frmSummaryReportRenderService)
            : base("TaskGenerateFundingRulesMonitoringReport", "Funding Rules Monitoring Report")
        {
            _frmReports = frmReports;
            _fileNameService = fileNameService;
            _excelService = excelService;
            _frmSummaryReportModelBuilder = frmSummaryReportModelBuilder;
            _frmSummaryReportRenderService = frmSummaryReportRenderService;
        }

        public virtual IEnumerable<Type> DependsOn => _frmReports.SelectMany(x => x.DependsOn).Distinct();

        public async Task<IEnumerable<string>> GenerateAsync(IReportServiceContext reportServiceContext, IReportServiceDependentData reportsDependentData, CancellationToken cancellationToken)
        {
            var fileName = _fileNameService.GetFilename(reportServiceContext, ReportName, OutputTypes.Excel);

            var reportsToBeGenerated = _frmReports.Where(x => reportServiceContext.Tasks.Contains(x.TaskName, StringComparer.OrdinalIgnoreCase));

            using (var workbook = _excelService.NewWorkbook())
            {
                workbook.Worksheets.Clear();
                List<IFrmSummary> rows = new List<IFrmSummary>();
                var summaryWorksheet = _excelService.GetWorksheetFromWorkbook(workbook, SummaryName);

                foreach (var frmReport in reportsToBeGenerated)
                {
                    rows.Add(frmReport.Generate(workbook, reportServiceContext, reportsDependentData, cancellationToken));
                }

                var frmSummaryReport = _frmSummaryReportModelBuilder.Build(reportServiceContext, reportsDependentData);
                frmSummaryReport.SummaryTable = rows;
                _frmSummaryReportRenderService.Render(frmSummaryReport, summaryWorksheet);

                await _excelService.SaveWorkbookAsync(workbook, fileName, reportServiceContext.Container, cancellationToken);
            }

            return new[] { fileName };
        }

    }
}
