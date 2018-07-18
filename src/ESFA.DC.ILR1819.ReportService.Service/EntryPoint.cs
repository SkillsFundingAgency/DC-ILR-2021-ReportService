using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ValidationErrors.Interface.Models;
using ESFA.DC.ILR1819.ReportService.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Configuration;
using ESFA.DC.ILR1819.ReportService.Interface.Reports;
using ESFA.DC.JobContext;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.ILR1819.ReportService.Service
{
    public sealed class EntryPoint
    {
        private readonly ILogger _logger;
        private readonly ITopicAndTaskSectionOptions _topicAndTaskSectionOptions;
        private readonly IValidationErrorsReport _validationErrorsReport;
        private readonly IAllbOccupancyReport _albAllbOccupancyReport;

        public EntryPoint(
            ILogger logger,
            ITopicAndTaskSectionOptions topicAndTaskSectionOptions,
            IValidationErrorsReport validationErrorsReport,
            IAllbOccupancyReport albAllbOccupancyReport)
        {
            _logger = logger;
            _topicAndTaskSectionOptions = topicAndTaskSectionOptions;
            _validationErrorsReport = validationErrorsReport;
            _albAllbOccupancyReport = albAllbOccupancyReport;
        }

        public async Task<bool> Callback(JobContextMessage jobContextMessage, CancellationToken cancellationToken)
        {
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

        private async Task GenerateReportAsync(string task, JobContextMessage jobContextMessage)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            if (task == _topicAndTaskSectionOptions.TopicReports_TaskGenerateValidationReport)
            {
                List<ValidationErrorDto> validationErrorDtos =
                    await _validationErrorsReport.ReadAndDeserialiseValidationErrorsAsync(jobContextMessage);
                await _validationErrorsReport.PeristValuesToStorage(jobContextMessage, validationErrorDtos);
                _logger.LogDebug($"Persisted validation errors to csv in: {stopWatch.ElapsedMilliseconds}");
            }
            else if (task == _topicAndTaskSectionOptions.TopicReports_TaskGenerateAllbOccupancyReport)
            {
                await _albAllbOccupancyReport.GenerateReport(jobContextMessage);
                _logger.LogDebug($"Persisted Alb occupancy report to csv in: {stopWatch.ElapsedMilliseconds}");
            }

            stopWatch.Stop();
        }
    }
}
