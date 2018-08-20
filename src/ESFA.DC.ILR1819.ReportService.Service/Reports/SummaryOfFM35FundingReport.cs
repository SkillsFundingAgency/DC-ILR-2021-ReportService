using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model;
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
    public class SummaryOfFm35FundingReport : AbstractReportBuilder, IReport
    {
        private readonly IFM35ProviderService _fm35ProviderService;
        private readonly IStringUtilitiesService _stringUtilitiesService;
        private readonly IKeyValuePersistenceService _storage;
        private readonly ILogger _logger;
        private readonly ISummaryOfFM35FundingModelBuilder _summaryOfFm35FundingModelBuilder;

        public SummaryOfFm35FundingReport(
            ILogger logger,
            [KeyFilter(PersistenceStorageKeys.Blob)] IKeyValuePersistenceService storage,
            IFM35ProviderService fm35ProviderService,
            IStringUtilitiesService stringUtilitiesService,
            IDateTimeProvider dateTimeProvider,
            ITopicAndTaskSectionOptions topicAndTaskSectionOptions,
            ISummaryOfFM35FundingModelBuilder builder)
            : base(dateTimeProvider)
        {
            _logger = logger;
            _fm35ProviderService = fm35ProviderService;
            _stringUtilitiesService = stringUtilitiesService;
            _summaryOfFm35FundingModelBuilder = builder;
            _storage = storage;

            ReportTaskName = topicAndTaskSectionOptions.TopicReports_TaskGenerateSummaryOfFM35FundingReport;
            ReportFileName = "Summary of Funding Model 35 Funding Report";
        }

        public async Task GenerateReport(IJobContextMessage jobContextMessage, ZipArchive archive, CancellationToken cancellationToken)
        {
            var jobId = jobContextMessage.JobId;
            var ukPrn = jobContextMessage.KeyValuePairs[JobContextMessageKey.UkPrn].ToString();
            var fileName = GetReportFilename(ukPrn, jobId);

            var summaryOfFm35FundingModels = await GetSummaryOfFm35FundingModels(jobContextMessage, cancellationToken);

            string csv = GetCsv(summaryOfFm35FundingModels);
            await _storage.SaveAsync($"{fileName}.csv", csv, cancellationToken);
            await WriteZipEntry(archive, $"{fileName}.csv", csv);
        }

        private string GetCsv(IList<SummaryOfFm35FundingModel> summaryOfFm35FundingModels)
        {
            using (var ms = new MemoryStream())
            {
                BuildCsvReport<SummaryOfFM35FundingMapper, SummaryOfFm35FundingModel>(ms, summaryOfFm35FundingModels);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        private async Task<IList<SummaryOfFm35FundingModel>> GetSummaryOfFm35FundingModels(IJobContextMessage jobContextMessage, CancellationToken cancellationToken)
        {
            Task<FM35FundingOutputs> fm35Task = _fm35ProviderService.GetFM35Data(jobContextMessage, cancellationToken);

            await Task.WhenAll(fm35Task);

            if (cancellationToken.IsCancellationRequested)
            {
                return null;
            }

            var ilrError = new List<string>();

            var fm35Data = fm35Task.Result;
            if (fm35Data == null)
            {
                ilrError.Add("No FM35 Data");
                return null;
            }

            var summaryOfFm35FundingModels = new List<SummaryOfFm35FundingModel>();
            foreach (var learnerAttribute in fm35Data.Learners)
            {
                foreach (var fundlineData in learnerAttribute.LearningDeliveryAttributes)
                {
                    summaryOfFm35FundingModels.AddRange(_summaryOfFm35FundingModelBuilder.BuildModel(fundlineData));
                }
            }

            if (ilrError.Any())
            {
                _logger.LogWarning($"Failed to get one or more ILR learners while generating {nameof(SummaryOfFm35FundingReport)}: {_stringUtilitiesService.JoinWithMaxLength(ilrError)}");
            }

            return summaryOfFm35FundingModels;
        }
    }
}
