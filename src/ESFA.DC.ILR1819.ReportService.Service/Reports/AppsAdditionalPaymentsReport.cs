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
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR1819.ReportService.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Configuration;
using ESFA.DC.ILR1819.ReportService.Interface.Reports;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;
using ESFA.DC.ILR1819.ReportService.Service.Mapper;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.JobContextManager.Model.Interface;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.ILR1819.ReportService.Service.Reports
{
    public class AppsAdditionalPaymentsReport : AbstractReportBuilder, IReport
    {
        private readonly ILogger _logger;
        private readonly IKeyValuePersistenceService _storage;
        private readonly IIlrProviderService _ilrProviderService;
        private readonly IFM36ProviderService _fm36ProviderService;
        private readonly IValidLearnersService _validLearnersService;
        private readonly IStringUtilitiesService _stringUtilitiesService;

        private readonly IAppsAdditionalPaymentsModelBuilder _modelBuilder;

        public AppsAdditionalPaymentsReport(
            ILogger logger,
            [KeyFilter(PersistenceStorageKeys.Blob)] IKeyValuePersistenceService storage,
            IIlrProviderService ilrProviderService,
            IValidLearnersService validLearnersService,
            IFM36ProviderService fm36ProviderService,
            IStringUtilitiesService stringUtilitiesService,
            IDateTimeProvider dateTimeProvider,
            IValueProvider valueProvider,
            IAppsAdditionalPaymentsModelBuilder modelBuilder,
            ITopicAndTaskSectionOptions topicAndTaskSectionOptions)
        : base(dateTimeProvider, valueProvider)
        {
            _logger = logger;
            _storage = storage;
            _ilrProviderService = ilrProviderService;
            _fm36ProviderService = fm36ProviderService;
            _validLearnersService = validLearnersService;
            _stringUtilitiesService = stringUtilitiesService;
            _modelBuilder = modelBuilder;

            ReportFileName = "Apps Additional Payments Report";
            ReportTaskName = topicAndTaskSectionOptions.TopicReports_TaskGenerateAppsAdditionalPaymentsReport;
        }

        public async Task GenerateReport(IJobContextMessage jobContextMessage, ZipArchive archive, bool isFis, CancellationToken cancellationToken)
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
            Task<FM36Global> fm36Task = _fm36ProviderService.GetFM36Data(jobContextMessage, cancellationToken);

            await Task.WhenAll(ilrFileTask, validLearnersTask, fm36Task);

            if (cancellationToken.IsCancellationRequested)
            {
                return null;
            }

            var ilrError = new List<string>();

            var additionalPaymentsModels = new List<AppsAdditionalPaymentsModel>();
            foreach (string validLearnerRefNum in validLearnersTask.Result)
            {
                var learner =
                    ilrFileTask.Result?.Learners?.SingleOrDefault(x => x.LearnRefNumber == validLearnerRefNum);

                var fm36Data = fm36Task.Result?.Learners?.SingleOrDefault(x => x.LearnRefNumber == validLearnerRefNum);

                if (learner == null || fm36Data == null)
                {
                    ilrError.Add(validLearnerRefNum);
                    continue;
                }

                additionalPaymentsModels.Add(_modelBuilder.BuildModel(learner, fm36Data));
            }

            if (ilrError.Any())
            {
                _logger.LogWarning($"Failed to get one or more ILR learners while generating {nameof(AppsAdditionalPaymentsReport)}: {_stringUtilitiesService.JoinWithMaxLength(ilrError)}");
            }

            using (MemoryStream ms = new MemoryStream())
            {
                UTF8Encoding utF8Encoding = new UTF8Encoding(false, true);
                using (TextWriter textWriter = new StreamWriter(ms, utF8Encoding))
                {
                    using (CsvWriter csvWriter = new CsvWriter(textWriter))
                    {
                        WriteCsvRecords<AppsAdditionalPaymentsMapper, AppsAdditionalPaymentsModel>(csvWriter, additionalPaymentsModels);
                    }

                    textWriter.Flush();
                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
        }
    }
}
