using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.FundingService.ALB.FundingOutput.Model;
using ESFA.DC.ILR.FundingService.ALB.FundingOutput.Model.Attribute;
using ESFA.DC.ILR.FundingService.FM25.Model.Output;
using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR1819.ReportService.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Configuration;
using ESFA.DC.ILR1819.ReportService.Interface.Reports;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;
using ESFA.DC.ILR1819.ReportService.Service.Mapper;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.ILR1819.ReportService.Service.Reports
{
    public sealed class FundingSummaryReport : AbstractReportBuilder, IReport
    {
        private const string AlbSupportPayment = "ALBSupportPayment";

        private const string AlbAreaUpliftBalPayment = "AreaUpliftBalPayment";

        private const string AlbAreaUpliftOnProgPayment = "AreaUpliftOnProgPayment";

        private readonly ILogger _logger;
        private readonly IKeyValuePersistenceService _storage;
        private readonly IIlrProviderService _ilrProviderService;
        private readonly IOrgProviderService _orgProviderService;
        private readonly IAllbProviderService _allbProviderService;
        private readonly IFM25ProviderService _fm25ProviderService;
        private readonly IFM35ProviderService _fm35ProviderService;
        private readonly IValidLearnersService _validLearnersService;
        private readonly IStringUtilitiesService _stringUtilitiesService;
        private readonly IPeriodProviderService _periodProviderService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ILarsProviderService _larsProviderService;
        private readonly IVersionInfo _versionInfo;

        public FundingSummaryReport(
            ILogger logger,
            [KeyFilter(PersistenceStorageKeys.Blob)] IKeyValuePersistenceService storage,
            IIlrProviderService ilrProviderService,
            IOrgProviderService orgProviderService,
            IAllbProviderService allbProviderService,
            IFM25ProviderService fm25ProviderService,
            IFM35ProviderService fm35ProviderService,
            IValidLearnersService validLearnersService,
            IStringUtilitiesService stringUtilitiesService,
            IPeriodProviderService periodProviderService,
            IDateTimeProvider dateTimeProvider,
            ILarsProviderService larsProviderService,
            IVersionInfo versionInfo,
            ITopicAndTaskSectionOptions topicAndTaskSectionOptions)
            : base(dateTimeProvider)
        {
            _logger = logger;
            _storage = storage;
            _ilrProviderService = ilrProviderService;
            _orgProviderService = orgProviderService;
            _allbProviderService = allbProviderService;
            _fm25ProviderService = fm25ProviderService;
            _fm35ProviderService = fm35ProviderService;
            _validLearnersService = validLearnersService;
            _stringUtilitiesService = stringUtilitiesService;
            _periodProviderService = periodProviderService;
            _larsProviderService = larsProviderService;
            _versionInfo = versionInfo;
            _dateTimeProvider = dateTimeProvider;

            ReportFileName = "Funding Summary Report";
            ReportTaskName = topicAndTaskSectionOptions.TopicReports_TaskGenerateFundingSummaryReport;
        }

        // Todo : Fm25 + Fm35
        public async Task GenerateReport(IJobContextMessage jobContextMessage, ZipArchive archive, CancellationToken cancellationToken)
        {
            Task<IMessage> ilrFileTask = _ilrProviderService.GetIlrFile(jobContextMessage, cancellationToken);
            Task<FundingOutputs> albDataTask = _allbProviderService.GetAllbData(jobContextMessage, cancellationToken);
            Task<Global> fm25Task = _fm25ProviderService.GetFM25Data(jobContextMessage, cancellationToken);
            Task<FM35FundingOutputs> fm35Task = _fm35ProviderService.GetFM35Data(jobContextMessage, cancellationToken);
            Task<List<string>> validLearnersTask = _validLearnersService.GetLearnersAsync(jobContextMessage, cancellationToken);
            Task<string> providerNameTask = _orgProviderService.GetProviderName(jobContextMessage, cancellationToken);
            Task<int> periodTask = _periodProviderService.GetPeriod(jobContextMessage);

            await Task.WhenAll(ilrFileTask, albDataTask, fm25Task, fm35Task, validLearnersTask, providerNameTask, periodTask);

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            List<string> ilrError = new List<string>();
            List<string> albLearnerError = new List<string>();

            FundingSummaryModel fundingSummaryModelAlbFunding = new FundingSummaryModel()
            {
                Title = "ILR Advanced Loans Bursary Funding (£)"
            };

            FundingSummaryModel fundingSummaryModelAlbAreaCosts = new FundingSummaryModel()
            {
                Title = "ILR Advanced Loans Bursary Area Costs (£)"
            };

            List<FundingSummaryModel> fundingSummaryModels = new List<FundingSummaryModel>()
            {
                fundingSummaryModelAlbFunding,
                fundingSummaryModelAlbAreaCosts
            };

            foreach (string validLearnerRefNum in validLearnersTask.Result)
            {
                var learner = ilrFileTask.Result?.Learners?.SingleOrDefault(x => x.LearnRefNumber == validLearnerRefNum);
                if (learner == null)
                {
                    ilrError.Add(validLearnerRefNum);
                    continue;
                }

                TotalAlb(albDataTask, validLearnerRefNum, albLearnerError, fundingSummaryModelAlbFunding, periodTask, fundingSummaryModelAlbAreaCosts);
            }

            FundingSummaryHeaderModel fundingSummaryHeaderModel = GetHeader(jobContextMessage, ilrFileTask, providerNameTask);
            FundingSummaryFooterModel fundingSummaryFooterModel = await GetFooterAsync(ilrFileTask, cancellationToken);

            LogWarnings(ilrError, albLearnerError);

            var jobId = jobContextMessage.JobId;
            var ukPrn = jobContextMessage.KeyValuePairs[JobContextMessageKey.UkPrn].ToString();
            var externalFileName = GetExternalFilename(ukPrn, jobId, jobContextMessage.SubmissionDateTimeUtc);
            var fileName = GetFilename(ukPrn, jobId, jobContextMessage.SubmissionDateTimeUtc);

            string csv = GetReportCsv(fundingSummaryModels, fundingSummaryHeaderModel, fundingSummaryFooterModel);
            await _storage.SaveAsync($"{externalFileName}.csv", csv, cancellationToken);
            await WriteZipEntry(archive, $"{fileName}.csv", csv);
        }

        private void TotalAlb(
            Task<FundingOutputs> albDataTask,
            string validLearnerRefNum,
            List<string> albLearnerError,
            FundingSummaryModel fundingSummaryModelAlbFunding,
            Task<int> periodTask,
            FundingSummaryModel fundingSummaryModelAlbAreaCosts)
        {
            LearnerAttribute albLearner =
                albDataTask.Result?.Learners?.SingleOrDefault(x => x.LearnRefNumber == validLearnerRefNum);
            if (albLearner == null)
            {
                albLearnerError.Add(validLearnerRefNum);
                return;
            }

            var albSupportPaymentObj =
                albLearner.LearnerPeriodisedAttributes.SingleOrDefault(x => x.AttributeName == AlbSupportPayment);
            var albAreaUpliftOnProgPaymentObj =
                albLearner.LearnerPeriodisedAttributes.SingleOrDefault(x => x.AttributeName == AlbAreaUpliftOnProgPayment);
            var albAreaUpliftBalPaymentObj =
                albLearner.LearnerPeriodisedAttributes.SingleOrDefault(x => x.AttributeName == AlbAreaUpliftBalPayment);

            fundingSummaryModelAlbFunding.Period1 =
                fundingSummaryModelAlbFunding.Period1 + albSupportPaymentObj?.Period1 ?? 0;
            fundingSummaryModelAlbFunding.Period2 =
                fundingSummaryModelAlbFunding.Period2 + albSupportPaymentObj?.Period2 ?? 0;
            fundingSummaryModelAlbFunding.Period3 =
                fundingSummaryModelAlbFunding.Period3 + albSupportPaymentObj?.Period3 ?? 0;
            fundingSummaryModelAlbFunding.Period4 =
                fundingSummaryModelAlbFunding.Period1 + albSupportPaymentObj?.Period4 ?? 0;
            fundingSummaryModelAlbFunding.Period5 =
                fundingSummaryModelAlbFunding.Period5 + albSupportPaymentObj?.Period5 ?? 0;
            fundingSummaryModelAlbFunding.Period6 =
                fundingSummaryModelAlbFunding.Period6 + albSupportPaymentObj?.Period6 ?? 0;
            fundingSummaryModelAlbFunding.Period7 =
                fundingSummaryModelAlbFunding.Period7 + albSupportPaymentObj?.Period7 ?? 0;
            fundingSummaryModelAlbFunding.Period8 =
                fundingSummaryModelAlbFunding.Period8 + albSupportPaymentObj?.Period8 ?? 0;
            fundingSummaryModelAlbFunding.Period9 =
                fundingSummaryModelAlbFunding.Period9 + albSupportPaymentObj?.Period9 ?? 0;
            fundingSummaryModelAlbFunding.Period10 =
                fundingSummaryModelAlbFunding.Period10 + albSupportPaymentObj?.Period10 ?? 0;
            fundingSummaryModelAlbFunding.Period11 =
                fundingSummaryModelAlbFunding.Period11 + albSupportPaymentObj?.Period11 ?? 0;
            fundingSummaryModelAlbFunding.Period12 =
                fundingSummaryModelAlbFunding.Period12 + albSupportPaymentObj?.Period12 ?? 0;
            fundingSummaryModelAlbFunding.Period1_8 =
                fundingSummaryModelAlbFunding.Period1_8 + (albSupportPaymentObj?.Period1 ?? 0) +
                (albSupportPaymentObj?.Period2 ?? 0) +
                (albSupportPaymentObj?.Period3 ?? 0) + (albSupportPaymentObj?.Period4 ?? 0) +
                (albSupportPaymentObj?.Period5 ?? 0) + (albSupportPaymentObj?.Period6 ?? 0) +
                (albSupportPaymentObj?.Period7 ?? 0) + (albSupportPaymentObj?.Period8 ?? 0);
            fundingSummaryModelAlbFunding.Period9_12 =
                fundingSummaryModelAlbFunding.Period9_12 + (albSupportPaymentObj?.Period9 ?? 0) +
                (albSupportPaymentObj?.Period10 ?? 0) +
                (albSupportPaymentObj?.Period11 ?? 0) + (albSupportPaymentObj?.Period12 ?? 0);
            fundingSummaryModelAlbFunding.YearToDate = fundingSummaryModelAlbFunding.YearToDate +
                                                       GetYearToDateTotal(albSupportPaymentObj, periodTask.Result);
            fundingSummaryModelAlbFunding.Total =
                fundingSummaryModelAlbFunding.Total + (albSupportPaymentObj?.Period1 ?? 0) +
                (albSupportPaymentObj?.Period2 ?? 0) +
                (albSupportPaymentObj?.Period3 ?? 0) + (albSupportPaymentObj?.Period4 ?? 0) +
                (albSupportPaymentObj?.Period5 ?? 0) + (albSupportPaymentObj?.Period6 ?? 0) +
                (albSupportPaymentObj?.Period7 ?? 0) + (albSupportPaymentObj?.Period8 ?? 0) +
                (albSupportPaymentObj?.Period9 ?? 0) + (albSupportPaymentObj?.Period10 ?? 0) +
                (albSupportPaymentObj?.Period11 ?? 0) + (albSupportPaymentObj?.Period12 ?? 0);

            fundingSummaryModelAlbAreaCosts.Period1 = fundingSummaryModelAlbAreaCosts.Period1 +
                                                      (albAreaUpliftBalPaymentObj?.Period1 ?? 0) +
                                                      (albAreaUpliftOnProgPaymentObj?.Period1 ?? 0);
            fundingSummaryModelAlbAreaCosts.Period2 = fundingSummaryModelAlbAreaCosts.Period2 +
                                                      (albAreaUpliftBalPaymentObj?.Period2 ?? 0) +
                                                      (albAreaUpliftOnProgPaymentObj?.Period2 ?? 0);
            fundingSummaryModelAlbAreaCosts.Period3 = fundingSummaryModelAlbAreaCosts.Period3 +
                                                      (albAreaUpliftBalPaymentObj?.Period3 ?? 0) +
                                                      (albAreaUpliftOnProgPaymentObj?.Period3 ?? 0);
            fundingSummaryModelAlbAreaCosts.Period4 = fundingSummaryModelAlbAreaCosts.Period4 +
                                                      (albAreaUpliftBalPaymentObj?.Period4 ?? 0) +
                                                      (albAreaUpliftOnProgPaymentObj?.Period4 ?? 0);
            fundingSummaryModelAlbAreaCosts.Period5 = fundingSummaryModelAlbAreaCosts.Period5 +
                                                      (albAreaUpliftBalPaymentObj?.Period5 ?? 0) +
                                                      (albAreaUpliftOnProgPaymentObj?.Period5 ?? 0);
            fundingSummaryModelAlbAreaCosts.Period6 = fundingSummaryModelAlbAreaCosts.Period6 +
                                                      (albAreaUpliftBalPaymentObj?.Period6 ?? 0) +
                                                      (albAreaUpliftOnProgPaymentObj?.Period6 ?? 0);
            fundingSummaryModelAlbAreaCosts.Period7 = fundingSummaryModelAlbAreaCosts.Period7 +
                                                      (albAreaUpliftBalPaymentObj?.Period7 ?? 0) +
                                                      (albAreaUpliftOnProgPaymentObj?.Period7 ?? 0);
            fundingSummaryModelAlbAreaCosts.Period8 = fundingSummaryModelAlbAreaCosts.Period8 +
                                                      (albAreaUpliftBalPaymentObj?.Period8 ?? 0) +
                                                      (albAreaUpliftOnProgPaymentObj?.Period8 ?? 0);
            fundingSummaryModelAlbAreaCosts.Period9 = fundingSummaryModelAlbAreaCosts.Period9 +
                                                      (albAreaUpliftBalPaymentObj?.Period9 ?? 0) +
                                                      (albAreaUpliftOnProgPaymentObj?.Period9 ?? 0);
            fundingSummaryModelAlbAreaCosts.Period10 = fundingSummaryModelAlbAreaCosts.Period10 +
                                                       (albAreaUpliftBalPaymentObj?.Period10 ?? 0) +
                                                       (albAreaUpliftOnProgPaymentObj?.Period10 ?? 0);
            fundingSummaryModelAlbAreaCosts.Period11 = fundingSummaryModelAlbAreaCosts.Period11 +
                                                       (albAreaUpliftBalPaymentObj?.Period11 ?? 0) +
                                                       (albAreaUpliftOnProgPaymentObj?.Period11 ?? 0);
            fundingSummaryModelAlbAreaCosts.Period12 = fundingSummaryModelAlbAreaCosts.Period12 +
                                                       (albAreaUpliftBalPaymentObj?.Period12 ?? 0) +
                                                       (albAreaUpliftOnProgPaymentObj?.Period12 ?? 0);
            fundingSummaryModelAlbAreaCosts.Period9_12 =
                fundingSummaryModelAlbAreaCosts.Period9_12 + (albAreaUpliftBalPaymentObj?.Period9 ?? 0) +
                (albAreaUpliftOnProgPaymentObj?.Period9 ?? 0)
                + (albAreaUpliftBalPaymentObj?.Period10 ?? 0) + (albAreaUpliftOnProgPaymentObj?.Period10 ?? 0)
                + (albAreaUpliftBalPaymentObj?.Period11 ?? 0) + (albAreaUpliftOnProgPaymentObj?.Period11 ?? 0)
                + (albAreaUpliftBalPaymentObj?.Period12 ?? 0) + (albAreaUpliftOnProgPaymentObj?.Period12 ?? 0);
            fundingSummaryModelAlbAreaCosts.Period1_8 =
                fundingSummaryModelAlbAreaCosts.Period1_8 + (albAreaUpliftBalPaymentObj?.Period1 ?? 0) +
                (albAreaUpliftOnProgPaymentObj?.Period1 ?? 0) +
                (albAreaUpliftBalPaymentObj?.Period2 ?? 0) + (albAreaUpliftOnProgPaymentObj?.Period2 ?? 0) +
                (albAreaUpliftBalPaymentObj?.Period3 ?? 0) + (albAreaUpliftOnProgPaymentObj?.Period3 ?? 0) +
                (albAreaUpliftBalPaymentObj?.Period4 ?? 0) + (albAreaUpliftOnProgPaymentObj?.Period4 ?? 0) +
                (albAreaUpliftBalPaymentObj?.Period5 ?? 0) + (albAreaUpliftOnProgPaymentObj?.Period5 ?? 0) +
                (albAreaUpliftBalPaymentObj?.Period6 ?? 0) + (albAreaUpliftOnProgPaymentObj?.Period6 ?? 0) +
                (albAreaUpliftBalPaymentObj?.Period7 ?? 0) + (albAreaUpliftOnProgPaymentObj?.Period7 ?? 0) +
                (albAreaUpliftBalPaymentObj?.Period8 ?? 0) + (albAreaUpliftOnProgPaymentObj?.Period8 ?? 0);
            fundingSummaryModelAlbAreaCosts.YearToDate = fundingSummaryModelAlbAreaCosts.YearToDate +
                                                         GetYearToDateTotal(albAreaUpliftBalPaymentObj, albAreaUpliftOnProgPaymentObj, periodTask.Result);
            fundingSummaryModelAlbAreaCosts.Total =
                fundingSummaryModelAlbAreaCosts.Total + (albAreaUpliftBalPaymentObj?.Period1 ?? 0) +
                (albAreaUpliftOnProgPaymentObj?.Period1 ?? 0) +
                (albAreaUpliftBalPaymentObj?.Period2 ?? 0) + (albAreaUpliftOnProgPaymentObj?.Period2 ?? 0) +
                (albAreaUpliftBalPaymentObj?.Period3 ?? 0) + (albAreaUpliftOnProgPaymentObj?.Period3 ?? 0) +
                (albAreaUpliftBalPaymentObj?.Period4 ?? 0) + (albAreaUpliftOnProgPaymentObj?.Period4 ?? 0) +
                (albAreaUpliftBalPaymentObj?.Period5 ?? 0) + (albAreaUpliftOnProgPaymentObj?.Period5 ?? 0) +
                (albAreaUpliftBalPaymentObj?.Period6 ?? 0) + (albAreaUpliftOnProgPaymentObj?.Period6 ?? 0) +
                (albAreaUpliftBalPaymentObj?.Period7 ?? 0) + (albAreaUpliftOnProgPaymentObj?.Period7 ?? 0) +
                (albAreaUpliftBalPaymentObj?.Period8 ?? 0) + (albAreaUpliftOnProgPaymentObj?.Period8 ?? 0) +
                (albAreaUpliftBalPaymentObj?.Period9 ?? 0) + (albAreaUpliftOnProgPaymentObj?.Period9 ?? 0)
                + (albAreaUpliftBalPaymentObj?.Period10 ?? 0) + (albAreaUpliftOnProgPaymentObj?.Period10 ?? 0)
                + (albAreaUpliftBalPaymentObj?.Period11 ?? 0) + (albAreaUpliftOnProgPaymentObj?.Period11 ?? 0)
                + (albAreaUpliftBalPaymentObj?.Period12 ?? 0) + (albAreaUpliftOnProgPaymentObj?.Period12 ?? 0);
        }

        private string GetReportCsv(List<FundingSummaryModel> fundingSummaryModels, FundingSummaryHeaderModel fundingSummaryHeaderModel, FundingSummaryFooterModel fundingSummaryFooterModel)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BuildCsvReport<FundingSummaryHeaderMapper, FundingSummaryHeaderModel>(ms, fundingSummaryHeaderModel);
                BuildCsvReport<FundingSummaryMapper, FundingSummaryModel>(ms, fundingSummaryModels);
                BuildCsvReport<FundingSummaryFooterMapper, FundingSummaryFooterModel>(ms, fundingSummaryFooterModel);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        private void LogWarnings(List<string> ilrError, List<string> albLearnerError)
        {
            if (ilrError.Any())
            {
                _logger.LogWarning($"Failed to get one or more ILR learners while generating Allb Occupancy Report: {_stringUtilitiesService.JoinWithMaxLength(ilrError)}");
            }

            if (albLearnerError.Any())
            {
                _logger.LogWarning($"Failed to get one or more ALB learners while generating Allb Occupancy Report: {_stringUtilitiesService.JoinWithMaxLength(albLearnerError)}");
            }
        }

        private FundingSummaryHeaderModel GetHeader(IJobContextMessage jobContextMessage, Task<IMessage> messageTask, Task<string> providerNameTask)
        {
            FundingSummaryHeaderModel fundingSummaryHeaderModel = new FundingSummaryHeaderModel
            {
                IlrFile = Path.GetFileName(jobContextMessage.KeyValuePairs[JobContextMessageKey.Filename].ToString()),
                Ukprn = messageTask.Result.HeaderEntity.SourceEntity.UKPRN,
                ProviderName = providerNameTask.Result,
                LastEasUpdate = "Todo", // Todo
                LastIlrFileUpdate = messageTask.Result.HeaderEntity.SourceEntity.DateTime.ToString("dd/MM/yyyy"),
                SecurityClassification = "OFFICIAL-SENSITIVE"
            };

            return fundingSummaryHeaderModel;
        }

        private async Task<FundingSummaryFooterModel> GetFooterAsync(Task<IMessage> messageTask, CancellationToken cancellationToken)
        {
            var dateTimeNowUtc = _dateTimeProvider.GetNowUtc();
            var dateTimeNowUk = _dateTimeProvider.ConvertUtcToUk(dateTimeNowUtc);

            FundingSummaryFooterModel fundingSummaryFooterModel = new FundingSummaryFooterModel
            {
                ReportGeneratedAt = "Report generated at " + dateTimeNowUk.ToString("HH:mm:ss") + " on " + dateTimeNowUk.ToString("dd/MM/yyyy"),
                ApplicationVersion = _versionInfo.ServiceReleaseVersion,
                ComponentSetVersion = "NA",
                FilePreparationDate = messageTask.Result.HeaderEntity.SourceEntity.DateTime.ToString("dd/MM/yyyy"),
                OrganisationData = await _orgProviderService.GetVersionAsync(cancellationToken),
                LargeEmployerData = "Todo", // Todo
                LarsData = await _larsProviderService.GetVersionAsync(cancellationToken),
                PostcodeData = "Todo" // Todo
            };

            return fundingSummaryFooterModel;
        }

        private decimal? GetYearToDateTotal(LearnerPeriodisedAttribute albSupportPaymentObj, int period)
        {
            decimal total = 0;
            for (int i = 0; i < period; i++)
            {
                switch (i)
                {
                    case 0:
                        total += albSupportPaymentObj?.Period1 ?? 0;
                        break;
                    case 1:
                        total += albSupportPaymentObj?.Period2 ?? 0;
                        break;
                    case 2:
                        total += albSupportPaymentObj?.Period3 ?? 0;
                        break;
                    case 3:
                        total += albSupportPaymentObj?.Period4 ?? 0;
                        break;
                    case 4:
                        total += albSupportPaymentObj?.Period5 ?? 0;
                        break;
                    case 5:
                        total += albSupportPaymentObj?.Period6 ?? 0;
                        break;
                    case 6:
                        total += albSupportPaymentObj?.Period7 ?? 0;
                        break;
                    case 7:
                        total += albSupportPaymentObj?.Period8 ?? 0;
                        break;
                    case 8:
                        total += albSupportPaymentObj?.Period9 ?? 0;
                        break;
                    case 9:
                        total += albSupportPaymentObj?.Period10 ?? 0;
                        break;
                    case 10:
                        total += albSupportPaymentObj?.Period11 ?? 0;
                        break;
                    case 11:
                        total += albSupportPaymentObj?.Period12 ?? 0;
                        break;
                }
            }

            return total;
        }

        private decimal? GetYearToDateTotal(LearnerPeriodisedAttribute albAreaUpliftBalPaymentObj, LearnerPeriodisedAttribute albAreaUpliftOnProgPaymentObj, int period)
        {
            decimal total = 0;
            for (int i = 0; i < period; i++)
            {
                switch (i)
                {
                    case 0:
                        total += (albAreaUpliftBalPaymentObj?.Period1 ?? 0) +
                                 (albAreaUpliftOnProgPaymentObj?.Period1 ?? 0);
                        break;
                    case 1:
                        total += (albAreaUpliftBalPaymentObj?.Period2 ?? 0) +
                                 (albAreaUpliftOnProgPaymentObj?.Period2 ?? 0);
                        break;
                    case 2:
                        total += (albAreaUpliftBalPaymentObj?.Period3 ?? 0) +
                                 (albAreaUpliftOnProgPaymentObj?.Period3 ?? 0);
                        break;
                    case 3:
                        total += (albAreaUpliftBalPaymentObj?.Period4 ?? 0) +
                                 (albAreaUpliftOnProgPaymentObj?.Period4 ?? 0);
                        break;
                    case 4:
                        total += (albAreaUpliftBalPaymentObj?.Period5 ?? 0) +
                                 (albAreaUpliftOnProgPaymentObj?.Period5 ?? 0);
                        break;
                    case 5:
                        total += (albAreaUpliftBalPaymentObj?.Period6 ?? 0) +
                                 (albAreaUpliftOnProgPaymentObj?.Period6 ?? 0);
                        break;
                    case 6:
                        total += (albAreaUpliftBalPaymentObj?.Period7 ?? 0) +
                                 (albAreaUpliftOnProgPaymentObj?.Period7 ?? 0);
                        break;
                    case 7:
                        total += (albAreaUpliftBalPaymentObj?.Period8 ?? 0) +
                                 (albAreaUpliftOnProgPaymentObj?.Period8 ?? 0);
                        break;
                    case 8:
                        total += (albAreaUpliftBalPaymentObj?.Period9 ?? 0) +
                                 (albAreaUpliftOnProgPaymentObj?.Period9 ?? 0);
                        break;
                    case 9:
                        total += (albAreaUpliftBalPaymentObj?.Period10 ?? 0) +
                                 (albAreaUpliftOnProgPaymentObj?.Period10 ?? 0);
                        break;
                    case 10:
                        total += (albAreaUpliftBalPaymentObj?.Period11 ?? 0) +
                                 (albAreaUpliftOnProgPaymentObj?.Period11 ?? 0);
                        break;
                    case 11:
                        total += (albAreaUpliftBalPaymentObj?.Period12 ?? 0) +
                                 (albAreaUpliftOnProgPaymentObj?.Period12 ?? 0);
                        break;
                }
            }

            return total;
        }
    }
}