using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Aspose.Cells;
using CsvHelper;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Interface.Builders;
using ESFA.DC.ILR.ReportService.Interface.Configuration;
using ESFA.DC.ILR.ReportService.Interface.Provider;
using ESFA.DC.ILR.ReportService.Interface.Service;
using ESFA.DC.ILR.ReportService.Model.ILR;
using ESFA.DC.ILR.ReportService.Model.ReportModels;
using ESFA.DC.ILR.ReportService.Service.Comparer;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Mapper;
using ESFA.DC.ILR.ReportService.Service.Reports.Abstract;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ReportTaskNameConstants = ESFA.DC.ILR.ReportService.Interface.ReportTaskNameConstants;

namespace ESFA.DC.ILR.ReportService.Service.Reports
{
    public class SummaryOfFm35FundingReport : AbstractLegacyReport
    {
        private const int REPORT_DATA_START_ROW = 6;

        private static readonly SummaryOfFm35FundingModelComparer comparer = new SummaryOfFm35FundingModelComparer();

        private readonly IFM35ProviderService _fm35ProviderService;
        private readonly IIlrProviderService _ilrProviderService;
        private readonly IIlrMetadataProviderService _ilrMetadataProviderService;

        private readonly IStringUtilitiesService _stringUtilitiesService;
        private readonly ILogger _logger;
        private readonly IFm35Builder _summaryOfFm35FundingModelBuilder;

        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IVersionInfo _versionInfo;
        private readonly ILarsProviderService _larsProviderService;
        private readonly IPostcodeProviderService _postcodeProviderService;
        private readonly ILargeEmployerProviderService _largeEmployerProviderService;
        private readonly IOrgProviderService _orgProviderService;

        public SummaryOfFm35FundingReport(
            ILogger logger,
            IStreamableKeyValuePersistenceService streamableKeyValuePersistenceService,
            IIlrProviderService ilrProviderService,
            IIlrMetadataProviderService ilrMetadataProviderService,
            IFM35ProviderService fm35ProviderService,
            IDateTimeProvider dateTimeProvider,
            IValueProvider valueProvider,
            IVersionInfo versionInfo,
            ILarsProviderService larsProviderService,
            IPostcodeProviderService postcodeProviderService,
            ILargeEmployerProviderService largeEmployerProviderService,
            IOrgProviderService orgProviderService,
            IFm35Builder builder)
            : base(dateTimeProvider, valueProvider, streamableKeyValuePersistenceService, logger)
        {
            _fm35ProviderService = fm35ProviderService;
            _ilrProviderService = ilrProviderService;
            _ilrMetadataProviderService = ilrMetadataProviderService;
            _summaryOfFm35FundingModelBuilder = builder;
            _versionInfo = versionInfo;
            _larsProviderService = larsProviderService;
            _postcodeProviderService = postcodeProviderService;
            _largeEmployerProviderService = largeEmployerProviderService;
            _orgProviderService = orgProviderService;
            _logger = logger;
            _dateTimeProvider = dateTimeProvider;
        }

        public override string ReportFileName => "Summary of Funding Model 35 Funding Report";

        public override string ReportTaskName => ReportTaskNameConstants.SummaryOfFM35FundingReport;

