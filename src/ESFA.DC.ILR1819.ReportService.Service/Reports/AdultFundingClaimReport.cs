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
using ESFA.DC.EAS1819.EF;
using ESFA.DC.ILR.FundingService.ALB.FundingOutput.Model.Output;
using ESFA.DC.ILR.FundingService.FM25.Model.Output;
using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using ESFA.DC.ILR.FundingService.FM81.FundingOutput.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR1819.DataStore.EF;
using ESFA.DC.ILR1819.DataStore.EF.Interfaces;
using ESFA.DC.ILR1819.ReportService.Interface.Builders;
using ESFA.DC.ILR1819.ReportService.Interface.Configuration;
using ESFA.DC.ILR1819.ReportService.Interface.Reports;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.Configuration;
using ESFA.DC.ILR1819.ReportService.Model.Generation;
using ESFA.DC.ILR1819.ReportService.Model.ILR;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;
using ESFA.DC.ILR1819.ReportService.Model.Styling;
using ESFA.DC.ILR1819.ReportService.Service.Builders;
using ESFA.DC.ILR1819.ReportService.Service.Mapper;
using ESFA.DC.ILR1819.ReportService.Service.Service;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.JobContextManager.Model.Interface;
using ESFA.DC.Logging.Interfaces;
using EasSubmissionValues = ESFA.DC.ILR1819.ReportService.Model.Eas.EasSubmissionValues;

namespace ESFA.DC.ILR1819.ReportService.Service.Reports
{
    public sealed class AdultFundingClaimReport : AbstractReportBuilder, IReport
    {
        private const string ProviderNameCellName = "D5";
        private const string UkprnCellName = "D6";
        private const string IlrFileCellName = "D7";
        private const string YearCellName = "G7";
        private const string OtherLearningProgrammeFunding6MonthsCellName = "F12";
        private const string OtherLearningProgrammeFunding12MonthsCellName = "G12";
        private const string OtherLearningLearningSupport6MonthsCellName = "F13";
        private const string OtherLearningLearningSupport12MonthsCellName = "G13";
        private const string Traineeships1924ProgrammeFunding6MonthsCellName = "F14";
        private const string Traineeships1924ProgrammeFunding12MonthsCellName = "G14";
        private const string Traineeships1924LearningSupport6MonthsCellName = "F15";
        private const string Traineeships1924LearningSupport12MonthsCellName = "G15";
        private const string Traineeships1924LearnerSupport6MonthsCellName = "F16";
        private const string Traineeships1924LearnerSupport12MonthsCellName = "G16";
        private const string LoansBursaryFunding6MonthsCellName = "F21";
        private const string LoansBursaryFunding12MonthsCellName = "G21";
        private const string LoansAreaCosts6MonthsCellName = "F22";
        private const string LoansAreaCosts12MonthsCellName = "G22";
        private const string LoansExcessSupport6MonthsCellName = "F23";
        private const string LoansExcessSupport12MonthsCellName = "G23";
        private const string ComponentSetVersionCellName = "D36";
        private const string ApplicationVersionCellName = "D37";
        private const string FilePreparationDateCellName = "D38";
        private const string LarsDataCellName = "F36";
        private const string OrganisationDataCellName = "F37";
        private const string PostcodeDataCellName = "F38";
        private const string LargeEmployerDataCellName = "F39";
        private const string ReportGeneratedAtCellName = "D40";

        private readonly FundingSummaryMapper _fundingSummaryMapper;
        private readonly ModelProperty[] _cachedModelProperties;
        private readonly List<FundingSummaryModel> fundingSummaryModels;
        private readonly ILogger _logger;
        private readonly IStreamableKeyValuePersistenceService _storage;
        private readonly IIlrProviderService _ilrProviderService;
        private readonly IOrgProviderService _orgProviderService;
        private readonly IAllbProviderService _allbProviderService;
        private readonly IFM35ProviderService _fm35ProviderService;
        private readonly IPeriodProviderService _periodProviderService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ILarsProviderService _larsProviderService;
        private readonly IEasProviderService _easProviderService;
        private readonly IPostcodeProviderService _postcodeProviderService;
        private readonly ILargeEmployerProviderService _largeEmployerProviderService;

        private readonly ITotalBuilder _totalBuilder;
        private readonly IVersionInfo _versionInfo;
        private readonly IExcelStyleProvider _excelStyleProvider;
        private readonly IEasBuilder _easBuilder;
        private readonly IAdultFundingClaimBuilder _adultFundingClaimBuilder;
        private readonly IAllbBuilder _allbBuilder;
        private readonly IFm25Builder _fm25Builder;
        private readonly IFm35Builder _fm35Builder;

