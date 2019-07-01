using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.Logging.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ReportService.Reports
{
    public class EntryPoint : IEntryPoint
    {
        private readonly ILogger _logger;
        private readonly IFileService _fileService;
        private readonly IReportsDependentDataPopulationService _reportsDependentDataPopulationService;
        private readonly IList<IReport> _reports;

        public EntryPoint(
            ILogger logger,
            IFileService fileService,
            IReportsDependentDataPopulationService reportsDependentDataPopulationService,
            IList<IReport> reports)
        {
            _logger = logger;
            _fileService = fileService;
            _reportsDependentDataPopulationService = reportsDependentDataPopulationService;
            _reports = reports;
        }
        public async Task<List<string>> Callback(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            List<string> reportOutputFilenames = new List<string>();
            var stopWatch = new Stopwatch();
            _logger.LogDebug("Inside ReportService callback");

            try
            {
                stopWatch.Start();
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogDebug("ReportService Callback  cancelling before Generating Reports");
                }

                // list of reports to be generated
                var reportsToBeGenerated = _reports.Where(x => reportServiceContext.Tasks.Contains(x.ReportTaskName)).ToList();

                if (!_reports.Any())
                {
                    _logger.LogDebug($"No reports found.");
                }

                // Populate Dependent data.
                var reportsDependsOn = reportsToBeGenerated.SelectMany(x => x.DependsOn).Distinct().ToList();
                var reportsDependentData = await _reportsDependentDataPopulationService.PopulateAsync(reportServiceContext, reportsDependsOn, cancellationToken);

                foreach (var report in reportsToBeGenerated)
                {
                    _logger.LogDebug($"Attempting to generate {report.GetType().Name}");
                    var reportsGenerated = await report.GenerateReportAsync(reportServiceContext, reportsDependentData, cancellationToken);
                    reportOutputFilenames.AddRange(reportsGenerated);
                }

                reportServiceContext.ReportOutputFileNames = string.Join("|", reportOutputFilenames);

                stopWatch.Restart();

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
