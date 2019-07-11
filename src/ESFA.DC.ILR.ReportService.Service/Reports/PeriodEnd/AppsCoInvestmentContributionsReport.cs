using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.ReportService.Interface.Configuration;
using ESFA.DC.ILR.ReportService.Interface.Provider;
using ESFA.DC.ILR.ReportService.Interface.Reports;
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
    public sealed class AppsCoInvestmentContributionsReport : AbstractReport, ILegacyReport
    {
        private readonly IIlrPeriodEndProviderService _ilrPeriodEndProviderService;
        private readonly IDASPaymentsProviderService _dasPaymentsProviderService;

        public AppsCoInvestmentContributionsReport(
            ILogger logger,
            IStreamableKeyValuePersistenceService streamableKeyValuePersistenceService,
            IDateTimeProvider dateTimeProvider,
            IValueProvider valueProvider,
            IIlrPeriodEndProviderService ilrPeriodEndProviderService,
            IDASPaymentsProviderService dasPaymentsProviderService)
        : base(dateTimeProvider, valueProvider, streamableKeyValuePersistenceService, logger)
        {
            _ilrPeriodEndProviderService = ilrPeriodEndProviderService;
            _dasPaymentsProviderService = dasPaymentsProviderService;
        }

        public override string ReportFileName => "Apps Co-Investment Contributions Report";

        public override string ReportTaskName => ReportTaskNameConstants.AppsCoInvestmentContributionsReport;

        public override async Task GenerateReport(IReportServiceContext reportServiceContext, ZipArchive archive, bool isFis, CancellationToken cancellationToken)
        {
            var externalFileName = GetFilename(reportServiceContext);
            var fileName = GetZipFilename(reportServiceContext);

            var appsCoInvestmentIlrInfo = await _ilrPeriodEndProviderService.GetILRInfoForAppsCoInvestmentReportAsync(reportServiceContext.Ukprn, cancellationToken);
            var appsCoInvestmentPaymentsInfo = await _dasPaymentsProviderService.GetPaymentsInfoForAppsCoInvestmentReportAsync(reportServiceContext.Ukprn, cancellationToken);
            string csv = await GetCsv(reportServiceContext, cancellationToken);
            await _streamableKeyValuePersistenceService.SaveAsync($"{externalFileName}.csv", csv, cancellationToken);
            await WriteZipEntry(archive, $"{fileName}.csv", csv);
        }

        private async Task<string> GetCsv(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            List<AppsCoInvestmentContributionsModel> appsCoInvestmentContributionsModels = new List<AppsCoInvestmentContributionsModel>();

            using (var ms = new MemoryStream())
            {
                UTF8Encoding utF8Encoding = new UTF8Encoding(false, true);
                using (TextWriter textWriter = new StreamWriter(ms, utF8Encoding))
                {
                    using (CsvWriter csvWriter = new CsvWriter(textWriter))
                    {
                        WriteCsvRecords<AppsCoInvestmentContributionsMapper, AppsCoInvestmentContributionsModel>(csvWriter, appsCoInvestmentContributionsModels);
                        csvWriter.Flush();
                        textWriter.Flush();
                        return Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
            }
        }
    }
}