        public AdultFundingClaimReport(
            ILogger logger,
            IStreamableKeyValuePersistenceService storage,
            IIlrProviderService ilrProviderService,
            IOrgProviderService orgProviderService,
            IAllbProviderService allbProviderService,
            IFM35ProviderService fm35ProviderService,
            IPeriodProviderService periodProviderService,
            IDateTimeProvider dateTimeProvider,
            IValueProvider valueProvider,
            ILarsProviderService larsProviderService,
            IEasProviderService easProviderService,
            IPostcodeProviderService postcodeProviderService,
            ILargeEmployerProviderService largeEmployerProviderService,
            IAllbBuilder allbBuilder,
            IFm35Builder fm35Builder,
            ITotalBuilder totalBuilder,
            IVersionInfo versionInfo,
            IExcelStyleProvider excelStyleProvider,
            ITopicAndTaskSectionOptions topicAndTaskSectionOptions,
            IEasBuilder easBuilder,
            IAdultFundingClaimBuilder adultFundingClaimBuilder)
            : base(dateTimeProvider, valueProvider)
        {
            _logger = logger;
            _storage = storage;
            _ilrProviderService = ilrProviderService;
            _orgProviderService = orgProviderService;
            _allbProviderService = allbProviderService;
            _fm35ProviderService = fm35ProviderService;
            _periodProviderService = periodProviderService;
            _larsProviderService = larsProviderService;
            _easProviderService = easProviderService;
            _postcodeProviderService = postcodeProviderService;
            _largeEmployerProviderService = largeEmployerProviderService;
            _allbBuilder = allbBuilder;
            _fm35Builder = fm35Builder;
            _totalBuilder = totalBuilder;
            _versionInfo = versionInfo;
            _excelStyleProvider = excelStyleProvider;
            _dateTimeProvider = dateTimeProvider;
            _easBuilder = easBuilder;
            _adultFundingClaimBuilder = adultFundingClaimBuilder;

            ReportFileName = "Adult Funding Claim Report";
            ReportTaskName = topicAndTaskSectionOptions.TopicReports_TaskGenerateFundingSummaryReport;

            fundingSummaryModels = new List<FundingSummaryModel>();
            _fundingSummaryMapper = new FundingSummaryMapper();
            _cachedModelProperties = _fundingSummaryMapper.MemberMaps.OrderBy(x => x.Data.Index).Select(x => new ModelProperty(x.Data.Names.Names.ToArray(), (PropertyInfo)x.Data.Member)).ToArray();
        }

