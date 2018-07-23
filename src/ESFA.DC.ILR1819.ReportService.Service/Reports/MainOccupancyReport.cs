using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using CsvHelper;
using ESFA.DC.ILR.FundingService.ALB.FundingOutput.Model.Interface;
using ESFA.DC.ILR.FundingService.ALB.FundingOutput.Model.Interface.Attribute;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR1819.ReportService.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Model;
using ESFA.DC.ILR1819.ReportService.Interface.Reports;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Service.Model;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.ILR1819.ReportService.Service.Reports
{
    public sealed class MainOccupancyReport : IMainOccupancyReport
    {
        private const string Fm25OnProgPayment = "OnProgPayment";

        private const string Fm35OnProgPayment = "OnProgPayment";

        private const string Fm35BalPayment = "BalancePayment";

        private const string Fm35EmpOutcomePay = "EmpOutcomePay";

        private const string Fm35AchievePayment = "AchievePayment";

        private const string Fm35LearnSuppFundCash = "LearnSuppFundCash";

        private readonly ILogger _logger;
        private readonly IKeyValuePersistenceService _storage;
        private readonly IIlrProviderService _ilrProviderService;
        private readonly IStringUtilitiesService _stringUtilitiesService;
        private readonly IValidLearnersService _validLearnersService;
        private readonly IAllbProviderService _allbProviderService;
        private readonly ILarsProviderService _larsProviderService;

        public MainOccupancyReport(
            ILogger logger,
            [KeyFilter(PersistenceStorageKeys.Blob)] IKeyValuePersistenceService storage,
            IIlrProviderService ilrProviderService,
            IStringUtilitiesService stringUtilitiesService,
            IValidLearnersService validLearnersService,
            IAllbProviderService allbProviderService,
            ILarsProviderService larsProviderService)
        {
            _logger = logger;
            _storage = storage;
            _ilrProviderService = ilrProviderService;
            _stringUtilitiesService = stringUtilitiesService;
            _validLearnersService = validLearnersService;
            _allbProviderService = allbProviderService;
            _larsProviderService = larsProviderService;
        }

        public async Task GenerateReport(IJobContextMessage jobContextMessage)
        {
            Task<IMessage> ilrFileTask = _ilrProviderService.GetIlrFile(jobContextMessage);
            Task<IFundingOutputs> albDataTask = _allbProviderService.GetAllbData(jobContextMessage);
            Task<List<string>> validLearnersTask = _validLearnersService.GetValidLearnersAsync(jobContextMessage);

            await Task.WhenAll(ilrFileTask, albDataTask, validLearnersTask);

            List<ILearner> learners =
                ilrFileTask.Result?.Learners?.Where(x => validLearnersTask.Result.Contains(x.LearnRefNumber)).ToList();
            if (learners == null)
            {
                _logger.LogWarning("Failed to get learners for Main Occupancy Report");
                return;
            }

            List<string> learnAimRefs = learners.SelectMany(x => x.LearningDeliveries).Select(x => x.LearnAimRef).ToList();

            Task<Dictionary<string, ILarsLearningDelivery>> larsLearningDeliveriesTask = _larsProviderService.GetLarsData(learnAimRefs);
            Task<Dictionary<string, ILarsFrameworkAim>> larsFrameworkAimsTask = _larsProviderService.GetFrameworkAims(learnAimRefs);

            await Task.WhenAll(larsLearningDeliveriesTask, larsFrameworkAimsTask);

            List<string> larsError = new List<string>();
            List<string> albLearnerError = new List<string>();

            List<MainOccupancyFM25Model> mainOccupancyFM25Models = new List<MainOccupancyFM25Model>(learnAimRefs.Count);
            List <MainOccupancyFM35Model> mainOccupancyFM35Models = new List<MainOccupancyFM35Model>(learnAimRefs.Count);
            foreach (var learner in learners)
            {
                if (!larsLearningDeliveriesTask.Result.TryGetValue(learner.LearnRefNumber, out ILarsLearningDelivery larsModel))
                {
                    larsError.Add(learner.LearnRefNumber);
                    continue;
                }

                if (!larsFrameworkAimsTask.Result.TryGetValue(learner.LearnRefNumber, out ILarsFrameworkAim frameworkAim))
                {
                    larsError.Add(learner.LearnRefNumber);
                    continue;
                }

                ILearnerAttribute albLearner =
                    albDataTask.Result?.Learners?.SingleOrDefault(x => x.LearnRefNumber == learner.LearnRefNumber);
                if (albLearner == null)
                {
                    albLearnerError.Add(learner.LearnRefNumber);
                    continue;
                }

                var albLearnerOnProgPayment =
                    albLearner?.LearnerPeriodisedAttributes.SingleOrDefault(x => x.AttributeName == Fm25OnProgPayment);

                foreach (ILearningDelivery learningDelivery in learner.LearningDeliveries)
                {
                    var albLearningDeliveryAttributeDatas = albLearner.LearningDeliveryAttributes
                        .SingleOrDefault(x => x.AimSeqNumber == learningDelivery.AimSeqNumber)
                        ?.LearningDeliveryAttributeDatas;
                    var albLearningDeliveryPeriodisedValues = albLearner.LearningDeliveryAttributes
                        .SingleOrDefault(x => x.AimSeqNumber == learningDelivery.AimSeqNumber)
                        ?.LearningDeliveryPeriodisedAttributes;

                    var alb = learningDelivery.LearningDeliveryFAMs.SingleOrDefault(x => x.LearnDelFAMType == "ALB");

                    mainOccupancyFM35Models.Add(new MainOccupancyFM35Model()
                    {
                        LearnRefNumber = learner.LearnRefNumber,
                        Uln = learner.ULN,
                        DateOfBirth = learner.DateOfBirthNullable?.ToString("dd/MM/yyyy"),
                        PostcodePrior = learner.PostcodePrior,
                        PmUkprn = learner.PMUKPRNNullable,
                        CampId = learner.CampId,
                        ProvSpecLearnMonA = learner.ProviderSpecLearnerMonitorings
                            .SingleOrDefault(x => x.ProvSpecLearnMonOccur == "A")?.ProvSpecLearnMon,
                        ProvSpecLearnMonB = learner.ProviderSpecLearnerMonitorings
                            .SingleOrDefault(x => x.ProvSpecLearnMonOccur == "B")?.ProvSpecLearnMon,
                        AimSeqNumber = learningDelivery.AimSeqNumber,
                        LearnAimRef = learningDelivery.LearnAimRef,
                        LearnAimRefTitle = larsModel.LearningAimTitle,
                        SwSupAimId = learningDelivery.SWSupAimId,
                        WeightedRateFromEsol = "Todo", // Todo
                        ApplicWeightFundRate = "Todo", // Todo
                        ApplicProgWeightFact = albLearningDeliveryAttributeDatas?.ApplicProgWeightFact,
                        AimValue = "Todo", // Todo
                        NotionalNvqLevelV2 = larsModel.NotionalNvqLevel,
                        SectorSubjectAreaTier2 = larsModel.Tier2SectorSubjectArea,
                        ProgType = learningDelivery.ProgTypeNullable,
                        FworkCode = learningDelivery.FworkCodeNullable,
                        PwayCode = learningDelivery.PwayCodeNullable,
                        AimType = learningDelivery.AimType,
                        FrameworkComponentType = frameworkAim.FrameworkComponentType,
                        FundModel = learningDelivery.FundModel,
                        PriorLearnFundAdj = learningDelivery.PriorLearnFundAdjNullable,
                        OtherFundAdj = learningDelivery.OtherFundAdjNullable,
                        OrigLearnStartDate = learningDelivery.OrigLearnStartDateNullable,
                        LearnStartDate = learningDelivery.LearnStartDate,
                        LearnPlanEndDate = learningDelivery.LearnPlanEndDate,
                        CompStatus = learningDelivery.CompStatus,
                        LearnActEndDate = learningDelivery.LearnActEndDateNullable,
                        Outcome = learningDelivery.OutcomeNullable,
                        AchDate = learningDelivery.AchDateNullable,
                        AddHours = learningDelivery.AddHoursNullable,
                        LearnDelFamCodeSof = learningDelivery.LearningDeliveryFAMs?.SingleOrDefault(x => x.LearnDelFAMType == "SOF")?.LearnDelFAMCode,
                        LearnDelFamCodeFfi = learningDelivery.LearningDeliveryFAMs?.SingleOrDefault(x => x.LearnDelFAMType == "FFI")?.LearnDelFAMCode,
                        LearnDelFamCodeEef = learningDelivery.LearningDeliveryFAMs?.SingleOrDefault(x => x.LearnDelFAMType == "EEF")?.LearnDelFAMCode,
                        LearnDelFamCodeLsfHighest = learningDelivery.LearningDeliveryFAMs?.Where(x => x.LearnDelFAMType == "LSF").Max(x => x.LearnDelFAMCode),
                        LearnDelFamCodeLsfEarliest = learningDelivery.LearningDeliveryFAMs?.Where(x => x.LearnDelFAMType == "LSF").Min(x => x.LearnDelFAMDateFromNullable)?.ToString("dd/MM/yyyy"),
                        LearnDelFamCodeLsfLatest = learningDelivery.LearningDeliveryFAMs?.Where(x => x.LearnDelFAMType == "LSF").Min(x => x.LearnDelFAMDateToNullable)?.ToString("dd/MM/yyyy"),
                        LearnDelFamCodeLdm1 = learningDelivery.LearningDeliveryFAMs?.SingleOrDefault(x => x.LearnDelFAMType == "LDM1")?.LearnDelFAMCode,
                        LearnDelFamCodeLdm2 = learningDelivery.LearningDeliveryFAMs?.SingleOrDefault(x => x.LearnDelFAMType == "LDM2")?.LearnDelFAMCode,
                        LearnDelFamCodeLdm3 = learningDelivery.LearningDeliveryFAMs?.SingleOrDefault(x => x.LearnDelFAMType == "LDM3")?.LearnDelFAMCode,
                        LearnDelFamCodeLdm4 = learningDelivery.LearningDeliveryFAMs?.SingleOrDefault(x => x.LearnDelFAMType == "LDM4")?.LearnDelFAMCode,
                        LearnDelFamCodeRes = learningDelivery.LearningDeliveryFAMs?.SingleOrDefault(x => x.LearnDelFAMType == "RES")?.LearnDelFAMCode,
                        ProvSpecDelMonA = learningDelivery.ProviderSpecDeliveryMonitorings?.SingleOrDefault(x => x.ProvSpecDelMonOccur == "A")?.ProvSpecDelMon,
                        ProvSpecDelMonB = learningDelivery.ProviderSpecDeliveryMonitorings?.SingleOrDefault(x => x.ProvSpecDelMonOccur == "B")?.ProvSpecDelMon,
                        ProvSpecDelMonC = learningDelivery.ProviderSpecDeliveryMonitorings?.SingleOrDefault(x => x.ProvSpecDelMonOccur == "C")?.ProvSpecDelMon,
                        ProvSpecDelMonD = learningDelivery.ProviderSpecDeliveryMonitorings?.SingleOrDefault(x => x.ProvSpecDelMonOccur == "D")?.ProvSpecDelMon,
                        FundLine = albLearningDeliveryAttributeDatas?.FundLine,
                        PlannedNumOnProgInstalm = albLearningDeliveryAttributeDatas?.PlannedNumOnProgInstalm,
                        PlannedNumOnProgInstalmTrans = albLearningDeliveryAttributeDatas?.PlannedNumOnProgInstalm,
                        StartPropTrans = "Todo", // Todo
                        AchieveElement = "Todo", // Todo
                        AchievePercentage = GetMaxPeriod(albLearningDeliveryPeriodisedValues?.SingleOrDefault(x => x.AttributeName == "AchievePayPct")),
                        NonGovCont = "Todo", // Todo
                        PartnerUkprn = learningDelivery.PartnerUKPRNNullable,
                        DelLocPostCode = learningDelivery.DelLocPostCode,
                        AreaCostFactAdj = albLearningDeliveryAttributeDatas?.AreaCostFactAdj,
                        DisUpFactAdj = "Todo", // Todo
                        LargeEmployerID = "Todo", // Todo
                        LargeEmployerFM35Fctr = "Todo", // Todo
                        CapFactor = "Todo", // Todo
                        TraineeWorkPlacement = "Todo", // Todo (albLearningDeliveryAttributeDatas?.TrnWorkPlaceAim || albLearningDeliveryAttributeDatas?.TrnWorkPrepAim) ? "Yes" : "No",
                        HigherApprentishipHeAim = "Todo", // Todo albLearningDeliveryAttributeDatas?.PrscHEAim ? "Yes" : "No",
                        ApplicEmpFactDate = "Todo", // Todo albLearningDeliveryAttributeDatas?.ApplicEmpFactDate,
                        ApplicFactDate = albLearningDeliveryAttributeDatas?.ApplicFactDate,
                        Period1OnProgPayment = albLearner?.LearnerPeriodisedAttributes.SingleOrDefault(x => x.AttributeName == Fm35OnProgPayment)?.Period1,
                        Period1BalancePayment = albLearner?.LearnerPeriodisedAttributes.SingleOrDefault(x => x.AttributeName == Fm35BalPayment)?.Period1,
                        Period1AchievePayment = albLearner?.LearnerPeriodisedAttributes.SingleOrDefault(x => x.AttributeName == Fm35AchievePayment)?.Period1,
                        Period1EmpOutcomePay = albLearner?.LearnerPeriodisedAttributes.SingleOrDefault(x => x.AttributeName == Fm35EmpOutcomePay)?.Period1,
                        Period1LearnSuppFundCash = albLearner?.LearnerPeriodisedAttributes.SingleOrDefault(x => x.AttributeName == Fm35LearnSuppFundCash)?.Period1,
                    });

                    mainOccupancyFM25Models.Add(new MainOccupancyFM25Model()
                    {
                        LearnRefNumber = learner.LearnRefNumber,
                        Uln = learner.ULN,
                        DateOfBirth = learner.DateOfBirthNullable?.ToString("dd/MM/yyyy"),
                        PostcodePrior = learner.PostcodePrior,
                        PmUkprn = learner.PMUKPRNNullable,
                        CampId = learner.CampId,
                        ProvSpecLearnMonA = learner.ProviderSpecLearnerMonitorings
                            .SingleOrDefault(x => x.ProvSpecLearnMonOccur == "A")?.ProvSpecLearnMon,
                        ProvSpecLearnMonB = learner.ProviderSpecLearnerMonitorings
                            .SingleOrDefault(x => x.ProvSpecLearnMonOccur == "B")?.ProvSpecLearnMon,
                        NatRate = "Todo", // Todo albLearningDeliveryAttributeDatas.NatRate,
                        FundModel = learningDelivery.FundModel,
                        LearnerStartDate = "Todo", // Todo albLearningDeliveryAttributeDatas?.LearnerStartDate,
                        LearnerPlanEndDate = "Todo", // Todo albLearningDeliveryAttributeDatas?.LearnerPlanEndDate,
                        LearnerActEndDate = "Todo", // Todo albLearningDeliveryAttributeDatas?.LearnerActEndDate,
                        FundLine = albLearningDeliveryAttributeDatas?.FundLine,
                        Period1OnProgPayment = albLearnerOnProgPayment?.Period1,
                        Period2OnProgPayment = albLearnerOnProgPayment?.Period2,
                        Period3OnProgPayment = albLearnerOnProgPayment?.Period3,
                        Period4OnProgPayment = albLearnerOnProgPayment?.Period4,
                        Period5OnProgPayment = albLearnerOnProgPayment?.Period5,
                        Period6OnProgPayment = albLearnerOnProgPayment?.Period6,
                        Period7OnProgPayment = albLearnerOnProgPayment?.Period7,
                        Period8OnProgPayment = albLearnerOnProgPayment?.Period8,
                        Period9OnProgPayment = albLearnerOnProgPayment?.Period9,
                        Period10OnProgPayment = albLearnerOnProgPayment?.Period10,
                        Period11OnProgPayment = albLearnerOnProgPayment?.Period11,
                        Period12OnProgPayment = albLearnerOnProgPayment?.Period12,
                        PeriodOnProgPaymentTotal = albLearnerOnProgPayment?.Period1 + albLearnerOnProgPayment?.Period2
                                                      + albLearnerOnProgPayment?.Period3 + albLearnerOnProgPayment?.Period4
                                                      + albLearnerOnProgPayment?.Period5 + albLearnerOnProgPayment?.Period6
                                                      + albLearnerOnProgPayment?.Period7 + albLearnerOnProgPayment?.Period8
                                                      + albLearnerOnProgPayment?.Period9 + albLearnerOnProgPayment?.Period10
                                                      + albLearnerOnProgPayment?.Period11 + albLearnerOnProgPayment?.Period12,
                        Total = albLearnerOnProgPayment?.Period1 + albLearnerOnProgPayment?.Period2
                                                                                      + albLearnerOnProgPayment?.Period3 + albLearnerOnProgPayment?.Period4
                                                                                      + albLearnerOnProgPayment?.Period5 + albLearnerOnProgPayment?.Period6
                                                                                      + albLearnerOnProgPayment?.Period7 + albLearnerOnProgPayment?.Period8
                                                                                      + albLearnerOnProgPayment?.Period9 + albLearnerOnProgPayment?.Period10
                                                                                      + albLearnerOnProgPayment?.Period11 + albLearnerOnProgPayment?.Period12
                    });
                }
            }

            LogWarnings(larsError, albLearnerError);

            await _storage.SaveAsync("Funding_Summary_Report.csv", GetReportCsv(mainOccupancyFM25Models, mainOccupancyFM35Models));
        }

        private string GetReportCsv(List<MainOccupancyFM25Model> mainOccupancyFm25Models, List<MainOccupancyFM35Model> mainOccupancyFm35Models)
        {
            StringBuilder sb = new StringBuilder();

            using (TextWriter textWriter = new StringWriter(sb))
            {
                using (CsvWriter csvWriter = new CsvWriter(textWriter))
                {
                    // Todo Add header
                    // csvWriter.Configuration.RegisterClassMap<FundingSummaryHeaderMapper>();
                    // csvWriter.Configuration.RegisterClassMap<FundingSummaryFooterMapper>();

                    csvWriter.WriteHeader<MainOccupancyFM25Model>();
                    csvWriter.NextRecord();
                    csvWriter.WriteRecord(mainOccupancyFm25Models);

                    csvWriter.WriteHeader<MainOccupancyFM35Model>();
                    csvWriter.NextRecord();
                    csvWriter.WriteRecords(mainOccupancyFm35Models);
                }
            }

            return sb.ToString();
        }

        private void LogWarnings(List<string> larsError, List<string> albLearnerError)
        {
            if (larsError.Any())
            {
                _logger.LogWarning($"Failed to get LARS data while generating Allb Occupancy Report: {_stringUtilitiesService.JoinWithMaxLength(larsError)}");
            }

            if (albLearnerError.Any())
            {
                _logger.LogWarning($"Failed to get one or more ALB learners while generating Allb Occupancy Report: {_stringUtilitiesService.JoinWithMaxLength(albLearnerError)}");
            }
        }

        private decimal GetMaxPeriod(ILearningDeliveryPeriodisedAttribute learningDeliveryPeriodisedAttribute)
        {
            decimal max = int.MinValue;
            if (learningDeliveryPeriodisedAttribute.Period1 > max)
            {
                max = learningDeliveryPeriodisedAttribute.Period1;
            }

            if (learningDeliveryPeriodisedAttribute.Period2 > max)
            {
                max = learningDeliveryPeriodisedAttribute.Period2;
            }

            if (learningDeliveryPeriodisedAttribute.Period3 > max)
            {
                max = learningDeliveryPeriodisedAttribute.Period3;
            }

            if (learningDeliveryPeriodisedAttribute.Period4 > max)
            {
                max = learningDeliveryPeriodisedAttribute.Period4;
            }

            if (learningDeliveryPeriodisedAttribute.Period5 > max)
            {
                max = learningDeliveryPeriodisedAttribute.Period5;
            }

            if (learningDeliveryPeriodisedAttribute.Period6 > max)
            {
                max = learningDeliveryPeriodisedAttribute.Period6;
            }

            if (learningDeliveryPeriodisedAttribute.Period7 > max)
            {
                max = learningDeliveryPeriodisedAttribute.Period7;
            }

            if (learningDeliveryPeriodisedAttribute.Period8 > max)
            {
                max = learningDeliveryPeriodisedAttribute.Period8;
            }

            if (learningDeliveryPeriodisedAttribute.Period9 > max)
            {
                max = learningDeliveryPeriodisedAttribute.Period9;
            }

            if (learningDeliveryPeriodisedAttribute.Period10 > max)
            {
                max = learningDeliveryPeriodisedAttribute.Period10;
            }

            if (learningDeliveryPeriodisedAttribute.Period11 > max)
            {
                max = learningDeliveryPeriodisedAttribute.Period11;
            }

            if (learningDeliveryPeriodisedAttribute.Period12 > max)
            {
                max = learningDeliveryPeriodisedAttribute.Period12;
            }

            return max;
        }
    }
}