        public override async Task GenerateReport(IReportServiceContext reportServiceContext, ZipArchive archive, bool isFis, CancellationToken cancellationToken)
        {
            var externalFileName = GetFilename(reportServiceContext);
            var fileName = GetZipFilename(reportServiceContext);

            Task<IMessage> ilrFileTask = _ilrProviderService.GetIlrFile(reportServiceContext, cancellationToken);
            Task<string> providerNameTask = _orgProviderService.GetProviderName(reportServiceContext, cancellationToken);
            Task<ILRSourceFileInfo> lastSubmittedIlrFileTask = _ilrMetadataProviderService.GetLastSubmittedIlrFile(reportServiceContext, cancellationToken);

            SummaryOfFM35FundingHeaderModel summaryOfFm35FundingHeaderModel = GetHeader(reportServiceContext, providerNameTask);
            SummaryOfFM35FundingFooterModel summaryOfFm35FundingFooterModel = await GetFooterAsync(ilrFileTask, lastSubmittedIlrFileTask, cancellationToken);

            var summaryOfFm35FundingModels = await GetSummaryOfFm35FundingModels(reportServiceContext, cancellationToken);
            if (summaryOfFm35FundingModels == null)
            {
                return;
            }

            _logger.LogInfo("CSV Report Start");

            string csv = GetCsv(summaryOfFm35FundingModels);
            await _streamableKeyValuePersistenceService.SaveAsync($"{externalFileName}.csv", csv, cancellationToken);
            await WriteZipEntry(archive, $"{fileName}.csv", csv);

            _logger.LogInfo("CSV Report End");

            _logger.LogInfo("Excel Report Start");

            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith("SummaryOfFM35FundingTemplate.xlsx"));
            var manifestResourceStream = assembly.GetManifestResourceStream(resourceName);
            Workbook workbook = new Workbook(manifestResourceStream);
            InsertHeaderFooter(workbook, summaryOfFm35FundingHeaderModel, summaryOfFm35FundingFooterModel);
            PopulateUsingSmartMarkers(workbook, summaryOfFm35FundingModels);
            workbook.CalculateFormula();
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Save(ms, SaveFormat.Xlsx);
                await _streamableKeyValuePersistenceService.SaveAsync($"{externalFileName}.xlsx", ms, cancellationToken);
                await WriteZipEntry(archive, $"{fileName}.xlsx", ms, cancellationToken);
            }

