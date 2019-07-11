﻿using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.ReportService.Interface.Builders.PeriodEnd;
using ESFA.DC.ILR.ReportService.Interface.Provider;
using ESFA.DC.ILR.ReportService.Interface.Reports;
using ESFA.DC.ILR.ReportService.Interface.Service;
using ESFA.DC.ILR.ReportService.Model.ReportModels.PeriodEnd;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Mapper.PeriodEnd;
using ESFA.DC.ILR.ReportService.Service.Provider;
using ESFA.DC.ILR.ReportService.Service.Reports.Abstract;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ReportTaskNameConstants = ESFA.DC.ILR.ReportService.Interface.ReportTaskNameConstants;

namespace ESFA.DC.ILR.ReportService.Service.Reports.PeriodEnd
{
    public class AppsMonthlyPaymentReport : AbstractReport, ILegacyReport
    {
        private readonly IIlrPeriodEndProviderService _ilrPeriodEndProviderService;
        private readonly IFM36PeriodEndProviderService _fm36ProviderService;
        private readonly IDASPaymentsProviderService _dasPaymentsProviderService;
        private readonly ILarsProviderService _larsProviderService;
        private readonly IAppsMonthlyPaymentModelBuilder _modelBuilder;

        public AppsMonthlyPaymentReport(
            ILogger logger,
            IStreamableKeyValuePersistenceService streamableKeyValuePersistenceService,
            IIlrPeriodEndProviderService ilrPeriodEndProviderService,
            IFM36PeriodEndProviderService fm36ProviderService,
            IDASPaymentsProviderService dasPaymentsProviderService,
            ILarsProviderService larsProviderService,
            IDateTimeProvider dateTimeProvider,
            IValueProvider valueProvider,
            IAppsMonthlyPaymentModelBuilder modelBuilder)
        : base(dateTimeProvider, valueProvider, streamableKeyValuePersistenceService, logger)
        {
            _ilrPeriodEndProviderService = ilrPeriodEndProviderService;
            _fm36ProviderService = fm36ProviderService;
            _dasPaymentsProviderService = dasPaymentsProviderService;
            _larsProviderService = larsProviderService;
            _modelBuilder = modelBuilder;
        }

        public override string ReportFileName => "Apps Monthly Payment Report";

        public override string ReportTaskName => ReportTaskNameConstants.AppsMonthlyPaymentReport;

        public override async Task GenerateReport(IReportServiceContext reportServiceContext, ZipArchive archive, bool isFis, CancellationToken cancellationToken)
        {
            var externalFileName = GetFilename(reportServiceContext);
            var fileName = GetZipFilename(reportServiceContext);

            var appsMonthlyPaymentIlrInfo = await _ilrPeriodEndProviderService.GetILRInfoForAppsMonthlyPaymentReportAsync(reportServiceContext.Ukprn, cancellationToken);
            var appsMonthlyPaymentRulebaseInfo = await _fm36ProviderService.GetFM36DataForAppsMonthlyPaymentReportAsync(reportServiceContext.Ukprn, cancellationToken);
            var appsMonthlyPaymentDasInfo = await _dasPaymentsProviderService.GetPaymentsInfoForAppsMonthlyPaymentReportAsync(reportServiceContext.Ukprn, cancellationToken);

            string[] learnAimRefs = appsMonthlyPaymentIlrInfo.Learners.SelectMany(x => x.LearningDeliveries).Select(x => x.LearnAimRef).Distinct().ToArray();

            var appsMonthlyPaymentLarsLearningDeliveryInfos = await _larsProviderService.GetLarsLearningDeliveryInfoForAppsMonthlyPaymentReportAsync(learnAimRefs, cancellationToken);

            var appsAdditionalPaymentsModel = _modelBuilder.BuildModel(appsMonthlyPaymentIlrInfo, appsMonthlyPaymentRulebaseInfo, appsMonthlyPaymentDasInfo, appsMonthlyPaymentLarsLearningDeliveryInfos);

            string csv = await GetCsv(appsAdditionalPaymentsModel, cancellationToken);
            await _streamableKeyValuePersistenceService.SaveAsync($"{externalFileName}.csv", csv, cancellationToken);
            await WriteZipEntry(archive, $"{fileName}.csv", csv);
        }

        private async Task<string> GetCsv(IReadOnlyList<AppsMonthlyPaymentModel> appsAdditionalPaymentsModel, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (MemoryStream ms = new MemoryStream())
            {
                UTF8Encoding utF8Encoding = new UTF8Encoding(false, true);
                using (TextWriter textWriter = new StreamWriter(ms, utF8Encoding))
                {
                    using (CsvWriter csvWriter = new CsvWriter(textWriter))
                    {
                        WriteCsvRecords<AppsMonthlyPaymentMapper, AppsMonthlyPaymentModel>(csvWriter, appsAdditionalPaymentsModel);

                        csvWriter.Flush();
                        textWriter.Flush();
                        return Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
            }
        }
    }
}
