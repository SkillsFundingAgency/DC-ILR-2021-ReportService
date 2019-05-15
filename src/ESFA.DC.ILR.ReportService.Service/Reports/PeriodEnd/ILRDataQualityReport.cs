using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Aspose.Cells;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.FundingService.ALB.FundingOutput.Model.Output;
using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Interface.Builders;
using ESFA.DC.ILR.ReportService.Interface.Configuration;
using ESFA.DC.ILR.ReportService.Interface.Context;
using ESFA.DC.ILR.ReportService.Interface.Provider;
using ESFA.DC.ILR.ReportService.Interface.Reports;
using ESFA.DC.ILR.ReportService.Interface.Service;
using ESFA.DC.ILR.ReportService.Model.Eas;
using ESFA.DC.ILR.ReportService.Model.ReportModels;
using ESFA.DC.ILR.ReportService.Service.Reports.Abstract;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.ILR.ReportService.Service.Reports.PeriodEnd
{
    public sealed class ILRDataQualityReport : AbstractReport, IReport
    {
        public ILRDataQualityReport(
            ILogger logger,
            IStreamableKeyValuePersistenceService streamableKeyValuePersistenceService,
            IDateTimeProvider dateTimeProvider,
            IValueProvider valueProvider)
            : base(dateTimeProvider, valueProvider, streamableKeyValuePersistenceService, logger)
        {
        }

        public override string ReportFileName => "ILR Data Quality Report";

        public override string ReportTaskName => ReportTaskNameConstants.ILRDataQualityReport;

        public override async Task GenerateReport(IReportServiceContext reportServiceContext, ZipArchive archive, bool isFis, CancellationToken cancellationToken)
        {
            var externalFileName = GetFilename(reportServiceContext);
            var fileName = GetZipFilename(reportServiceContext);
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith("ILRDataQualityReportTemplate.xlsx"));
            var manifestResourceStream = assembly.GetManifestResourceStream(resourceName);
            Workbook workbook = new Workbook(manifestResourceStream);
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Save(ms, SaveFormat.Xlsx);
                await _streamableKeyValuePersistenceService.SaveAsync($"{externalFileName}.xlsx", ms, cancellationToken);
                await WriteZipEntry(archive, $"{fileName}.xlsx", ms, cancellationToken);
            }
        }
    }
}