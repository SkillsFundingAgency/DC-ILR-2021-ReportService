using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.FundingService.FM25.Model.Output;
using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Interface.Provider;
using ESFA.DC.ILR.ReportService.Interface.Service;
using ESFA.DC.ILR.ReportService.Model.Lars;
using ESFA.DC.ILR.ReportService.Model.ReportModels;
using ESFA.DC.ILR.ReportService.Service.Comparer;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Mapper;
using ESFA.DC.ILR.ReportService.Service.Reports.Abstract;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ReportTaskNameConstants = ESFA.DC.ILR.ReportService.Interface.ReportTaskNameConstants;

namespace ESFA.DC.ILR.ReportService.Service.Reports
{
    public sealed class MainOccupancyReport : AbstractLegacyReport
    {
        private static readonly MainOccupancyModelComparer MainOccupancyModelComparer = new MainOccupancyModelComparer();

        private readonly IIlrProviderService _ilrProviderService;
        private readonly IStringUtilitiesService _stringUtilitiesService;
        private readonly IValidLearnersService _validLearnersService;
        private readonly IFM25ProviderService _fm25ProviderService;
        private readonly IFM35ProviderService _fm35ProviderService;
        private readonly ILarsProviderService _larsProviderService;
        private readonly IMainOccupancyReportModelBuilder _mainOccupancyReportModelBuilder;

        public MainOccupancyReport(
            ILogger logger,
            IStreamableKeyValuePersistenceService streamableKeyValuePersistenceService,
            IIlrProviderService ilrProviderService,
            IStringUtilitiesService stringUtilitiesService,
            IValidLearnersService validLearnersService,
            IFM25ProviderService fm25ProviderService,
            IFM35ProviderService fm35ProviderService,
            ILarsProviderService larsProviderService,
            IDateTimeProvider dateTimeProvider,
            IValueProvider valueProvider,
            IMainOccupancyReportModelBuilder mainOccupancyReportModelBuilder)
        : base(dateTimeProvider, valueProvider, streamableKeyValuePersistenceService, logger)
        {
            _ilrProviderService = ilrProviderService;
            _stringUtilitiesService = stringUtilitiesService;
            _validLearnersService = validLearnersService;
            _fm25ProviderService = fm25ProviderService;
            _fm35ProviderService = fm35ProviderService;
            _larsProviderService = larsProviderService;
            _mainOccupancyReportModelBuilder = mainOccupancyReportModelBuilder;
        }

        public override string ReportFileName => "Main Occupancy Report";

        public override string ReportTaskName => ReportTaskNameConstants.MainOccupancyReport;

