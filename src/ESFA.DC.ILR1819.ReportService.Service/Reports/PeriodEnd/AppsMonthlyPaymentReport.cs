using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using CsvHelper;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR1819.ReportService.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Builders.PeriodEnd;
using ESFA.DC.ILR1819.ReportService.Interface.Configuration;
using ESFA.DC.ILR1819.ReportService.Interface.Context;
using ESFA.DC.ILR1819.ReportService.Interface.Reports;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels.PeriodEnd;
using ESFA.DC.ILR1819.ReportService.Service.Mapper.PeriodEnd;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.ILR1819.ReportService.Service.Reports.PeriodEnd
{
    public class AppsMonthlyPaymentReport : AbstractReportBuilder, IReport
    {
        private readonly ILogger _logger;
        private readonly IKeyValuePersistenceService _storage;
        private readonly IIlrProviderService _ilrProviderService;
        private readonly IFM36ProviderService _fm36ProviderService;
        private readonly IStringUtilitiesService _stringUtilitiesService;

        private readonly IAppsMonthlyPaymentModelBuilder _modelBuilder;

        public AppsMonthlyPaymentReport(
            ILogger logger,
            [KeyFilter(PersistenceStorageKeys.Blob)] IKeyValuePersistenceService storage,
            IIlrProviderService ilrProviderService,
            IFM36ProviderService fm36ProviderService,
            IStringUtilitiesService stringUtilitiesService,
            IDateTimeProvider dateTimeProvider,
            IValueProvider valueProvider,
            ITopicAndTaskSectionOptions topicAndTaskSectionOptions,
            IAppsMonthlyPaymentModelBuilder modelBuilder)
        : base(dateTimeProvider, valueProvider)
        {
            _logger = logger;
            _storage = storage;
            _ilrProviderService = ilrProviderService;
            _fm36ProviderService = fm36ProviderService;
            _stringUtilitiesService = stringUtilitiesService;
            _modelBuilder = modelBuilder;

            ReportFileName = "Apps Monthly Payment Report";
            ReportTaskName = topicAndTaskSectionOptions.TopicReports_TaskGenerateAppsMonthlyPaymentReport;
        }

        public async Task GenerateReport(IReportServiceContext reportServiceContext, ZipArchive archive, bool isFis, CancellationToken cancellationToken)
        {
            var jobId = reportServiceContext.JobId;
            var ukPrn = reportServiceContext.Ukprn.ToString();
            var externalFileName = GetExternalFilename(ukPrn, jobId, reportServiceContext.SubmissionDateTimeUtc);
            var fileName = GetFilename(ukPrn, jobId, reportServiceContext.SubmissionDateTimeUtc);

            string csv = await GetCsv(reportServiceContext, cancellationToken);
            await _storage.SaveAsync($"{externalFileName}.csv", csv, cancellationToken);
            await WriteZipEntry(archive, $"{fileName}.csv", csv);
        }

        private async Task<string> GetCsv(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return null;
            }

            var appsMonthlyPaymentModels = new List<AppsMonthlyPaymentModel>();

            using (MemoryStream ms = new MemoryStream())
            {
                UTF8Encoding utF8Encoding = new UTF8Encoding(false, true);
                using (TextWriter textWriter = new StreamWriter(ms, utF8Encoding))
                {
                    using (CsvWriter csvWriter = new CsvWriter(textWriter))
                    {
                        WriteCsvRecords<AppsMonthlyPaymentMapper, AppsMonthlyPaymentModel>(csvWriter, appsMonthlyPaymentModels);

                        csvWriter.Flush();
                        textWriter.Flush();
                        return Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
            }
        }
    }
}
