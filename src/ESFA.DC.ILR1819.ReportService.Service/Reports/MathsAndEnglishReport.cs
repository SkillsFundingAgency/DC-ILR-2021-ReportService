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
    public sealed class MathsAndEnglishReport : AbstractReportBuilder, IReport
    {
        private readonly ILogger _logger;
        private readonly IKeyValuePersistenceService _storage;
        private readonly IIlrProviderService _ilrProviderService;
        private readonly IFM25ProviderService _fm25ProviderService;
        private readonly IValidLearnersService _validLearnersService;
        private readonly IStringUtilitiesService _stringUtilitiesService;
        private readonly IMathsAndEnglishFm25Rules _mathsAndEnglishFm25Rules;
        private readonly IMathsAndEnglishModelBuilder _mathsAndEnglishModelBuilder;

        private static readonly MathsAndEnglishModelComparer MathsAndEnglishModelComparer = new MathsAndEnglishModelComparer();

        public MathsAndEnglishReport(
            ILogger logger,
            [KeyFilter(PersistenceStorageKeys.Blob)] IKeyValuePersistenceService storage,
            IIlrProviderService ilrProviderService,
            IValidLearnersService validLearnersService,
            IFM25ProviderService fm25ProviderService,
            IStringUtilitiesService stringUtilitiesService,
            IDateTimeProvider dateTimeProvider,
            IMathsAndEnglishFm25Rules mathsAndEnglishFm25Rules,
            IMathsAndEnglishModelBuilder mathsAndEnglishModelBuilder,
            ITopicAndTaskSectionOptions topicAndTaskSectionOptions)
        : base(dateTimeProvider)
        {
            _logger = logger;
            _storage = storage;
            _ilrProviderService = ilrProviderService;
            _fm25ProviderService = fm25ProviderService;
            _validLearnersService = validLearnersService;
            _stringUtilitiesService = stringUtilitiesService;
            _mathsAndEnglishFm25Rules = mathsAndEnglishFm25Rules;
            _mathsAndEnglishModelBuilder = mathsAndEnglishModelBuilder;

            ReportFileName = "Maths and English Report";
            ReportTaskName = topicAndTaskSectionOptions.TopicReports_TaskGenerateMathsAndEnglishReport;
        }

        public async Task GenerateReport(IJobContextMessage jobContextMessage, ZipArchive archive, CancellationToken cancellationToken)
        {
            var jobId = jobContextMessage.JobId;
            var ukPrn = jobContextMessage.KeyValuePairs[JobContextMessageKey.UkPrn].ToString();
            var externalFileName = GetExternalFilename(ukPrn, jobId, jobContextMessage.SubmissionDateTimeUtc);
            var fileName = GetFilename(ukPrn, jobId, jobContextMessage.SubmissionDateTimeUtc);

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

            var ilrError = new List<string>();

            var mathsAndEnglishModels = new List<MathsAndEnglishModel>(validLearnersTask.Result.Count);
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

                if (!_mathsAndEnglishFm25Rules.IsApplicableLearner(fm25Learner))
                {
                    continue;
                }

                mathsAndEnglishModels.Add(_mathsAndEnglishModelBuilder.BuildModel(learner, fm25Learner));
            }

            if (ilrError.Any())
            {
                _logger.LogWarning($"Failed to get one or more ILR learners while generating {nameof(MathsAndEnglishReport)}: {_stringUtilitiesService.JoinWithMaxLength(ilrError)}");
            }

            mathsAndEnglishModels.Sort(MathsAndEnglishModelComparer);

            using (MemoryStream ms = new MemoryStream())
            {
                UTF8Encoding utF8Encoding = new UTF8Encoding(false, true);
                using (TextWriter textWriter = new StreamWriter(ms, utF8Encoding))
                {
                    using (CsvWriter csvWriter = new CsvWriter(textWriter))
                    {
                        WriteCsvRecords<MathsAndEnglishMapper, MathsAndEnglishModel>(csvWriter, mathsAndEnglishModels);
                        csvWriter.Flush();
                        textWriter.Flush();
                        return Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
            }
        }
    }
}
