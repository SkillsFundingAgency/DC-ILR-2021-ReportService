using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using Aspose.Cells;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.ReportService.Interface.Context;
using ESFA.DC.ILR.ReportService.Interface.Provider;
using ESFA.DC.ILR.ReportService.Interface.Reports;
using ESFA.DC.ILR.ReportService.Interface.Service;
using ESFA.DC.ILR.ReportService.Model.PeriodEnd.ILRDataQuality;
using ESFA.DC.ILR.ReportService.Service.Reports.Abstract;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.ILR.ReportService.Service.Reports.PeriodEnd
{
    public sealed class ILRDataQualityReport : AbstractReport, IReport
    {
        private readonly IIlrPeriodEndProviderService _ilrPeriodEndProviderService;

        public ILRDataQualityReport(
            ILogger logger,
            IStreamableKeyValuePersistenceService streamableKeyValuePersistenceService,
            IDateTimeProvider dateTimeProvider,
            IValueProvider valueProvider,
            IIlrPeriodEndProviderService ilrPeriodEndProviderService)
            : base(dateTimeProvider, valueProvider, streamableKeyValuePersistenceService, logger)
        {
            _ilrPeriodEndProviderService = ilrPeriodEndProviderService;
        }

        public override string ReportFileName => "ILR Data Quality Report";

        public override string ReportTaskName => ReportTaskNameConstants.ILRDataQualityReport;

        public override async Task GenerateReport(IReportServiceContext reportServiceContext, ZipArchive archive, bool isFis, CancellationToken cancellationToken)
        {
            var externalFileName = GetFilename(reportServiceContext);
            var fileName = GetZipFilename(reportServiceContext);

            List<RuleViolationsInfo> top20RuleViolationsAsync = await _ilrPeriodEndProviderService.GetTop20RuleViolationsAsync(cancellationToken);
            var designer = new WorkbookDesigner
            {
                Workbook = GetWorkbookFromTemplate("ILRDataQualityReportTemplate.xlsx")
            };

            designer.SetDataSource("RuleViolationsInfo", top20RuleViolationsAsync);
            designer.Process();

            using (MemoryStream ms = new MemoryStream())
            {
                designer.Workbook.Save(ms, SaveFormat.Xlsx);
                await _streamableKeyValuePersistenceService.SaveAsync($"{externalFileName}.xlsx", ms, cancellationToken);
                await WriteZipEntry(archive, $"{fileName}.xlsx", ms, cancellationToken);
            }
        }
    }
}