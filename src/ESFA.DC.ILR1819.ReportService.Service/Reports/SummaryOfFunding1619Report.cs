using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using CsvHelper;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.FundingService.FM25.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR1819.ReportService.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Configuration;
using ESFA.DC.ILR1819.ReportService.Interface.Reports;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;
using ESFA.DC.ILR1819.ReportService.Service.Comparer;
using ESFA.DC.ILR1819.ReportService.Service.Mapper;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.JobContextManager.Model.Interface;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.ILR1819.ReportService.Service.Reports
{
    public sealed class SummaryOfFunding1619Report : AbstractReportBuilder, IReport
    {
        private static readonly SummaryOfFunding1619ModelComparer SummaryOfFunding1619ModelComparer = new SummaryOfFunding1619ModelComparer();

        private readonly ILogger _logger;
        private readonly IKeyValuePersistenceService _storage;
        private readonly IIlrProviderService _ilrProviderService;
        private readonly IValidLearnersService _validLearnersService;
        private readonly IFM25ProviderService _fm25ProviderService;
        private readonly IStringUtilitiesService _stringUtilitiesService;

        public SummaryOfFunding1619Report(
            ILogger logger,
            [KeyFilter(PersistenceStorageKeys.Blob)] IKeyValuePersistenceService blob,
            IIlrProviderService ilrProviderService,
            IValidLearnersService validLearnersService,
            IFM25ProviderService fm25ProviderService,
            IStringUtilitiesService stringUtilitiesService,
            IDateTimeProvider dateTimeProvider,
            IValueProvider valueProvider,
            ITopicAndTaskSectionOptions topicAndTaskSectionOptions)
        : base(dateTimeProvider, valueProvider)
        {
            _logger = logger;
            _storage = blob;
            _ilrProviderService = ilrProviderService;
            _validLearnersService = validLearnersService;
            _fm25ProviderService = fm25ProviderService;
            _stringUtilitiesService = stringUtilitiesService;

            ReportFileName = "16-19 Summary of Funding by Student Report";
            ReportTaskName = topicAndTaskSectionOptions.TopicReports_TaskGenerateSummaryOfFunding1619Report;
        }

        public async Task GenerateReport(IJobContextMessage jobContextMessage, ZipArchive archive, bool isFis, CancellationToken cancellationToken)
        {
            var jobId = jobContextMessage.JobId;
            var ukPrn = jobContextMessage.KeyValuePairs[JobContextMessageKey.UkPrn].ToString();
            string externalFileName = GetExternalFilename(ukPrn, jobId, jobContextMessage.SubmissionDateTimeUtc);
            string fileName = GetFilename(ukPrn, jobId, jobContextMessage.SubmissionDateTimeUtc);

            string csv = await GetCsv(jobContextMessage, cancellationToken);
            await _storage.SaveAsync($"{externalFileName}.csv", csv, cancellationToken);
            await WriteZipEntry(archive, $"{fileName}.csv", csv);
        }

        private async Task<string> GetCsv(IJobContextMessage jobContextMessage, CancellationToken cancellationToken)
        {
            Task<IMessage> ilrFileTask = _ilrProviderService.GetIlrFile(jobContextMessage, cancellationToken);
            Task<List<string>> validLearnersTask = _validLearnersService.GetLearnersAsync(jobContextMessage, cancellationToken);
            Task<FM25Global> fm25Task = _fm25ProviderService.GetFM25Data(jobContextMessage, cancellationToken);

            await Task.WhenAll(ilrFileTask, validLearnersTask, fm25Task);

            if (cancellationToken.IsCancellationRequested)
            {
                return null;
            }

            List<string> ilrError = new List<string>();

            List<SummaryOfFunding1619Model> summaryOfFunding1619Models = new List<SummaryOfFunding1619Model>(validLearnersTask.Result.Count);
            foreach (string validLearnerRefNum in validLearnersTask.Result)
            {
                var learner =
                    ilrFileTask.Result?.Learners?.SingleOrDefault(x => x.LearnRefNumber == validLearnerRefNum);

                var fm25Learner = fm25Task.Result?.Learners?.SingleOrDefault(x => x.LearnRefNumber == validLearnerRefNum);

                if (learner == null || fm25Learner == null)
                {
                    ilrError.Add(validLearnerRefNum);
                    continue;
                }

                summaryOfFunding1619Models.Add(new SummaryOfFunding1619Model
                {
                    FundLine = fm25Learner.FundLine,
                    LearnRefNumber = learner.LearnRefNumber,
                    FamilyName = learner.FamilyName,
                    GivenNames = learner.GivenNames,
                    DateOfBirth = learner.DateOfBirthNullable?.ToString("dd/MM/yyyy"),
                    CampId = learner.CampId,
                    PlanLearnHours = learner.PlanLearnHoursNullable,
                    PlanEepHours = learner.PlanEEPHoursNullable,
                    TotalPlannedHours = (learner.PlanLearnHoursNullable ?? 0) + (learner.PlanEEPHoursNullable ?? 0),
                    RateBand = fm25Learner.RateBand,
                    StartFund = fm25Learner.StartFund ?? false,
                    OnProgPayment = fm25Learner.OnProgPayment
                });
            }

            summaryOfFunding1619Models.Sort(SummaryOfFunding1619ModelComparer);

            if (ilrError.Any())
            {
                _logger.LogWarning($"Failed to get one or more ILR learners while generating S{nameof(MathsAndEnglishReport)}: {_stringUtilitiesService.JoinWithMaxLength(ilrError)}");
            }

            using (MemoryStream ms = new MemoryStream())
            {
                UTF8Encoding utF8Encoding = new UTF8Encoding(false, true);
                using (TextWriter textWriter = new StreamWriter(ms, utF8Encoding))
                {
                    using (CsvWriter csvWriter = new CsvWriter(textWriter))
                    {
                        WriteCsvRecords<SummaryOfFunding1619Mapper, SummaryOfFunding1619Model>(csvWriter, summaryOfFunding1619Models);
                        csvWriter.Flush();
                        textWriter.Flush();
                        return Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
            }
        }
    }
}
