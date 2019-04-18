using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.ReportService.Interface.Builders.PeriodEnd;
using ESFA.DC.ILR.ReportService.Interface.Configuration;
using ESFA.DC.ILR.ReportService.Interface.Context;
using ESFA.DC.ILR.ReportService.Interface.Reports;
using ESFA.DC.ILR.ReportService.Interface.Service;
using ESFA.DC.ILR.ReportService.Model.ReportModels.PeriodEnd;
using ESFA.DC.ILR.ReportService.Service.Mapper.PeriodEnd;
using ESFA.DC.ILR.ReportService.Service.Reports.Abstract;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.ILR.ReportService.Service.Reports.PeriodEnd
{
    public class AppsMonthlyPaymentReport : AbstractReport, IReport
    {
        private readonly ILogger _logger;
        private readonly IIlrProviderService _ilrProviderService;
        private readonly IFM36ProviderService _fm36ProviderService;
        private readonly IDASPaymentsProviderService _dasPaymentsProviderService;
        private readonly IStringUtilitiesService _stringUtilitiesService;

        private readonly IAppsMonthlyPaymentModelBuilder _modelBuilder;

        public AppsMonthlyPaymentReport(
            ILogger logger,
            IStreamableKeyValuePersistenceService streamableKeyValuePersistenceService,
            IIlrProviderService ilrProviderService,
            IFM36ProviderService fm36ProviderService,
            IDASPaymentsProviderService dasPaymentsProviderService,
            IStringUtilitiesService stringUtilitiesService,
            IDateTimeProvider dateTimeProvider,
            IValueProvider valueProvider,
            ITopicAndTaskSectionOptions topicAndTaskSectionOptions,
            IAppsMonthlyPaymentModelBuilder modelBuilder)
        : base(dateTimeProvider, valueProvider, streamableKeyValuePersistenceService)
        {
            _logger = logger;
            _ilrProviderService = ilrProviderService;
            _fm36ProviderService = fm36ProviderService;
            _dasPaymentsProviderService = dasPaymentsProviderService;
            _stringUtilitiesService = stringUtilitiesService;
            _modelBuilder = modelBuilder;

            ReportFileName = "Apps Monthly Payment Report";
            ReportTaskName = topicAndTaskSectionOptions.TopicReports_TaskGenerateAppsMonthlyPaymentReport;
        }

        public async Task GenerateReport(IReportServiceContext reportServiceContext, ZipArchive archive, bool isFis, CancellationToken cancellationToken)
        {
            var jobId = reportServiceContext.JobId;
            var ukPrn = reportServiceContext.Ukprn;
            var externalFileName = GetExternalFilename(ukPrn.ToString(), jobId, reportServiceContext.SubmissionDateTimeUtc);
            var fileName = GetFilename(ukPrn.ToString(), jobId, reportServiceContext.SubmissionDateTimeUtc);
            var appsMonthlyPaymentIlrInfo = await _ilrProviderService.GetILRInfoForAppsMonthlyPaymentReportAsync(ukPrn, cancellationToken);
            var appsMonthlyPaymentRulebaseInfo = await _fm36ProviderService.GetFM36DataForAppsMonthlyPaymentReportAsync(ukPrn, cancellationToken);
            var appsMonthlyPaymentDasInfo = await _dasPaymentsProviderService.GetPaymentsInfoForAppsMonthlyPaymentReportAsync(ukPrn, cancellationToken);

            var appsAdditionalPaymentsModel = _modelBuilder.BuildModel(appsMonthlyPaymentIlrInfo, appsMonthlyPaymentRulebaseInfo, appsMonthlyPaymentDasInfo);

            string csv = await GetCsv(reportServiceContext, appsAdditionalPaymentsModel, cancellationToken);
            await _streamableKeyValuePersistenceService.SaveAsync($"{externalFileName}.csv", csv, cancellationToken);
            await WriteZipEntry(archive, $"{fileName}.csv", csv);
        }

        private async Task<string> GetCsv(IReportServiceContext reportServiceContext, List<AppsMonthlyPaymentModel> appsAdditionalPaymentsModel, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (MemoryStream ms = new MemoryStream())
            {
                UTF8Encoding utF8Encoding = new UTF8Encoding(false, true);
                using (TextWriter textWriter = new StreamWriter(ms, utF8Encoding))
                {
                    using (CsvWriter csvWriter = new CsvWriter(textWriter))
                    {
                        WriteCsvRecords<AppsMonthlyPaymentMapper, AppsMonthlyPaymentModel>(csvWriter, appsAdditionalPaymentsModel);

                        csvWriter.Flush();
                        textWriter.Flush();
                        return Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
            }
        }
    }
}