        public override async Task GenerateReport(IReportServiceContext reportServiceContext, ZipArchive archive, bool isFis, CancellationToken cancellationToken)
        {
            Task<IMessage> ilrFileTask = _ilrProviderService.GetIlrFile(reportServiceContext, cancellationToken);
            Task<FM25Global> fm25Task = _fm25ProviderService.GetFM25Data(reportServiceContext, cancellationToken);
            Task<FM35Global> fm35Task = _fm35ProviderService.GetFM35Data(reportServiceContext, cancellationToken);
            Task<List<string>> validLearnersTask = _validLearnersService.GetLearnersAsync(reportServiceContext, cancellationToken);

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

            Task<Dictionary<string, LarsLearningDelivery>> larsLearningDeliveriesTask = _larsProviderService.GetLearningDeliveriesAsync(learnAimRefs, cancellationToken);
            Task<List<LearnerAndDeliveries>> larsFrameworkAimsTask = _larsProviderService.GetFrameworkAimsAsync(learnAimRefs, learners, cancellationToken);

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

            List<string> larsErrors = new List<string>();

            List<MainOccupancyModel> mainOccupancyModels = new List<MainOccupancyModel>();
            var fm25Data = fm25Task.Result;
            var fm35Data = fm35Task.Result;
            foreach (var learner in learners)
            {
                if (learner.LearningDeliveries == null)
                {
                    continue;
                }

                var validLearningDeliveries = learner.LearningDeliveries.Where(CheckIsApplicableLearner).ToList();
                if (!validLearningDeliveries.Any())
                {
                    continue;
                }

                var learnerFm25Data = fm25Data?.Learners?.SingleOrDefault(l => string.Equals(l.LearnRefNumber, learner.LearnRefNumber, StringComparison.OrdinalIgnoreCase));
                if (learnerFm25Data != null)
                {
                    var fundModel = validLearningDeliveries.SingleOrDefault(x => x.LearnAimRef == learnerFm25Data.LearnRefNumber)?.FundModel ?? 25;
                    mainOccupancyModels.Add(_mainOccupancyReportModelBuilder.BuildFm25Model(learner, learnerFm25Data, fundModel));
                }

                var fm35LearningDeliveries = validLearningDeliveries.Where(x => x.FundModel == 35);
                foreach (ILearningDelivery fm35LearningDelivery in fm35LearningDeliveries)
                {
                    if (!larsLearningDeliveriesTask.Result.TryGetValue(fm35LearningDelivery.LearnAimRef, out LarsLearningDelivery larsModel))
                    {
                        larsErrors.Add(fm35LearningDelivery.LearnAimRef);
                        continue;
                    }

                    var frameworkAim = larsFrameworkAimsTask.Result?.SingleOrDefault(x => string.Equals(x.LearnerLearnRefNumber, learner.LearnRefNumber, StringComparison.OrdinalIgnoreCase))
                        ?.LearningDeliveries?.SingleOrDefault(x =>
                            string.Equals(x.LearningDeliveryLearnAimRef, fm35LearningDelivery.LearnAimRef, StringComparison.OrdinalIgnoreCase)
                            && x.LearningDeliveryAimSeqNumber == fm35LearningDelivery.AimSeqNumber);
                    if (frameworkAim == null)
                    {
                        larsErrors.Add(fm35LearningDelivery.LearnAimRef);
                        continue;
                    }

                    var learnerFm35Data = fm35Data
                        ?.Learners?.SingleOrDefault(l => string.Equals(l.LearnRefNumber, learner.LearnRefNumber, StringComparison.OrdinalIgnoreCase))
                        ?.LearningDeliveries
                        ?.SingleOrDefault(l => l.AimSeqNumber == fm35LearningDelivery.AimSeqNumber);

                    if (learnerFm35Data != null)
                    {
                        mainOccupancyModels.Add(_mainOccupancyReportModelBuilder.BuildFm35Model(
                            learner,
                            fm35LearningDelivery,
                            larsModel,
                            frameworkAim,
                            learnerFm35Data,
                            _stringUtilitiesService));
                    }
                }
            }

            LogWarnings(larsErrors);

            mainOccupancyModels.Sort(MainOccupancyModelComparer);

            string csv = GetReportCsv(mainOccupancyModels);

            var externalFileName = GetFilename(reportServiceContext);
            var fileName = GetZipFilename(reportServiceContext);

            await _streamableKeyValuePersistenceService.SaveAsync($"{externalFileName}.csv", csv, cancellationToken);
            await WriteZipEntry(archive, $"{fileName}.csv", csv);
        }

        private bool CheckIsApplicableLearner(ILearningDelivery learningDelivery)
        {
            return learningDelivery.FundModel == 35
                    || (learningDelivery.FundModel == 25 && learningDelivery.LearningDeliveryFAMs.Any(fam =>
                              string.Equals(fam.LearnDelFAMType, "SOF", StringComparison.OrdinalIgnoreCase) &&
                              string.Equals(fam.LearnDelFAMCode, "105", StringComparison.OrdinalIgnoreCase)));
        }

        private string GetReportCsv(List<MainOccupancyModel> mainOccupancyModels)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                UTF8Encoding utF8Encoding = new UTF8Encoding(false, true);
                using (TextWriter textWriter = new StreamWriter(ms, utF8Encoding))
                {
                    using (CsvWriter csvWriter = new CsvWriter(textWriter))
                    {
                        WriteCsvRecords<MainOccupancyMapper, MainOccupancyModel>(csvWriter, mainOccupancyModels);
                        csvWriter.Flush();
                        textWriter.Flush();
                        return Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
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
