﻿using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ReportService.Interface.Context;
using ESFA.DC.ILR.ReportService.Interface.Reports;
using ESFA.DC.ILR.ReportService.Interface.Service;
using ILogger = ESFA.DC.Logging.Interfaces.ILogger;

namespace ESFA.DC.ILR.ReportService.Service.Service
{
    public class ReportsProvider : IReportsProvider
    {
        private readonly IList<IReport> _reports;
        private readonly ILogger _logger;

        public ReportsProvider(IList<IReport> reports, ILogger logger)
        {
            _reports = reports;
            _logger = logger;
        }

        public IEnumerable<IReport> ProvideReportsForContext(IReportServiceContext reportServiceContext)
        {
            var missingReportTasks =
                reportServiceContext
                    .Tasks
                    .Where(t => !_reports.Select(r => r.ReportTaskName).Contains(t, StringComparer.OrdinalIgnoreCase));

            foreach (var missingReportTask in missingReportTasks)
            {
                _logger.LogWarning($"Missing Report Task - {missingReportTask}");
            }

            return _reports.Where(r => reportServiceContext.Tasks.Contains(r.ReportTaskName, StringComparer.OrdinalIgnoreCase));
        }
    }
}
