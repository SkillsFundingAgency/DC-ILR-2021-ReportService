using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Aspose.Cells;
using CsvHelper;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.FundingService.ALB.FundingOutput.Model.Output;
using ESFA.DC.ILR.FundingService.FM25.Model.Output;
using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Builders;
using ESFA.DC.ILR1819.ReportService.Interface.Configuration;
using ESFA.DC.ILR1819.ReportService.Interface.Reports;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;
using ESFA.DC.ILR1819.ReportService.Model.Styling;
using ESFA.DC.ILR1819.ReportService.Service.Mapper;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.JobContextManager.Model.Interface;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.ILR1819.ReportService.Service.Reports
{
    public sealed class FundingSummaryReport : AbstractReportBuilder, IReport
    {
        private readonly FundingSummaryMapper _albFundingSummaryMapper = new FundingSummaryMapper("Advanced Loans Bursary");
        private readonly FundingSummaryMapper _traineeship1618Mapper = new FundingSummaryMapper("16-18 Traineeships");

        private readonly Dictionary<string, List<FundingSummaryModel>> fundingSummaryModels;

        private readonly ILogger _logger;
        private readonly IStreamableKeyValuePersistenceService _storage;
        private readonly IIlrProviderService _ilrProviderService;
        private readonly IOrgProviderService _orgProviderService;
        private readonly IAllbProviderService _allbProviderService;
        private readonly IFM25ProviderService _fm25ProviderService;
        private readonly IFM35ProviderService _fm35ProviderService;
        private readonly IValidLearnersService _validLearnersService;
        private readonly IStringUtilitiesService _stringUtilitiesService;
        private readonly IPeriodProviderService _periodProviderService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ILarsProviderService _larsProviderService;
        private readonly IEasProviderService _easProviderService;
        private readonly IPostcodeProviderService _postcodeProviderService;
        private readonly ILargeEmployerProviderService _largeEmployerProviderService;
        private readonly IAllbBuilder _allbBuilder;
        private readonly IFm25Builder _fm25Builder;
        private readonly IVersionInfo _versionInfo;
        private readonly IExcelStyleProvider _excelStyleProvider;

        public FundingSummaryReport(
            ILogger logger,
            IStreamableKeyValuePersistenceService storage,
            IIlrProviderService ilrProviderService,
            IOrgProviderService orgProviderService,
            IAllbProviderService allbProviderService,
            IFM25ProviderService fm25ProviderService,
            IFM35ProviderService fm35ProviderService,
            IValidLearnersService validLearnersService,
            IStringUtilitiesService stringUtilitiesService,
            IPeriodProviderService periodProviderService,
            IDateTimeProvider dateTimeProvider,
            ILarsProviderService larsProviderService,
            IEasProviderService easProviderService,
            IPostcodeProviderService postcodeProviderService,
            ILargeEmployerProviderService largeEmployerProviderService,
            IAllbBuilder allbBuilder,
            IFm25Builder fm25Builder,
            IVersionInfo versionInfo,
            IExcelStyleProvider excelStyleProvider,
            ITopicAndTaskSectionOptions topicAndTaskSectionOptions)
            : base(dateTimeProvider)
        {
            _logger = logger;
            _storage = storage;
            _ilrProviderService = ilrProviderService;
            _orgProviderService = orgProviderService;
            _allbProviderService = allbProviderService;
            _fm25ProviderService = fm25ProviderService;
            _fm35ProviderService = fm35ProviderService;
            _validLearnersService = validLearnersService;
            _stringUtilitiesService = stringUtilitiesService;
            _periodProviderService = periodProviderService;
            _larsProviderService = larsProviderService;
            _easProviderService = easProviderService;
            _postcodeProviderService = postcodeProviderService;
            _largeEmployerProviderService = largeEmployerProviderService;
            _allbBuilder = allbBuilder;
            _fm25Builder = fm25Builder;
            _versionInfo = versionInfo;
            _excelStyleProvider = excelStyleProvider;
            _dateTimeProvider = dateTimeProvider;

            ReportFileName = "Funding Summary Report";
            ReportTaskName = topicAndTaskSectionOptions.TopicReports_TaskGenerateFundingSummaryReport;

            fundingSummaryModels = new Dictionary<string, List<FundingSummaryModel>>();
        }

        public async Task GenerateReport(IJobContextMessage jobContextMessage, ZipArchive archive, CancellationToken cancellationToken)
        {
            Task<IMessage> ilrFileTask = _ilrProviderService.GetIlrFile(jobContextMessage, cancellationToken);
            Task<ALBGlobal> albDataTask = _allbProviderService.GetAllbData(jobContextMessage, cancellationToken);
            Task<FM25Global> fm25Task = _fm25ProviderService.GetFM25Data(jobContextMessage, cancellationToken);
            Task<FM35Global> fm35Task = _fm35ProviderService.GetFM35Data(jobContextMessage, cancellationToken);
            Task<List<string>> validLearnersTask = _validLearnersService.GetLearnersAsync(jobContextMessage, cancellationToken);
            Task<string> providerNameTask = _orgProviderService.GetProviderName(jobContextMessage, cancellationToken);
            Task<int> periodTask = _periodProviderService.GetPeriod(jobContextMessage, cancellationToken);

            await Task.WhenAll(ilrFileTask, albDataTask, fm25Task, fm35Task, validLearnersTask, providerNameTask, periodTask);

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            FundingSummaryHeaderModel fundingSummaryHeaderModel = await GetHeader(jobContextMessage, ilrFileTask, providerNameTask, cancellationToken);
            FundingSummaryFooterModel fundingSummaryFooterModel = await GetFooterAsync(ilrFileTask, cancellationToken);

            // Todo: Check keys & titles
            fundingSummaryModels["ALB"] = await _allbBuilder.BuildAsync(jobContextMessage, cancellationToken);
            fundingSummaryModels["16-18 Traineeships (Adult Funded)"] = new List<FundingSummaryModel> { _fm25Builder.BuildWithFundLine("16-18 Traineeships (Adult Funded)", fm25Task.Result, validLearnersTask.Result, "16-18 Traineeships (Adult Funded)", periodTask.Result) };

            var jobId = jobContextMessage.JobId;
            var ukPrn = jobContextMessage.KeyValuePairs[JobContextMessageKey.UkPrn].ToString();
            var externalFileName = GetExternalFilename(ukPrn, jobId, jobContextMessage.SubmissionDateTimeUtc);
            var fileName = GetFilename(ukPrn, jobId, jobContextMessage.SubmissionDateTimeUtc);

            string csv = GetReportCsv(fundingSummaryHeaderModel, fundingSummaryFooterModel);
            await _storage.SaveAsync($"{externalFileName}.csv", csv, cancellationToken);
            await WriteZipEntry(archive, $"{fileName}.csv", csv);

            Workbook workbook = GetWorkbookReport(fundingSummaryHeaderModel, fundingSummaryFooterModel);

            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Save(ms, SaveFormat.Xlsx);
                await _storage.SaveAsync($"{externalFileName}.xlsx", ms, cancellationToken);
                await WriteZipEntry(archive, $"{fileName}.xlsx", ms, cancellationToken);
            }
        }

        private string GetReportCsv(FundingSummaryHeaderModel fundingSummaryHeaderModel, FundingSummaryFooterModel fundingSummaryFooterModel)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                UTF8Encoding utF8Encoding = new UTF8Encoding(false, true);
                using (TextWriter textWriter = new StreamWriter(ms, utF8Encoding))
                {
                    using (CsvWriter csvWriter = new CsvWriter(textWriter))
                    {
                        WriteCsvRecords<FundingSummaryHeaderMapper, FundingSummaryHeaderModel>(csvWriter, fundingSummaryHeaderModel);
                        WriteCsvRecords(csvWriter, fundingSummaryModels["16-18 Traineeships (Adult Funded)"], _traineeship1618Mapper);
                        WriteCsvRecords(csvWriter);
                        WriteCsvRecords(csvWriter, fundingSummaryModels["ALB"], _albFundingSummaryMapper);
                        WriteCsvRecords<FundingSummaryFooterMapper, FundingSummaryFooterModel>(csvWriter, fundingSummaryFooterModel);
                        csvWriter.Flush();
                        textWriter.Flush();
                        return Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
            }
        }

        private Workbook GetWorkbookReport(
            FundingSummaryHeaderModel fundingSummaryHeaderModel,
            FundingSummaryFooterModel fundingSummaryFooterModel)
        {
            Workbook workbook = new Workbook();
            CellStyle[] cellStyles = _excelStyleProvider.GetFundingSummaryStyles(workbook);
            Worksheet sheet = workbook.Worksheets[0];

            WriteExcelRecords(sheet, new FundingSummaryHeaderMapper(), new List<FundingSummaryHeaderModel> { fundingSummaryHeaderModel }, cellStyles[5], cellStyles[5], true);
            WriteExcelRecords(sheet);
            WriteExcelRecords(sheet, "16-18 Traineeships Budget", cellStyles[0], 17);
            WriteExcelRecords(
                sheet,
                _traineeship1618Mapper,
                fundingSummaryModels["16-18 Traineeships (Adult Funded)"],
                cellStyles[2],
                null);
            WriteExcelRecords(sheet);
            WriteExcelRecords(sheet, "Advanced Loans Bursary Budget", cellStyles[0], 17);
            WriteExcelRecords(sheet, _albFundingSummaryMapper, fundingSummaryModels["ALB"], cellStyles[2], cellStyles[4]);
            WriteExcelRecords(sheet);
            WriteExcelRecords(sheet, new FundingSummaryFooterMapper(), new List<FundingSummaryFooterModel> { fundingSummaryFooterModel }, cellStyles[5], cellStyles[5], true);
            return workbook;
        }

        private async Task<FundingSummaryHeaderModel> GetHeader(IJobContextMessage jobContextMessage, Task<IMessage> messageTask, Task<string> providerNameTask, CancellationToken cancellationToken)
        {
            FundingSummaryHeaderModel fundingSummaryHeaderModel = new FundingSummaryHeaderModel
            {
                IlrFile = Path.GetFileName(jobContextMessage.KeyValuePairs[JobContextMessageKey.Filename].ToString()),
                Ukprn = messageTask.Result.HeaderEntity.SourceEntity.UKPRN,
                ProviderName = providerNameTask.Result,
                LastEasUpdate = (await _easProviderService.GetLastEasUpdate(messageTask.Result.HeaderEntity.SourceEntity.UKPRN, cancellationToken)).ToString("dd/MM/yyyy"),
                LastIlrFileUpdate = messageTask.Result.HeaderEntity.SourceEntity.DateTime.ToString("dd/MM/yyyy"),
                SecurityClassification = "OFFICIAL - SENSITIVE"
            };

            return fundingSummaryHeaderModel;
        }

        private async Task<FundingSummaryFooterModel> GetFooterAsync(Task<IMessage> messageTask, CancellationToken cancellationToken)
        {
            var dateTimeNowUtc = _dateTimeProvider.GetNowUtc();
            var dateTimeNowUk = _dateTimeProvider.ConvertUtcToUk(dateTimeNowUtc);

            FundingSummaryFooterModel fundingSummaryFooterModel = new FundingSummaryFooterModel
            {
                ReportGeneratedAt = "Report generated at " + dateTimeNowUk.ToString("HH:mm:ss") + " on " + dateTimeNowUk.ToString("dd/MM/yyyy"),
                ApplicationVersion = _versionInfo.ServiceReleaseVersion,
                ComponentSetVersion = "NA",
                FilePreparationDate = messageTask.Result.HeaderEntity.SourceEntity.DateTime.ToString("dd/MM/yyyy"),
                OrganisationData = await _orgProviderService.GetVersionAsync(cancellationToken),
                LargeEmployerData = await _largeEmployerProviderService.GetVersionAsync(cancellationToken),
                LarsData = await _larsProviderService.GetVersionAsync(cancellationToken),
                PostcodeData = await _postcodeProviderService.GetVersionAsync(cancellationToken)
            };

            return fundingSummaryFooterModel;
        }
    }
}