using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using ESFA.DC.DateTimeProvider.Interface;
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
    public class AppsAdditionalPaymentsReport : AbstractReportBuilder, IReport
    {
        private readonly ILogger _logger;
        private readonly IKeyValuePersistenceService _storage;
        private readonly IIlrProviderService _ilrProviderService;
        private readonly IFM36ProviderService _fm36ProviderService;
        private readonly IDASPaymentsProviderService _dasPaymentsProviderService;
        private readonly IAppsAdditionalPaymentsModelBuilder _modelBuilder;

        public AppsAdditionalPaymentsReport(
            ILogger logger,
            IKeyValuePersistenceService storage,
            IIlrProviderService ilrProviderService,
            IFM36ProviderService fm36ProviderService,
            IDateTimeProvider dateTimeProvider,
            IValueProvider valueProvider,
            ITopicAndTaskSectionOptions topicAndTaskSectionOptions,
            IDASPaymentsProviderService dasPaymentsProviderService,
            IAppsAdditionalPaymentsModelBuilder modelBuilder)
        : base(dateTimeProvider, valueProvider)
        {
            _logger = logger;
            _storage = storage;
            _ilrProviderService = ilrProviderService;
            _fm36ProviderService = fm36ProviderService;
            _dasPaymentsProviderService = dasPaymentsProviderService;
            _modelBuilder = modelBuilder;

            ReportFileName = "Apps Additional Payments Report";
            ReportTaskName = topicAndTaskSectionOptions.TopicReports_TaskGenerateAppsAdditionalPaymentsReport;
        }

        public async Task GenerateReport(IReportServiceContext reportServiceContext, ZipArchive archive, bool isFis, CancellationToken cancellationToken)
        {
            var jobId = reportServiceContext.JobId;
            var ukPrn = reportServiceContext.Ukprn;
            var externalFileName = GetExternalFilename(ukPrn.ToString(), jobId, reportServiceContext.SubmissionDateTimeUtc);
            var fileName = GetFilename(ukPrn.ToString(), jobId, reportServiceContext.SubmissionDateTimeUtc);
            var appsAdditionalPaymentIlrInfo = await _ilrProviderService.GetILRInfoForAppsAdditionalPaymentsReportAsync(ukPrn, cancellationToken);
            var appsAdditionalPaymentRulebaseInfo = await _fm36ProviderService.GetFM36DataForAppsAdditionalPaymentReportAsync(ukPrn, cancellationToken);
            var appsAdditionalPaymentDasPaymentsInfo = await _dasPaymentsProviderService.GetPaymentsInfoForAppsAdditionalPaymentsReportAsync(ukPrn, cancellationToken);

            var appsAdditionalPaymentsModel = _modelBuilder.BuildModel(appsAdditionalPaymentIlrInfo, appsAdditionalPaymentRulebaseInfo, appsAdditionalPaymentDasPaymentsInfo);
            string csv = await GetCsv(appsAdditionalPaymentsModel, cancellationToken);
            await _storage.SaveAsync($"{externalFileName}.csv", csv, cancellationToken);
            await WriteZipEntry(archive, $"{fileName}.csv", csv);
        }

        private async Task<string> GetCsv(List<AppsAdditionalPaymentsModel> appsAdditionalPaymentsModel, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (MemoryStream ms = new MemoryStream())
            {
                UTF8Encoding utF8Encoding = new UTF8Encoding(false, true);
                using (TextWriter textWriter = new StreamWriter(ms, utF8Encoding))
                {
                    using (CsvWriter csvWriter = new CsvWriter(textWriter))
                    {
                        WriteCsvRecords<AppsAdditionalPaymentsMapper, AppsAdditionalPaymentsModel>(csvWriter, appsAdditionalPaymentsModel);

                        csvWriter.Flush();
                        textWriter.Flush();
                        return Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
            }
        }
    }
}
