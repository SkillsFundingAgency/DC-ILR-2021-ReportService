using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR1819.ReportService.Interface.Configuration;
using ESFA.DC.ILR1819.ReportService.Interface.Reports;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.Report;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.JobContext;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.ILR1819.ReportService.Service
{
    public sealed class EntryPoint
    {
        private readonly ILogger _logger;

        private readonly IStreamableKeyValuePersistenceService _streamableKeyValuePersistenceService;

        private readonly IList<IReport> _reports;

        private readonly Dictionary<string, ReportType> _reportsAvailable;

        private readonly IIlrFileHelper _ilrFileHelper;

        public EntryPoint(
            ILogger logger,
            ITopicAndTaskSectionOptions topicAndTaskSectionOptions,
            IStreamableKeyValuePersistenceService streamableKeyValuePersistenceService,
            IList<IReport> reports,
            IIlrFileHelper ilrFileHelper)
        {
            _logger = logger;
            _streamableKeyValuePersistenceService = streamableKeyValuePersistenceService;
            _reports = reports;
            _ilrFileHelper = ilrFileHelper;

            _reportsAvailable = new Dictionary<string, ReportType>
            {
                { topicAndTaskSectionOptions.TopicReports_TaskGenerateValidationReport, ReportType.ValidationErrors },
                { topicAndTaskSectionOptions.TopicReports_TaskGenerateAllbOccupancyReport, ReportType.AllbOccupancy },
                { topicAndTaskSectionOptions.TopicReports_TaskGenerateFundingSummaryReport, ReportType.FundingSummary },
                { topicAndTaskSectionOptions.TopicReports_TaskGenerateMainOccupancyReport, ReportType.MainOccupancy },
                { topicAndTaskSectionOptions.TopicReports_TaskGenerateMathsAndEnglishReport, ReportType.MathsAndEnglish },
                { topicAndTaskSectionOptions.TopicReports_TaskGenerateSummaryOfFunding1619Report, ReportType.SummaryOfFunding1619 }
            };
        }

        public async Task<bool> Callback(JobContextMessage jobContextMessage, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Reporting callback invoked");

            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    await ExecuteTasks(jobContextMessage, archive, cancellationToken);
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    return false;
                }

                await _streamableKeyValuePersistenceService.SaveAsync($"{jobContextMessage.KeyValuePairs[JobContextMessageKey.UkPrn]}_{jobContextMessage.JobId}_Reports.zip", memoryStream, cancellationToken);
            }

            return true;
        }

        private async Task ExecuteTasks(IJobContextMessage jobContextMessage, ZipArchive archive, CancellationToken cancellationToken)
        {
            if (!_ilrFileHelper.CheckIlrFileNameIsValid(jobContextMessage))
            {
                return;
            }

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
            if (!_reportsAvailable.TryGetValue(task, out var reportType))
            {
                _logger.LogDebug($"Unknown report task '{task}'");
                return;
            }

            IReport report = _reports.Single(x => x.ReportType == reportType);

            if (report == null)
            {
                _logger.LogDebug($"Unable to find report '{task}'");
                return;
            }

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            _logger.LogDebug($"Attempting to generate {report.GetType().Name}");
            await report.GenerateReport(jobContextMessage, archive, cancellationToken);
            stopWatch.Stop();
            _logger.LogDebug($"Persisted {report.GetType().Name} to csv/json in: {stopWatch.ElapsedMilliseconds}");
        }


    }
}
