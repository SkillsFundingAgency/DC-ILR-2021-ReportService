using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.ReportService.Interface;
using ESFA.DC.ILR.ReportService.Interface.Builders.PeriodEnd;
using ESFA.DC.ILR.ReportService.Interface.Configuration;
using ESFA.DC.ILR.ReportService.Interface.Provider;
using ESFA.DC.ILR.ReportService.Interface.Service;
using ESFA.DC.ILR.ReportService.Model.ReportModels.PeriodEnd;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Mapper.PeriodEnd;
using ESFA.DC.ILR.ReportService.Service.Reports.Abstract;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ReportTaskNameConstants = ESFA.DC.ILR.ReportService.Interface.ReportTaskNameConstants;

namespace ESFA.DC.ILR.ReportService.Service.Reports.PeriodEnd
{
    public class AppsAdditionalPaymentsReport : AbstractLegacyReport, ILegacyReport
    {
        private readonly IIlrPeriodEndProviderService _ilrPeriodEndProviderService;
        private readonly IFM36PeriodEndProviderService _fm36ProviderService;
        private readonly IDASPaymentsProviderService _dasPaymentsProviderService;
        private readonly IAppsAdditionalPaymentsModelBuilder _modelBuilder;

        public AppsAdditionalPaymentsReport(
            ILogger logger,
            IStreamableKeyValuePersistenceService streamableKeyValuePersistenceService,
            IIlrPeriodEndProviderService ilrPeriodEndProviderService,
            IFM36PeriodEndProviderService fm36ProviderService,
            IDateTimeProvider dateTimeProvider,
            IValueProvider valueProvider,
            IDASPaymentsProviderService dasPaymentsProviderService,
            IAppsAdditionalPaymentsModelBuilder modelBuilder)
        : base(dateTimeProvider, valueProvider, streamableKeyValuePersistenceService, logger)
        {
            _ilrPeriodEndProviderService = ilrPeriodEndProviderService;
            _fm36ProviderService = fm36ProviderService;
            _dasPaymentsProviderService = dasPaymentsProviderService;
            _modelBuilder = modelBuilder;
        }

        public override string ReportFileName => "Apps Additional Payments Report";

        public override string ReportTaskName => ReportTaskNameConstants.AppsAdditionalPaymentsReport;

        public override async Task GenerateReport(IReportServiceContext reportServiceContext, ZipArchive archive, bool isFis, CancellationToken cancellationToken)
        {
            var externalFileName = GetFilename(reportServiceContext);
            var fileName = GetZipFilename(reportServiceContext);

            var appsAdditionalPaymentIlrInfo = await _ilrPeriodEndProviderService.GetILRInfoForAppsAdditionalPaymentsReportAsync(reportServiceContext.Ukprn, cancellationToken);
            var appsAdditionalPaymentRulebaseInfo = await _fm36ProviderService.GetFM36DataForAppsAdditionalPaymentReportAsync(reportServiceContext.Ukprn, cancellationToken);
            var appsAdditionalPaymentDasPaymentsInfo = await _dasPaymentsProviderService.GetPaymentsInfoForAppsAdditionalPaymentsReportAsync(reportServiceContext.Ukprn, cancellationToken);

            var appsAdditionalPaymentsModel = _modelBuilder.BuildModel(appsAdditionalPaymentIlrInfo, appsAdditionalPaymentRulebaseInfo, appsAdditionalPaymentDasPaymentsInfo);
            string csv = await GetCsv(appsAdditionalPaymentsModel, cancellationToken);
            await _streamableKeyValuePersistenceService.SaveAsync($"{externalFileName}.csv", csv, cancellationToken);
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
