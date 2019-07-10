using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.ReportService.Interface.Builders;
using ESFA.DC.ILR.ReportService.Interface.Provider;
using ESFA.DC.ILR.ReportService.Interface.Service;
using ESFA.DC.ILR.ReportService.Model.Lars;
using ESFA.DC.ILR.ReportService.Model.ReportModels;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Mapper;
using ESFA.DC.ILR.ReportService.Service.Reports.Abstract;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using IReport = ESFA.DC.ILR.ReportService.Interface.Reports.IReport;
using ReportTaskNameConstants = ESFA.DC.ILR.ReportService.Interface.ReportTaskNameConstants;

namespace ESFA.DC.ILR.ReportService.Service.Reports
{
    public class NonContractedAppsActivityReport : AbstractReport, IReport
    {
        private readonly IIlrProviderService _ilrProviderService;
        private readonly IFCSProviderService _fcsProviderService;
        private readonly IValidLearnersService _validLearnersService;
        private readonly IFM36NonContractedActivityProviderService _fm36ProviderService;
        private readonly ILarsProviderService _larsProviderService;
        private readonly INonContractedAppsActivityModelBuilder _modelBuilder;

        public NonContractedAppsActivityReport(
            ILogger logger,
            IStreamableKeyValuePersistenceService streamableKeyValuePersistenceService,
            IIlrProviderService ilrProviderService,
            IFCSProviderService fcsProviderService,
            IValidLearnersService validLearnersService,
            IFM36NonContractedActivityProviderService fm36ProviderService,
            ILarsProviderService larsProviderService,
            IDateTimeProvider dateTimeProvider,
            IValueProvider valueProvider,
            INonContractedAppsActivityModelBuilder modelBuilder)
        : base(dateTimeProvider, valueProvider, streamableKeyValuePersistenceService, logger)
        {
            _ilrProviderService = ilrProviderService;
            _fcsProviderService = fcsProviderService;
            _validLearnersService = validLearnersService;
            _fm36ProviderService = fm36ProviderService;
            _larsProviderService = larsProviderService;
            _modelBuilder = modelBuilder;
        }

        public override string ReportFileName => "Non-Contracted Apprenticeships Activity Report";

        public override string ReportTaskName => ReportTaskNameConstants.NonContractedAppsActivityReport;

        public override async Task GenerateReport(IReportServiceContext reportServiceContext, ZipArchive archive, bool isFis, CancellationToken cancellationToken)
        {
            var externalFileName = GetFilename(reportServiceContext);
            var fileName = GetZipFilename(reportServiceContext);

            var validLearnersList = await _validLearnersService.GetLearnersAsync(reportServiceContext, cancellationToken);
            var nonContractedAppsActivityIlrInfo = await _ilrProviderService.GetILRInfoForNonContractedAppsActivityReportAsync(validLearnersList, reportServiceContext, cancellationToken);
            var nonContractedActivityRuleBaseInfo = await _fm36ProviderService.GetFM36InfoForNonContractedActivityReportAsync(validLearnersList, reportServiceContext, cancellationToken);
            var contractAllocationInfos = await _fcsProviderService.GetContractAllocationsForProviderAsync(reportServiceContext.Ukprn, cancellationToken);

            string[] learnAimRefs = nonContractedAppsActivityIlrInfo.Learners?.SelectMany(x => x.LearningDeliveries).Select(x => x.LearnAimRef).Distinct().ToArray();
            Dictionary<string, LarsLearningDelivery> larsLearningDeliveries = await _larsProviderService.GetLearningDeliveriesAsync(learnAimRefs, cancellationToken);

            var nonContractedAppsActivityModels = _modelBuilder.BuildModel(nonContractedAppsActivityIlrInfo, nonContractedActivityRuleBaseInfo, contractAllocationInfos, larsLearningDeliveries);
            string csv = await GetCsv(nonContractedAppsActivityModels, cancellationToken);
            await _streamableKeyValuePersistenceService.SaveAsync($"{externalFileName}.csv", csv, cancellationToken);
            await WriteZipEntry(archive, $"{fileName}.csv", csv);
        }

        private async Task<string> GetCsv(List<NonContractedAppsActivityModel> nonContractedAppsActivityModels, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (MemoryStream ms = new MemoryStream())
            {
                UTF8Encoding utF8Encoding = new UTF8Encoding(false, true);
                using (TextWriter textWriter = new StreamWriter(ms, utF8Encoding))
                {
                    using (CsvWriter csvWriter = new CsvWriter(textWriter))
                    {
                        WriteCsvRecords<NonContractedAppsActivityMapper, NonContractedAppsActivityModel>(csvWriter, nonContractedAppsActivityModels);

                        csvWriter.Flush();
                        textWriter.Flush();
                        return Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
            }
        }
    }
}
