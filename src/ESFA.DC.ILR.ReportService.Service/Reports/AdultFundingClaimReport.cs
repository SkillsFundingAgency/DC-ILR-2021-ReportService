﻿using System.Collections.Generic;
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
using ESFA.DC.ILR.ReportService.Interface;
using ESFA.DC.ILR.ReportService.Interface.Builders;
using ESFA.DC.ILR.ReportService.Interface.Configuration;
using ESFA.DC.ILR.ReportService.Interface.Provider;
using ESFA.DC.ILR.ReportService.Interface.Service;
using ESFA.DC.ILR.ReportService.Model.Eas;
using ESFA.DC.ILR.ReportService.Model.ReportModels;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Reports.Abstract;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ReportTaskNameConstants = ESFA.DC.ILR.ReportService.Interface.ReportTaskNameConstants;

namespace ESFA.DC.ILR.ReportService.Service.Reports
{
    public sealed class AdultFundingClaimReport : AbstractReport, ILegacyReport
    {
        private const string ProviderNameCellName = "D5";
        private const string UkprnCellName = "D6";
        private const string IlrFileCellName = "D7";
        private const string YearCellName = "G7";

        private const string OtherLearningProgrammeFunding6MonthsCellName = "F11";
        private const string OtherLearningProgrammeFunding10MonthsCellName = "G11";
        private const string OtherLearningProgrammeFunding12MonthsCellName = "H11";

        private const string OtherLearningLearningSupport6MonthsCellName = "F12";
        private const string OtherLearningLearningSupport10MonthsCellName = "G12";
        private const string OtherLearningLearningSupport12MonthsCellName = "H12";

        private const string Traineeships1924ProgrammeFunding6MonthsCellName = "F13";
        private const string Traineeships1924ProgrammeFunding10MonthsCellName = "G13";
        private const string Traineeships1924ProgrammeFunding12MonthsCellName = "H13";

        private const string Traineeships1924LearningSupport6MonthsCellName = "F14";
        private const string Traineeships1924LearningSupport10MonthsCellName = "G14";
        private const string Traineeships1924LearningSupport12MonthsCellName = "H14";

        private const string Traineeships1924LearnerSupport6MonthsCellName = "F15";
        private const string Traineeships1924LearnerSupport10MonthsCellName = "G15";
        private const string Traineeships1924LearnerSupport12MonthsCellName = "H15";

        private const string LoansBursaryFunding6MonthsCellName = "F20";
        private const string LoansBursaryFunding10MonthsCellName = "G20";
        private const string LoansBursaryFunding12MonthsCellName = "H20";

        private const string LoansAreaCosts6MonthsCellName = "F21";
        private const string LoansAreaCosts10MonthsCellName = "G21";
        private const string LoansAreaCosts12MonthsCellName = "H21";

        private const string LoansExcessSupport6MonthsCellName = "F22";
        private const string LoansExcessSupport10MonthsCellName = "G22";
        private const string LoansExcessSupport12MonthsCellName = "H22";

        private const string ComponentSetVersionCellName = "D36";
        private const string ApplicationVersionCellName = "D37";
        private const string FilePreparationDateCellName = "D38";
        private const string LarsDataCellName = "G35";
        private const string OrganisationDataCellName = "G36";
        private const string PostcodeDataCellName = "G37";
        private const string LargeEmployerDataCellName = "G38";
        private const string ReportGeneratedAtCellName = "B40";

        private const string OtherLearningProgrammeFunding6MonthsCellNameFis = "F11";
        private const string OtherLearningProgrammeFunding10MonthsCellNameFis = "G11";
        private const string OtherLearningProgrammeFunding12MonthsCellNameFis = "H11";

        private const string OtherLearningLearningSupport6MonthsCellNameFis = "F13";
        private const string OtherLearningLearningSupport10MonthsCellNameFis = "G13";
        private const string OtherLearningLearningSupport12MonthsCellNameFis = "H13";

