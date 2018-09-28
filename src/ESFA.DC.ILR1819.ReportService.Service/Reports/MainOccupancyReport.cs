using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.FundingService.FM25.Model.Output;
using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR1819.ReportService.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Configuration;
using ESFA.DC.ILR1819.ReportService.Interface.Reports;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.Lars;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;
using ESFA.DC.ILR1819.ReportService.Service.Mapper;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.Logging.Interfaces;
using LearningDelivery = ESFA.DC.ILR1819.ReportService.Model.Lars.LearningDelivery;

namespace ESFA.DC.ILR1819.ReportService.Service.Reports
{
    public sealed class MainOccupancyReport : AbstractReportBuilder, IReport
    {
        private readonly ILogger _logger;
        private readonly IKeyValuePersistenceService _storage;
        private readonly IIlrProviderService _ilrProviderService;
        private readonly IStringUtilitiesService _stringUtilitiesService;
        private readonly IValidLearnersService _validLearnersService;
        private readonly IFM25ProviderService _fm25ProviderService;
        private readonly IFM35ProviderService _fm35ProviderService;
        private readonly ILarsProviderService _larsProviderService;
        private readonly IMainOccupancyReportModelBuilder _mainOccupancyReportModelBuilder;

        public MainOccupancyReport(
            ILogger logger,
            [KeyFilter(PersistenceStorageKeys.Blob)] IKeyValuePersistenceService storage,
            IIlrProviderService ilrProviderService,
            IStringUtilitiesService stringUtilitiesService,
            IValidLearnersService validLearnersService,
            IFM25ProviderService fm25ProviderService,
            IFM35ProviderService fm35ProviderService,
            ILarsProviderService larsProviderService,
            IDateTimeProvider dateTimeProvider,
            ITopicAndTaskSectionOptions topicAndTaskSectionOptions,
            IMainOccupancyReportModelBuilder mainOccupancyReportModelBuilder)
        : base(dateTimeProvider)
        {
            _logger = logger;
            _storage = storage;
            _ilrProviderService = ilrProviderService;
            _stringUtilitiesService = stringUtilitiesService;
            _validLearnersService = validLearnersService;
            _fm25ProviderService = fm25ProviderService;
            _fm35ProviderService = fm35ProviderService;
            _larsProviderService = larsProviderService;
            _mainOccupancyReportModelBuilder = mainOccupancyReportModelBuilder;

            ReportFileName = "Main Occupancy Report";
            ReportTaskName = topicAndTaskSectionOptions.TopicReports_TaskGenerateMainOccupancyReport;
        }

