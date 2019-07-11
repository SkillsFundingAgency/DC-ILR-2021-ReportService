using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Aspose.Cells;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.FundingService.FM25.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Interface.Builders;
using ESFA.DC.ILR.ReportService.Interface.Configuration;
using ESFA.DC.ILR.ReportService.Interface.Provider;
using ESFA.DC.ILR.ReportService.Interface.Reports;
using ESFA.DC.ILR.ReportService.Interface.Service;
using ESFA.DC.ILR.ReportService.Model.ILR;
using ESFA.DC.ILR.ReportService.Model.ReportModels;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Reports.Abstract;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ReportTaskNameConstants = ESFA.DC.ILR.ReportService.Interface.ReportTaskNameConstants;

namespace ESFA.DC.ILR.ReportService.Service.Reports
{
    public sealed class HNSSummaryReport : AbstractReport, ILegacyReport
    {
        private const string FundLine1416LearnDelFamACellName = "E4";
        private const string FundLine1416LearnDelFamBCellName = "E5";
        private const string FundLine1416LearnDelFamCCellName = "E6";
        private const string FundLine1416LearnDelFamDCellName = "E7";
        private const string FundLine1416LearnDelFamECellName = "E8";

        private const string FundLine1619LearnDelFamACellName = "E12";
        private const string FundLine1619LearnDelFamBCellName = "E13";
        private const string FundLine1619LearnDelFamCCellName = "E14";
        private const string FundLine1619LearnDelFamDCellName = "E15";
        private const string FundLine1619LearnDelFamECellName = "E16";

        private const string FundLine1924LearnDelFamACellName = "E20";
        private const string FundLine1924LearnDelFamBCellName = "E21";
        private const string FundLine1924LearnDelFamCCellName = "E22";
        private const string FundLine1924LearnDelFamDCellName = "E23";
        private const string FundLine1924LearnDelFamECellName = "E24";

        private const string FundLine19PlusLearnDelFamACellName = "E33";
        private const string FundLine19PlusLearnDelFamBCellName = "E34";
        private const string FundLine19PlusLearnDelFamCCellName = "E35";
        private const string FundLine19PlusLearnDelFamDCellName = "E36";
        private const string FundLine19PlusLearnDelFamECellName = "E37";

        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IIlrProviderService _ilrProviderService;
        private readonly IIlrMetadataProviderService _ilrMetadataProviderService;
        private readonly IOrgProviderService _orgProviderService;
        private readonly IFM25ProviderService _fm25ProviderService;
        private readonly IPostcodeProviderService _postcodeProviderService;
        private readonly ILargeEmployerProviderService _largeEmployerProviderService;
        private readonly ILarsProviderService _larsProviderService;
        private readonly IValidLearnersService _validLearnersService;
        private readonly IVersionInfo _versionInfo;
        private readonly IHNSSummaryModelBuilder _hnsSummaryModelBuilder;

        private readonly string[] _applicableFundingLineTypes = {
            "14-16 Direct Funded Students",
            "16-19 Students (excluding High Needs Students)",
            "16-19 High Needs Students",
            "19-24 Students with an EHCP",
            "19+ Continuing Students (excluding EHCP)"
        };

        public HNSSummaryReport(
            IStreamableKeyValuePersistenceService streamableKeyValuePersistenceService,
            IDateTimeProvider dateTimeProvider,
            IValueProvider valueProvider,
            IIlrProviderService ilrProviderService,
            IIlrMetadataProviderService ilrMetadataProviderService,
            IOrgProviderService orgProviderService,
            IFM25ProviderService fm25ProviderService,
            IPostcodeProviderService postcodeProviderService,
            ILargeEmployerProviderService largeEmployerProviderService,
            ILarsProviderService larsProviderService,
            IValidLearnersService validLearnersService,
            IVersionInfo versionInfo,
            ILogger logger,
            IHNSSummaryModelBuilder hnsSummaryModelBuilder)
            : base(dateTimeProvider, valueProvider, streamableKeyValuePersistenceService, logger)
        {
            _dateTimeProvider = dateTimeProvider;
            _ilrProviderService = ilrProviderService;
            _ilrMetadataProviderService = ilrMetadataProviderService;
            _orgProviderService = orgProviderService;
            _fm25ProviderService = fm25ProviderService;
            _postcodeProviderService = postcodeProviderService;
            _largeEmployerProviderService = largeEmployerProviderService;
            _larsProviderService = larsProviderService;
            _validLearnersService = validLearnersService;
            _versionInfo = versionInfo;
            _hnsSummaryModelBuilder = hnsSummaryModelBuilder;
        }

