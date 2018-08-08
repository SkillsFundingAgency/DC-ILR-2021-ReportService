using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR1819.ReportService.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Reports;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Model.Report;
using ESFA.DC.ILR1819.ReportService.Service.Mapper;
using ESFA.DC.ILR1819.ReportService.Service.Model;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR1819.ReportService.Service.Reports
{
    public sealed class MathsAndEnglishReport : AbstractReportBuilder, IReport
    {
        private readonly ILogger _logger;
        private readonly IKeyValuePersistenceService _storage;
        private readonly IKeyValuePersistenceService _redis;
        private readonly IXmlSerializationService _xmlSerializationService;
        private readonly IJsonSerializationService _jsonSerializationService;
        private readonly IIlrProviderService _ilrProviderService;
        private readonly IValidLearnersService _validLearnersService;
        private readonly IStringUtilitiesService _stringUtilitiesService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public MathsAndEnglishReport(
            ILogger logger,
            [KeyFilter(PersistenceStorageKeys.Blob)] IKeyValuePersistenceService storage,
            [KeyFilter(PersistenceStorageKeys.Redis)] IKeyValuePersistenceService redis,
            IXmlSerializationService xmlSerializationService,
            IJsonSerializationService jsonSerializationService,
            IIlrProviderService ilrProviderService,
            IValidLearnersService validLearnersService,
            IStringUtilitiesService stringUtilitiesService,
            IDateTimeProvider dateTimeProvider)
        {
            _logger = logger;
            _storage = storage;
            _redis = redis;
            _xmlSerializationService = xmlSerializationService;
            _jsonSerializationService = jsonSerializationService;
            _ilrProviderService = ilrProviderService;
            _validLearnersService = validLearnersService;
            _stringUtilitiesService = stringUtilitiesService;
            _dateTimeProvider = dateTimeProvider;
        }

        public ReportType ReportType { get; } = ReportType.MathsAndEnglish;

        public string GetReportFilename()
        {
            System.DateTime dateTime = _dateTimeProvider.ConvertUtcToUk(_dateTimeProvider.GetNowUtc());
            return $"Maths and English Report {dateTime:yyyyMMdd-HHmmss}";
        }

        public async Task GenerateReport(IJobContextMessage jobContextMessage, ZipArchive archive, CancellationToken cancellationToken)
        {
            string filename = GetReportFilename();
            string csv = await GetCsv(jobContextMessage, cancellationToken);
            await _storage.SaveAsync($"{filename}.csv", csv, cancellationToken);
            await WriteZipEntry(archive, $"{filename}.csv", csv);
        }

        private async Task<string> GetCsv(IJobContextMessage jobContextMessage, CancellationToken cancellationToken)
        {
            Task<IMessage> ilrFileTask = _ilrProviderService.GetIlrFile(jobContextMessage, cancellationToken);
            Task<List<string>> validLearnersTask = _validLearnersService.GetLearnersAsync(jobContextMessage, cancellationToken);

            await Task.WhenAll(ilrFileTask, validLearnersTask);

            if (cancellationToken.IsCancellationRequested)
            {
                return null;
            }

            List<string> ilrError = new List<string>();

            List<MathsAndEnglishModel> mathsAndEnglishModels = new List<MathsAndEnglishModel>(validLearnersTask.Result.Count);
            foreach (string validLearnerRefNum in validLearnersTask.Result)
            {
                var learner =
                    ilrFileTask.Result?.Learners?.SingleOrDefault(x => x.LearnRefNumber == validLearnerRefNum);
                if (learner == null)
                {
                    ilrError.Add(validLearnerRefNum);
                    continue;
                }

                mathsAndEnglishModels.Add(new MathsAndEnglishModel()
                {
                    FundLine = "Todo", // Todo
                    LearnRefNumber = learner.LearnRefNumber,
                    FamilyName = learner.FamilyName,
                    GivenNames = learner.GivenNames,
                    DateOfBirth = learner.DateOfBirthNullable?.ToString("dd/MM/yyyy"),
                    CampId = learner.CampId,
                    ConditionOfFundingMaths = "Todo", // Todo
                    ConditionOfFundingEnglish = "Todo", // Todo
                    RateBand = "Todo" // Todo
                });
            }

            if (ilrError.Any())
            {
                _logger.LogWarning($"Failed to get one or more ILR learners while generating {nameof(MathsAndEnglishReport)}: {_stringUtilitiesService.JoinWithMaxLength(ilrError)}");
            }

            using (MemoryStream ms = new MemoryStream())
            {
                BuildCsvReport<MathsAndEnglishMapper, MathsAndEnglishModel>(ms, mathsAndEnglishModels);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
    }
}
