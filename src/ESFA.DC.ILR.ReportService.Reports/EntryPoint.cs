using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.Logging.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;

namespace ESFA.DC.ILR.ReportService.Reports
{
    public class EntryPoint : IEntryPoint
    {
        private readonly ILogger _logger;
        private readonly IExcelService _excelService;
        private readonly IReportsDependentDataPopulationService _reportsDependentDataPopulationService;
        private readonly IZipService _zipService;
        private readonly IList<IReport> _reports;

        public EntryPoint(
            ILogger logger,
            IExcelService excelService,
            IReportsDependentDataPopulationService reportsDependentDataPopulationService,
            IZipService zipService,
            IList<IReport> reports)
        {
            _logger = logger;
            _excelService = excelService;
            _reportsDependentDataPopulationService = reportsDependentDataPopulationService;
            _zipService = zipService;
            _reports = reports;
        }
        public async Task<List<string>> Callback(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            List<string> reportOutputFilenames = new List<string>();

            if (!string.IsNullOrWhiteSpace(reportServiceContext.ReportOutputFileNames))
            {
                reportOutputFilenames.AddRange(reportServiceContext.ReportOutputFileNames.Split('|').ToList());
            }

            _logger.LogDebug("Inside ReportService callback");

            try
            {
                _excelService.ApplyLicense();

                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogDebug("ReportService Callback  cancelling before Generating Reports");
                }

                // list of reports to be generated
                var reportsToBeGenerated = _reports.Where(x => reportServiceContext.Tasks.Contains(x.TaskName, StringComparer.OrdinalIgnoreCase)).ToList();

                if (!_reports.Any())
                {
                    _logger.LogDebug($"No reports found.");
                }

                // Populate Dependent data.
                _logger.LogInfo("Starting External Data");

                var reportsDependsOn = reportsToBeGenerated.SelectMany(x => x.DependsOn).Distinct().ToList();
                var reportsDependentData = await _reportsDependentDataPopulationService.PopulateAsync(reportServiceContext, reportsDependsOn, cancellationToken);

                _logger.LogInfo("Finishing External Data");

                foreach (var report in reportsToBeGenerated)
                {
                    _logger.LogInfo($"Starting {report.GetType().Name}");

                    var reportsGenerated = await report.GenerateAsync(reportServiceContext, reportsDependentData, cancellationToken);
                    reportOutputFilenames.AddRange(reportsGenerated);

                    _logger.LogInfo($"Finishing {report.GetType().Name}");
                }

                await _zipService.CreateZipAsync(reportServiceContext, reportOutputFilenames, reportServiceContext.Container, cancellationToken);

                reportServiceContext.ReportOutputFileNames = string.Join("|", reportOutputFilenames);
                
                _logger.LogDebug("Completed ReportService callback");
            }
            catch (Exception ex)
            {
                _logger.LogError($"ReportService callback exception {ex.Message}", ex);
                throw;
            }

            return reportOutputFilenames;
        }
    }
}
