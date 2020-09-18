﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ExcelService.Interface;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Frm.Summary;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;

namespace ESFA.DC.ILR.ReportService.Reports.Frm
{
    public class FrmReport : AbstractReport, IReport
    {
        private readonly IExcelFileService _excelService;
        private readonly IFileNameService _fileNameService;
        private readonly IList<IWorksheetReport> _frmReports;

        public FrmReport(IList<IWorksheetReport> frmReports, IFileNameService fileNameService, IExcelFileService excelService)
            : base("TaskGenerateFundingRulesMonitoringReport", "Funding Rules Monitoring Report")
        {
            _frmReports = frmReports;
            _fileNameService = fileNameService;
            _excelService = excelService;
        }

        public virtual IEnumerable<Type> DependsOn => _frmReports.SelectMany(x => x.DependsOn).Distinct();

        public async Task<IEnumerable<string>> GenerateAsync(IReportServiceContext reportServiceContext, IReportServiceDependentData reportsDependentData,
            CancellationToken cancellationToken)
        {
            var fileName = _fileNameService.GetFilename(reportServiceContext, ReportName, OutputTypes.Excel);

            var reportsToBeGenerated = _frmReports.Where(x => reportServiceContext.Tasks.Contains(x.TaskName, StringComparer.OrdinalIgnoreCase));

            using (var workbook = _excelService.NewWorkbook())
            {
                workbook.Worksheets.Clear();
                // provision Summary sheet

                foreach (var frmReport in reportsToBeGenerated.Skip(1))
                {
                    frmReport.Generate(workbook, reportServiceContext, reportsDependentData, cancellationToken);
                }

                var summary = reportsToBeGenerated.First();
                summary.Generate(workbook, reportServiceContext, reportsDependentData, cancellationToken);

                await _excelService.SaveWorkbookAsync(workbook, fileName, reportServiceContext.Container, cancellationToken);
            }

            return new[] { fileName };
        }
    }
}