        public async Task GenerateReport(IJobContextMessage jobContextMessage, ZipArchive archive, bool isFis, CancellationToken cancellationToken)
        {
            //Task<IMessage> ilrFileTask = _ilrProviderService.GetIlrFile(jobContextMessage, cancellationToken);
            //Task<ALBGlobal> albDataTask = _allbProviderService.GetAllbData(jobContextMessage, cancellationToken);

            IILR1819_DataStoreEntities _ilrContext = new ILR1819_DataStoreEntities("data source=(local);initial catalog=Easdb;integrated security=True;multipleactiveresultsets=True;Connect Timeout=90");
            var _easdbContext = new EasdbContext("data source=(local);initial catalog=Easdb;integrated security=True;multipleactiveresultsets=True;Connect Timeout=90");

            var easProviderService = new EasProviderService(null, new EasConfiguration { EasConnectionString = "data source=(local);initial catalog=Easdb;integrated security=True;multipleactiveresultsets=True;Connect Timeout=90" });

           // var easProviderService = new EasProviderService(null, new EasConfiguration() { EasConnectionString = "data source=(local);initial catalog=Easdb;integrated security=True;multipleactiveresultsets=True;Connect Timeout=90" });
            var ilrConnectionString =
                "metadata=res://*/DataStoreModel.csdl|res://*/DataStoreModel.ssdl|res://*/DataStoreModel.msl;provider=System.Data.SqlClient;provider connection string='data source =(local); initial catalog = ilr1819DataStore; integrated security = True; MultipleActiveResultSets = True; App = EntityFramework'";
            var fm35ProviderService = new FM35ProviderService(_logger, null, null, null, new ILRConfiguration
            {
                ILRConnectionString = ilrConnectionString
            });

            var allbProviderService = new AllbProviderService(_logger, null, null, null, new ILRConfiguration
            {
                ILRConnectionString = ilrConnectionString
            });

            Task<List<EasSubmissionValues>> easSubmissionValuesAsync = easProviderService.GetEasSubmissionValuesAsync(jobContextMessage, cancellationToken);
            Task<List<FM35LearningDeliveryValues>> fm35AdultFundingLineDataFromDataStore = fm35ProviderService.GetFM35AdultFundingLineDataFromDataStore(jobContextMessage, cancellationToken);
            Task<List<ALBLearningDeliveryValues>> albfm35AdultFundingLineDataFromDataStore = allbProviderService.GetALBFM35AdultFundingLineDataFromDataStore(jobContextMessage, cancellationToken);

            // await Task.WhenAll(easSubmissionValuesAsync, fm35AdultFundingLineDataFromDataStore, albfm35AdultFundingLineDataFromDataStore);

            var fundingClaimModel = new AdultFundingClaimBuilder().BuildAdultFundingClaimModel(fm35AdultFundingLineDataFromDataStore.Result, easSubmissionValuesAsync.Result, albfm35AdultFundingLineDataFromDataStore.Result);
            //Task<string> providerNameTask = _orgProviderService.GetProviderName(jobContextMessage, cancellationToken);
            //Task<int> periodTask = _periodProviderService.GetPeriod(jobContextMessage, cancellationToken);

            //Task<List<EasSubmissionValues>> easSubmissionsValuesTask = new Task<List<EasSubmissionValues>>(null);
            //if (!isFis)
            //{
            //    easSubmissionsValuesTask = _easProviderService.GetEasSubmissionValuesAsync(jobContextMessage, cancellationToken);
            //}

            //await Task.WhenAll(ilrFileTask, albDataTask, fm35Task, providerNameTask, periodTask, easSubmissionsValuesTask);
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            var jobId = jobContextMessage.JobId;
            var ukPrn = jobContextMessage.KeyValuePairs[JobContextMessageKey.UkPrn].ToString();
            var externalFileName = GetExternalFilename(ukPrn, jobId, jobContextMessage.SubmissionDateTimeUtc);
            var fileName = GetFilename(ukPrn, jobId, jobContextMessage.SubmissionDateTimeUtc);

            var adultFundingClaimModel = new AdultFundingClaimModel()
            {
                ProviderName = "Milton Keynes Council",
                Ukprn = 10004376,
                IlrFile = "ILR-A-10004376-1819-20181119-151322-01.xml",
                Year = "2018/19",

                OtherLearningProgrammeFunding6Months = 1006,
                OtherLearningProgrammeFunding12Months = 1012,
                OtherLearningLearningSupport6Months = 2006,
                OtherLearningLearningSupport12Months = 2012,
                Traineeships1924ProgrammeFunding6Months = 3006,
                Traineeships1924ProgrammeFunding12Months = 3012,
                Traineeships1924LearningSupport6Months = 4006,
                Traineeships1924LearningSupport12Months = 4012,
                Traineeships1924LearnerSupport6Months = 5006,
                Traineeships1924LearnerSupport12Months = 5012,
                LoansBursaryFunding6Months = 6006,
                LoansBursaryFunding12Months = 6012,
                LoansAreaCosts6Months = 7006,
                LoansAreaCosts12Months = 7012,
                LoansExcessSupport6Months = 8006,
                LoansExcessSupport12Months = 8012,

                ComponentSetVersion = "12",
                ApplicationVersion = "1.2.3.4",
                FilePreparationDate = "07/11/2018",
                LarsData = "Version 1.1.1: " + _dateTimeProvider.GetNowUtc().ToString("dd MMM yyyy HH:mm:ss"),
                OrganisationData = "Version 2.2.2: " + _dateTimeProvider.GetNowUtc().ToString("dd MMM yyyy HH:mm:ss"),
                PostcodeData = "Version 3.3.3: " + _dateTimeProvider.GetNowUtc().ToString("dd MMM yyyy HH:mm:ss"),
                LargeEmployerData = "Version 4.4.4: " + _dateTimeProvider.GetNowUtc().ToString("dd MMM yyyy HH:mm:ss"),
                ReportGeneratedAt = _dateTimeProvider.GetNowUtc().ToString("HH:mm:ss tt") + " on " + _dateTimeProvider.GetNowUtc().ToString("dd/MM/yyyy")
            };

            using (MemoryStream memoryStream = new MemoryStream())
            {
                _storage.GetAsync("AdultFundingClaimReportTemplate", memoryStream, cancellationToken).GetAwaiter().GetResult();

                memoryStream.Position = 0;
                Workbook workbook = new Workbook(memoryStream);
                PopulateWorkbook(workbook, fundingClaimModel);
                using (MemoryStream ms = new MemoryStream())
                {
                    workbook.Save(ms, SaveFormat.Xlsx);
                    await _storage.SaveAsync($"{externalFileName}.xlsx", ms, cancellationToken);
                    await WriteZipEntry(archive, $"{fileName}.xlsx", ms, cancellationToken);
                }
            }
        }

