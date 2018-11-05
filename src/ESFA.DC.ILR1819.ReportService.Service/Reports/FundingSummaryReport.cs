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
using ESFA.DC.ILR1819.ReportService.Interface.Builders;
using ESFA.DC.ILR1819.ReportService.Interface.Configuration;
using ESFA.DC.ILR1819.ReportService.Interface.Reports;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.Generation;
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
        private readonly FundingSummaryMapper _fundingSummaryMapper;

        private readonly ModelProperty[] _cachedModelProperties;

        private readonly List<FundingSummaryModel> fundingSummaryModels;

        private readonly ILogger _logger;
        private readonly IStreamableKeyValuePersistenceService _storage;
        private readonly IIlrProviderService _ilrProviderService;
        private readonly IOrgProviderService _orgProviderService;
        private readonly IAllbProviderService _allbProviderService;
        private readonly IFM25ProviderService _fm25ProviderService;
        private readonly IFM35ProviderService _fm35ProviderService;
        private readonly IFM36ProviderService _fm36ProviderService;
        private readonly IFM81TrailBlazerProviderService _fm81TrailBlazerProviderService;
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
        private readonly IFm35Builder _fm35Builder;
        private readonly IFm36Builder _fm36Builder;
        private readonly IFm81Builder _fm81Builder;
        private readonly ITotalBuilder _totalBuilder;
        private readonly IVersionInfo _versionInfo;
        private readonly IExcelStyleProvider _excelStyleProvider;
        private readonly IEasBuilder _easBuilder;

        public FundingSummaryReport(
            ILogger logger,
            IStreamableKeyValuePersistenceService storage,
            IIlrProviderService ilrProviderService,
            IOrgProviderService orgProviderService,
            IAllbProviderService allbProviderService,
            IFM25ProviderService fm25ProviderService,
            IFM35ProviderService fm35ProviderService,
            IFM36ProviderService fm36ProviderService,
            IFM81TrailBlazerProviderService fm81TrailBlazerProviderService,
            IValidLearnersService validLearnersService,
            IStringUtilitiesService stringUtilitiesService,
            IPeriodProviderService periodProviderService,
            IDateTimeProvider dateTimeProvider,
            IValueProvider valueProvider,
            ILarsProviderService larsProviderService,
            IEasProviderService easProviderService,
            IPostcodeProviderService postcodeProviderService,
            ILargeEmployerProviderService largeEmployerProviderService,
            IAllbBuilder allbBuilder,
            IFm25Builder fm25Builder,
            IFm35Builder fm35Builder,
            IFm36Builder fm36Builder,
            IFm81Builder fm81Builder,
            ITotalBuilder totalBuilder,
            IVersionInfo versionInfo,
            IExcelStyleProvider excelStyleProvider,
            ITopicAndTaskSectionOptions topicAndTaskSectionOptions,
            IEasBuilder easBuilder)
            : base(dateTimeProvider)
        {
            _logger = logger;
            _storage = storage;
            _ilrProviderService = ilrProviderService;
            _orgProviderService = orgProviderService;
            _allbProviderService = allbProviderService;
            _fm25ProviderService = fm25ProviderService;
            _fm35ProviderService = fm35ProviderService;
            _fm36ProviderService = fm36ProviderService;
            _fm81TrailBlazerProviderService = fm81TrailBlazerProviderService;
            _validLearnersService = validLearnersService;
            _stringUtilitiesService = stringUtilitiesService;
            _periodProviderService = periodProviderService;
            _larsProviderService = larsProviderService;
            _easProviderService = easProviderService;
            _postcodeProviderService = postcodeProviderService;
            _largeEmployerProviderService = largeEmployerProviderService;
            _allbBuilder = allbBuilder;
            _fm25Builder = fm25Builder;
            _fm35Builder = fm35Builder;
            _fm36Builder = fm36Builder;
            _fm81Builder = fm81Builder;
            _totalBuilder = totalBuilder;
            _versionInfo = versionInfo;
            _excelStyleProvider = excelStyleProvider;
            _dateTimeProvider = dateTimeProvider;
            _easBuilder = easBuilder;

            ReportFileName = "Funding Summary Report";
            ReportTaskName = topicAndTaskSectionOptions.TopicReports_TaskGenerateFundingSummaryReport;

            fundingSummaryModels = new List<FundingSummaryModel>();
            _fundingSummaryMapper = new FundingSummaryMapper();
            _cachedModelProperties = _fundingSummaryMapper.MemberMaps.OrderBy(x => x.Data.Index).Select(x => new ModelProperty(x.Data.Names.Names.ToArray(), (PropertyInfo)x.Data.Member)).ToArray();
        }

        public async Task GenerateReport(IJobContextMessage jobContextMessage, ZipArchive archive, bool isFis, CancellationToken cancellationToken)
        {
            Task<IMessage> ilrFileTask = _ilrProviderService.GetIlrFile(jobContextMessage, cancellationToken);
            Task<ALBGlobal> albDataTask = _allbProviderService.GetAllbData(jobContextMessage, cancellationToken);
            Task<FM25Global> fm25Task = _fm25ProviderService.GetFM25Data(jobContextMessage, cancellationToken);
            Task<FM35Global> fm35Task = _fm35ProviderService.GetFM35Data(jobContextMessage, cancellationToken);
            Task<FM36Global> fm36Task = _fm36ProviderService.GetFM36Data(jobContextMessage, cancellationToken);
            Task<FM81Global> fm81Task = _fm81TrailBlazerProviderService.GetFM81Data(jobContextMessage, cancellationToken);
            Task<List<string>> validLearnersTask = _validLearnersService.GetLearnersAsync(jobContextMessage, cancellationToken);
            Task<string> providerNameTask = _orgProviderService.GetProviderName(jobContextMessage, cancellationToken);
            Task<int> periodTask = _periodProviderService.GetPeriod(jobContextMessage, cancellationToken);

            Task<List<EasSubmissionValues>> easSubmissionsValuesTask =
                _easProviderService.GetEasSubmissionValuesAsync(jobContextMessage, cancellationToken);

            await Task.WhenAll(ilrFileTask, albDataTask, fm25Task, fm35Task, fm36Task, fm81Task, validLearnersTask, providerNameTask, periodTask, easSubmissionsValuesTask);

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            FundingSummaryHeaderModel fundingSummaryHeaderModel = await GetHeader(jobContextMessage, ilrFileTask, providerNameTask, cancellationToken);
            FundingSummaryFooterModel fundingSummaryFooterModel = await GetFooterAsync(ilrFileTask, cancellationToken);

            // Todo: Check keys & titles
            fundingSummaryModels.Add(new FundingSummaryModel());
            fundingSummaryModels.Add(new FundingSummaryModel("16-18 Traineeships Budget", HeaderType.TitleOnly, 0));
            fundingSummaryModels.Add(new FundingSummaryModel("16-18 Traineeships", HeaderType.All, 2));
            FundingSummaryModel traineeships1618 = _fm25Builder.BuildWithFundLine("ILR 16-18 Traineeships Programme Funding (£)", fm25Task.Result, validLearnersTask.Result, "16-18 Traineeships (Adult Funded)", periodTask.Result);
            fundingSummaryModels.Add(traineeships1618);
            FundingSummaryModel traineeships1924 = _fm25Builder.BuildWithFundLine("ILR 19-24 Traineeships (16-19 Model) Programme Funding (£)", fm25Task.Result, validLearnersTask.Result, "19+ Traineeships (Adult Funded)", periodTask.Result);
            fundingSummaryModels.Add(traineeships1924);
            FundingSummaryModel ilrTraineeshipsTotal = _totalBuilder.TotalRecords("ILR Total 16-18 Traineeships (£)", traineeships1618, traineeships1924);

            FundingSummaryModel easTraineeshipsTotal = new FundingSummaryModel(); // Todo: EAS
            if (!isFis)
            {
                ilrTraineeshipsTotal.ExcelHeaderStyle = 3;
                ilrTraineeshipsTotal.ExcelRecordStyle = 3;
                fundingSummaryModels.Add(ilrTraineeshipsTotal);

                // Todo: EAS
                FundingSummaryModel eas1618TraineeshipsAuditAdjustments = _easBuilder.BuildWithEasSubValueLine("EAS 16-18 Traineeships Audit Adjustments (£)", easSubmissionsValuesTask.Result, "Audit Adjustments: 16-18 Traineeships", periodTask.Result);
                fundingSummaryModels.Add(eas1618TraineeshipsAuditAdjustments);

                FundingSummaryModel eas1618TraineeshipsAuthorisedClaims = _easBuilder.BuildWithEasSubValueLine("EAS 16-18 Traineeships Authorised Claims (£)", easSubmissionsValuesTask.Result, "Authorised Claims: 16-18 Traineeships", periodTask.Result);
                fundingSummaryModels.Add(eas1618TraineeshipsAuthorisedClaims);

                FundingSummaryModel eas1618TraineeshipsExcessLearningSupport = _easBuilder.BuildWithEasSubValueLine("EAS 16-18 Traineeships Excess Learning Support (£)", easSubmissionsValuesTask.Result, "Excess Learning Support: 16-18 Traineeships", periodTask.Result);
                fundingSummaryModels.Add(eas1618TraineeshipsExcessLearningSupport);

                FundingSummaryModel eas1619TraineeshipsVulStudentBursery = _easBuilder.BuildWithEasSubValueLine("EAS 16-19 Traineeships Bursary Vulnerable Student Bursary (£)", easSubmissionsValuesTask.Result, "Vulnerable Bursary: 16-19 Traineeships Bursary", periodTask.Result);
                fundingSummaryModels.Add(eas1619TraineeshipsVulStudentBursery);

                FundingSummaryModel eas1619TraineeshipsFreeMealsBursery = _easBuilder.BuildWithEasSubValueLine("EAS 16-19 Traineeships Bursary Free Meals (£)", easSubmissionsValuesTask.Result, "Free Meals: 16-19 Traineeships Bursary", periodTask.Result);
                fundingSummaryModels.Add(eas1619TraineeshipsFreeMealsBursery);

                FundingSummaryModel eas1619TraineeshipsDiscretionaryBursary = _easBuilder.BuildWithEasSubValueLine("EAS 16-19 Traineeships Bursary Discretionary Bursary (£)", easSubmissionsValuesTask.Result, "Discretionary Bursary: 16-19 Traineeships Bursary", periodTask.Result);
                fundingSummaryModels.Add(eas1619TraineeshipsDiscretionaryBursary);

                easTraineeshipsTotal = _totalBuilder.TotalRecords("EAS Total 16-18 Traineeships Earnings Adjustment (£)", eas1618TraineeshipsAuditAdjustments, eas1618TraineeshipsAuthorisedClaims, eas1618TraineeshipsExcessLearningSupport, eas1619TraineeshipsVulStudentBursery, eas1619TraineeshipsFreeMealsBursery, eas1619TraineeshipsDiscretionaryBursary);
            }

            FundingSummaryModel traineeshipsTotal = _totalBuilder.TotalRecords("Total 16-18 Traineeships (£)", ilrTraineeshipsTotal, easTraineeshipsTotal);
            traineeshipsTotal.ExcelHeaderStyle = 2;
            traineeshipsTotal.ExcelRecordStyle = 2;
            fundingSummaryModels.Add(traineeshipsTotal);
            FundingSummaryModel traineeshipsTotalBudget = (FundingSummaryModel)traineeshipsTotal.Clone();
            traineeshipsTotalBudget.Title = "Total 16-18 Traineeships Budget (£)";
            traineeshipsTotalBudget.ExcelHeaderStyle = 0;
            traineeshipsTotalBudget.ExcelRecordStyle = 0;
            fundingSummaryModels.Add(traineeshipsTotalBudget);
            FundingSummaryModel traineeshipsTotalBudgetCumulative = _totalBuilder.TotalRecordsCumulative("Total 16-18 Traineeships Budget Cumulative (£)", traineeshipsTotalBudget);
            traineeshipsTotalBudgetCumulative.ExcelHeaderStyle = 0;
            traineeshipsTotalBudgetCumulative.ExcelRecordStyle = 0;
            fundingSummaryModels.Add(traineeshipsTotalBudgetCumulative);

            fundingSummaryModels.Add(new FundingSummaryModel());
            fundingSummaryModels.Add(new FundingSummaryModel("Carry-in Apprenticeships Budget (for starts before 1 May 2017 and non-procured delivery)", HeaderType.TitleOnly, 0));
            fundingSummaryModels.Add(new FundingSummaryModel());

            fundingSummaryModels.Add(new FundingSummaryModel("16-18 Apprenticeship Frameworks for starts before 1 May 2017", HeaderType.All, 2));
            FundingSummaryModel ilrApprenticeshipProgramme = _fm35Builder.BuildWithFundLine("ILR 16-18 Apprenticeship Frameworks Programme Funding (£)", fm35Task.Result, validLearnersTask.Result, new[] { "16-18 Apprenticeship" }, new[] { Constants.Fm35OnProgrammeAttributeName, Constants.Fm35AimAchievementAttributeName, Constants.Fm35JobOutcomeAchievementAttributeName, Constants.Fm35BalancingAttributeName });
            fundingSummaryModels.Add(ilrApprenticeshipProgramme);
            FundingSummaryModel ilrApprenticeshipFrameworks = _fm35Builder.BuildWithFundLine("ILR 16-18 Apprenticeship Frameworks Learning Support (£)", fm35Task.Result, validLearnersTask.Result, new[] { "16-18 Apprenticeship" }, new[] { Constants.Fm35LearningSupportAttributeName });
            fundingSummaryModels.Add(ilrApprenticeshipFrameworks);
            FundingSummaryModel ilrTotal1618Apprenticeship = _totalBuilder.TotalRecords("ILR Total 16-18 Apprenticeship Frameworks (£)", ilrApprenticeshipProgramme, ilrApprenticeshipFrameworks);
            FundingSummaryModel easTotal1618Apprenticeship = new FundingSummaryModel(); // Todo: EAS
            if (!isFis)
            {
                ilrTotal1618Apprenticeship.ExcelHeaderStyle = 3;
                ilrTotal1618Apprenticeship.ExcelRecordStyle = 3;
                fundingSummaryModels.Add(ilrTotal1618Apprenticeship);

                // Todo: EAS
            }

            FundingSummaryModel totalApprenticeships1618 = _totalBuilder.TotalRecords("Total 16-18 Apprenticeship Frameworks for starts before 1 May 2017 (£)", ilrTotal1618Apprenticeship, easTotal1618Apprenticeship);
            totalApprenticeships1618.ExcelHeaderStyle = 2;
            totalApprenticeships1618.ExcelRecordStyle = 2;
            fundingSummaryModels.Add(totalApprenticeships1618);

            fundingSummaryModels.Add(new FundingSummaryModel());
            fundingSummaryModels.Add(new FundingSummaryModel("16-18 Trailblazer Apprenticeships for starts before 1 May 2017", HeaderType.All, 2));
            FundingSummaryModel trailblazer1618Programme = _fm81Builder.BuildWithFundLine("ILR 16-18 Trailblazer Apprenticeships Programme Funding (Core Government Contribution, Maths and English) (£)", fm81Task.Result, validLearnersTask.Result, "16-18 Trailblazer Apprenticeship", new[] { Constants.Fm81CoreGovContPayment, Constants.Fm81MathEngBalPayment, Constants.Fm81MathEngOnProgPayment });
            fundingSummaryModels.Add(trailblazer1618Programme);
            FundingSummaryModel trailblazer1618Incentive = _fm81Builder.BuildWithFundLine("ILR 16-18 Trailblazer Apprenticeships Employer Incentive Payments (Achievement, Small Employer, 16 - 18) (£)", fm81Task.Result, validLearnersTask.Result, "16-18 Trailblazer Apprenticeship", new[] { Constants.Fm81AchPayment, Constants.Fm81SmallBusPayment, Constants.Fm81YoungAppPayment });
            fundingSummaryModels.Add(trailblazer1618Incentive);
            FundingSummaryModel trailblazer1618Support = _fm81Builder.BuildWithFundLine("ILR 16-18 Trailblazer Apprenticeships Learning Support (£)", fm81Task.Result, validLearnersTask.Result, "16-18 Trailblazer Apprenticeship", new[] { Constants.Fm81LearnSuppFundCash });
            fundingSummaryModels.Add(trailblazer1618Support);
            FundingSummaryModel trailblazerTotal = _totalBuilder.TotalRecords("ILR Total 16-18 Trailblazer Apprenticeships (£)", trailblazer1618Programme, trailblazer1618Incentive, trailblazer1618Support);
            if (!isFis)
            {
                trailblazerTotal.ExcelHeaderStyle = 3;
                trailblazerTotal.ExcelRecordStyle = 3;
                fundingSummaryModels.Add(trailblazerTotal);

                // Todo: EAS
            }

            FundingSummaryModel totalTrailblazer1618Before = _totalBuilder.TotalRecords("Total 16-18 Trailblazer Apprenticeships for starts before 1 May 2017 (£)", trailblazerTotal); // Todo: EAS
            totalTrailblazer1618Before.ExcelHeaderStyle = 2;
            totalTrailblazer1618Before.ExcelRecordStyle = 2;
            fundingSummaryModels.Add(totalTrailblazer1618Before);

            fundingSummaryModels.Add(new FundingSummaryModel());
            fundingSummaryModels.Add(new FundingSummaryModel("16-18 Non-Levy Contracted Apprenticeships - Non-procured delivery", HeaderType.All, 2));
            fundingSummaryModels.Add(_fm36Builder.BuildWithFundLine("ILR 16-18 Non-Levy Contracted Apprenticeships Programme Aim Indicative Earnings (£)", fm36Task.Result, validLearnersTask.Result, new[] { "16-18 Apprenticeship (From May 2017) Non-Levy Contract", "16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured)" }, new[] { Constants.Fm36ProgrammeAimOnProgPayment, Constants.Fm36ProgrammeAimBalPayment, Constants.Fm36ProgrammeAimCompletionPayment }));
            FundingSummaryModel nonLevyCoInvest = _fm36Builder.BuildWithFundLine("...of which Indicative Government Co-Investment Earnings (£)", fm36Task.Result, validLearnersTask.Result, new[] { "16-18 Apprenticeship (From May 2017) Non-Levy Contract", "16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured)" }, new[] { Constants.Fm36ProgrammeAimProgFundIndMinCoInvest });
            fundingSummaryModels.Add(nonLevyCoInvest);
            FundingSummaryModel nonLevyMathsAndEnglish = _fm36Builder.BuildWithFundLine("ILR 16-18 Non-Levy Contracted Apprenticeships Maths and English Programme Funding (£)", fm36Task.Result, validLearnersTask.Result, new[] { "16-18 Apprenticeship (From May 2017) Non-Levy Contract", "16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured)" }, new[] { Constants.Fm36MathEngOnProgPayment, Constants.Fm36MathEngBalPayment });
            fundingSummaryModels.Add(nonLevyMathsAndEnglish);
            FundingSummaryModel nonLevyUplift = _fm36Builder.BuildWithFundLine("ILR 16-18 Non-Levy Contracted Apprenticeships Framework Uplift (£)", fm36Task.Result, validLearnersTask.Result, new[] { "16-18 Apprenticeship (From May 2017) Non-Levy Contract", "16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured)" }, new[] { Constants.Fm36LDApplic1618FrameworkUpliftBalancingPayment, Constants.Fm36LDApplic1618FrameworkUpliftCompletionPayment, Constants.Fm36LDApplic1618FrameworkUpliftOnProgPayment });
            fundingSummaryModels.Add(nonLevyUplift);
            FundingSummaryModel nonLevyDisadvantage = _fm36Builder.BuildWithFundLine("ILR 16-18 Non-Levy Contracted Apprenticeships Disadvantage Payments (£)", fm36Task.Result, validLearnersTask.Result, new[] { "16-18 Apprenticeship (From May 2017) Non-Levy Contract", "16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured)" }, new[] { Constants.Fm36DisadvFirstPayment, Constants.Fm36DisadvSecondPayment });
            fundingSummaryModels.Add(nonLevyDisadvantage);
            FundingSummaryModel nonLevyProviders = _fm36Builder.BuildWithFundLine("ILR 16-18 Non-Levy Contracted Apprenticeships Additional Payments for Providers (£)", fm36Task.Result, validLearnersTask.Result, new[] { "16-18 Apprenticeship (From May 2017) Non-Levy Contract", "16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured)" }, new[] { Constants.Fm36LearnDelFirstProv1618Pay, Constants.Fm36LearnDelSecondProv1618Pay });
            fundingSummaryModels.Add(nonLevyProviders);
            FundingSummaryModel nonLevyEmployers = _fm36Builder.BuildWithFundLine("ILR 16-18 Non-Levy Contracted Apprenticeships Additional Payments for Employers (£)", fm36Task.Result, validLearnersTask.Result, new[] { "16-18 Apprenticeship (From May 2017) Non-Levy Contract", "16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured)" }, new[] { Constants.Fm36LearnDelFirstEmp1618Pay, Constants.Fm36LearnDelSecondEmp1618Pay });
            fundingSummaryModels.Add(nonLevyEmployers);
            FundingSummaryModel nonLevySupport = _fm36Builder.BuildWithFundLine("ILR 16-18 Non-Levy Contracted Apprenticeships Learning Support (£)", fm36Task.Result, validLearnersTask.Result, new[] { "16-18 Apprenticeship (From May 2017) Non-Levy Contract", "16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured)" }, new[] { Constants.Fm36LearnSuppFundCash });
            fundingSummaryModels.Add(nonLevySupport);
            FundingSummaryModel nonLevyTotal = _totalBuilder.TotalRecords("ILR Total 16-18 Non-Levy Contracted Apprenticeships (£)", nonLevyCoInvest, nonLevyMathsAndEnglish, nonLevyUplift, nonLevyDisadvantage, nonLevyProviders, nonLevyEmployers, nonLevySupport);
            if (!isFis)
            {
                nonLevyTotal.ExcelHeaderStyle = 3;
                nonLevyTotal.ExcelRecordStyle = 3;
                fundingSummaryModels.Add(nonLevyTotal);

                // Todo: EAS
            }

            FundingSummaryModel total1618NonLevyNonProcured = _totalBuilder.TotalRecords("Total 16-18 Non-Levy Contracted Apprenticeships – Non-procured delivery (£)", nonLevyTotal); // Todo: EAS
            total1618NonLevyNonProcured.ExcelHeaderStyle = 2;
            total1618NonLevyNonProcured.ExcelRecordStyle = 2;
            fundingSummaryModels.Add(total1618NonLevyNonProcured);

            fundingSummaryModels.Add(new FundingSummaryModel());
            fundingSummaryModels.Add(new FundingSummaryModel("19-23 Apprenticeship Frameworks for starts before 1 May 2017", HeaderType.All, 2));
            FundingSummaryModel apprenticeshipProgramme = _fm35Builder.BuildWithFundLine("ILR 19-23 Apprenticeship Frameworks Programme Funding (£)", fm35Task.Result, validLearnersTask.Result, new[] { "19-23 Apprenticeship" }, new[] { Constants.Fm35OnProgrammeAttributeName, Constants.Fm35AimAchievementAttributeName, Constants.Fm35JobOutcomeAchievementAttributeName, Constants.Fm35BalancingAttributeName });
            fundingSummaryModels.Add(apprenticeshipProgramme);
            FundingSummaryModel apprenticeshipSupport = _fm35Builder.BuildWithFundLine("ILR 19-23 Apprenticeship Frameworks Programme Funding (£)", fm35Task.Result, validLearnersTask.Result, new[] { "19-23 Apprenticeship" }, new[] { Constants.Fm35LearningSupportAttributeName });
            fundingSummaryModels.Add(apprenticeshipSupport);
            FundingSummaryModel apprenticeshipTotal = _totalBuilder.TotalRecords("ILR Total 19-23 Apprenticeship Frameworks (£)", apprenticeshipProgramme, apprenticeshipSupport);
            if (!isFis)
            {
                apprenticeshipTotal.ExcelHeaderStyle = 3;
                apprenticeshipTotal.ExcelRecordStyle = 3;
                fundingSummaryModels.Add(apprenticeshipTotal);

                // Todo: EAS
            }

            FundingSummaryModel totalApprenticeship1923Before = _totalBuilder.TotalRecords("Total 19-23 Apprenticeship Frameworks for starts before 1 May 2017 (£)", apprenticeshipTotal); // Todo: EAS
            totalApprenticeship1923Before.ExcelHeaderStyle = 2;
            totalApprenticeship1923Before.ExcelRecordStyle = 2;
            fundingSummaryModels.Add(totalApprenticeship1923Before);

            fundingSummaryModels.Add(new FundingSummaryModel("19-23 Trailblazer Apprenticeships for starts before 1 May 2017", HeaderType.All, 2));
            FundingSummaryModel trailblazer1923Funding = _fm81Builder.BuildWithFundLine("ILR 19-23 Trailblazer Apprenticeships Programme Funding (Core Government Contribution, Maths and English) (£)", fm81Task.Result, validLearnersTask.Result, "19-23 Trailblazer Apprenticeship", new[] { Constants.Fm81CoreGovContPayment, Constants.Fm81MathEngBalPayment, Constants.Fm81MathEngOnProgPayment });
            fundingSummaryModels.Add(trailblazer1923Funding);
            FundingSummaryModel trailblazer1923Payment = _fm81Builder.BuildWithFundLine("ILR 19-23 Trailblazer Apprenticeships Employer Incentive Payments (Achievement, Small Employer, 16 - 18) (£)", fm81Task.Result, validLearnersTask.Result, "19-23 Trailblazer Apprenticeship", new[] { Constants.Fm81AchPayment, Constants.Fm81SmallBusPayment, Constants.Fm81YoungAppPayment });
            fundingSummaryModels.Add(trailblazer1923Funding);
            FundingSummaryModel trailblazer1923Support = _fm81Builder.BuildWithFundLine("ILR 19-23 Trailblazer Apprenticeships Learning Support (£)", fm81Task.Result, validLearnersTask.Result, "19-23 Trailblazer Apprenticeship", new[] { Constants.Fm81LearnSuppFundCash });
            fundingSummaryModels.Add(trailblazer1923Support);
            FundingSummaryModel trailblazer1923Total = _totalBuilder.TotalRecords("ILR Total 19-23 Trailblazer Apprenticeships (£)", trailblazer1923Funding, trailblazer1923Payment, trailblazer1923Support);
            if (!isFis)
            {
                trailblazer1923Total.ExcelHeaderStyle = 3;
                trailblazer1923Total.ExcelRecordStyle = 3;
                fundingSummaryModels.Add(trailblazer1923Total);

                // Todo: EAS
            }

            FundingSummaryModel totalTrailblazer1923Before = _totalBuilder.TotalRecords("Total 19-23 Trailblazer Apprenticeships for starts before 1 May 2017 (£)", trailblazer1923Total); // Todo: EAS
            totalTrailblazer1923Before.ExcelHeaderStyle = 2;
            totalTrailblazer1923Before.ExcelRecordStyle = 2;
            fundingSummaryModels.Add(totalTrailblazer1923Before);

            fundingSummaryModels.Add(new FundingSummaryModel());
            fundingSummaryModels.Add(new FundingSummaryModel("24+ Apprenticeship Frameworks for starts before 1 May 2017", HeaderType.All, 2));
            FundingSummaryModel apprenticeship24Programme = _fm35Builder.BuildWithFundLine("ILR 24+ Apprenticeship Frameworks Programme Funding (£)", fm35Task.Result, validLearnersTask.Result, new[] { "24+ Apprenticeship" }, new[] { Constants.Fm35OnProgrammeAttributeName, Constants.Fm35AimAchievementAttributeName, Constants.Fm35JobOutcomeAchievementAttributeName, Constants.Fm35BalancingAttributeName });
            fundingSummaryModels.Add(apprenticeship24Programme);
            FundingSummaryModel apprenticeship24Support = _fm35Builder.BuildWithFundLine("ILR 24+ Apprenticeship Frameworks Learning Support (£)", fm35Task.Result, validLearnersTask.Result, new[] { "24+ Apprenticeship" }, new[] { Constants.Fm35LearningSupportAttributeName });
            fundingSummaryModels.Add(apprenticeship24Support);
            FundingSummaryModel apprenticeship24Total = _totalBuilder.TotalRecords("ILR Total 24+ Apprenticeship Frameworks (£)", apprenticeship24Programme, apprenticeship24Support);
            if (!isFis)
            {
                apprenticeship24Total.ExcelHeaderStyle = 3;
                apprenticeship24Total.ExcelRecordStyle = 3;
                fundingSummaryModels.Add(apprenticeship24Total);

                // Todo: EAS
            }

            FundingSummaryModel totalApprenticeship24Before = _totalBuilder.TotalRecords("Total 24+ Apprenticeship Frameworks for starts before 1 May 2017 (£)", apprenticeship24Total); // Todo: EAS
            totalApprenticeship24Before.ExcelHeaderStyle = 2;
            totalApprenticeship24Before.ExcelRecordStyle = 2;
            fundingSummaryModels.Add(totalApprenticeship24Before);

            fundingSummaryModels.Add(new FundingSummaryModel("24+ Trailblazer Apprenticeships for starts before 1 May 2017", HeaderType.All, 2));
            FundingSummaryModel trailblazer24Funding = _fm81Builder.BuildWithFundLine("ILR 24+ Trailblazer Apprenticeships Programme Funding (Core Government Contribution, Maths and English)(£)", fm81Task.Result, validLearnersTask.Result, "24+ Trailblazer Apprenticeship", new[] { Constants.Fm81CoreGovContPayment, Constants.Fm81MathEngBalPayment, Constants.Fm81MathEngOnProgPayment });
            fundingSummaryModels.Add(trailblazer24Funding);
            FundingSummaryModel trailblazer24Payment = _fm81Builder.BuildWithFundLine("ILR 24+ Trailblazer Apprenticeships Employer Incentive Payments (Achievement, Small Employer, 16 - 18) (£)", fm81Task.Result, validLearnersTask.Result, "24+ Trailblazer Apprenticeship", new[] { Constants.Fm81AchPayment, Constants.Fm81SmallBusPayment, Constants.Fm81YoungAppPayment });
            fundingSummaryModels.Add(trailblazer24Payment);
            FundingSummaryModel trailblazer24Support = _fm81Builder.BuildWithFundLine("ILR 24+ Trailblazer Apprenticeships Learning Support (£)", fm81Task.Result, validLearnersTask.Result, "24+ Trailblazer Apprenticeship", new[] { Constants.Fm81LearnSuppFundCash });
            fundingSummaryModels.Add(trailblazer24Support);
            FundingSummaryModel trailblazer24Total = _totalBuilder.TotalRecords("ILR Total 24+ Trailblazer Apprenticeships (£)", trailblazer24Funding, trailblazer24Payment, trailblazer24Support);
            if (!isFis)
            {
                trailblazer24Total.ExcelHeaderStyle = 3;
                trailblazer24Total.ExcelRecordStyle = 3;
                fundingSummaryModels.Add(trailblazer24Total);

                // Todo: EAS
            }

            FundingSummaryModel totalTrailblazer24Before = _totalBuilder.TotalRecords("Total 24+ Trailblazer Apprenticeships for starts before 1 May 2017 (£)", trailblazer24Total); // Todo: EAS
            totalTrailblazer24Before.ExcelHeaderStyle = 2;
            totalTrailblazer24Before.ExcelRecordStyle = 2;
            fundingSummaryModels.Add(totalTrailblazer24Before);

            fundingSummaryModels.Add(new FundingSummaryModel());
            fundingSummaryModels.Add(new FundingSummaryModel("Adult Non-Levy Contracted Apprenticeships – Non-procured delivery", HeaderType.All, 2));
            fundingSummaryModels.Add(_fm36Builder.BuildWithFundLine("ILR Adult Non-Levy Contracted Apprenticeships Programme Aim Indicative Earnings (£)", fm36Task.Result, validLearnersTask.Result, new[] { "19+ Apprenticeship (From May 2017) Non-Levy Contract", "19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured)" }, new[] { Constants.Fm36ProgrammeAimOnProgPayment, Constants.Fm36ProgrammeAimBalPayment, Constants.Fm36ProgrammeAimCompletionPayment }));
            FundingSummaryModel nonLevyCoInvestAdult = _fm36Builder.BuildWithFundLine("...of which Indicative Government Co-Investment Earnings (£)", fm36Task.Result, validLearnersTask.Result, new[] { "19+ Apprenticeship (From May 2017) Non-Levy Contract", "19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured)" }, new[] { Constants.Fm36ProgrammeAimProgFundIndMinCoInvest });
            fundingSummaryModels.Add(nonLevyCoInvestAdult);
            FundingSummaryModel nonLevyMathsAndEnglishAdult = _fm36Builder.BuildWithFundLine("ILR Adult Non-Levy Contracted Apprenticeships Maths and English Programme Funding (£)", fm36Task.Result, validLearnersTask.Result, new[] { "19+ Apprenticeship (From May 2017) Non-Levy Contract", "19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured)" }, new[] { Constants.Fm36MathEngOnProgPayment, Constants.Fm36MathEngBalPayment });
            fundingSummaryModels.Add(nonLevyMathsAndEnglishAdult);
            FundingSummaryModel nonLevyUpliftAdult = _fm36Builder.BuildWithFundLine("ILR Adult Non-Levy Contracted Apprenticeships Framework Uplift (£)", fm36Task.Result, validLearnersTask.Result, new[] { "19+ Apprenticeship (From May 2017) Non-Levy Contract", "19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured)" }, new[] { Constants.Fm36LDApplic1618FrameworkUpliftBalancingPayment, Constants.Fm36LDApplic1618FrameworkUpliftCompletionPayment, Constants.Fm36LDApplic1618FrameworkUpliftOnProgPayment });
            fundingSummaryModels.Add(nonLevyUpliftAdult);
            FundingSummaryModel nonLevyDisadvantageAdult = _fm36Builder.BuildWithFundLine("ILR Adult Non-Levy Contracted Apprenticeships Disadvantage Payments (£)", fm36Task.Result, validLearnersTask.Result, new[] { "19+ Apprenticeship (From May 2017) Non-Levy Contract", "19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured)" }, new[] { Constants.Fm36DisadvFirstPayment, Constants.Fm36DisadvSecondPayment });
            fundingSummaryModels.Add(nonLevyDisadvantageAdult);
            FundingSummaryModel nonLevyProvidersAdult = _fm36Builder.BuildWithFundLine("ILR Adult Non-Levy Contracted Apprenticeships Additional Payments for Providers (£)", fm36Task.Result, validLearnersTask.Result, new[] { "19+ Apprenticeship (From May 2017) Non-Levy Contract", "19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured)" }, new[] { Constants.Fm36LearnDelFirstProv1618Pay, Constants.Fm36LearnDelSecondProv1618Pay });
            fundingSummaryModels.Add(nonLevyProvidersAdult);
            FundingSummaryModel nonLevyEmployersAdult = _fm36Builder.BuildWithFundLine("ILR Adult Non-Levy Contracted Apprenticeships Additional Payments for Employers (£)", fm36Task.Result, validLearnersTask.Result, new[] { "19+ Apprenticeship (From May 2017) Non-Levy Contract", "19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured)" }, new[] { Constants.Fm36LearnDelFirstEmp1618Pay, Constants.Fm36LearnDelSecondEmp1618Pay });
            fundingSummaryModels.Add(nonLevyEmployersAdult);
            FundingSummaryModel nonLevySupportAdult = _fm36Builder.BuildWithFundLine("ILR Adult Non-Levy Contracted Apprenticeships Learning Support (£)", fm36Task.Result, validLearnersTask.Result, new[] { "19+ Apprenticeship (From May 2017) Non-Levy Contract", "19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured)" }, new[] { Constants.Fm36LearnSuppFundCash });
            fundingSummaryModels.Add(nonLevySupportAdult);
            FundingSummaryModel nonLevyTotalAdult = _totalBuilder.TotalRecords("ILR Total Adult Non-Levy Contracted Apprenticeships (£)", nonLevyCoInvestAdult, nonLevyMathsAndEnglishAdult, nonLevyUpliftAdult, nonLevyDisadvantageAdult, nonLevyProvidersAdult, nonLevyEmployersAdult, nonLevySupportAdult);
            if (!isFis)
            {
                nonLevyTotalAdult.ExcelHeaderStyle = 3;
                nonLevyTotalAdult.ExcelRecordStyle = 3;
                fundingSummaryModels.Add(nonLevyTotalAdult);

                // Todo: EAS
            }

            FundingSummaryModel totalNonLevyNonProcuredAdult = _totalBuilder.TotalRecords("Total Adult Non-Levy Contracted Apprenticeships – Non-procured delivery (£)", nonLevyTotalAdult); // Todo: EAS
            totalNonLevyNonProcuredAdult.ExcelHeaderStyle = 2;
            totalNonLevyNonProcuredAdult.ExcelRecordStyle = 2;
            fundingSummaryModels.Add(totalNonLevyNonProcuredAdult);

            fundingSummaryModels.Add(new FundingSummaryModel());
            FundingSummaryModel totalCarryInBefore = _totalBuilder.TotalRecords("Total Carry-in Apprenticeships Budget (£) (for starts before 1 May 2017 and non-procured delivery)", totalApprenticeships1618, totalTrailblazer1618Before, total1618NonLevyNonProcured, totalApprenticeship1923Before, totalTrailblazer1923Before, totalApprenticeship24Before, totalTrailblazer24Before, totalNonLevyNonProcuredAdult);
            totalCarryInBefore.ExcelHeaderStyle = 0;
            totalCarryInBefore.ExcelRecordStyle = 0;
            fundingSummaryModels.Add(totalCarryInBefore);
            FundingSummaryModel totalCarryInCumulativeBefore = _totalBuilder.TotalRecordsCumulative("Total Carry-in Apprenticeships Budget Cumulative (£) (for starts before 1 May 2017 and non-procured delivery)", totalCarryInBefore);
            totalCarryInCumulativeBefore.ExcelHeaderStyle = 0;
            totalCarryInCumulativeBefore.ExcelRecordStyle = 0;
            fundingSummaryModels.Add(totalCarryInCumulativeBefore);

            fundingSummaryModels.Add(new FundingSummaryModel());
            fundingSummaryModels.Add(new FundingSummaryModel("Levy Contracted Apprenticeships Budget for starts on or after 1 May 2017", HeaderType.TitleOnly, 0));
            fundingSummaryModels.Add(new FundingSummaryModel());
            fundingSummaryModels.Add(new FundingSummaryModel("16-18 Levy Contracted Apprenticeships", HeaderType.All, 2));
            fundingSummaryModels.Add(_fm36Builder.BuildWithFundLine("ILR 16-18 Levy Contracted Apprenticeships Programme Aim Indicative Earnings (£)", fm36Task.Result, validLearnersTask.Result, new[] { "16-18 Apprenticeship (From May 2017) Levy Contract" }, new[] { Constants.Fm36ProgrammeAimOnProgPayment, Constants.Fm36ProgrammeAimBalPayment, Constants.Fm36ProgrammeAimCompletionPayment }));
            FundingSummaryModel levyCoInvest1618 = _fm36Builder.BuildWithFundLine("...of which Indicative Government Co-Investment Earnings (£)", fm36Task.Result, validLearnersTask.Result, new[] { "16-18 Apprenticeship (From May 2017) Levy Contract" }, new[] { Constants.Fm36ProgrammeAimProgFundIndMinCoInvest });
            fundingSummaryModels.Add(nonLevyCoInvestAdult);
            FundingSummaryModel levyMathsAndEnglish1618 = _fm36Builder.BuildWithFundLine("ILR Adult Non-Levy Contracted Apprenticeships Maths and English Programme Funding (£)", fm36Task.Result, validLearnersTask.Result, new[] { "16-18 Apprenticeship (From May 2017) Levy Contract" }, new[] { Constants.Fm36MathEngOnProgPayment, Constants.Fm36MathEngBalPayment });
            fundingSummaryModels.Add(nonLevyMathsAndEnglishAdult);
            FundingSummaryModel levyUplift1618 = _fm36Builder.BuildWithFundLine("ILR Adult Non-Levy Contracted Apprenticeships Framework Uplift (£)", fm36Task.Result, validLearnersTask.Result, new[] { "16-18 Apprenticeship (From May 2017) Levy Contract" }, new[] { Constants.Fm36LDApplic1618FrameworkUpliftBalancingPayment, Constants.Fm36LDApplic1618FrameworkUpliftCompletionPayment, Constants.Fm36LDApplic1618FrameworkUpliftOnProgPayment });
            fundingSummaryModels.Add(nonLevyUpliftAdult);
            FundingSummaryModel levyDisadvantage1618 = _fm36Builder.BuildWithFundLine("ILR Adult Non-Levy Contracted Apprenticeships Disadvantage Payments (£)", fm36Task.Result, validLearnersTask.Result, new[] { "16-18 Apprenticeship (From May 2017) Levy Contract" }, new[] { Constants.Fm36DisadvFirstPayment, Constants.Fm36DisadvSecondPayment });
            fundingSummaryModels.Add(nonLevyDisadvantageAdult);
            FundingSummaryModel levyProviders1618 = _fm36Builder.BuildWithFundLine("ILR Adult Non-Levy Contracted Apprenticeships Additional Payments for Providers (£)", fm36Task.Result, validLearnersTask.Result, new[] { "16-18 Apprenticeship (From May 2017) Levy Contract" }, new[] { Constants.Fm36LearnDelFirstProv1618Pay, Constants.Fm36LearnDelSecondProv1618Pay });
            fundingSummaryModels.Add(nonLevyProvidersAdult);
            FundingSummaryModel levyEmployers1618 = _fm36Builder.BuildWithFundLine("ILR Adult Non-Levy Contracted Apprenticeships Additional Payments for Employers (£)", fm36Task.Result, validLearnersTask.Result, new[] { "16-18 Apprenticeship (From May 2017) Levy Contract" }, new[] { Constants.Fm36LearnDelFirstEmp1618Pay, Constants.Fm36LearnDelSecondEmp1618Pay });
            fundingSummaryModels.Add(nonLevyEmployersAdult);
            FundingSummaryModel levySupport1618 = _fm36Builder.BuildWithFundLine("ILR Adult Non-Levy Contracted Apprenticeships Learning Support (£)", fm36Task.Result, validLearnersTask.Result, new[] { "16-18 Apprenticeship (From May 2017) Levy Contract" }, new[] { Constants.Fm36LearnSuppFundCash });
            fundingSummaryModels.Add(nonLevySupportAdult);
            FundingSummaryModel levyTotal1618 = _totalBuilder.TotalRecords("ILR Total Adult Non-Levy Contracted Apprenticeships (£)", levyCoInvest1618, levyMathsAndEnglish1618, levyUplift1618, levyDisadvantage1618, levyProviders1618, levyEmployers1618, levySupport1618);
            if (!isFis)
            {
                levyTotal1618.ExcelHeaderStyle = 3;
                levyTotal1618.ExcelRecordStyle = 3;
                fundingSummaryModels.Add(levyTotal1618);

                // Todo: EAS
            }

            FundingSummaryModel totalLevyNonProcured1618 = _totalBuilder.TotalRecords("Total 16-18 Levy Contracted Apprenticeships (£)", levyTotal1618); // Todo: EAS
            totalLevyNonProcured1618.ExcelHeaderStyle = 2;
            totalLevyNonProcured1618.ExcelRecordStyle = 2;
            fundingSummaryModels.Add(totalLevyNonProcured1618);

            fundingSummaryModels.Add(new FundingSummaryModel());
            fundingSummaryModels.Add(new FundingSummaryModel("Adult Levy Contracted Apprenticeships", HeaderType.All, 2));
            FundingSummaryModel levyIndicativeAdult = _fm36Builder.BuildWithFundLine("ILR Adult Levy Contracted Apprenticeships Programme Aim Indicative Earnings (£)", fm36Task.Result, validLearnersTask.Result, new[] { "19+ Apprenticeship (From May 2017) Levy Contract" }, new[] { Constants.Fm36ProgrammeAimOnProgPayment, Constants.Fm36ProgrammeAimBalPayment, Constants.Fm36ProgrammeAimCompletionPayment });
            fundingSummaryModels.Add(levyIndicativeAdult);
            FundingSummaryModel levyMathsAndEnglishAdult = _fm36Builder.BuildWithFundLine("ILR Adult Levy Contracted Apprenticeships Maths and English Programme Funding (£)", fm36Task.Result, validLearnersTask.Result, new[] { "19+ Apprenticeship (From May 2017) Levy Contract" }, new[] { Constants.Fm36MathEngOnProgPayment, Constants.Fm36MathEngBalPayment });
            fundingSummaryModels.Add(levyMathsAndEnglishAdult);
            FundingSummaryModel levyUpliftAdult = _fm36Builder.BuildWithFundLine("ILR Adult Levy Contracted Apprenticeships Framework Uplift (£)", fm36Task.Result, validLearnersTask.Result, new[] { "19+ Apprenticeship (From May 2017) Levy Contract" }, new[] { Constants.Fm36LDApplic1618FrameworkUpliftBalancingPayment, Constants.Fm36LDApplic1618FrameworkUpliftCompletionPayment, Constants.Fm36LDApplic1618FrameworkUpliftOnProgPayment });
            fundingSummaryModels.Add(levyUpliftAdult);
            FundingSummaryModel levyDisadvantageAdult = _fm36Builder.BuildWithFundLine("ILR Adult Levy Contracted Apprenticeships Disadvantage Payments (£)", fm36Task.Result, validLearnersTask.Result, new[] { "19+ Apprenticeship (From May 2017) Levy Contract" }, new[] { Constants.Fm36DisadvFirstPayment, Constants.Fm36DisadvSecondPayment });
            fundingSummaryModels.Add(levyDisadvantageAdult);
            FundingSummaryModel levyProvidersAdult = _fm36Builder.BuildWithFundLine("ILR Adult Levy Contracted Apprenticeships Additional Payments for Providers (£)", fm36Task.Result, validLearnersTask.Result, new[] { "19+ Apprenticeship (From May 2017) Levy Contract" }, new[] { Constants.Fm36LearnDelFirstProv1618Pay, Constants.Fm36LearnDelSecondProv1618Pay });
            fundingSummaryModels.Add(levyProvidersAdult);
            FundingSummaryModel levyEmployersAdult = _fm36Builder.BuildWithFundLine("ILR Adult Levy Contracted Apprenticeships Additional Payments for Employers (£)", fm36Task.Result, validLearnersTask.Result, new[] { "19+ Apprenticeship (From May 2017) Levy Contract" }, new[] { Constants.Fm36LearnDelFirstEmp1618Pay, Constants.Fm36LearnDelSecondEmp1618Pay });
            fundingSummaryModels.Add(levyEmployersAdult);
            FundingSummaryModel levyApprenticesAdult = _fm36Builder.BuildWithFundLine("ILR Adult Levy Contracted Apprenticeships Additional Payments for Employers (£)", fm36Task.Result, validLearnersTask.Result, new[] { "19+ Apprenticeship (From May 2017) Levy Contract" }, new[] { Constants.Fm36LearnSuppFundCash });
            fundingSummaryModels.Add(levyApprenticesAdult);
            FundingSummaryModel levySupportAdult = _fm36Builder.BuildWithFundLine("ILR Adult Levy Contracted Apprenticeships Learning Support (£)", fm36Task.Result, validLearnersTask.Result, new[] { "19+ Apprenticeship (From May 2017) Levy Contract" }, new[] { Constants.Fm36LearnSuppFundCash });
            fundingSummaryModels.Add(nonLevySupportAdult);
            FundingSummaryModel levyTotalAdultIlr = _totalBuilder.TotalRecords("ILR Total Adult Levy Contracted Apprenticeships (£)", levyIndicativeAdult, levyMathsAndEnglishAdult, levyUpliftAdult, levyDisadvantageAdult, levyProvidersAdult, levyEmployersAdult, levyApprenticesAdult, levySupportAdult);
            if (!isFis)
            {
                levyTotalAdultIlr.ExcelHeaderStyle = 3;
                levyTotalAdultIlr.ExcelRecordStyle = 3;
                fundingSummaryModels.Add(levyTotalAdultIlr);

                // Todo: EAS
            }

            FundingSummaryModel totalLevyNonProcuredAdult = _totalBuilder.TotalRecords("Total Adult Levy Contracted Apprenticeships (£)", levyTotalAdultIlr); // Todo: EAS
            totalLevyNonProcuredAdult.ExcelHeaderStyle = 2;
            totalLevyNonProcuredAdult.ExcelRecordStyle = 2;
            fundingSummaryModels.Add(totalLevyNonProcuredAdult);

            fundingSummaryModels.Add(new FundingSummaryModel());
            FundingSummaryModel totalLevyApprenticeshipAfter = _totalBuilder.TotalRecords("Total Levy Contracted Apprenticeships Budget for starts on or after 1 May 2017 (£)", levyTotalAdultIlr, totalLevyNonProcured1618);
            totalLevyApprenticeshipAfter.ExcelHeaderStyle = 0;
            totalLevyApprenticeshipAfter.ExcelRecordStyle = 0;
            fundingSummaryModels.Add(totalLevyApprenticeshipAfter);
            FundingSummaryModel totalLevyNonProcuredAdultCumulative = _totalBuilder.TotalRecordsCumulative("Total Levy Contracted Apprenticeships Budget Cumulative for starts on or after 1 May 2017 (£)", totalLevyApprenticeshipAfter);
            totalLevyNonProcuredAdultCumulative.ExcelHeaderStyle = 0;
            totalLevyNonProcuredAdultCumulative.ExcelRecordStyle = 0;
            fundingSummaryModels.Add(totalLevyNonProcuredAdultCumulative);

            fundingSummaryModels.Add(new FundingSummaryModel());
            fundingSummaryModels.Add(new FundingSummaryModel("Non-Levy Contracted Apprenticeships Budget - Procured delivery", HeaderType.TitleOnly, 0));

            fundingSummaryModels.Add(new FundingSummaryModel());
            fundingSummaryModels.Add(new FundingSummaryModel("16-18 Non-Levy Contracted Apprenticeships", HeaderType.All, 2));
            fundingSummaryModels.Add(_fm36Builder.BuildWithFundLine("ILR 16-18 Non-Levy Contracted Apprenticeships Programme Aim Indicative Earnings (£)", fm36Task.Result, validLearnersTask.Result, new[] { "16-18 Apprenticeship Non-Levy Contract (procured)" }, new[] { Constants.Fm36ProgrammeAimOnProgPayment, Constants.Fm36ProgrammeAimBalPayment, Constants.Fm36ProgrammeAimCompletionPayment }));
            FundingSummaryModel nonLevyCoInvest1618 = _fm36Builder.BuildWithFundLine("...of which Indicative Government Co-Investment Earnings (£)", fm36Task.Result, validLearnersTask.Result, new[] { "16-18 Apprenticeship Non-Levy Contract (procured)" }, new[] { Constants.Fm36ProgrammeAimProgFundIndMinCoInvest });
            fundingSummaryModels.Add(nonLevyCoInvest1618);
            FundingSummaryModel nonLevyMathsAndEnglish1618 = _fm36Builder.BuildWithFundLine("ILR Adult Non-Levy Contracted Apprenticeships Maths and English Programme Funding (£)", fm36Task.Result, validLearnersTask.Result, new[] { "16-18 Apprenticeship Non-Levy Contract (procured)" }, new[] { Constants.Fm36MathEngOnProgPayment, Constants.Fm36MathEngBalPayment });
            fundingSummaryModels.Add(nonLevyMathsAndEnglish1618);
            FundingSummaryModel nonLevyUplift1618 = _fm36Builder.BuildWithFundLine("ILR Adult Non-Levy Contracted Apprenticeships Framework Uplift (£)", fm36Task.Result, validLearnersTask.Result, new[] { "16-18 Apprenticeship Non-Levy Contract (procured)" }, new[] { Constants.Fm36LDApplic1618FrameworkUpliftBalancingPayment, Constants.Fm36LDApplic1618FrameworkUpliftCompletionPayment, Constants.Fm36LDApplic1618FrameworkUpliftOnProgPayment });
            fundingSummaryModels.Add(nonLevyUplift1618);
            FundingSummaryModel nonLevyDisadvantage1618 = _fm36Builder.BuildWithFundLine("ILR Adult Non-Levy Contracted Apprenticeships Disadvantage Payments (£)", fm36Task.Result, validLearnersTask.Result, new[] { "16-18 Apprenticeship Non-Levy Contract (procured)" }, new[] { Constants.Fm36DisadvFirstPayment, Constants.Fm36DisadvSecondPayment });
            fundingSummaryModels.Add(nonLevyDisadvantage1618);
            FundingSummaryModel nonLevyProviders1618 = _fm36Builder.BuildWithFundLine("ILR Adult Non-Levy Contracted Apprenticeships Additional Payments for Providers (£)", fm36Task.Result, validLearnersTask.Result, new[] { "16-18 Apprenticeship Non-Levy Contract (procured)" }, new[] { Constants.Fm36LearnDelFirstProv1618Pay, Constants.Fm36LearnDelSecondProv1618Pay });
            fundingSummaryModels.Add(nonLevyProviders1618);
            FundingSummaryModel nonLevyEmployers1618 = _fm36Builder.BuildWithFundLine("ILR Adult Non-Levy Contracted Apprenticeships Additional Payments for Employers (£)", fm36Task.Result, validLearnersTask.Result, new[] { "16-18 Apprenticeship Non-Levy Contract (procured)" }, new[] { Constants.Fm36LearnDelFirstEmp1618Pay, Constants.Fm36LearnDelSecondEmp1618Pay });
            fundingSummaryModels.Add(nonLevyEmployers1618);
            FundingSummaryModel nonLevyApprentices1618 = _fm36Builder.BuildWithFundLine("ILR Adult Non-Levy Contracted Apprenticeships Additional Payments for Employers (£)", fm36Task.Result, validLearnersTask.Result, new[] { "16-18 Apprenticeship Non-Levy Contract (procured)" }, new[] { Constants.Fm36LearnDelLearnAddPayment });
            fundingSummaryModels.Add(nonLevyApprentices1618);
            FundingSummaryModel nonLevySupport1618 = _fm36Builder.BuildWithFundLine("ILR Adult Non-Levy Contracted Apprenticeships Learning Support (£)", fm36Task.Result, validLearnersTask.Result, new[] { "16-18 Apprenticeship Non-Levy Contract (procured)" }, new[] { Constants.Fm36LearnSuppFundCash });
            fundingSummaryModels.Add(nonLevySupport1618);
            FundingSummaryModel nonLevyTotal1618 = _totalBuilder.TotalRecords("ILR Total Adult Non-Levy Contracted Apprenticeships (£)", nonLevyCoInvest1618, nonLevyMathsAndEnglish1618, nonLevyUplift1618, nonLevyDisadvantage1618, nonLevyProviders1618, nonLevyEmployers1618, nonLevyApprentices1618, nonLevySupport1618);
            if (!isFis)
            {
                nonLevyTotal1618.ExcelHeaderStyle = 3;
                nonLevyTotal1618.ExcelRecordStyle = 3;
                fundingSummaryModels.Add(nonLevyTotal1618);

                // Todo: EAS
            }

            FundingSummaryModel totalNonLevyNonProcured1618 = _totalBuilder.TotalRecords("Total 16-18 Non-Levy Contracted Apprenticeships (£)", nonLevyTotal1618); // Todo: EAS
            totalNonLevyNonProcured1618.ExcelHeaderStyle = 2;
            totalNonLevyNonProcured1618.ExcelRecordStyle = 2;
            fundingSummaryModels.Add(totalNonLevyNonProcured1618);

            fundingSummaryModels.Add(new FundingSummaryModel());
            fundingSummaryModels.Add(new FundingSummaryModel("Adult Non-Levy Contracted Apprenticeships", HeaderType.All, 2));
            fundingSummaryModels.Add(_fm36Builder.BuildWithFundLine("ILR Adult Non-Levy Contracted Apprenticeships Programme Aim Indicative Earnings (£)", fm36Task.Result, validLearnersTask.Result, new[] { "19+ Apprenticeship Non-Levy Contract (procured)" }, new[] { Constants.Fm36ProgrammeAimOnProgPayment, Constants.Fm36ProgrammeAimBalPayment, Constants.Fm36ProgrammeAimCompletionPayment }));
            FundingSummaryModel nonLevyCoInvestProcuredAdult = _fm36Builder.BuildWithFundLine("...of which Indicative Government Co-Investment Earnings (£)", fm36Task.Result, validLearnersTask.Result, new[] { "19+ Apprenticeship Non-Levy Contract (procured)" }, new[] { Constants.Fm36ProgrammeAimProgFundIndMinCoInvest });
            fundingSummaryModels.Add(nonLevyCoInvestProcuredAdult);
            FundingSummaryModel nonLevyMathsAndEnglishProcuredAdult = _fm36Builder.BuildWithFundLine("ILR Adult Non-Levy Contracted Apprenticeships Maths and English Programme Funding (£)", fm36Task.Result, validLearnersTask.Result, new[] { "19+ Apprenticeship Non-Levy Contract (procured)" }, new[] { Constants.Fm36MathEngOnProgPayment, Constants.Fm36MathEngBalPayment });
            fundingSummaryModels.Add(nonLevyMathsAndEnglishProcuredAdult);
            FundingSummaryModel nonLevyUpliftProcuredAdult = _fm36Builder.BuildWithFundLine("ILR Adult Non-Levy Contracted Apprenticeships Framework Uplift (£)", fm36Task.Result, validLearnersTask.Result, new[] { "19+ Apprenticeship Non-Levy Contract (procured)" }, new[] { Constants.Fm36LDApplic1618FrameworkUpliftBalancingPayment, Constants.Fm36LDApplic1618FrameworkUpliftCompletionPayment, Constants.Fm36LDApplic1618FrameworkUpliftOnProgPayment });
            fundingSummaryModels.Add(nonLevyUpliftProcuredAdult);
            FundingSummaryModel nonLevyDisadvantageProcuredAdult = _fm36Builder.BuildWithFundLine("ILR Adult Non-Levy Contracted Apprenticeships Disadvantage Payments (£)", fm36Task.Result, validLearnersTask.Result, new[] { "19+ Apprenticeship Non-Levy Contract (procured)" }, new[] { Constants.Fm36DisadvFirstPayment, Constants.Fm36DisadvSecondPayment });
            fundingSummaryModels.Add(nonLevyDisadvantageProcuredAdult);
            FundingSummaryModel nonLevyProvidersProcuredAdult = _fm36Builder.BuildWithFundLine("ILR Adult Non-Levy Contracted Apprenticeships Additional Payments for Providers (£)", fm36Task.Result, validLearnersTask.Result, new[] { "19+ Apprenticeship Non-Levy Contract (procured)" }, new[] { Constants.Fm36LearnDelFirstProv1618Pay, Constants.Fm36LearnDelSecondProv1618Pay });
            fundingSummaryModels.Add(nonLevyProvidersProcuredAdult);
            FundingSummaryModel nonLevyEmployersProcuredAdult = _fm36Builder.BuildWithFundLine("ILR Adult Non-Levy Contracted Apprenticeships Additional Payments for Employers (£)", fm36Task.Result, validLearnersTask.Result, new[] { "19+ Apprenticeship Non-Levy Contract (procured)" }, new[] { Constants.Fm36LearnDelFirstEmp1618Pay, Constants.Fm36LearnDelSecondEmp1618Pay });
            fundingSummaryModels.Add(nonLevyEmployersProcuredAdult);
            FundingSummaryModel nonLevyApprenticesProcuredAdult = _fm36Builder.BuildWithFundLine("ILR Adult Non-Levy Contracted Apprenticeships Additional Payments for Employers (£)", fm36Task.Result, validLearnersTask.Result, new[] { "19+ Apprenticeship Non-Levy Contract (procured)" }, new[] { Constants.Fm36LearnDelLearnAddPayment });
            fundingSummaryModels.Add(nonLevyApprenticesProcuredAdult);
            FundingSummaryModel nonLevySupportProcuredAdult = _fm36Builder.BuildWithFundLine("ILR Adult Non-Levy Contracted Apprenticeships Learning Support (£)", fm36Task.Result, validLearnersTask.Result, new[] { "19+ Apprenticeship Non-Levy Contract (procured)" }, new[] { Constants.Fm36LearnSuppFundCash });
            fundingSummaryModels.Add(nonLevySupportProcuredAdult);
            FundingSummaryModel nonLevyTotalProcuredAdult = _totalBuilder.TotalRecords("ILR Total Adult Non-Levy Contracted Apprenticeships (£)", nonLevyCoInvestProcuredAdult, nonLevyMathsAndEnglishProcuredAdult, nonLevyUpliftProcuredAdult, nonLevyDisadvantageProcuredAdult, nonLevyProvidersProcuredAdult, nonLevyEmployersProcuredAdult, nonLevyApprenticesProcuredAdult, nonLevySupportProcuredAdult);
            if (!isFis)
            {
                nonLevyTotalProcuredAdult.ExcelHeaderStyle = 3;
                nonLevyTotalProcuredAdult.ExcelRecordStyle = 3;
                fundingSummaryModels.Add(nonLevyTotalProcuredAdult);

                // Todo: EAS
            }

            fundingSummaryModels.Add(new FundingSummaryModel());
            FundingSummaryModel totalNonLevyProcuredAdult = _totalBuilder.TotalRecords("Total Adult Non-Levy Contracted Apprenticeships (£)", nonLevyTotalProcuredAdult); // Todo: EAS
            totalNonLevyProcuredAdult.ExcelHeaderStyle = 0;
            totalNonLevyProcuredAdult.ExcelRecordStyle = 0;
            fundingSummaryModels.Add(totalNonLevyProcuredAdult);
            FundingSummaryModel totalNonLevyProcuredAdultCumulative = _totalBuilder.TotalRecordsCumulative("Total Non-Levy Contracted Apprenticeships Budget - Procured delivery (£)", nonLevyTotalProcuredAdult);
            totalNonLevyProcuredAdultCumulative.ExcelHeaderStyle = 0;
            totalNonLevyProcuredAdultCumulative.ExcelRecordStyle = 0;
            fundingSummaryModels.Add(totalNonLevyProcuredAdultCumulative);
            fundingSummaryModels.Add(new FundingSummaryModel());

            fundingSummaryModels.Add(new FundingSummaryModel("Adult Education Budget – Non-procured delivery", HeaderType.TitleOnly, 0));
            fundingSummaryModels.Add(new FundingSummaryModel());

            fundingSummaryModels.Add(new FundingSummaryModel("19-24 Traineeships", HeaderType.All, 2));
            FundingSummaryModel traineeship1924Funding = _fm35Builder.BuildWithFundLine("ILR 19-24 Traineeships Programme Funding (£)", fm35Task.Result, validLearnersTask.Result, new[] { "19-24 Traineeship", "19-24 Traineeship (non-procured)" }, new[] { Constants.Fm35OnProgrammeAttributeName, Constants.Fm35AimAchievementAttributeName, Constants.Fm35JobOutcomeAchievementAttributeName, Constants.Fm35BalancingAttributeName });
            fundingSummaryModels.Add(traineeship1924Funding);
            FundingSummaryModel traineeship1924Support = _fm35Builder.BuildWithFundLine("ILR 19-24 Traineeships Learning Support (£)", fm35Task.Result, validLearnersTask.Result, new[] { "19-24 Traineeship", "19-24 Traineeship (non-procured)" }, new[] { Constants.Fm35LearningSupportAttributeName });
            fundingSummaryModels.Add(traineeship1924Support);
            FundingSummaryModel traineeship1924Total = _totalBuilder.TotalRecords("ILR Total 19-24 Traineeships (£)", traineeship1924Funding, traineeship1924Support);
            if (!isFis)
            {
                traineeship1924Total.ExcelHeaderStyle = 3;
                traineeship1924Total.ExcelRecordStyle = 3;
                fundingSummaryModels.Add(traineeship1924Total);

                // Todo: EAS
            }

            FundingSummaryModel totalTraineeship1924 = _totalBuilder.TotalRecords("Total 19-24 Traineeships (£)", traineeship1924Total); // Todo: EAS
            totalTraineeship1924.ExcelHeaderStyle = 2;
            totalTraineeship1924.ExcelRecordStyle = 2;
            fundingSummaryModels.Add(totalTraineeship1924);

            fundingSummaryModels.Add(new FundingSummaryModel());
            fundingSummaryModels.Add(new FundingSummaryModel("AEB - Other Learning", HeaderType.All, 2));
            FundingSummaryModel aebFunding = _fm35Builder.BuildWithFundLine("ILR AEB - Other Learning Programme Funding (£)", fm35Task.Result, validLearnersTask.Result, new[] { "AEB - Other Learning", "AEB - Other Learning (non-procured)" }, new[] { Constants.Fm35OnProgrammeAttributeName, Constants.Fm35AimAchievementAttributeName, Constants.Fm35JobOutcomeAchievementAttributeName, Constants.Fm35BalancingAttributeName });
            fundingSummaryModels.Add(aebFunding);
            FundingSummaryModel aebSupport = _fm35Builder.BuildWithFundLine("ILR AEB - Other Learning Learning Support (£)", fm35Task.Result, validLearnersTask.Result, new[] { "AEB - Other Learning", "AEB - Other Learning (non-procured)" }, new[] { Constants.Fm35LearningSupportAttributeName });
            fundingSummaryModels.Add(aebSupport);
            FundingSummaryModel aebTotal = _totalBuilder.TotalRecords("ILR Total AEB - Other Learning (£)", aebFunding, aebSupport);
            if (!isFis)
            {
                aebTotal.ExcelHeaderStyle = 3;
                aebTotal.ExcelRecordStyle = 3;
                fundingSummaryModels.Add(aebTotal);

                // Todo: EAS
            }

            FundingSummaryModel totalAeb = _totalBuilder.TotalRecords("Total AEB - Other Learning (£)", aebTotal); // Todo: EAS
            totalAeb.ExcelHeaderStyle = 2;
            totalAeb.ExcelRecordStyle = 2;
            fundingSummaryModels.Add(totalAeb);

            fundingSummaryModels.Add(new FundingSummaryModel());
            FundingSummaryModel totalNonProcuredAdult = _totalBuilder.TotalRecords("Total Adult Education Budget – Non-procured delivery (£)", traineeship1924Total, aebTotal); // Todo: EAS
            totalNonProcuredAdult.ExcelHeaderStyle = 0;
            totalNonProcuredAdult.ExcelRecordStyle = 0;
            fundingSummaryModels.Add(totalNonProcuredAdult);
            FundingSummaryModel totalNonProcuredAdultCumulative = _totalBuilder.TotalRecordsCumulative("Total Adult Education Budget Cumulative – Non-procured delivery (£)", totalNonProcuredAdult);
            totalNonProcuredAdultCumulative.ExcelHeaderStyle = 0;
            totalNonProcuredAdultCumulative.ExcelRecordStyle = 0;
            fundingSummaryModels.Add(totalNonProcuredAdultCumulative);

            fundingSummaryModels.Add(new FundingSummaryModel());
            fundingSummaryModels.Add(new FundingSummaryModel("Adult Education Budget – Procured delivery from 1 Nov 2017", HeaderType.TitleOnly, 0));

            fundingSummaryModels.Add(new FundingSummaryModel());
            fundingSummaryModels.Add(new FundingSummaryModel("19-24 Traineeships", HeaderType.All, 2));
            FundingSummaryModel traineeship1924FundingProcured = _fm35Builder.BuildWithFundLine("ILR 19-24 Traineeships Programme Funding (£)", fm35Task.Result, validLearnersTask.Result, new[] { "19-24 Traineeship (procured from Nov 2017)" }, new[] { Constants.Fm35OnProgrammeAttributeName, Constants.Fm35AimAchievementAttributeName, Constants.Fm35JobOutcomeAchievementAttributeName, Constants.Fm35BalancingAttributeName });
            fundingSummaryModels.Add(traineeship1924FundingProcured);
            FundingSummaryModel traineeship1924SupportProcured = _fm35Builder.BuildWithFundLine("ILR 19-24 Traineeships Learning Support (£)", fm35Task.Result, validLearnersTask.Result, new[] { "19-24 Traineeship (procured from Nov 2017)" }, new[] { Constants.Fm35LearningSupportAttributeName });
            fundingSummaryModels.Add(traineeship1924SupportProcured);
            FundingSummaryModel traineeship1924ProcuredTotal = _totalBuilder.TotalRecords("ILR Total 19-24 Traineeships (£)", traineeship1924FundingProcured, traineeship1924SupportProcured);
            if (!isFis)
            {
                traineeship1924ProcuredTotal.ExcelHeaderStyle = 3;
                traineeship1924ProcuredTotal.ExcelRecordStyle = 3;
                fundingSummaryModels.Add(traineeship1924ProcuredTotal);

                // Todo: EAS
            }

            FundingSummaryModel totalTraineeship1924Procured = _totalBuilder.TotalRecords("Total 19-24 Traineeships (£)", traineeship1924ProcuredTotal); // Todo: EAS
            totalTraineeship1924Procured.ExcelHeaderStyle = 2;
            totalTraineeship1924Procured.ExcelRecordStyle = 2;
            fundingSummaryModels.Add(totalTraineeship1924Procured);

            fundingSummaryModels.Add(new FundingSummaryModel());
            fundingSummaryModels.Add(new FundingSummaryModel("AEB - Other Learning", HeaderType.All, 2));
            FundingSummaryModel aebFundingProcured = _fm35Builder.BuildWithFundLine("ILR AEB - Other Learning Programme Funding (£)", fm35Task.Result, validLearnersTask.Result, new[] { "AEB - Other Learning (procured from Nov 2017)" }, new[] { Constants.Fm35OnProgrammeAttributeName, Constants.Fm35AimAchievementAttributeName, Constants.Fm35JobOutcomeAchievementAttributeName, Constants.Fm35BalancingAttributeName });
            fundingSummaryModels.Add(aebFundingProcured);
            FundingSummaryModel aebSupportProcured = _fm35Builder.BuildWithFundLine("ILR AEB - Other Learning Learning Support (£)", fm35Task.Result, validLearnersTask.Result, new[] { "AEB - Other Learning (procured from Nov 2017)" }, new[] { Constants.Fm35LearningSupportAttributeName });
            fundingSummaryModels.Add(aebSupportProcured);
            FundingSummaryModel aebProcuredTotal = _totalBuilder.TotalRecords("ILR Total AEB - Other Learning (£)", aebFundingProcured, aebSupportProcured);
            if (!isFis)
            {
                aebProcuredTotal.ExcelHeaderStyle = 3;
                aebProcuredTotal.ExcelRecordStyle = 3;
                fundingSummaryModels.Add(aebProcuredTotal);

                // Todo: EAS
            }

            FundingSummaryModel totalAebProcured = _totalBuilder.TotalRecords("Total AEB - Other Learning (£)", aebProcuredTotal); // Todo: EAS
            totalAebProcured.ExcelHeaderStyle = 2;
            totalAebProcured.ExcelRecordStyle = 2;
            fundingSummaryModels.Add(totalAebProcured);

            fundingSummaryModels.Add(new FundingSummaryModel());
            FundingSummaryModel totalProcuredAdult = _totalBuilder.TotalRecords("Total Adult Education Budget – Procured delivery from 1 Nov 2017 (£)", traineeship1924Total, totalAebProcured); // Todo: EAS
            totalProcuredAdult.ExcelHeaderStyle = 0;
            totalProcuredAdult.ExcelRecordStyle = 0;
            fundingSummaryModels.Add(totalProcuredAdult);
            FundingSummaryModel totalProcuredAdultCumulative = _totalBuilder.TotalRecordsCumulative("Total Adult Education Budget Cumulative – Procured delivery from 1 Nov 2017 (£)", totalProcuredAdult);
            totalProcuredAdultCumulative.ExcelHeaderStyle = 0;
            totalProcuredAdultCumulative.ExcelRecordStyle = 0;
            fundingSummaryModels.Add(totalProcuredAdultCumulative);

            fundingSummaryModels.Add(new FundingSummaryModel());
            fundingSummaryModels.Add(new FundingSummaryModel("Advanced Loans Bursary Budget", HeaderType.TitleOnly, 0));
            fundingSummaryModels.Add(new FundingSummaryModel("Advanced Loans Bursary", HeaderType.All, 2));
            List<FundingSummaryModel> albModels = await _allbBuilder.BuildAsync(jobContextMessage, cancellationToken);
            fundingSummaryModels.AddRange(albModels);
            FundingSummaryModel albIlrTotal = _totalBuilder.TotalRecords("ILR Total Advanced Loans Bursary (£)", albModels[0], albModels[1]);
            if (!isFis)
            {
                albIlrTotal.ExcelHeaderStyle = 3;
                albIlrTotal.ExcelRecordStyle = 3;
                fundingSummaryModels.Add(albIlrTotal);
            }

            FundingSummaryModel totalAlb = _totalBuilder.TotalRecords("Total Advanced Loans Bursary (£)", albIlrTotal); // Todo: EAS
            totalAlb.ExcelHeaderStyle = 2;
            totalAlb.ExcelRecordStyle = 2;
            fundingSummaryModels.Add(totalAlb);

            FundingSummaryModel totalAlbBudget = (FundingSummaryModel)totalAlb.Clone();
            totalAlbBudget.ExcelHeaderStyle = 0;
            totalAlbBudget.ExcelRecordStyle = 0;
            fundingSummaryModels.Add(totalAlbBudget);
            FundingSummaryModel totalAlbBudgetCumulative = _totalBuilder.TotalRecordsCumulative("Total Advanced Loans Bursary Budget Cumulative (£)", totalAlbBudget);
            totalAlbBudgetCumulative.ExcelHeaderStyle = 0;
            totalAlbBudgetCumulative.ExcelRecordStyle = 0;
            fundingSummaryModels.Add(totalAlbBudgetCumulative);

            fundingSummaryModels.Add(new FundingSummaryModel(Constants.ALBInfoText, HeaderType.TitleOnly));
            fundingSummaryModels.Add(new FundingSummaryModel());

            // Todo: EAS

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
                        foreach (FundingSummaryModel fundingSummaryModel in fundingSummaryModels)
                        {
                            if (string.IsNullOrEmpty(fundingSummaryModel.Title))
                            {
                                WriteCsvRecords(csvWriter);
                                continue;
                            }

                            if (fundingSummaryModel.HeaderType == HeaderType.TitleOnly)
                            {
                                WriteCsvRecords(csvWriter, (object)fundingSummaryModel.Title);
                                continue;
                            }

                            if (fundingSummaryModel.HeaderType == HeaderType.All)
                            {
                                _fundingSummaryMapper.MemberMaps.Single(x => x.Data.Index == 0).Name(fundingSummaryModel.Title);
                                WriteCsvRecords(csvWriter, _fundingSummaryMapper);
                                continue;
                            }

                            WriteCsvRecords(csvWriter, _fundingSummaryMapper, _cachedModelProperties, fundingSummaryModel);
                        }

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
            foreach (FundingSummaryModel fundingSummaryModel in fundingSummaryModels)
            {
                if (string.IsNullOrEmpty(fundingSummaryModel.Title))
                {
                    WriteExcelRecords(sheet);
                    continue;
                }

                CellStyle excelHeaderStyle = _excelStyleProvider.GetCellStyle(cellStyles, fundingSummaryModel.ExcelHeaderStyle);

                if (fundingSummaryModel.HeaderType == HeaderType.TitleOnly)
                {
                    WriteExcelRecords(sheet, fundingSummaryModel.Title, excelHeaderStyle, 17);
                    continue;
                }

                if (fundingSummaryModel.HeaderType == HeaderType.All)
                {
                    _fundingSummaryMapper.MemberMaps.Single(x => x.Data.Index == 0).Name(fundingSummaryModel.Title);
                    WriteExcelRecords(sheet, _fundingSummaryMapper, excelHeaderStyle);
                    continue;
                }

                CellStyle excelRecordStyle = _excelStyleProvider.GetCellStyle(cellStyles, fundingSummaryModel.ExcelRecordStyle);

                WriteExcelRecords(sheet, _fundingSummaryMapper, _cachedModelProperties, fundingSummaryModel, excelRecordStyle);
            }

            WriteExcelRecords(sheet, new FundingSummaryFooterMapper(), new List<FundingSummaryFooterModel> { fundingSummaryFooterModel }, cellStyles[5], cellStyles[5], true);

            return workbook;
        }

        private async Task<FundingSummaryHeaderModel> GetHeader(IJobContextMessage jobContextMessage, Task<IMessage> messageTask, Task<string> providerNameTask, CancellationToken cancellationToken)
        {
            FundingSummaryHeaderModel fundingSummaryHeaderModel = new FundingSummaryHeaderModel
            {
                IlrFile = Path.GetFileName(jobContextMessage.KeyValuePairs[JobContextMessageKey.Filename].ToString()),
                Ukprn = messageTask.Result.HeaderEntity.SourceEntity.UKPRN,
                ProviderName = providerNameTask.Result ?? "Unknown",
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