        public override string ReportFileName => "High Needs Students Summary Report";

        public override string ReportTaskName => ReportTaskNameConstants.HNSSummaryReport;

        public override async Task GenerateReport(IReportServiceContext reportServiceContext, ZipArchive archive, bool isFis, CancellationToken cancellationToken)
        {
            Task<IMessage> ilrFileTask = _ilrProviderService.GetIlrFile(reportServiceContext, cancellationToken);
            Task<FM25Global> fm25DataTask = _fm25ProviderService.GetFM25Data(reportServiceContext, cancellationToken);
            Task<string> providerNameTask = _orgProviderService.GetProviderName(reportServiceContext, cancellationToken);
            Task<ILRSourceFileInfo> lastSubmittedIlrFileTask = _ilrMetadataProviderService.GetLastSubmittedIlrFile(reportServiceContext, cancellationToken);
            Task<List<string>> validLearnersTask = _validLearnersService.GetLearnersAsync(reportServiceContext, cancellationToken);

            await Task.WhenAll(ilrFileTask, fm25DataTask, providerNameTask, lastSubmittedIlrFileTask, validLearnersTask);

            HNSSummaryHeaderModel hnsSummaryHeaderModel = await GetHeaderAsync(reportServiceContext, providerNameTask, cancellationToken, isFis);
            HNSSummaryFooterModel hnsSummaryFooterModel = await GetFooterAsync(ilrFileTask, lastSubmittedIlrFileTask, cancellationToken);

            var hnsSummaryModel = _hnsSummaryModelBuilder.BuildHNSSummaryModel(
                _logger,
                ilrFileTask,
                validLearnersTask.Result,
                fm25DataTask.Result,
                cancellationToken);

            var externalFileName = GetFilename(reportServiceContext);
            var fileName = GetZipFilename(reportServiceContext);

            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith("HNSSummaryReportTemplate.xlsx"));
            var manifestResourceStream = assembly.GetManifestResourceStream(resourceName);
            Workbook workbook = new Workbook(manifestResourceStream);
            Worksheet worksheet = workbook.Worksheets[0];
            InsertHeaderFooter(workbook, hnsSummaryHeaderModel, hnsSummaryFooterModel);
            PopulateWorkbook(workbook, hnsSummaryModel, isFis);
            workbook.CalculateFormula();
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Save(ms, SaveFormat.Xlsx);
                await _streamableKeyValuePersistenceService.SaveAsync($"{externalFileName}.xlsx", ms, cancellationToken);
                await WriteZipEntry(archive, $"{fileName}.xlsx", ms, cancellationToken);
            }
        }

        private void InsertHeaderFooter(Workbook workbook, HNSSummaryHeaderModel headerModel, HNSSummaryFooterModel footerModel)
        {
            PageSetup pageSetup = workbook.Worksheets[0].PageSetup;

            pageSetup.SetHeader(0, "&14&\"Bold\"High Needs Students (HNS) Summary Report");
            pageSetup.SetHeader(2, "&10&\"Bold\"OFFICIAL-SENSITIVE");

            pageSetup.SetHeader(0, "&16&\"Bold\"High Needs Students (HNS) Summary Report\n\n&8&\"Bold\"Provider:" + headerModel.ProviderName + "\nUKPRN: " + headerModel.Ukprn + "\nILR File:" + headerModel.IlrFile);
            pageSetup.SetHeader(2, "&10&\"Bold\"OFFICIAL-SENSITIVE\n\n&8&\"Bold\"Year:" + Constants.Year);
            pageSetup.SetFooter(0, $"&8Component Set Version: \t{footerModel.ComponentSetVersion}\nApplication Version: \t{footerModel.ApplicationVersion}\nFile Prepartion Date: \t{footerModel.FilePreparationDate}\n{footerModel.ReportGeneratedAt}");
            pageSetup.SetFooter(2, $"&8Lars Data: \t{footerModel.LarsData}\nPostcode Data: \t{footerModel.PostcodeData}\n&8Large Employer Data: \t{footerModel.LargeEmployerData}\nOrganisation Data: \t{footerModel.OrganisationData}");
        }

        private void PopulateWorkbook(Workbook workbook, HNSSummaryModel model, bool isFis)
        {
            Worksheet worksheet = workbook.Worksheets[0];
            Cells cells = worksheet.Cells;
            if (!isFis)
            {
                cells[FundLine1416LearnDelFamACellName].PutValue(model.TotalDirectFunded1416_WithEHCP);
                cells[FundLine1416LearnDelFamBCellName].PutValue(model.TotalDirectFunded1416_WithoutEHCP);
                cells[FundLine1416LearnDelFamCCellName].PutValue(model.TotalDirectFunded1416_HNSWithEHCP);
                cells[FundLine1416LearnDelFamDCellName].PutValue(model.TotalDirectFunded1416_HNSWithoutEHCP);
                cells[FundLine1416LearnDelFamECellName].PutValue(model.TotalDirectFunded1416_EHCPWithoutHNS);

                cells[FundLine1619LearnDelFamACellName].PutValue(model.Total1619IncludingHNS_WithEHCP);
                cells[FundLine1619LearnDelFamBCellName].PutValue(model.Total1619IncludingHNS_WithoutEHCP);
                cells[FundLine1619LearnDelFamCCellName].PutValue(model.Total1619IncludingHNS_HNSWithEHCP);
                cells[FundLine1619LearnDelFamDCellName].PutValue(model.Total1619IncludingHNS_HNSWithoutEHCP);
                cells[FundLine1619LearnDelFamECellName].PutValue(model.Total1619IncludingHNS_EHCPWithoutHNS);

                cells[FundLine1924LearnDelFamACellName].PutValue(model.Total1924WithEHCP_WithEHCP);
                cells[FundLine1924LearnDelFamBCellName].PutValue(model.Total1924WithEHCP_WithoutEHCP);
                cells[FundLine1924LearnDelFamCCellName].PutValue(model.Total1924WithEHCP_HNSWithEHCP);
                cells[FundLine1924LearnDelFamDCellName].PutValue(model.Total1924WithEHCP_HNSWithoutEHCP);
                cells[FundLine1924LearnDelFamECellName].PutValue(model.Total1924WithEHCP_EHCPWithoutHNS);

                cells[FundLine19PlusLearnDelFamACellName].PutValue(model.Total19PlusWithoutEHCP_WithEHCP);
                cells[FundLine19PlusLearnDelFamBCellName].PutValue(model.Total19PlusWithoutEHCP_WithoutEHCP);
                cells[FundLine19PlusLearnDelFamCCellName].PutValue(model.Total19PlusWithoutEHCP_HNSWithEHCP);
                cells[FundLine19PlusLearnDelFamDCellName].PutValue(model.Total19PlusWithoutEHCP_HNSWithoutEHCP);
                cells[FundLine19PlusLearnDelFamECellName].PutValue(model.Total19PlusWithoutEHCP_EHCPWithoutHNS);
            }
        }

       private async Task<HNSSummaryHeaderModel> GetHeaderAsync(IReportServiceContext reportServiceContext, Task<string> providerNameTask, CancellationToken cancellationToken, bool isFis)
        {
            var ilrFileName = reportServiceContext.OriginalFilename ?? reportServiceContext.Filename;

            HNSSummaryHeaderModel headerModel = new HNSSummaryHeaderModel
            {
                ProviderName = providerNameTask.Result ?? "Unknown",
                Ukprn = reportServiceContext.Ukprn,
                IlrFile = ilrFileName,
                Year = Constants.Year
            };

            return headerModel;
        }

        private async Task<HNSSummaryFooterModel> GetFooterAsync(Task<IMessage> messageTask, Task<ILRSourceFileInfo> lastSubmittedIlrFileTask, CancellationToken cancellationToken)
        {
            DateTime dateTimeNowUtc = _dateTimeProvider.GetNowUtc();
            DateTime dateTimeNowUk = _dateTimeProvider.ConvertUtcToUk(dateTimeNowUtc);
            HNSSummaryFooterModel footerModel = new HNSSummaryFooterModel
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
                footerModel.FilePreparationDate = messageTask.Result.HeaderEntity.CollectionDetailsEntity.FilePreparationDate.ToString("dd/MM/yyyy");
            }
            else
            {
                footerModel.FilePreparationDate = lastSubmittedIlrFileTask.Result != null ?
                    lastSubmittedIlrFileTask.Result.FilePreparationDate.GetValueOrDefault().ToString("dd/MM/yyyy") : string.Empty;
            }

            return footerModel;
        }
    }
}