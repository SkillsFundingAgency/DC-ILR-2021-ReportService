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
using ESFA.DC.ILR.ReportService.Interface.Context;
using ESFA.DC.ILR.ReportService.Interface.Reports;
using ESFA.DC.ILR.ReportService.Interface.Service;
using ESFA.DC.ILR.ReportService.Model.Generation;
using ESFA.DC.ILR.ReportService.Model.ILR;
using ESFA.DC.ILR.ReportService.Model.ReportModels;
using ESFA.DC.ILR.ReportService.Service.Comparer;
using ESFA.DC.ILR.ReportService.Service.Mapper;
using ESFA.DC.ILR.ReportService.Service.Reports.Abstract;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.ILR.ReportService.Service.Reports
{
    public class SummaryOfFm35FundingReport : AbstractReport, IReport
    {
        private static readonly SummaryOfFm35FundingModelComparer comparer = new SummaryOfFm35FundingModelComparer();

        private readonly IFM35ProviderService _fm35ProviderService;
        private readonly IIlrProviderService _ilrProviderService;
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
            IFM35ProviderService fm35ProviderService,
            IStringUtilitiesService stringUtilitiesService,
            IDateTimeProvider dateTimeProvider,
            IValueProvider valueProvider,
            IVersionInfo versionInfo,
            ILarsProviderService larsProviderService,
            IPostcodeProviderService postcodeProviderService,
            ILargeEmployerProviderService largeEmployerProviderService,
            IOrgProviderService orgProviderService,
            ITopicAndTaskSectionOptions topicAndTaskSectionOptions,
            IFm35Builder builder)
            : base(dateTimeProvider, valueProvider, streamableKeyValuePersistenceService)
        {
            _logger = logger;
            _fm35ProviderService = fm35ProviderService;
            _ilrProviderService = ilrProviderService;
            _stringUtilitiesService = stringUtilitiesService;
            _summaryOfFm35FundingModelBuilder = builder;
            _versionInfo = versionInfo;
            _larsProviderService = larsProviderService;
            _postcodeProviderService = postcodeProviderService;
            _largeEmployerProviderService = largeEmployerProviderService;
            _orgProviderService = orgProviderService;

            _dateTimeProvider = dateTimeProvider;

            ReportTaskName = topicAndTaskSectionOptions.TopicReports_TaskGenerateSummaryOfFM35FundingReport;
            ReportFileName = "Summary of Funding Model 35 Funding Report";
        }

        public async Task GenerateReport(IReportServiceContext reportServiceContext, ZipArchive archive, bool isFis, CancellationToken cancellationToken)
        {
            var jobId = reportServiceContext.JobId;
            var ukPrn = reportServiceContext.Ukprn.ToString();
            var externalFileName = GetExternalFilename(ukPrn, jobId, reportServiceContext.SubmissionDateTimeUtc);
            var fileName = GetFilename(ukPrn, jobId, reportServiceContext.SubmissionDateTimeUtc);

            Task<IMessage> ilrFileTask = _ilrProviderService.GetIlrFile(reportServiceContext, cancellationToken);
            Task<string> providerNameTask = _orgProviderService.GetProviderName(reportServiceContext, cancellationToken);
            Task<ILRSourceFileInfo> lastSubmittedIlrFileTask = _ilrProviderService.GetLastSubmittedIlrFile(reportServiceContext, cancellationToken);

            SummaryOfFM35FundingHeaderModel summaryOfFm35FundingHeaderModel = await GetHeader(reportServiceContext, ilrFileTask, lastSubmittedIlrFileTask, providerNameTask, cancellationToken, isFis);
            SummaryOfFM35FundingFooterModel summaryOfFm35FundingFooterModel = await GetFooterAsync(ilrFileTask, lastSubmittedIlrFileTask, cancellationToken);

            IList<SummaryOfFm35FundingModel> summaryOfFm35FundingModels = await GetSummaryOfFm35FundingModels(reportServiceContext, cancellationToken);
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
            string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith("SummaryOfFM35FundingTemplate.xltx"));
            var manifestResourceStream = assembly.GetManifestResourceStream(resourceName);
            Workbook workbook = new Workbook(manifestResourceStream);
            Worksheet worksheet = workbook.Worksheets[0];
            Cells cells = worksheet.Cells;
            InsertHeaderFooter(workbook, summaryOfFm35FundingHeaderModel, summaryOfFm35FundingFooterModel);
            PopulateMainData(workbook, summaryOfFm35FundingModels);
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
                    summaryOfFm35FundingModels.AddRange(_summaryOfFm35FundingModelBuilder.BuildModel(fundLineData));
                }
            }

            summaryOfFm35FundingModels.Sort(comparer);
            return summaryOfFm35FundingModels;
        }

        private async Task<SummaryOfFM35FundingHeaderModel> GetHeader(IReportServiceContext reportServiceContext, Task<IMessage> messageTask, Task<ILRSourceFileInfo> lastSubmittedIlrFileTask, Task<string> providerNameTask, CancellationToken cancellationToken, bool isFis)
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
            pageSetup.SetHeader(2, "&10&\"Bold\"OFFICIAL-SENSITIVE");
            pageSetup.SetFooter(0, $"&8Component Set Version: \t{footerModel.ComponentSetVersion}\nApplication Version: \t{footerModel.ApplicationVersion}\nFile Prepartion Date: \t{footerModel.FilePreparationDate}\n{footerModel.ReportGeneratedAt}");
            pageSetup.SetFooter(1, $"&8Lars Data: \t{footerModel.LarsData}\nPostcode Data: \t{footerModel.PostcodeData}");
            pageSetup.SetFooter(2, $"&8Large Employer Data: \t{footerModel.LargeEmployerData}\nOrganisation Data: \t{footerModel.OrganisationData}");
        }

        private void PopulateMainData(Workbook workbook, IEnumerable<SummaryOfFm35FundingModel> summaryOfFm35FundingModels)
        {
            var sheet = workbook.Worksheets[0];
            var summaryOfFm35FundingMapper = new SummaryOfFM35FundingMapper();
            var modelProperties = summaryOfFm35FundingMapper.MemberMaps.OrderBy(x => x.Data.Index).Select(x => new ModelProperty(x.Data.Names.Names.ToArray(), (PropertyInfo)x.Data.Member)).ToArray();
            WriteExcelRecords(sheet);
            WriteExcelRecords(sheet);
            WriteExcelRecords(sheet);
            WriteExcelRecords(sheet);
            WriteExcelRecords(sheet);

            var summaryOfFm35FundingModelsArray = summaryOfFm35FundingModels as SummaryOfFm35FundingModel[] ?? summaryOfFm35FundingModels.ToArray();
            IGrouping<string, SummaryOfFm35FundingModel>[] groupedModels = summaryOfFm35FundingModelsArray.ToArray().GroupBy(x => x.FundingLineType).ToArray();

            var writeHeader = true;
            foreach (var summaryOfFm35FundingModel in summaryOfFm35FundingModelsArray)
            {
                if (writeHeader)
                {
                    WriteExcelRecords(sheet, summaryOfFm35FundingMapper, new List<SummaryOfFm35FundingModel> { }, null, null);
                    writeHeader = false;
                }

                WriteExcelRecords(sheet, summaryOfFm35FundingMapper, modelProperties, summaryOfFm35FundingModel, null); //, new SummaryOfFM35FundingMapper(), new List<SummaryOfFm35FundingModel> { summaryOfFm35FundingModel }, null, null, true);
            }
        }
    }
}
