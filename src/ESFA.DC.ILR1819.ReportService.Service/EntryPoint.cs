using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR1819.ReportService.Interface.Configuration;
using ESFA.DC.ILR1819.ReportService.Interface.Reports;
using ESFA.DC.ILR1819.ReportService.Model.Report;
using ESFA.DC.JobContext;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.ILR1819.ReportService.Service
{
    public sealed class EntryPoint
    {
        private readonly ILogger _logger;

        private readonly IReport[] _reports;

        private readonly Dictionary<string, ReportType> reportsAvailable;

        public EntryPoint(
            ILogger logger,
            ITopicAndTaskSectionOptions topicAndTaskSectionOptions,
            IReport[] reports)
        {
            _logger = logger;
            _reports = reports;

            reportsAvailable = new Dictionary<string, ReportType>
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
            _logger.LogInfo("Callback invoked");
            foreach (ITaskItem taskItem in jobContextMessage.Topics[jobContextMessage.TopicPointer].Tasks)
            {
                if (taskItem.SupportsParallelExecution)
                {
                    Parallel.ForEach(
                        taskItem.Tasks,
                        new ParallelOptions { CancellationToken = cancellationToken },
                        async task => { await GenerateReportAsync(task, jobContextMessage); });
                }
                else
                {
                    foreach (string task in taskItem.Tasks)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            break;
                        }

                        await GenerateReportAsync(task, jobContextMessage);
                    }
                }
            }

            return true;
        }

        private async Task GenerateReportAsync(string task, IJobContextMessage jobContextMessage)
        {
            if (!reportsAvailable.TryGetValue(task, out var reportType))
            {
                _logger.LogDebug($"Unknown report task '{task}'");
                return;
            }

            IReport report = _reports.Single(x => x.ReportType == reportType);

            if (report == null)
            {
                _logger.LogDebug($"Unknown to find report '{task}'");
                return;
            }

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            _logger.LogDebug($"Attempting to generate {report.GetType().Name}");
            await report.GenerateReport(jobContextMessage);
            stopWatch.Stop();
            _logger.LogDebug($"Persisted {report.GetType().Name} to csv/json in: {stopWatch.ElapsedMilliseconds}");
        }
    }
}
