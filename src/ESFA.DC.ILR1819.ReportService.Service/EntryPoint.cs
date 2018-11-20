using System;

namespace ESFA.DC.ILR1819.ReportService.Service
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Compression;
    using System.Threading;
    using System.Threading.Tasks;
    using ESFA.DC.ILR1819.ReportService.Interface.Reports;
    using ESFA.DC.ILR1819.ReportService.Interface.Service;
    using ESFA.DC.IO.Interfaces;
    using ESFA.DC.JobContext.Interface;
    using ESFA.DC.JobContextManager.Model;
    using ESFA.DC.JobContextManager.Model.Interface;
    using ESFA.DC.Logging.Interfaces;

    public sealed class EntryPoint
    {
        private readonly ILogger _logger;

        private readonly IStreamableKeyValuePersistenceService _streamableKeyValuePersistenceService;

        private readonly IList<IReport> _reports;

        private readonly IIlrFileHelper _ilrFileHelper;

        public EntryPoint(
            ILogger logger,
            IStreamableKeyValuePersistenceService streamableKeyValuePersistenceService,
            IList<IReport> reports,
            IIlrFileHelper ilrFileHelper)
        {
            _logger = logger;
            _streamableKeyValuePersistenceService = streamableKeyValuePersistenceService;
            _reports = reports;
            _ilrFileHelper = ilrFileHelper;
        }

        public async Task<bool> Callback(JobContextMessage jobContextMessage, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Reporting callback invoked");

            var reportZipFileKey = $"{jobContextMessage.KeyValuePairs[JobContextMessageKey.UkPrn]}_{jobContextMessage.JobId}_Reports.zip";
            if (cancellationToken.IsCancellationRequested)
            {
                return false;
            }

            MemoryStream memoryStream = new MemoryStream();
            var zipFileExists = await _streamableKeyValuePersistenceService.ContainsAsync(reportZipFileKey, cancellationToken);
            if (zipFileExists)
            {
                await _streamableKeyValuePersistenceService.GetAsync(reportZipFileKey, memoryStream, cancellationToken);
            }

            using (memoryStream)
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Update, true))
                {
                    await ExecuteTasks(jobContextMessage, archive, cancellationToken);
                }

                await _streamableKeyValuePersistenceService.SaveAsync(reportZipFileKey, memoryStream, cancellationToken);
            }

            return true;
        }

        private async Task ExecuteTasks(IJobContextMessage jobContextMessage, ZipArchive archive, CancellationToken cancellationToken)
        {
            foreach (ITaskItem taskItem in jobContextMessage.Topics[jobContextMessage.TopicPointer].Tasks)
            {
                if (taskItem.SupportsParallelExecution)
                {
                    Parallel.ForEach(
                        taskItem.Tasks,
                        new ParallelOptions { CancellationToken = cancellationToken },
                        async task => { await GenerateReportAsync(task, jobContextMessage, archive, cancellationToken); });
                }
                else
                {
                    foreach (string task in taskItem.Tasks)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            break;
                        }

                        await GenerateReportAsync(task, jobContextMessage, archive, cancellationToken);
                    }
                }
            }
        }

        private async Task GenerateReportAsync(string task, IJobContextMessage jobContextMessage, ZipArchive archive, CancellationToken cancellationToken)
        {
            var foundReport = false;
            foreach (var report in _reports)
            {
                if (!report.IsMatch(task))
                {
                    continue;
                }

                var stopWatch = new Stopwatch();
                stopWatch.Start();
                _logger.LogDebug($"Attempting to generate {report.GetType().Name}");
                await report.GenerateReport(jobContextMessage, archive, false, cancellationToken);
                stopWatch.Stop();
                _logger.LogDebug($"Persisted {report.GetType().Name} to csv/json in: {stopWatch.ElapsedMilliseconds}");

                foundReport = true;
                break;
            }

            if (!foundReport)
            {
                _logger.LogDebug($"Unable to find report '{task}'");
            }
        }
    }
}
