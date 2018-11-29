using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.FundingService.FM81.FundingOutput.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Builders;
using ESFA.DC.ILR1819.ReportService.Interface.Configuration;
using ESFA.DC.ILR1819.ReportService.Interface.Reports;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.Lars;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;
using ESFA.DC.ILR1819.ReportService.Service.Comparer;
using ESFA.DC.ILR1819.ReportService.Service.Helper;
using ESFA.DC.ILR1819.ReportService.Service.Mapper;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.JobContextManager.Model.Interface;
using ESFA.DC.Logging.Interfaces;
using Microsoft.EntityFrameworkCore.Internal;

namespace ESFA.DC.ILR1819.ReportService.Service.Reports
{
    public sealed class TrailblazerEmployerIncentivesReport : AbstractReportBuilder, IReport
    {
        private readonly ILogger _logger;
        private readonly IStreamableKeyValuePersistenceService _storage;
        private readonly IFM81TrailBlazerProviderService _fm81TrailBlazerProviderService;
        private readonly IIlrProviderService _ilrProviderService;
        private readonly IValidLearnersService _validLearnersService;

        private readonly ITrailblazerEmployerIncentivesModelBuilder _trailblazerEmployerIncentivesModelBuilder;

        public TrailblazerEmployerIncentivesReport(
            ILogger logger,
            IStreamableKeyValuePersistenceService storage,
            IFM81TrailBlazerProviderService fm81TrailBlazerProviderService,
            IIlrProviderService ilrProviderService,
            IValidLearnersService validLearnersService,
            ITrailblazerEmployerIncentivesModelBuilder trailblazerEmployerIncentivesModelBuilder,
            ITopicAndTaskSectionOptions topicAndTaskSectionOptions,
            IValueProvider valueProvider,
            IDateTimeProvider dateTimeProvider)
            : base(dateTimeProvider, valueProvider)
        {
            _logger = logger;
            _storage = storage;

            _fm81TrailBlazerProviderService = fm81TrailBlazerProviderService;
            _validLearnersService = validLearnersService;
            _ilrProviderService = ilrProviderService;

            _trailblazerEmployerIncentivesModelBuilder = trailblazerEmployerIncentivesModelBuilder;

            ReportFileName = "Trailblazer Apprenticeships Employer Incentives Report";
            ReportTaskName = topicAndTaskSectionOptions.TopicReports_TaskGenerateTrailblazerEmployerIncentivesReport;
        }

        public async Task GenerateReport(IJobContextMessage jobContextMessage, ZipArchive archive, bool isFis, CancellationToken cancellationToken)
        {
            Task<IMessage> ilrFileTask = _ilrProviderService.GetIlrFile(jobContextMessage, cancellationToken);
            Task<FM81Global> fm81Task =
                _fm81TrailBlazerProviderService.GetFM81Data(jobContextMessage, cancellationToken);
            Task<List<string>> validLearnersTask =
                _validLearnersService.GetLearnersAsync(jobContextMessage, cancellationToken);

            await Task.WhenAll(ilrFileTask, fm81Task, validLearnersTask);

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            List<ILearner> learners =
                ilrFileTask.Result?.Learners?.Where(x => validLearnersTask.Result.Contains(x.LearnRefNumber)).ToList();

            if (learners == null)
            {
                _logger.LogWarning("Failed to get learners for Trailblazer Employer Incentives Report");
                return;
            }

            var fm81Data = fm81Task.Result;
            var trailblazerEmployerIncentivesModels = new List<TrailblazerEmployerIncentivesModel>();

            var fm81EmployerIdentifierList = fm81Data?.Learners?.Select(l => l.LearningDeliveries?.Select(x =>
                string.Join(",", x.LearningDeliveryValues.EmpIdSmallBusDate, x.LearningDeliveryValues.EmpIdFirstYoungAppDate, x.LearningDeliveryValues.EmpIdSecondYoungAppDate, x.LearningDeliveryValues.EmpIdAchDate))).FirstOrDefault();

            if (fm81EmployerIdentifierList != null)
            {
                var fm81EmployerIdentifierUniqueList = fm81EmployerIdentifierList.First().Split(',').ToArray()
                    .Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();

                foreach (string empIdentifier in fm81EmployerIdentifierUniqueList)
                {
                    var learnerFm81Data = fm81Data?.Learners?.SelectMany(x => x.LearningDeliveries).ToList();

                    trailblazerEmployerIncentivesModels.Add(
                        _trailblazerEmployerIncentivesModelBuilder.BuildTrailblazerEmployerIncentivesModel(
                            Convert.ToInt32(empIdentifier),
                            null,
                            learnerFm81Data));
                }
            }

            trailblazerEmployerIncentivesModels.Sort(new TrailblazerEmployerIncentivesModelComparer());

            var csv = GetReportCsv(trailblazerEmployerIncentivesModels);

            var jobId = jobContextMessage.JobId;
            var ukPrn = jobContextMessage.KeyValuePairs[JobContextMessageKey.UkPrn].ToString();
            var externalFileName = GetExternalFilename(ukPrn, jobId, jobContextMessage.SubmissionDateTimeUtc);
            var fileName = GetFilename(ukPrn, jobId, jobContextMessage.SubmissionDateTimeUtc);

            await _storage.SaveAsync($"{externalFileName}.csv", csv, cancellationToken);
            await WriteZipEntry(archive, $"{fileName}.csv", csv);
        }

        private bool CheckIsApplicableLearner(ILearningDelivery learningDelivery)
        {
            return learningDelivery.FundModel == 81;
        }

        private string GetReportCsv(List<TrailblazerEmployerIncentivesModel> listModels)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                UTF8Encoding utF8Encoding = new UTF8Encoding(false, true);
                using (TextWriter textWriter = new StreamWriter(ms, utF8Encoding))
                {
                    using (CsvWriter csvWriter = new CsvWriter(textWriter))
                    {
                        WriteCsvRecords<TrailblazerEmployerIncentiveMapper, TrailblazerEmployerIncentivesModel>(csvWriter, listModels);
                        csvWriter.Flush();
                        textWriter.Flush();
                        return Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
            }
        }
    }
}