using Aspose.Cells;
using ESFA.DC.ExcelService.Interface;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Extensions;
using ESFA.DC.ILR.ReportService.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ReportService.Reports.Frm.Summary
{
    public class FrmSummaryReport : AbstractReport, IWorksheetReport
    {
        private readonly IExcelFileService _excelService;
        private readonly IModelBuilder<FrmSummaryReportModel> _fundingRulesMonitoringModelBuilder;
        private readonly IRenderService<FrmSummaryReportModel> _fundingRulesMonitoringRenderService;

        private Dictionary<string, string> FrmTitles = new Dictionary<string, string>
        {
            { "FRM06", "Continuance Issues" },
            { "FRM07", "Breaks in Learning: Planned End Date" },
            { "FRM08", "Breaks in Learning: Duration" },
            { "FRM09", "Transfers with no return" },
            { "FRM015", "End Point Assessment Organisations" }

        };
        public FrmSummaryReport(
            IExcelFileService excelService,
            IModelBuilder<FrmSummaryReportModel> fundingRulesMonitoringModelBuilder,
            IRenderService<FrmSummaryReportModel> fundingRulesMonitoringRenderService
            ) : base(
                "TaskGenerateFundingRulesMonitoringReport",
                "Summary")
        {
            _excelService = excelService;
            _fundingRulesMonitoringModelBuilder = fundingRulesMonitoringModelBuilder;
            _fundingRulesMonitoringRenderService = fundingRulesMonitoringRenderService;
        }

        public virtual IEnumerable<Type> DependsOn
           => new[]
           {
                DependentDataCatalog.ValidIlr,
                DependentDataCatalog.ReferenceData
           };

        public void Generate(Workbook workbook, IReportServiceContext reportServiceContext, IReportServiceDependentData reportsDependentData, CancellationToken cancellationToken)
        {
            var fundingSummaryReportModel = _fundingRulesMonitoringModelBuilder.Build(reportServiceContext, reportsDependentData);

            var summaryTable = workbook.Worksheets.Select(ws => new FrmSummaryReportTableRow()
            {
                Report = ws.Name,
                Title = FrmTitles.GetValueOrDefault(ws.Name),
                NumberOfQueries = ws.Cells.Rows.Count - 1
            }).ToList();

            fundingSummaryReportModel.FundingRulesMonitoring = summaryTable;
            fundingSummaryReportModel.GenerateTotalRow();


            var worksheet = _excelService.GetWorksheetFromWorkbook(workbook, ReportName);
            worksheet.MoveTo(0);
            _fundingRulesMonitoringRenderService.Render(fundingSummaryReportModel, worksheet);

        }
    }
}