            _logger.LogInfo("Excel Report End");
        }

        private string GetCsv(IList<SummaryOfFm35FundingModel> summaryOfFm35FundingModels)
        {
            using (var ms = new MemoryStream())
            {
                UTF8Encoding utF8Encoding = new UTF8Encoding(false, true);
                using (TextWriter textWriter = new StreamWriter(ms, utF8Encoding))
                {
                    using (CsvWriter csvWriter = new CsvWriter(textWriter))
                    {
                        WriteCsvRecords<SummaryOfFM35FundingMapper, SummaryOfFm35FundingModel>(csvWriter, summaryOfFm35FundingModels);
                        csvWriter.Flush();
                        textWriter.Flush();
                        return Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
            }
        }

        private async Task<IList<SummaryOfFm35FundingModel>> GetSummaryOfFm35FundingModels(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            Task<FM35Global> fm35Task = _fm35ProviderService.GetFM35Data(reportServiceContext, cancellationToken);

            await Task.WhenAll(fm35Task);

            cancellationToken.ThrowIfCancellationRequested();

            List<SummaryOfFm35FundingModel> summaryOfFm35FundingModels = new List<SummaryOfFm35FundingModel>();

            FM35Global fm35Data = fm35Task.Result;
            if (fm35Data?.Learners == null)
            {
                _logger.LogWarning($"No Fm35 data for {nameof(SummaryOfFm35FundingReport)}");
                return summaryOfFm35FundingModels;
            }

            foreach (FM35Learner learnerAttribute in fm35Data.Learners)
            {
                foreach (LearningDelivery fundLineData in learnerAttribute.LearningDeliveries)
                {
                    var summaryOfFm35FundingModelsList = _summaryOfFm35FundingModelBuilder.BuildModel(fundLineData).ToArray();
                    summaryOfFm35FundingModels.AddRange(summaryOfFm35FundingModelsList);
                }
            }

            summaryOfFm35FundingModels.Sort(comparer);
            return summaryOfFm35FundingModels;
        }

        private SummaryOfFM35FundingHeaderModel GetHeader(IReportServiceContext reportServiceContext, Task<string> providerNameTask)
        {
            var fileName = reportServiceContext.OriginalFilename ?? reportServiceContext.Filename;
            var summaryOfFm35FundingHeaderModel = new SummaryOfFM35FundingHeaderModel
            {
                IlrFile = string.Equals(reportServiceContext.CollectionName, "ILR1819", StringComparison.OrdinalIgnoreCase) ? fileName : "N/A",
                Ukprn = reportServiceContext.Ukprn,
                ProviderName = providerNameTask.Result ?? "Unknown"
            };

            return summaryOfFm35FundingHeaderModel;
        }

        private async Task<SummaryOfFM35FundingFooterModel> GetFooterAsync(Task<IMessage> messageTask, Task<ILRSourceFileInfo> lastSubmittedIlrFileTask, CancellationToken cancellationToken)
        {
            var dateTimeNowUtc = _dateTimeProvider.GetNowUtc();
            var dateTimeNowUk = _dateTimeProvider.ConvertUtcToUk(dateTimeNowUtc);
            var summaryOfFm35FundingFooterModel = new SummaryOfFM35FundingFooterModel
            {
                ReportGeneratedAt = "Report generated at " + dateTimeNowUk.ToString("HH:mm:ss") + " on " + dateTimeNowUk.ToString("dd/MM/yyyy"),
                ApplicationVersion = _versionInfo.ServiceReleaseVersion,
                ComponentSetVersion = "NA",
                OrganisationData = await _orgProviderService.GetVersionAsync(cancellationToken),
                LargeEmployerData = await _largeEmployerProviderService.GetVersionAsync(cancellationToken),
                LarsData = await _larsProviderService.GetVersionAsync(cancellationToken),
                PostcodeData = await _postcodeProviderService.GetVersionAsync(cancellationToken)
            };

            if (messageTask.Result != null)
            {
                summaryOfFm35FundingFooterModel.FilePreparationDate = messageTask.Result.HeaderEntity.SourceEntity.DateTime.ToString("dd/MM/yyyy");
            }
            else
            {
                summaryOfFm35FundingFooterModel.FilePreparationDate = lastSubmittedIlrFileTask.Result != null ?
                                                                                lastSubmittedIlrFileTask.Result.FilePreparationDate.GetValueOrDefault().ToString("dd/MM/yyyy") : string.Empty;
            }

            return summaryOfFm35FundingFooterModel;
        }

        private void InsertHeaderFooter(Workbook workbook, SummaryOfFM35FundingHeaderModel headerModel, SummaryOfFM35FundingFooterModel footerModel)
        {
            PageSetup pageSetup = workbook.Worksheets[0].PageSetup;

            pageSetup.SetHeader(0, "&16&\"Bold\"Summary of Funding Model 35 Funding\n\n&8&\"Bold\"Provider:" + headerModel.ProviderName + "\nUKPRN: " + headerModel.Ukprn + "\nILR File:" + headerModel.IlrFile);
            pageSetup.SetHeader(2, "&10&\"Bold\"OFFICIAL-SENSITIVE\n\n&8&\"Bold\"Year:" + Constants.Year);
            pageSetup.SetFooter(0, $"&8Component Set Version: \t{footerModel.ComponentSetVersion}\nApplication Version: \t{footerModel.ApplicationVersion}\nFile Prepartion Date: \t{footerModel.FilePreparationDate}\n{footerModel.ReportGeneratedAt}");
            pageSetup.SetFooter(1, $"&8Lars Data: \t{footerModel.LarsData}\nPostcode Data: \t{footerModel.PostcodeData}");
            pageSetup.SetFooter(2, $"&8Large Employer Data: \t{footerModel.LargeEmployerData}\nOrganisation Data: \t{footerModel.OrganisationData}");
        }

        private void PopulateUsingSmartMarkers(Workbook workbook, IEnumerable<SummaryOfFm35FundingModel> summaryOfFm35FundingModels)
        {
            var designer = new WorkbookDesigner();
            designer.Workbook = workbook;
            designer.SetDataSource("GroupedModels", summaryOfFm35FundingModels.ToList());
            designer.Process();
            var worksheet = designer.Workbook.Worksheets[0];
            var designerWorkbookCells = worksheet.Cells;

            var dataCellArea = new CellArea
            {
                StartRow = REPORT_DATA_START_ROW,
                StartColumn = 0,
                EndRow = designerWorkbookCells.MaxDataRow,
                EndColumn = designerWorkbookCells.MaxDataColumn
            };

            designerWorkbookCells.Subtotal(dataCellArea, 0, ConsolidationFunction.Sum, new[] { 2, 3, 4, 5, 6, 7, 8 }, true, true, true);
            worksheet.PageSetup.PrintTitleColumns = "$A:$I";
            worksheet.PageSetup.PrintTitleRows = "$" + REPORT_DATA_START_ROW + ":$" + REPORT_DATA_START_ROW;
        }
    }
}