        public async Task GenerateReport(IJobContextMessage jobContextMessage, ZipArchive archive, CancellationToken cancellationToken)
        {
            Task<IMessage> ilrFileTask = _ilrProviderService.GetIlrFile(jobContextMessage, cancellationToken);
            Task<FM25Global> fm25Task = _fm25ProviderService.GetFM25Data(jobContextMessage, cancellationToken);
            Task<FM35Global> fm35Task = _fm35ProviderService.GetFM35Data(jobContextMessage, cancellationToken);
            Task<List<string>> validLearnersTask = _validLearnersService.GetLearnersAsync(jobContextMessage, cancellationToken);

            await Task.WhenAll(ilrFileTask, fm25Task, fm35Task, validLearnersTask);

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            List<ILearner> learners =
                ilrFileTask.Result?.Learners?.Where(x => validLearnersTask.Result.Contains(x.LearnRefNumber)).ToList();
            if (learners == null)
            {
                _logger.LogWarning("Failed to get learners for Main Occupancy Report");
                return;
            }

            string[] learnAimRefs = learners.SelectMany(x => x.LearningDeliveries).Select(x => x.LearnAimRef).Distinct().ToArray();

            Task<Dictionary<string, LarsLearningDelivery>> larsLearningDeliveriesTask = _larsProviderService.GetLearningDeliveries(learnAimRefs, cancellationToken);
            Task<List<LearnerAndDeliveries>> larsFrameworkAimsTask = _larsProviderService.GetFrameworkAims(learnAimRefs, learners, cancellationToken);

            await Task.WhenAll(larsLearningDeliveriesTask, larsFrameworkAimsTask);

            if (larsLearningDeliveriesTask.Result == null || larsFrameworkAimsTask.Result == null)
            {
                _logger.LogWarning("Failed to get LARS data for Main Occupancy Report");
                return;
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            var larsErrors = new List<string>();

            var mainOccupancyFm25Models = new List<MainOccupancyFM25Model>();
            var mainOccupancyFm35Models = new List<MainOccupancyFM35Model>();
            foreach (var learner in learners)
            {
                var fm25Data = fm25Task.Result;
                var fm35Data = fm35Task.Result;

                if (learner.LearningDeliveries == null)
                {
                    continue;
                }

                foreach (ILearningDelivery learningDelivery in learner.LearningDeliveries)
                {
                    if (!CheckIsApplicableLearner(learningDelivery))
                    {
                        continue;
                    }

                    if (!larsLearningDeliveriesTask.Result.TryGetValue(learningDelivery.LearnAimRef, out LarsLearningDelivery larsModel))
                    {
                        larsErrors.Add(learningDelivery.LearnAimRef);
                        continue;
                    }

                    LearningDelivery frameworkAim = larsFrameworkAimsTask.Result?.SingleOrDefault(x => x.LearnerLearnRefNumber == learner.LearnRefNumber)
                        ?.LearningDeliveries?.SingleOrDefault(x =>
                            x.LearningDeliveryLearnAimRef == learningDelivery.LearnAimRef && x.LearningDeliveryAimSeqNumber == learningDelivery.AimSeqNumber);
                    if (frameworkAim == null)
                    {
                        larsErrors.Add(learningDelivery.LearnAimRef);
                        continue;
                    }

                    var learnerFm35Data = fm35Data?.Learners?.SingleOrDefault(l => l.LearnRefNumber == learner.LearnRefNumber)
                        ?.LearningDeliveries?.SingleOrDefault(l => l.AimSeqNumber == learningDelivery.AimSeqNumber);

                    mainOccupancyFm35Models.Add(_mainOccupancyReportModelBuilder.BuildFm35Model(
                        learner,
                        learningDelivery,
                        larsModel,
                        frameworkAim,
                        learnerFm35Data));

                    var learnerFm25Data =
                        fm25Data?.Learners?.SingleOrDefault(l => l.LearnRefNumber == learner.LearnRefNumber);

                    mainOccupancyFm25Models.Add(_mainOccupancyReportModelBuilder.BuildFm25Model(
                        learner,
                        learningDelivery,
                        learnerFm25Data));
                }
            }

            LogWarnings(larsErrors);

            string csv = GetReportCsv(mainOccupancyFm25Models, mainOccupancyFm35Models);

            var jobId = jobContextMessage.JobId;
            var ukPrn = jobContextMessage.KeyValuePairs[JobContextMessageKey.UkPrn].ToString();
            var externalFileName = GetExternalFilename(ukPrn, jobId, jobContextMessage.SubmissionDateTimeUtc);
            var fileName = GetFilename(ukPrn, jobId, jobContextMessage.SubmissionDateTimeUtc);

            await _storage.SaveAsync($"{externalFileName}.csv", csv, cancellationToken);
            await WriteZipEntry(archive, $"{fileName}.csv", csv);
        }

        private bool CheckIsApplicableLearner(ILearningDelivery learningDelivery)
        {
            return learningDelivery.FundModel == 35
                      || (learningDelivery.FundModel == 25 && learningDelivery.LearningDeliveryFAMs.Any(fam => fam.LearnDelFAMType == "SOF" && fam.LearnDelFAMCode == "105"));
        }

        private string GetReportCsv(List<MainOccupancyFM25Model> mainOccupancyFm25Models, List<MainOccupancyFM35Model> mainOccupancyFm35Models)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BuildCsvReport<MainOccupancyFM25Mapper, MainOccupancyFM25Model>(ms, mainOccupancyFm25Models);
                BuildCsvReport<MainOccupancyFM35Mapper, MainOccupancyFM35Model>(ms, mainOccupancyFm35Models);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        private void LogWarnings(List<string> larsErrors)
        {
            if (larsErrors.Any())
            {
                _logger.LogWarning($"Failed to get LARS data while generating Main Occupancy Report: {_stringUtilitiesService.JoinWithMaxLength(larsErrors)}");
            }
        }
    }
}