        private const string Traineeships1924ProgrammeFunding6MonthsCellNameFis = "F14";
        private const string Traineeships1924ProgrammeFunding10MonthsCellNameFis = "G14";
        private const string Traineeships1924ProgrammeFunding12MonthsCellNameFis = "H14";

        private const string Traineeships1924LearningSupport6MonthsCellNameFis = "F15";
        private const string Traineeships1924LearningSupport10MonthsCellNameFis = "G15";
        private const string Traineeships1924LearningSupport12MonthsCellNameFis = "H15";

        private const string Traineeships1924LearnerSupport6MonthsCellNameFis = "F16";
        private const string Traineeships1924LearnerSupport10MonthsCellNameFis = "G16";
        private const string Traineeships1924LearnerSupport12MonthsCellNameFis = "H16";

        private const string LoansBursaryFunding6MonthsCellNameFis = "F21";
        private const string LoansBursaryFunding10MonthsCellNameFis = "G21";
        private const string LoansBursaryFunding12MonthsCellNameFis = "H21";

        private const string LoansAreaCosts6MonthsCellNameFis = "F22";
        private const string LoansAreaCosts10MonthsCellNameFis = "G22";
        private const string LoansAreaCosts12MonthsCellNameFis = "H22";

        private const string LoansExcessSupport6MonthsCellNameFis = "F23";
        private const string LoansExcessSupport10MonthsCellNameFis = "G23";
        private const string LoansExcessSupport12MonthsCellNameFis = "H23";

        private const string ComponentSetVersionCellNameFis = "D36";
        private const string ApplicationVersionCellNameFis = "D37";
        private const string FilePreparationDateCellNameFis = "D38";
        private const string LarsDataCellNameFis = "F36";
        private const string OrganisationDataCellNameFis = "F37";
        private const string PostcodeDataCellNameFis = "F38";
        private const string LargeEmployerDataCellNameFis = "F39";
        private const string ReportGeneratedAtCellNameFis = "B40";

        private readonly IIlrProviderService _ilrProviderService;
        private readonly IIlrMetadataProviderService _ilrMetadataProviderService;
        private readonly IOrgProviderService _orgProviderService;
        private readonly IAllbProviderService _allbProviderService;
        private readonly IFM35ProviderService _fm35ProviderService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IIntUtilitiesService _intUtilitiesService;
        private readonly ILarsProviderService _larsProviderService;
        private readonly IEasProviderService _easProviderService;
        private readonly IPostcodeProviderService _postcodeProviderService;
        private readonly ILargeEmployerProviderService _largeEmployerProviderService;
        private readonly IVersionInfo _versionInfo;
        private readonly IAdultFundingClaimBuilder _adultFundingClaimBuilder;

        public AdultFundingClaimReport(
            ILogger logger,
            IStreamableKeyValuePersistenceService streamableKeyValuePersistenceService,
            IIlrProviderService ilrProviderService,
            IIlrMetadataProviderService ilrMetadataProviderService,
            IOrgProviderService orgProviderService,
            IAllbProviderService allbProviderService,
            IFM35ProviderService fm35ProviderService,
            IDateTimeProvider dateTimeProvider,
            IIntUtilitiesService intUtilitiesService,
            IValueProvider valueProvider,
            ILarsProviderService larsProviderService,
            IEasProviderService easProviderService,
            IPostcodeProviderService postcodeProviderService,
            ILargeEmployerProviderService largeEmployerProviderService,
            IVersionInfo versionInfo,
            IAdultFundingClaimBuilder adultFundingClaimBuilder)
            : base(dateTimeProvider, valueProvider, streamableKeyValuePersistenceService, logger)
        {
            _ilrProviderService = ilrProviderService;
            _ilrMetadataProviderService = ilrMetadataProviderService;
            _orgProviderService = orgProviderService;
            _allbProviderService = allbProviderService;
            _fm35ProviderService = fm35ProviderService;
            _larsProviderService = larsProviderService;
            _easProviderService = easProviderService;
            _postcodeProviderService = postcodeProviderService;
            _largeEmployerProviderService = largeEmployerProviderService;
            _versionInfo = versionInfo;
            _dateTimeProvider = dateTimeProvider;
            _intUtilitiesService = intUtilitiesService;
            _adultFundingClaimBuilder = adultFundingClaimBuilder;
        }