        private void PopulateWorkbook(Workbook workbook, AdultFundingClaimModel adultFundingClaimModel)
        {
            Worksheet worksheet = workbook.Worksheets[0];
            Cells cells = worksheet.Cells;
            cells[ProviderNameCellName].PutValue(adultFundingClaimModel.ProviderName);
            cells[UkprnCellName].PutValue(adultFundingClaimModel.Ukprn);
            cells[IlrFileCellName].PutValue(adultFundingClaimModel.IlrFile);
            cells[YearCellName].PutValue(adultFundingClaimModel.Year);

            cells[OtherLearningProgrammeFunding6MonthsCellName]
                .PutValue(adultFundingClaimModel.OtherLearningProgrammeFunding6Months);
            cells[OtherLearningProgrammeFunding12MonthsCellName]
                .PutValue(adultFundingClaimModel.OtherLearningProgrammeFunding12Months);
            cells[OtherLearningLearningSupport6MonthsCellName]
                .PutValue(adultFundingClaimModel.OtherLearningLearningSupport6Months);
            cells[OtherLearningLearningSupport12MonthsCellName]
                .PutValue(adultFundingClaimModel.OtherLearningLearningSupport12Months);
            cells[Traineeships1924ProgrammeFunding6MonthsCellName]
                .PutValue(adultFundingClaimModel.Traineeships1924ProgrammeFunding6Months);
            cells[Traineeships1924ProgrammeFunding12MonthsCellName]
                .PutValue(adultFundingClaimModel.Traineeships1924ProgrammeFunding12Months);
            cells[Traineeships1924LearningSupport6MonthsCellName]
                .PutValue(adultFundingClaimModel.Traineeships1924LearningSupport6Months);
            cells[Traineeships1924LearningSupport12MonthsCellName]
                .PutValue(adultFundingClaimModel.Traineeships1924LearningSupport12Months);
            cells[Traineeships1924LearnerSupport6MonthsCellName]
                .PutValue(adultFundingClaimModel.Traineeships1924LearnerSupport6Months);
            cells[Traineeships1924LearnerSupport12MonthsCellName]
                .PutValue(adultFundingClaimModel.Traineeships1924LearnerSupport12Months);
            cells[LoansBursaryFunding6MonthsCellName].PutValue(adultFundingClaimModel.LoansBursaryFunding6Months);
            cells[LoansBursaryFunding12MonthsCellName].PutValue(adultFundingClaimModel.LoansBursaryFunding12Months);
            cells[LoansAreaCosts6MonthsCellName].PutValue(adultFundingClaimModel.LoansAreaCosts6Months);
            cells[LoansAreaCosts12MonthsCellName].PutValue(adultFundingClaimModel.LoansAreaCosts12Months);
            cells[LoansExcessSupport6MonthsCellName].PutValue(adultFundingClaimModel.LoansExcessSupport6Months);
            cells[LoansExcessSupport12MonthsCellName].PutValue(adultFundingClaimModel.LoansExcessSupport12Months);

            cells[ComponentSetVersionCellName].PutValue(adultFundingClaimModel.ComponentSetVersion);
            cells[ApplicationVersionCellName].PutValue(adultFundingClaimModel.ApplicationVersion);
            cells[FilePreparationDateCellName].PutValue(adultFundingClaimModel.FilePreparationDate);
            cells[LarsDataCellName].PutValue(adultFundingClaimModel.LarsData);
            cells[OrganisationDataCellName].PutValue(adultFundingClaimModel.OrganisationData);
            cells[PostcodeDataCellName].PutValue(adultFundingClaimModel.PostcodeData);
            cells[LargeEmployerDataCellName].PutValue(adultFundingClaimModel.LargeEmployerData);
            cells[ReportGeneratedAtCellName].PutValue(adultFundingClaimModel.ReportGeneratedAt);
        }
    }
}