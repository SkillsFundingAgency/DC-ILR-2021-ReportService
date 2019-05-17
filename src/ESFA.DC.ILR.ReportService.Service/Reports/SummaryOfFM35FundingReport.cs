using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Aspose.Cells;
using Aspose.Cells.Drawing;
using Aspose.Cells.Rendering;
using CsvHelper;
using CsvHelper.Configuration;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Interface.Builders;
using ESFA.DC.ILR.ReportService.Interface.Configuration;
using ESFA.DC.ILR.ReportService.Interface.Context;
using ESFA.DC.ILR.ReportService.Interface.Provider;
using ESFA.DC.ILR.ReportService.Interface.Reports;
using ESFA.DC.ILR.ReportService.Interface.Service;
using ESFA.DC.ILR.ReportService.Model.Generation;
using ESFA.DC.ILR.ReportService.Model.ILR;
using ESFA.DC.ILR.ReportService.Model.ReportModels;
using ESFA.DC.ILR.ReportService.Service.Comparer;
using ESFA.DC.ILR.ReportService.Service.Extensions;
using ESFA.DC.ILR.ReportService.Service.Mapper;
using ESFA.DC.ILR.ReportService.Service.Reports.Abstract;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.ILR.ReportService.Service.Reports
{
    public class SummaryOfFm35FundingReport : AbstractReport, IReport
    {
        private static readonly SummaryOfFm35FundingModelComparer comparer = new SummaryOfFm35FundingModelComparer();

        private readonly IFM35ProviderService _fm35ProviderService;
        private readonly IIlrProviderService _ilrProviderService;
        private readonly IIlrMetadataProviderService _ilrMetadataProviderService;

        private readonly IStringUtilitiesService _stringUtilitiesService;
        private readonly ILogger _logger;
        private readonly IFm35Builder _summaryOfFm35FundingModelBuilder;
        private readonly ITotalBuilder _totalBuilder;

        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IVersionInfo _versionInfo;
        private readonly ILarsProviderService _larsProviderService;
        private readonly IPostcodeProviderService _postcodeProviderService;
        private readonly ILargeEmployerProviderService _largeEmployerProviderService;
        private readonly IOrgProviderService _orgProviderService;

        public SummaryOfFm35FundingReport(
            ILogger logger,
            IStreamableKeyValuePersistenceService streamableKeyValuePersistenceService,
            IIlrProviderService ilrProviderService,
            IIlrMetadataProviderService ilrMetadataProviderService,
            IFM35ProviderService fm35ProviderService,
            IDateTimeProvider dateTimeProvider,
            IValueProvider valueProvider,
            IVersionInfo versionInfo,
            ILarsProviderService larsProviderService,
            IPostcodeProviderService postcodeProviderService,
            ILargeEmployerProviderService largeEmployerProviderService,
            IOrgProviderService orgProviderService,
            ITotalBuilder totalBuilder,
            IFm35Builder builder)
            : base(dateTimeProvider, valueProvider, streamableKeyValuePersistenceService, logger)
        {
            _fm35ProviderService = fm35ProviderService;
            _ilrProviderService = ilrProviderService;
            _ilrMetadataProviderService = ilrMetadataProviderService;
            _summaryOfFm35FundingModelBuilder = builder;
            _versionInfo = versionInfo;
            _larsProviderService = larsProviderService;
            _postcodeProviderService = postcodeProviderService;
            _largeEmployerProviderService = largeEmployerProviderService;
            _orgProviderService = orgProviderService;
            _totalBuilder = totalBuilder;
            _logger = logger;
            _dateTimeProvider = dateTimeProvider;
        }

        public override string ReportFileName => "Summary of Funding Model 35 Funding Report";

        public override string ReportTaskName => ReportTaskNameConstants.SummaryOfFM35FundingReport;

        public override async Task GenerateReport(IReportServiceContext reportServiceContext, ZipArchive archive, bool isFis, CancellationToken cancellationToken)
        {
            var externalFileName = GetFilename(reportServiceContext);
            var fileName = GetZipFilename(reportServiceContext);

            Task<IMessage> ilrFileTask = _ilrProviderService.GetIlrFile(reportServiceContext, cancellationToken);
            Task<string> providerNameTask = _orgProviderService.GetProviderName(reportServiceContext, cancellationToken);
            Task<ILRSourceFileInfo> lastSubmittedIlrFileTask = _ilrMetadataProviderService.GetLastSubmittedIlrFile(reportServiceContext, cancellationToken);

            SummaryOfFM35FundingHeaderModel summaryOfFm35FundingHeaderModel = await GetHeader(reportServiceContext, ilrFileTask, lastSubmittedIlrFileTask, providerNameTask, cancellationToken, isFis);
            SummaryOfFM35FundingFooterModel summaryOfFm35FundingFooterModel = await GetFooterAsync(ilrFileTask, lastSubmittedIlrFileTask, cancellationToken);

            IList<SummaryOfFm35FundingModel> summaryOfFm35FundingModels = await GetSummaryOfFm35FundingModels(reportServiceContext, cancellationToken);
            if (summaryOfFm35FundingModels == null)
            {
                return;
            }

            _logger.LogInfo("CSV Report Start");

            string csv = GetCsv(summaryOfFm35FundingModels);
            await _streamableKeyValuePersistenceService.SaveAsync($"{externalFileName}.csv", csv, cancellationToken);
            await WriteZipEntry(archive, $"{fileName}.csv", csv);

            _logger.LogInfo("CSV Report End");

            _logger.LogInfo("Excel Report Start");

            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith("SummaryOfFM35FundingTemplate.xlsx"));
            var manifestResourceStream = assembly.GetManifestResourceStream(resourceName);
            Workbook workbook = new Workbook(manifestResourceStream);
            Worksheet worksheet = workbook.Worksheets[0];
            Cells cells = worksheet.Cells;
            InsertHeaderFooter(workbook, summaryOfFm35FundingHeaderModel, summaryOfFm35FundingFooterModel);
            //PopulateMainData(workbook, summaryOfFm35FundingModels);
            //PopulateReportData(workbook, summaryOfFm35FundingModels);
            PopulateUsingSmartMarkers(workbook, summaryOfFm35FundingModels);
            workbook.CalculateFormula();
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Save(ms, SaveFormat.Xlsx);
                await _streamableKeyValuePersistenceService.SaveAsync($"{externalFileName}.xlsx", ms, cancellationToken);
                await WriteZipEntry(archive, $"{fileName}.xlsx", ms, cancellationToken);
            }

            _logger.LogInfo("Excel Report End");
        }

        private string GetCsv(IList<SummaryOfFm35FundingModel> summaryOfFm35FundingModels)
        {
            using (var ms = new MemoryStream())
            {
                UTF8Encoding utF8Encoding = new UTF8Encoding(false, true);
                using (TextWriter textWriter = new StreamWriter(ms, utF8Encoding))
                {
                    using (CsvWriter csvWriter = new CsvWriter(textWriter))
                    {
                        WriteCsvRecords<SummaryOfFM35FundingMapper, SummaryOfFm35FundingModel>(csvWriter, summaryOfFm35FundingModels);
                        csvWriter.Flush();
                        textWriter.Flush();
                        return Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
            }
        }

        private async Task<IList<SummaryOfFm35FundingModel>> GetSummaryOfFm35FundingModels(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            Task<FM35Global> fm35Task = _fm35ProviderService.GetFM35Data(reportServiceContext, cancellationToken);

            await Task.WhenAll(fm35Task);

            cancellationToken.ThrowIfCancellationRequested();

            List<SummaryOfFm35FundingModel> summaryOfFm35FundingModels = new List<SummaryOfFm35FundingModel>();

            FM35Global fm35Data = fm35Task.Result;
            if (fm35Data?.Learners == null)
            {
                _logger.LogWarning($"No Fm35 data for {nameof(SummaryOfFm35FundingReport)}");
                return summaryOfFm35FundingModels;
            }

            SummaryOfFm35FundingModel grandTotalBuilder = new SummaryOfFm35FundingModel();
            foreach (FM35Learner learnerAttribute in fm35Data.Learners)
            {
                foreach (LearningDelivery fundLineData in learnerAttribute.LearningDeliveries)
                {
                    var summaryOfFm35FundingModelsList = _summaryOfFm35FundingModelBuilder.BuildModel(fundLineData).ToArray();
                    //SummaryOfFm35FundingModel totalBuilder = _totalBuilder.TotalRecords(summaryOfFm35FundingModelsList);
                    summaryOfFm35FundingModels.AddRange(summaryOfFm35FundingModelsList);
                    //summaryOfFm35FundingModels.Add(totalBuilder);
                }
            }

            summaryOfFm35FundingModels.Sort(comparer);
            return summaryOfFm35FundingModels;
        }

        private async Task<SummaryOfFM35FundingHeaderModel> GetHeader(IReportServiceContext reportServiceContext, Task<IMessage> messageTask, Task<ILRSourceFileInfo> lastSubmittedIlrFileTask, Task<string> providerNameTask, CancellationToken cancellationToken, bool isFis)
        {
            var fileName = reportServiceContext.OriginalFilename ?? reportServiceContext.Filename;
            var summaryOfFm35FundingHeaderModel = new SummaryOfFM35FundingHeaderModel
            {
                IlrFile = string.Equals(reportServiceContext.CollectionName, "ILR1819", StringComparison.OrdinalIgnoreCase) ? fileName : "N/A",
                Ukprn = reportServiceContext.Ukprn,
                ProviderName = providerNameTask.Result ?? "Unknown"
            };

            return summaryOfFm35FundingHeaderModel;
        }

        private async Task<SummaryOfFM35FundingFooterModel> GetFooterAsync(Task<IMessage> messageTask, Task<ILRSourceFileInfo> lastSubmittedIlrFileTask, CancellationToken cancellationToken)
        {
            var dateTimeNowUtc = _dateTimeProvider.GetNowUtc();
            var dateTimeNowUk = _dateTimeProvider.ConvertUtcToUk(dateTimeNowUtc);
            var summaryOfFm35FundingFooterModel = new SummaryOfFM35FundingFooterModel
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
                summaryOfFm35FundingFooterModel.FilePreparationDate = messageTask.Result.HeaderEntity.SourceEntity.DateTime.ToString("dd/MM/yyyy");
            }
            else
            {
                summaryOfFm35FundingFooterModel.FilePreparationDate = lastSubmittedIlrFileTask.Result != null ?
                                                                                lastSubmittedIlrFileTask.Result.FilePreparationDate.GetValueOrDefault().ToString("dd/MM/yyyy") : string.Empty;
            }

            return summaryOfFm35FundingFooterModel;
        }

        private void InsertHeaderFooter(Workbook workbook, SummaryOfFM35FundingHeaderModel headerModel, SummaryOfFM35FundingFooterModel footerModel)
        {
            PageSetup pageSetup = workbook.Worksheets[0].PageSetup;

            pageSetup.SetHeader(0, "&16&\"Bold\"Summary of Funding Model 35 Funding\n\n&8&\"Bold\"Provider:" + headerModel.ProviderName + "\nUKPRN: " + headerModel.Ukprn + "\nILR File:" + headerModel.IlrFile);
            pageSetup.SetHeader(2, "&10&\"Bold\"OFFICIAL-SENSITIVE");
            pageSetup.SetFooter(0, $"&8Component Set Version: \t{footerModel.ComponentSetVersion}\nApplication Version: \t{footerModel.ApplicationVersion}\nFile Prepartion Date: \t{footerModel.FilePreparationDate}\n{footerModel.ReportGeneratedAt}");
            pageSetup.SetFooter(1, $"&8Lars Data: \t{footerModel.LarsData}\nPostcode Data: \t{footerModel.PostcodeData}");
            pageSetup.SetFooter(2, $"&8Large Employer Data: \t{footerModel.LargeEmployerData}\nOrganisation Data: \t{footerModel.OrganisationData}");
        }

        private void PopulateMainData(Workbook workbook, IEnumerable<SummaryOfFm35FundingModel> summaryOfFm35FundingModels)
        {
            var sheet = workbook.Worksheets[0];
            var summaryOfFm35FundingMapper = new SummaryOfFM35FundingMapper();
            var modelProperties = summaryOfFm35FundingMapper.MemberMaps.OrderBy(x => x.Data.Index).Select(x => new ModelProperty(x.Data.Names.Names.ToArray(), (PropertyInfo)x.Data.Member)).ToArray();
            WriteExcelRecords(sheet);
            WriteExcelRecords(sheet);
            WriteExcelRecords(sheet);
            WriteExcelRecords(sheet);
            WriteExcelRecords(sheet);

            WriteExcelRecords(sheet, summaryOfFm35FundingMapper, new List<SummaryOfFm35FundingModel> { }, null, null);
            foreach (var model in summaryOfFm35FundingModels)
            {
                if (model.Period != 1)
                {
                    model.FundingLineType = string.Empty;
                }

                if (model.Period == 0)
                {
                    model.FundingLineType = "Totals";
                    model.Period = null;
                }

                WriteExcelRecords(sheet, summaryOfFm35FundingMapper, modelProperties, model, null);
                if (model.FundingLineType == "totals")
                {
                    WriteExcelRecords(sheet);
                }
            }
        }

        private void PopulateReportData(Workbook workbook, IEnumerable<SummaryOfFm35FundingModel> summaryOfFm35FundingModels)
        {
            var sheet = workbook.Worksheets[0];
            Cells cells = workbook.Worksheets[0].Cells;
            var summaryOfFm35FundingModelsArray = summaryOfFm35FundingModels as SummaryOfFm35FundingModel[] ?? summaryOfFm35FundingModels.ToArray();
            IGrouping<string, SummaryOfFm35FundingModel>[] groupedSummaryOfFm35FundingModels = summaryOfFm35FundingModelsArray.ToArray().GroupBy(x => x.FundingLineType).ToArray();
            DataSet ds = new DataSet();

            foreach (var groupedModel in groupedSummaryOfFm35FundingModels)
            {
                System.Data.DataTable dt = new DataTable(); //("Table1");)
                dt.Columns.Add("Funding Line Type", typeof(string));
                dt.Columns.Add("Period", typeof(decimal));
                dt.Columns.Add("On Programme", typeof(decimal));
                dt.Columns.Add("Balancing", typeof(decimal));
                dt.Columns.Add("Job Outcome Achievement", typeof(decimal));
                dt.Columns.Add("Aim Achievement", typeof(decimal));
                dt.Columns.Add("Total Achievement", typeof(decimal));
                dt.Columns.Add("Learning Support", typeof(decimal));
                dt.Columns.Add("Total", typeof(decimal));

                foreach (var model in groupedModel.OrderBy(x => x.Period))
                {
                    DataRow dr = dt.NewRow();
                    dr["Funding Line Type"] = model.Period == 99 ? "Totals" : model.Period == 1 ? groupedModel.Key : string.Empty;
                    dr["Period"] = model.Period == 99 ? DBNull.Value : (object)model.Period;
                    dr["On Programme"] = decimal.Round(model.OnProgramme.GetValueOrDefault(), 2);
                    dr["Balancing"] = decimal.Round(model.Balancing.GetValueOrDefault(), 2);
                    dr["Job Outcome Achievement"] = decimal.Round(model.JobOutcomeAchievement.GetValueOrDefault(), 2);
                    dr["Aim Achievement"] = decimal.Round(model.AimAchievement.GetValueOrDefault(), 2);
                    dr["Total Achievement"] = decimal.Round(model.TotalAchievement.GetValueOrDefault(), 2);
                    dr["Learning Support"] = decimal.Round(model.LearningSupport.GetValueOrDefault(), 2);
                    dr["Total"] = decimal.Round(model.Total.GetValueOrDefault(), 2);
                    dt.Rows.Add(dr);
                }

                dt.Rows.Add();
                ds.Tables.Add(dt);
            }

            Style stl = workbook.CreateStyle();
            stl.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            stl.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
            stl.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            stl.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            stl.Font.Size = 8;
            StyleFlag flg = new StyleFlag();
            flg.Borders = true;

            var startRow = 5;

            ImageOrPrintOptions printoption = new ImageOrPrintOptions();
            printoption.PrintingPage = PrintingPageType.Default;
            var printingPageBreaks = sheet.GetPrintingPageBreaks(printoption);
            cells.StandardWidthInch = 1;
            cells.SetColumnWidthInch(0, 2.20);
            cells.SetColumnWidthInch(1, 0.6);

            foreach (DataTable dt in ds.Tables)
            {
                cells.ImportData(dt, startRow, 0, new ImportTableOptions() { });
                Range range = cells.CreateRange(startRow, 0, 14, 9);
                startRow += printingPageBreaks[0].EndRow;
                range.ApplyStyle(stl, flg);
            }
        }

        private void PopulateUsingSmartMarkers(Workbook workbook, IEnumerable<SummaryOfFm35FundingModel> summaryOfFm35FundingModels)
        {
            //var sheet = workbook.Worksheets[0];
            Cells cells = workbook.Worksheets[0].Cells;
            //var summaryOfFm35FundingModelsArray = summaryOfFm35FundingModels as SummaryOfFm35FundingModel[] ?? summaryOfFm35FundingModels.ToArray();
            //IGrouping<string, SummaryOfFm35FundingModel>[] groupedSummaryOfFm35FundingModels = summaryOfFm35FundingModelsArray.ToArray().GroupBy(x => x.FundingLineType).ToArray();

            //var startRow = 5;
            //ImageOrPrintOptions printoption = new ImageOrPrintOptions();
            //printoption.PrintingPage = PrintingPageType.Default;
            //var printingPageBreaks = sheet.GetPrintingPageBreaks(printoption);

            //foreach (var groupedSummaryOfFm35FundingModel in groupedSummaryOfFm35FundingModels)
            //{
            //    designer.SetDataSource("FundLine", groupedSummaryOfFm35FundingModel.Key);
            //    designer.SetDataSource("GroupedModels", groupedSummaryOfFm35FundingModel.Take(12).ToImmutableList());
            //    designer.Process();
            //    designer.Workbook.CalculateFormula();
            //    var destRange = cells.CreateRange("A6:I19"); // (5, 0, 14, 9);
            //    var sourceRange = designer.Workbook.Worksheets[0].Cells.CreateRange("A1:I14");
            //    destRange.Copy(sourceRange, new PasteOptions() { PasteType = PasteType.All });
            //    startRow += printingPageBreaks[0].EndRow;
            //    designer.ClearDataSource();
            //}

            //designer.SetDataSource("GroupedModels", summaryOfFm35FundingModels);
            //designer.Process(false);
            //designer.Workbook.CalculateFormula();
            //Range sourceRange = designer.Workbook.Worksheets[0].Cells.MaxDisplayRange;
            ////var sourceCellArea = new CellArea() { StartRow = sourceRange.FirstRow, StartColumn = sourceRange.FirstColumn, EndRow = sourceRange.r};
            //var destRange = cells.CreateRange(0, 0, sourceRange.RowCount, sourceRange.ColumnCount);
            //destRange.Copy(sourceRange, new PasteOptions() { PasteType = PasteType.All });
            ////designer.ClearDataSource();

            var startRow = 5;
            var summaryOfFm35FundingModelsArray = summaryOfFm35FundingModels as SummaryOfFm35FundingModel[] ?? summaryOfFm35FundingModels.ToArray();
            IGrouping<string, SummaryOfFm35FundingModel>[] groupedSummaryOfFm35FundingModels = summaryOfFm35FundingModelsArray.ToArray().GroupBy(x => x.FundingLineType).ToArray();

            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith("SummaryOfFM35FundingBody.xlsx"));
            var manifestResourceStream = assembly.GetManifestResourceStream(resourceName);
            WorkbookDesigner designer = new WorkbookDesigner();

            foreach (var groupedSummaryOfFm35FundingModel in groupedSummaryOfFm35FundingModels)
            {
                designer.Workbook = new Workbook(manifestResourceStream);
                designer.SetDataSource("GroupedModels", groupedSummaryOfFm35FundingModel.ToList()); //.ToImmutableList());
                designer.Process();
                //designer.Workbook.CalculateFormula();
                Range sourceRange = designer.Workbook.Worksheets[0].Cells.CreateRange("A1:I14"); //.MaxDisplayRange;
                var destRange = cells.CreateRange(startRow, 0, 14, 9); //sourceRange.RowCount, sourceRange.ColumnCount);
                destRange.Copy(sourceRange, new PasteOptions() { PasteType = PasteType.All });

                workbook.Worksheets[0].HorizontalPageBreaks.Add("Y30");
                startRow += 30;
                designer.ClearDataSource();
                designer.Process();
            }
        }
    }
}