        public override string ReportFileName => "Adult Funding Claim Report";

        public override string ReportTaskName => ReportTaskNameConstants.AdultFundingClaimReport;

        public override async Task GenerateReport(IReportServiceContext reportServiceContext, ZipArchive archive, bool isFis, CancellationToken cancellationToken)
        {
            Task<IMessage> ilrFileTask = reportServiceContext.CollectionName == "ILR1819" ? _ilrProviderService.GetIlrFile(reportServiceContext, cancellationToken) : null;
            Task<string> providerNameTask = _orgProviderService.GetProviderName(reportServiceContext, cancellationToken);
            Task<List<EasSubmissionValues>> easSubmissionValuesAsync = _easProviderService.GetEasSubmissionValuesAsync(reportServiceContext, cancellationToken);
            Task<FM35Global> fm35Task = _fm35ProviderService.GetFM35Data(reportServiceContext, cancellationToken);
            Task<ALBGlobal> albGlobalTask = _allbProviderService.GetAllbData(reportServiceContext, cancellationToken);
            var lastSubmittedIlrFileTask = _ilrMetadataProviderService.GetLastSubmittedIlrFile(reportServiceContext, cancellationToken);

            var organisationDataTask = _orgProviderService.GetVersionAsync(cancellationToken);
            var largeEmployerDataTask = _largeEmployerProviderService.GetVersionAsync(cancellationToken);
            var larsDataTask = _larsProviderService.GetVersionAsync(cancellationToken);
            var postcodeDataTask = _postcodeProviderService.GetVersionAsync(cancellationToken);

            await Task.WhenAll(
                easSubmissionValuesAsync,
                fm35Task,
                albGlobalTask,
                providerNameTask,
                ilrFileTask ?? Task.CompletedTask,
                lastSubmittedIlrFileTask,
                organisationDataTask,
                largeEmployerDataTask,
                larsDataTask,
                postcodeDataTask);

            var fundingClaimModel = _adultFundingClaimBuilder.BuildAdultFundingClaimModel(
                _logger,
                reportServiceContext,
                fm35Task.Result,
                easSubmissionValuesAsync.Result,
                albGlobalTask.Result,
                providerNameTask.Result,
                lastSubmittedIlrFileTask.Result,
                _dateTimeProvider,
                _intUtilitiesService,
                ilrFileTask?.Result,
                _versionInfo,
                organisationDataTask.Result,
                largeEmployerDataTask.Result,
                postcodeDataTask.Result,
                larsDataTask.Result);

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            var externalFileName = GetFilename(reportServiceContext);
            var fileName = GetZipFilename(reportServiceContext);

            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith("AdultFundingClaimReportTemplate.xlsx"));
            var manifestResourceStream = assembly.GetManifestResourceStream(resourceName);
            Workbook workbook = new Workbook(manifestResourceStream);
            PopulateWorkbook(workbook, fundingClaimModel, isFis);
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Save(ms, SaveFormat.Xlsx);
                await _streamableKeyValuePersistenceService.SaveAsync($"{externalFileName}.xlsx", ms, cancellationToken);
                await WriteZipEntry(archive, $"{fileName}.xlsx", ms, cancellationToken);
            }
        }

        private void PopulateWorkbook(Workbook workbook, AdultFundingClaimModel adultFundingClaimModel, bool isFis)
        {
            Worksheet worksheet = workbook.Worksheets[0];
            Cells cells = worksheet.Cells;
            cells[ProviderNameCellName].PutValue(adultFundingClaimModel.ProviderName);
            cells[UkprnCellName].PutValue(adultFundingClaimModel.Ukprn);
            cells[IlrFileCellName].PutValue(adultFundingClaimModel.IlrFile);
            if (!isFis)
            {
                cells.DeleteRow(8);
                cells[OtherLearningProgrammeFunding6MonthsCellName].PutValue(adultFundingClaimModel.OtherLearningProgrammeFunding6Months);
                cells[OtherLearningProgrammeFunding10MonthsCellName].PutValue(adultFundingClaimModel.OtherLearningProgrammeFunding10Months);
                cells[OtherLearningProgrammeFunding12MonthsCellName].PutValue(adultFundingClaimModel.OtherLearningProgrammeFunding12Months);
                cells[OtherLearningLearningSupport6MonthsCellName].PutValue(adultFundingClaimModel.OtherLearningLearningSupport6Months);
                cells[OtherLearningLearningSupport10MonthsCellName].PutValue(adultFundingClaimModel.OtherLearningLearningSupport10Months);
                cells[OtherLearningLearningSupport12MonthsCellName].PutValue(adultFundingClaimModel.OtherLearningLearningSupport12Months);
                cells[Traineeships1924ProgrammeFunding6MonthsCellName].PutValue(adultFundingClaimModel.Traineeships1924ProgrammeFunding6Months);
                cells[Traineeships1924ProgrammeFunding10MonthsCellName].PutValue(adultFundingClaimModel.Traineeships1924ProgrammeFunding10Months);
                cells[Traineeships1924ProgrammeFunding12MonthsCellName].PutValue(adultFundingClaimModel.Traineeships1924ProgrammeFunding12Months);
                cells[Traineeships1924LearningSupport6MonthsCellName].PutValue(adultFundingClaimModel.Traineeships1924LearningSupport6Months);
                cells[Traineeships1924LearningSupport10MonthsCellName].PutValue(adultFundingClaimModel.Traineeships1924LearningSupport10Months);
                cells[Traineeships1924LearningSupport12MonthsCellName].PutValue(adultFundingClaimModel.Traineeships1924LearningSupport12Months);
                cells[Traineeships1924LearnerSupport6MonthsCellName].PutValue(adultFundingClaimModel.Traineeships1924LearnerSupport6Months);
                cells[Traineeships1924LearnerSupport10MonthsCellName].PutValue(adultFundingClaimModel.Traineeships1924LearnerSupport10Months);
                cells[Traineeships1924LearnerSupport12MonthsCellName].PutValue(adultFundingClaimModel.Traineeships1924LearnerSupport12Months);
                cells[LoansBursaryFunding6MonthsCellName].PutValue(adultFundingClaimModel.LoansBursaryFunding6Months);
                cells[LoansBursaryFunding10MonthsCellName].PutValue(adultFundingClaimModel.LoansBursaryFunding10Months);
                cells[LoansBursaryFunding12MonthsCellName].PutValue(adultFundingClaimModel.LoansBursaryFunding12Months);
                cells[LoansAreaCosts6MonthsCellName].PutValue(adultFundingClaimModel.LoansAreaCosts6Months);
                cells[LoansAreaCosts10MonthsCellName].PutValue(adultFundingClaimModel.LoansAreaCosts10Months);
                cells[LoansAreaCosts12MonthsCellName].PutValue(adultFundingClaimModel.LoansAreaCosts12Months);
                cells[LoansExcessSupport6MonthsCellName].PutValue(adultFundingClaimModel.LoansExcessSupport6Months);
                cells[LoansExcessSupport10MonthsCellName].PutValue(adultFundingClaimModel.LoansExcessSupport10Months);
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
            else
            {
                cells[OtherLearningProgrammeFunding6MonthsCellNameFis].PutValue(adultFundingClaimModel.OtherLearningProgrammeFunding6Months);
                cells[OtherLearningProgrammeFunding10MonthsCellNameFis].PutValue(adultFundingClaimModel.OtherLearningProgrammeFunding10Months);
                cells[OtherLearningProgrammeFunding12MonthsCellNameFis].PutValue(adultFundingClaimModel.OtherLearningProgrammeFunding12Months);
                cells[OtherLearningLearningSupport6MonthsCellNameFis].PutValue(adultFundingClaimModel.OtherLearningLearningSupport6Months);
                cells[OtherLearningLearningSupport10MonthsCellNameFis].PutValue(adultFundingClaimModel.OtherLearningLearningSupport10Months);
                cells[OtherLearningLearningSupport12MonthsCellNameFis].PutValue(adultFundingClaimModel.OtherLearningLearningSupport12Months);
                cells[Traineeships1924ProgrammeFunding6MonthsCellNameFis].PutValue(adultFundingClaimModel.Traineeships1924ProgrammeFunding6Months);
                cells[Traineeships1924ProgrammeFunding10MonthsCellNameFis].PutValue(adultFundingClaimModel.Traineeships1924ProgrammeFunding10Months);
                cells[Traineeships1924ProgrammeFunding12MonthsCellNameFis].PutValue(adultFundingClaimModel.Traineeships1924ProgrammeFunding12Months);
                cells[Traineeships1924LearningSupport6MonthsCellNameFis].PutValue(adultFundingClaimModel.Traineeships1924LearningSupport6Months);
                cells[Traineeships1924LearningSupport10MonthsCellNameFis].PutValue(adultFundingClaimModel.Traineeships1924LearningSupport10Months);
                cells[Traineeships1924LearningSupport12MonthsCellNameFis].PutValue(adultFundingClaimModel.Traineeships1924LearningSupport12Months);
                cells[Traineeships1924LearnerSupport6MonthsCellNameFis].PutValue(adultFundingClaimModel.Traineeships1924LearnerSupport6Months);
                cells[Traineeships1924LearnerSupport10MonthsCellNameFis].PutValue(adultFundingClaimModel.Traineeships1924LearnerSupport10Months);
                cells[Traineeships1924LearnerSupport12MonthsCellNameFis].PutValue(adultFundingClaimModel.Traineeships1924LearnerSupport12Months);
                cells[LoansBursaryFunding6MonthsCellNameFis].PutValue(adultFundingClaimModel.LoansBursaryFunding6Months);
                cells[LoansBursaryFunding10MonthsCellNameFis].PutValue(adultFundingClaimModel.LoansBursaryFunding10Months);
                cells[LoansBursaryFunding12MonthsCellNameFis].PutValue(adultFundingClaimModel.LoansBursaryFunding12Months);
                cells[LoansAreaCosts6MonthsCellNameFis].PutValue(adultFundingClaimModel.LoansAreaCosts6Months);
                cells[LoansAreaCosts10MonthsCellNameFis].PutValue(adultFundingClaimModel.LoansAreaCosts10Months);
                cells[LoansAreaCosts12MonthsCellNameFis].PutValue(adultFundingClaimModel.LoansAreaCosts12Months);
                cells[LoansExcessSupport6MonthsCellNameFis].PutValue(adultFundingClaimModel.LoansExcessSupport6Months);
                cells[LoansExcessSupport10MonthsCellNameFis].PutValue(adultFundingClaimModel.LoansExcessSupport10Months);
                cells[LoansExcessSupport12MonthsCellNameFis].PutValue(adultFundingClaimModel.LoansExcessSupport12Months);
                cells[ComponentSetVersionCellNameFis].PutValue(adultFundingClaimModel.ComponentSetVersion);
                cells[ApplicationVersionCellNameFis].PutValue(adultFundingClaimModel.ApplicationVersion);
                cells[FilePreparationDateCellNameFis].PutValue(adultFundingClaimModel.FilePreparationDate);
                cells[LarsDataCellNameFis].PutValue(adultFundingClaimModel.LarsData);
                cells[OrganisationDataCellNameFis].PutValue(adultFundingClaimModel.OrganisationData);
                cells[PostcodeDataCellNameFis].PutValue(adultFundingClaimModel.PostcodeData);
                cells[LargeEmployerDataCellNameFis].PutValue(adultFundingClaimModel.LargeEmployerData);
                cells[ReportGeneratedAtCellNameFis].PutValue(adultFundingClaimModel.ReportGeneratedAt);
            }

            workbook.CalculateFormula();
        }
    }
}