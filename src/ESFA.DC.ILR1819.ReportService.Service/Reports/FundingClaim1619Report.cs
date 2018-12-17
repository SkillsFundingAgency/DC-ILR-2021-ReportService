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
        private const string RetentionFactorCellName = "C1";
        private const string ProgrammeCostWeightingCellName = "C2";
        private const string AreaCostAllowanceCellName = "C3";
        private const string DisAdvProportionCellName = "C4";
        private const string LargeProgrammeProportion = "C5";

        private const string DirectFunded1416Band5StudentNumbersCellName = "D8";
        private const string DirectFunded1416Band4aStudentNumbersCellName = "D9";
        private const string DirectFunded1416Band4bStudentNumbersCellName = "D10";
        private const string DirectFunded1416Band3StudentNumbersCellName = "D11";
        private const string DirectFunded1416Band4StudentNumbersCellName = "D12";
        private const string DirectFunded1416Band1StudentNumbersCellName = "D13";
        private const string DirectFunded1416StudentNumbersSubTotal = "D14";
        private const string DirectFunded1416TotalFundingSubTotal = "E14";

        private const string EHCPlan1924Band5StudentNumbersCellName = "D26";
        private const string EHCPlan1924Band4aStudentNumbersCellName = "D27";
        private const string EHCPlan1924Band4bStudentNumbersCellName = "D28";
        private const string EHCPlan1924Band3StudentNumbersCellName = "D29";
        private const string EHCPlan1924Band4StudentNumbersCellName = "D30";
        private const string EHCPlan1924Band1StudentNumbersCellName = "D31";
        private const string EHCPlan1924StudentNumbersSubTotal = "D32";
        private const string EHCPlan1924TotalFundingSubTotal = "E32";

        private const string Continuing19PlusBand5StudentNumbersCellName = "D35";
        private const string Continuing19PlusBand4aStudentNumbersCellName = "D36";
        private const string Continuing19PlusBand4bStudentNumbersCellName = "D37";
        private const string Continuing19PlusBand3StudentNumbersCellName = "D38";
        private const string Continuing19PlusBand4StudentNumbersCellName = "D39";
        private const string Continuing19PlusBand1StudentNumbersCellName = "D40";
        private const string Continuing19PlusStudentNumbersSubTotal = "D41";
        private const string Continuing19PlusTotalFundingSubTotal = "E41";

        private const string OverallStudentNumbersSubTotalCellName = "D43";
        private const string OverallTotalFundingSubTotalCellName = "E43";
        private const string Overall1819FundingRemovalTotalFundingCellName = "E45";
        private const string OverallStudentNumbersTotalCellName = "D47";
        private const string OverallTotalFundingTotalCellName = "E47";

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

            // Todo: Get the 1819 Condition of Funding Removal
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

        private void PopulateCofRemoval(Cells cells, decimal? cofRemoval)
        {
            // populating cof removal
            cells["E45"].PutValue(cofRemoval);
        }

        private void PopulateAllocationValues(Cells cells, FundingClaim1619FundingFactorModel fundingClaimFactorModel)
        {
            // populating allocation values
            cells["C1"].PutValue(fundingClaimFactorModel.PrvRetentFactHist);
            cells["C2"].PutValue(fundingClaimFactorModel.ProgWeightHist);
            cells["C3"].PutValue(fundingClaimFactorModel.AreaCostFact1618Hist);
            cells["C4"].PutValue(fundingClaimFactorModel.PrvDisadvPropnHist);
            cells["C5"].PutValue(fundingClaimFactorModel.PrvHistLrgProgPropn);
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
            if (string.Equals(fundline, "14-16 Direct Funded Students", StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals(band, "540+ hours (Band 5)", StringComparison.OrdinalIgnoreCase))
                {
                    cells["D8"].PutValue(studentNumber);
                }
                else if (string.Equals(band, "450+ hours (Band 4a)", StringComparison.OrdinalIgnoreCase))
                {
                    cells["D9"].PutValue(studentNumber);
                }
                else if (string.Equals(band, "450 to 539 hours (Band 4b)", StringComparison.OrdinalIgnoreCase))
                {
                    cells["D10"].PutValue(studentNumber);
                }
                else if (string.Equals(band, "360 to 449 hours (Band 3)", StringComparison.OrdinalIgnoreCase))
                {
                    cells["D11"].PutValue(studentNumber);
                }
                else if (string.Equals(band, "280 to 359 hours (Band 2)", StringComparison.OrdinalIgnoreCase))
                {
                    cells["D12"].PutValue(studentNumber);
                }
                else if (string.Equals(band, "Up to 279 hours (Band 1)", StringComparison.OrdinalIgnoreCase))
                {
                    cells["D13"].PutValue(studentNumber);
                }
            }
            else if (string.Equals(fundline, "16-19 Students (including High Needs Students)", StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals(band, "540+ hours (Band 5)", StringComparison.OrdinalIgnoreCase))
                {
                    cells["D17"].PutValue(studentNumber);
                    cells["E17"].PutValue(totalFunding);
                }
                else if (string.Equals(band, "450+ hours (Band 4a)", StringComparison.OrdinalIgnoreCase))
                {
                    cells["D18"].PutValue(studentNumber);
                    cells["E18"].PutValue(totalFunding);
                }
                else if (string.Equals(band, "450 to 539 hours (Band 4b)", StringComparison.OrdinalIgnoreCase))
                {
                    cells["D19"].PutValue(studentNumber);
                    cells["E19"].PutValue(totalFunding);
                }
                else if (string.Equals(band, "360 to 449 hours (Band 3)", StringComparison.OrdinalIgnoreCase))
                {
                    cells["D20"].PutValue(studentNumber);
                    cells["E20"].PutValue(totalFunding);
                }
                else if (string.Equals(band, "280 to 359 hours (Band 2)", StringComparison.OrdinalIgnoreCase))
                {
                    cells["D21"].PutValue(studentNumber);
                    cells["E21"].PutValue(totalFunding);
                }
                else if (string.Equals(band, "Up to 279 hours (Band 1)", StringComparison.OrdinalIgnoreCase))
                {
                    cells["D22"].PutValue(studentNumber);
                    cells["E22"].PutValue(totalFunding);
                }
            }
            else if (string.Equals(fundline, "19-24 Students with an EHC plan", StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals(band, "540+ hours (Band 5)", StringComparison.OrdinalIgnoreCase))
                {
                    cells["D26"].PutValue(studentNumber);
                    cells["E26"].PutValue(totalFunding);
                }
                else if (string.Equals(band, "450+ hours (Band 4a)", StringComparison.OrdinalIgnoreCase))
                {
                    cells["D27"].PutValue(studentNumber);
                    cells["E27"].PutValue(totalFunding);
                }
                else if (string.Equals(band, "450 to 539 hours (Band 4b)", StringComparison.OrdinalIgnoreCase))
                {
                    cells["D28"].PutValue(studentNumber);
                    cells["E28"].PutValue(totalFunding);
                }
                else if (string.Equals(band, "360 to 449 hours (Band 3)", StringComparison.OrdinalIgnoreCase))
                {
                    cells["D29"].PutValue(studentNumber);
                    cells["E29"].PutValue(totalFunding);
                }
                else if (string.Equals(band, "280 to 359 hours (Band 2)", StringComparison.OrdinalIgnoreCase))
                {
                    cells["D30"].PutValue(studentNumber);
                    cells["E30"].PutValue(totalFunding);
                }
                else if (string.Equals(band, "Up to 279 hours (Band 1)", StringComparison.OrdinalIgnoreCase))
                {
                    cells["D31"].PutValue(studentNumber);
                    cells["E31"].PutValue(totalFunding);
                }
            }
            else if (string.Equals(fundline, "19+ Continuing Students (excluding EHC plans)", StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals(band, "540+ hours (Band 5)", StringComparison.OrdinalIgnoreCase))
                {
                    cells["D35"].PutValue(studentNumber);
                    cells["E35"].PutValue(totalFunding);
                }
                else if (string.Equals(band, "450+ hours (Band 4a)", StringComparison.OrdinalIgnoreCase))
                {
                    cells["D36"].PutValue(studentNumber);
                    cells["E36"].PutValue(totalFunding);
                }
                else if (string.Equals(band, "450 to 539 hours (Band 4b)", StringComparison.OrdinalIgnoreCase))
                {
                    cells["D37"].PutValue(studentNumber);
                    cells["E37"].PutValue(totalFunding);
                }
                else if (string.Equals(band, "360 to 449 hours (Band 3)", StringComparison.OrdinalIgnoreCase))
                {
                    cells["D38"].PutValue(studentNumber);
                    cells["E38"].PutValue(totalFunding);
                }
                else if (string.Equals(band, "280 to 359 hours (Band 2)", StringComparison.OrdinalIgnoreCase))
                {
                    cells["D39"].PutValue(studentNumber);
                    cells["E39"].PutValue(totalFunding);
                }
                else if (string.Equals(band, "Up to 279 hours (Band 1)", StringComparison.OrdinalIgnoreCase))
                {
                    cells["D40"].PutValue(studentNumber);
                    cells["E40"].PutValue(totalFunding);
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
