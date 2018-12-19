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
using ESFA.DC.ILR1819.ReportService.Interface.Configuration;
using ESFA.DC.ILR1819.ReportService.Interface.Context;
using ESFA.DC.ILR1819.ReportService.Interface.Reports;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.ILR;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.ILR1819.ReportService.Service.Reports
{
    public sealed class FundingClaim1619Report : AbstractReportBuilder, IReport
    {
        private const string RetentionFactorCellName = "D3";
        private const string ProgrammeCostWeightingCellName = "D4";
        private const string AreaCostAllowanceCellName = "D5";
        private const string DisAdvProportionCellName = "D6";
        private const string LargeProgrammeProportionCellName = "D7";

        private List<Tuple<string, string, string, string>> CellPositions = new List<Tuple<string, string, string, string>>()
        {
            new Tuple<string, string, string, string>("14-16 Direct Funded Students", "540+ hours (Band 5)", "E10", string.Empty),
            new Tuple<string, string, string, string>("14-16 Direct Funded Students", "450+ hours (Band 4a)", "E11", string.Empty),
            new Tuple<string, string, string, string>("14-16 Direct Funded Students", "450 to 539 hours (Band 4b)", "E12", string.Empty),
            new Tuple<string, string, string, string>("14-16 Direct Funded Students", "360 to 449 hours (Band 3)", "E13", string.Empty),
            new Tuple<string, string, string, string>("14-16 Direct Funded Students", "280 to 359 hours (Band 2)", "E14", string.Empty),
            new Tuple<string, string, string, string>("14-16 Direct Funded Students", "Up to 279 hours (Band 1)", "E15", string.Empty),

            new Tuple<string, string, string, string>("16-19 Students (including High Needs Students)", "540+ hours (Band 5)", "E19", "F19"),
            new Tuple<string, string, string, string>("16-19 Students (including High Needs Students)", "450+ hours (Band 4a)", "E20", "F20"),
            new Tuple<string, string, string, string>("16-19 Students (including High Needs Students)", "450 to 539 hours (Band 4b)", "E21", "F21"),
            new Tuple<string, string, string, string>("16-19 Students (including High Needs Students)", "360 to 449 hours (Band 3)", "E22", "F22"),
            new Tuple<string, string, string, string>("16-19 Students (including High Needs Students)", "280 to 359 hours (Band 2)", "E23", "F23"),
            new Tuple<string, string, string, string>("16-19 Students (including High Needs Students)", "Up to 279 hours (Band 1)", "E24", "F24"),

            new Tuple<string, string, string, string>("19-24 Students with an EHC plan", "540+ hours (Band 5)", "E28", "F28"),
            new Tuple<string, string, string, string>("19-24 Students with an EHC plan", "450+ hours (Band 4a)", "E29", "F29"),
            new Tuple<string, string, string, string>("19-24 Students with an EHC plan", "450 to 539 hours (Band 4b)", "E30", "F30"),
            new Tuple<string, string, string, string>("19-24 Students with an EHC plan", "360 to 449 hours (Band 3)", "E31", "F31"),
            new Tuple<string, string, string, string>("19-24 Students with an EHC plan", "280 to 359 hours (Band 2)", "E32", "F32"),
            new Tuple<string, string, string, string>("19-24 Students with an EHC plan", "Up to 279 hours (Band 1)", "E33", "F33"),

            new Tuple<string, string, string, string>("19+ Continuing Students (excluding EHC plans)", "540+ hours (Band 5)", "E37", "F37"),
            new Tuple<string, string, string, string>("19+ Continuing Students (excluding EHC plans)", "450+ hours (Band 4a)", "E38", "F38"),
            new Tuple<string, string, string, string>("19+ Continuing Students (excluding EHC plans)", "450 to 539 hours (Band 4b)", "E39", "F39"),
            new Tuple<string, string, string, string>("19+ Continuing Students (excluding EHC plans)", "360 to 449 hours (Band 3)", "E40", "F40"),
            new Tuple<string, string, string, string>("19+ Continuing Students (excluding EHC plans)", "280 to 359 hours (Band 2)", "E41", "F41"),
            new Tuple<string, string, string, string>("19+ Continuing Students (excluding EHC plans)", "Up to 279 hours (Band 1)", "E42", "F42")
        };

        private readonly ILogger _logger;
        private readonly IStreamableKeyValuePersistenceService _storage;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IIlrProviderService _ilrProviderService;
        private readonly IOrgProviderService _orgProviderService;
        private readonly IFM25ProviderService _fm25ProviderService;
        private readonly IPostcodeProviderService _postcodeProviderService;
        private readonly ILargeEmployerProviderService _largeEmployerProviderService;
        private readonly ILarsProviderService _larsProviderService;
        private readonly IVersionInfo _versionInfo;

        private readonly Dictionary<string, string[]> _applicableFundingLineTypes = new Dictionary<string, string[]>
        {
            { "14-16 Direct Funded Students", new[] { "14-16 Direct Funded Students" } },
            { "16-19 Students (including High Needs Students)", new[] { "16-19 Students (excluding High Needs Students)", "16-19 High Needs Students" } },
            { "19-24 Students with an EHC plan", new[] { "19-24 Students with an EHCP" } },
            { "19+ Continuing Students (excluding EHC plans)", new[] { "19+ Continuing Students (excluding EHCP)" } }
        };

        public FundingClaim1619Report(
            ILogger logger,
            IStreamableKeyValuePersistenceService storage,
            IDateTimeProvider dateTimeProvider,
            IValueProvider valueProvider,
            IIlrProviderService ilrProviderService,
            IOrgProviderService orgProviderService,
            IFM25ProviderService fm25ProviderService,
            IPostcodeProviderService postcodeProviderService,
            ILargeEmployerProviderService largeEmployerProviderService,
            ILarsProviderService larsProviderService,
            IVersionInfo versionInfo,
            ITopicAndTaskSectionOptions topicAndTaskSectionOptions)
            : base(dateTimeProvider, valueProvider)
        {
            _logger = logger;
            _storage = storage;
            _dateTimeProvider = dateTimeProvider;
            _ilrProviderService = ilrProviderService;
            _orgProviderService = orgProviderService;
            _fm25ProviderService = fm25ProviderService;
            _postcodeProviderService = postcodeProviderService;
            _largeEmployerProviderService = largeEmployerProviderService;
            _larsProviderService = larsProviderService;
            _versionInfo = versionInfo;

            ReportFileName = "16-19 Funding Claim Report";
            ReportTaskName = topicAndTaskSectionOptions.TopicReports_TaskGenerateFundingClaim1619Report;
        }

        public async Task GenerateReport(IReportServiceContext reportServiceContext, ZipArchive archive, bool isFis, CancellationToken cancellationToken)
        {
            Task<IMessage> ilrFileTask = _ilrProviderService.GetIlrFile(reportServiceContext, cancellationToken);
            Task<FM25Global> fm25DataTask = _fm25ProviderService.GetFM25Data(reportServiceContext, cancellationToken);
            Task<string> providerNameTask = _orgProviderService.GetProviderName(reportServiceContext, cancellationToken);
            Task<decimal?> cofRemovalTask = _orgProviderService.GetCofRemoval(reportServiceContext, cancellationToken);
            Task<ILRSourceFileInfo> lastSubmittedIlrFileTask = _ilrProviderService.GetLastSubmittedIlrFile(reportServiceContext, cancellationToken);

            await Task.WhenAll(ilrFileTask, fm25DataTask, providerNameTask, cofRemovalTask, lastSubmittedIlrFileTask);

            FundingClaim1619HeaderModel fundingSummaryHeaderModel = await GetHeaderAsync(reportServiceContext, ilrFileTask, lastSubmittedIlrFileTask, providerNameTask, cancellationToken, isFis);
            FundingClaim1619FooterModel fundingSummaryFooterModel = await GetFooterAsync(ilrFileTask, lastSubmittedIlrFileTask, cancellationToken);

            string[] applicableFundingLineTypes = _applicableFundingLineTypes.Values.SelectMany(x => x).ToArray();

            // Funding factors are at provider level but are output by the funding calculation against every learner record.
            // All learners in one provider will have the same funding factor value so the report can pull the minimum value
            // of each factor from the funding calc output at provider level to populate the factors table.
            FundingClaim1619FundingFactorModel fundingClaim1619FundingFactorModel = null;

            Dictionary<string, List<FundingClaim1619Model>> fundLineAndRateBandData = new Dictionary<string, List<FundingClaim1619Model>>();

            foreach (ILearner learner in ilrFileTask.Result.Learners)
            {
                FM25Learner learnerFm25Data =
                    fm25DataTask.Result?.Learners?.SingleOrDefault(l => string.Equals(l.LearnRefNumber, learner.LearnRefNumber, StringComparison.OrdinalIgnoreCase));

                if (learnerFm25Data == null)
                {
                    continue;
                }

                if (!ApplicableLearner(learnerFm25Data, applicableFundingLineTypes))
                {
                    continue;
                }

                if (fundingClaim1619FundingFactorModel == null)
                {
                    fundingClaim1619FundingFactorModel = new FundingClaim1619FundingFactorModel
                    {
                        AreaCostFact1618Hist = learnerFm25Data.AreaCostFact1618Hist.GetValueOrDefault(0).ToString("N3"),
                        ProgWeightHist = learnerFm25Data.ProgWeightHist.GetValueOrDefault(0).ToString("N3"),
                        PrvDisadvPropnHist = learnerFm25Data.PrvDisadvPropnHist.GetValueOrDefault(0).ToString("N3"),
                        PrvHistLrgProgPropn = learnerFm25Data.PrvHistLrgProgPropn.GetValueOrDefault(0).ToString("N3"),
                        PrvRetentFactHist = learnerFm25Data.PrvRetentFactHist.GetValueOrDefault(0).ToString("N3")
                    };
                }

                if (learner.LearningDeliveries.Any(ApplicableLearningDelivery))
                {
                    GetFundLineAndBandRateData(learnerFm25Data, ref fundLineAndRateBandData);
                }
            }

            // probably don't need this as this will be calculated automatically through the template
            //FundingClaim1619Model subTotal = TotalFundLineAndBandRateData(ref fundLineAndRateBandData);

            // probably don't need this as this will be calculated automatically through the template
            //FundingClaim1619Model total = new FundingClaim1619Model
            //{
            //    StudentNumber = subTotal.StudentNumber,
            //    TotalFunding = 0 // Todo
            //};

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            long jobId = reportServiceContext.JobId;
            string ukPrn = reportServiceContext.Ukprn.ToString();
            var externalFileName = GetExternalFilename(ukPrn, jobId, reportServiceContext.SubmissionDateTimeUtc);
            var fileName = GetFilename(ukPrn, jobId, reportServiceContext.SubmissionDateTimeUtc);

            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith("1619FundingClaimReportTemplate.xlsx"));
            var manifestResourceStream = assembly.GetManifestResourceStream(resourceName);
            Workbook workbook = new Workbook(manifestResourceStream);
            Worksheet worksheet = workbook.Worksheets[0];
            Cells cells = worksheet.Cells;
            InsertHeaderFooter(workbook, fundingSummaryHeaderModel, fundingSummaryFooterModel);
            PopulateAllocationValues(cells, fundingClaim1619FundingFactorModel);
            PopulateMainData(cells, fundLineAndRateBandData);
            PopulateCofRemoval(cells, cofRemovalTask.Result);
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Save(ms, SaveFormat.Xlsx);
                await _storage.SaveAsync($"{externalFileName}.xlsx", ms, cancellationToken);
                await WriteZipEntry(archive, $"{fileName}.xlsx", ms, cancellationToken);
            }
        }

        private void InsertHeaderFooter(Workbook workbook, FundingClaim1619HeaderModel headerModel, FundingClaim1619FooterModel footerModel)
        {
            PageSetup pageSetup = workbook.Worksheets[0].PageSetup;
            pageSetup.SetHeader(0, "&14&\"Bold\"16-19 Funding Claim Report&8\n\nProvider: " + headerModel.ProviderName + "\nUKPRN: " + headerModel.Ukprn + "\nILR File: " + headerModel.IlrFile);
            pageSetup.SetHeader(1, string.Empty);
            pageSetup.SetHeader(2, "&12&\"Bold\"OFFICIAL-SENSITIVE\n\n&8Reference Data: All\nCampus Identifier: All\nYear: 20118/19");

            pageSetup.SetFooter(0, "&8Component Set Version: \t" + footerModel.ComponentSetVersion +
                                   "\nApplication Version: \t" + footerModel.ApplicationVersion +
                                   "\nFile Prepartion Date: \t" + footerModel.FilePreparationDate +
                                   "\n\n\n" + footerModel.ReportGeneratedAt); // .Replace(" on ", " on \n"
            pageSetup.SetFooter(2, "&8Lars Data: \t" + footerModel.LarsData +
                                   "\nOrganisation Data: \t" + footerModel.OrganisationData +
                                   "\nPostcode Data: \t" + footerModel.PostcodeData +
                                   "\nLarge Employer Data: \t" + footerModel.LargeEmployerData +
                                   "\nCoF Removal Data: \t" + footerModel.CofRemovalData);
        }

        private void PopulateCofRemoval(Cells cells, decimal? cofRemoval)
        {
            // populating cof removal
            cells["F47"].PutValue(cofRemoval);
        }

        private void PopulateAllocationValues(Cells cells, FundingClaim1619FundingFactorModel fundingClaimFactorModel)
        {
            // populating allocation values
            cells[RetentionFactorCellName].PutValue(fundingClaimFactorModel.PrvRetentFactHist);
            cells[ProgrammeCostWeightingCellName].PutValue(fundingClaimFactorModel.ProgWeightHist);
            cells[AreaCostAllowanceCellName].PutValue(fundingClaimFactorModel.AreaCostFact1618Hist);
            cells[DisAdvProportionCellName].PutValue(fundingClaimFactorModel.PrvDisadvPropnHist);
            cells[LargeProgrammeProportionCellName].PutValue(fundingClaimFactorModel.PrvHistLrgProgPropn);
        }

        private void PopulateMainData(Cells cells, Dictionary<string, List<FundingClaim1619Model>> fundingClaimModels)
        {
            foreach (var model in fundingClaimModels)
            {
                foreach (var singleClaim1619Model in model.Value)
                {
                    PopulateStudentNumberAndTotalFundingCells(cells, model.Key, singleClaim1619Model.Title, singleClaim1619Model.StudentNumber, singleClaim1619Model.TotalFunding);
                }
            }
        }

        private void PopulateStudentNumberAndTotalFundingCells(Cells cells, string fundline, string band, int studentNumber, decimal? totalFunding)
        {
            foreach (var cp in CellPositions)
            {
                if (string.Equals(fundline, cp.Item1, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(band, cp.Item2, StringComparison.OrdinalIgnoreCase))
                {
                    cells[cp.Item3].PutValue(studentNumber);
                    cells[cp.Item4].PutValue(totalFunding);
                }
            }
        }

        private FundingClaim1619Model TotalFundLineAndBandRateData(ref Dictionary<string, List<FundingClaim1619Model>> fundLineAndRateBandData)
        {
            int studentNumberSubTotal = 0;
            decimal totalFundingSubTotal = 0;

            foreach (KeyValuePair<string, List<FundingClaim1619Model>> keyValuePair in fundLineAndRateBandData)
            {
                int studentNumber = 0;
                decimal totalFunding = 0;

                foreach (FundingClaim1619Model fundingClaim1619Model in keyValuePair.Value)
                {
                    studentNumber += fundingClaim1619Model.StudentNumber;
                    totalFunding += fundingClaim1619Model.TotalFunding;
                }

                keyValuePair.Value.Add(new FundingClaim1619Model
                {
                    Title = "Sub-Total",
                    StudentNumber = studentNumber,
                    TotalFunding = totalFunding
                });

                studentNumberSubTotal += studentNumber;
                totalFundingSubTotal += totalFunding;
            }

            return new FundingClaim1619Model
            {
                Title = "Sub-Total",
                StudentNumber = studentNumberSubTotal,
                TotalFunding = totalFundingSubTotal
            };
        }

        private void GetFundLineAndBandRateData(FM25Learner learnerFm25Data, ref Dictionary<string, List<FundingClaim1619Model>> fundLineAndRateBandData)
        {
            List<FundingClaim1619Model> models;
            string title = FundLineToTitle(learnerFm25Data.FundLine);
            if (!fundLineAndRateBandData.TryGetValue(title, out models))
            {
                models = new List<FundingClaim1619Model>();
                fundLineAndRateBandData[title] = models;
            }

            FundingClaim1619Model model = models.SingleOrDefault(x =>
                string.Equals(x.Title, learnerFm25Data.RateBand, StringComparison.OrdinalIgnoreCase));
            if (model == null)
            {
                model = new FundingClaim1619Model
                {
                    Title = learnerFm25Data.RateBand
                };
                models.Add(model);
            }

            model.StudentNumber += 1;
            model.TotalFunding += learnerFm25Data.OnProgPayment.GetValueOrDefault(0);
        }

        private string FundLineToTitle(string fundLine)
        {
            foreach (KeyValuePair<string, string[]> applicableFundingLineType in _applicableFundingLineTypes)
            {
                if (applicableFundingLineType.Value.Any(x => string.Equals(x, fundLine, StringComparison.OrdinalIgnoreCase)))
                {
                    return applicableFundingLineType.Key;
                }
            }

            return string.Empty;
        }

        private bool ApplicableLearner(FM25Learner learnerFm25Data, string[] applicableFundingLineTypes)
        {
            return applicableFundingLineTypes.Any(x =>
                        string.Equals(x, learnerFm25Data.FundLine, StringComparison.OrdinalIgnoreCase))
                    && learnerFm25Data.StartFund.GetValueOrDefault(false);
        }

        private bool ApplicableLearningDelivery(ILearningDelivery learningDelivery)
        {
            return learningDelivery.FundModel == 25
                   && learningDelivery.LearningDeliveryFAMs.Any(x =>
                       string.Equals(x.LearnDelFAMType, Constants.LearningDeliveryFAMCodeSOF, StringComparison.OrdinalIgnoreCase) &&
                       string.Equals(x.LearnDelFAMCode, "107", StringComparison.OrdinalIgnoreCase));
        }

        private async Task<FundingClaim1619HeaderModel> GetHeaderAsync(IReportServiceContext reportServiceContext, Task<IMessage> ilrFileTask, Task<ILRSourceFileInfo> lastSubmittedIlrFileTask, Task<string> providerNameTask, CancellationToken cancellationToken, bool isFis)
        {
            string fileName = Path.GetFileName(reportServiceContext.Filename);
            FundingClaim1619HeaderModel fundingClaim1619HeaderModel = new FundingClaim1619HeaderModel
            {
                ProviderName = providerNameTask.Result ?? "Unknown",
                Ukprn = reportServiceContext.Ukprn,
                IlrFile = string.Equals(reportServiceContext.CollectionName, "ILR1819", StringComparison.OrdinalIgnoreCase) ? fileName : "N/A",
                Year = Constants.Year
            };

            return fundingClaim1619HeaderModel;
        }

        private async Task<FundingClaim1619FooterModel> GetFooterAsync(Task<IMessage> messageTask, Task<ILRSourceFileInfo> lastSubmittedIlrFileTask, CancellationToken cancellationToken)
        {
            DateTime dateTimeNowUtc = _dateTimeProvider.GetNowUtc();
            DateTime dateTimeNowUk = _dateTimeProvider.ConvertUtcToUk(dateTimeNowUtc);
            FundingClaim1619FooterModel fundingSummaryFooterModel = new FundingClaim1619FooterModel
            {
                ReportGeneratedAt = "Report generated at " + dateTimeNowUk.ToString("HH:mm:ss") + " on " + dateTimeNowUk.ToString("dd/MM/yyyy"),
                ApplicationVersion = _versionInfo.ServiceReleaseVersion,
                ComponentSetVersion = "NA",
                OrganisationData = await _orgProviderService.GetVersionAsync(cancellationToken),
                LargeEmployerData = await _largeEmployerProviderService.GetVersionAsync(cancellationToken),
                LarsData = await _larsProviderService.GetVersionAsync(cancellationToken),
                PostcodeData = await _postcodeProviderService.GetVersionAsync(cancellationToken),
                CofRemovalData = string.Empty // todo: populate this
            };

            if (messageTask.Result != null)
            {
                fundingSummaryFooterModel.FilePreparationDate = messageTask.Result.HeaderEntity.SourceEntity.DateTime.ToString("dd/MM/yyyy");
            }
            else
            {
                fundingSummaryFooterModel.FilePreparationDate = lastSubmittedIlrFileTask.Result != null ?
                    lastSubmittedIlrFileTask.Result.FilePreparationDate.GetValueOrDefault().ToString("dd/MM/yyyy") : string.Empty;
            }

            return fundingSummaryFooterModel;
        }
    }
}
