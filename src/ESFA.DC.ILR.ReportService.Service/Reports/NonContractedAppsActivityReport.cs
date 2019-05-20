using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Interface.Builders;
using ESFA.DC.ILR.ReportService.Interface.Context;
using ESFA.DC.ILR.ReportService.Interface.Provider;
using ESFA.DC.ILR.ReportService.Interface.Reports;
using ESFA.DC.ILR.ReportService.Interface.Service;
using ESFA.DC.ILR.ReportService.Model.FCS;
using ESFA.DC.ILR.ReportService.Model.ReportModels;
using ESFA.DC.ILR.ReportService.Service.Mapper;
using ESFA.DC.ILR.ReportService.Service.Reports.Abstract;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.ILR.ReportService.Service.Reports
{
    public class NonContractedAppsActivityReport : AbstractReport, IReport
    {
        private readonly IIlrProviderService _ilrProviderService;
        private readonly IFCSProviderService _fcsProviderService;
        private readonly IValidLearnersService _validLearnersService;
        private readonly IFM36ProviderService _fm36ProviderService;
        private readonly INonContractedAppsActivityModelBuilder _modelBuilder;

        public NonContractedAppsActivityReport(
            ILogger logger,
            IStreamableKeyValuePersistenceService streamableKeyValuePersistenceService,
            IIlrProviderService ilrProviderService,
            IFCSProviderService fcsProviderService,
            IValidLearnersService validLearnersService,
            IFM36ProviderService fm36ProviderService,
            IDateTimeProvider dateTimeProvider,
            IValueProvider valueProvider,
            INonContractedAppsActivityModelBuilder modelBuilder)
        : base(dateTimeProvider, valueProvider, streamableKeyValuePersistenceService, logger)
        {
            _ilrProviderService = ilrProviderService;
            _fcsProviderService = fcsProviderService;
            _validLearnersService = validLearnersService;
            _fm36ProviderService = fm36ProviderService;
            _modelBuilder = modelBuilder;
        }

        public override string ReportFileName => "Non-Contracted Apprenticeships Activity Report";

        public override string ReportTaskName => ReportTaskNameConstants.NonContractedAppsActivityReport;

        public override async Task GenerateReport(IReportServiceContext reportServiceContext, ZipArchive archive, bool isFis, CancellationToken cancellationToken)
        {
            var externalFileName = GetFilename(reportServiceContext);
            var fileName = GetZipFilename(reportServiceContext);

            Task<IMessage> ilrFileTask = _ilrProviderService.GetIlrFile(reportServiceContext, cancellationToken);
            Task<List<string>> validLearnersTask = _validLearnersService.GetLearnersAsync(reportServiceContext, cancellationToken);
            Task<FM36Global> fm36Task = _fm36ProviderService.GetFM36Data(reportServiceContext, cancellationToken);
            Task<List<ContractAllocationInfo>> fcsProviderTask = _fcsProviderService.GetContractAllocationsForProviderAsync(reportServiceContext.Ukprn, cancellationToken);
            await Task.WhenAll(ilrFileTask, validLearnersTask, fm36Task, fcsProviderTask);

            var nonContractedAppsActivityModels = _modelBuilder.BuildModel(fcsProviderTask.Result);
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
