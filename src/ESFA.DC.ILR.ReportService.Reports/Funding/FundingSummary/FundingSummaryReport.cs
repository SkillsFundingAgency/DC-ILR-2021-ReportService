using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model.Interface;
using ESFA.DC.ILR.ReportService.Reports.Model.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary
{
    public class FundingSummaryReport : AbstractReport, IReport
    {
        private readonly IFileNameService _fileNameService;
        private readonly IModelBuilder<IFundingSummaryReport> _fundingSummaryReportModelBuilder;
        private readonly IExcelService _excelService;
        private readonly IRenderService<IFundingSummaryReport> _fundingSummaryReportRenderService;

        public FundingSummaryReport(
            IFileNameService fileNameService,
            IModelBuilder<IFundingSummaryReport> fundingSummaryReportModelBuilder,
            IExcelService excelService,
            IRenderService<IFundingSummaryReport> fundingSummaryReportRenderService)
            : base(ReportTaskNameConstants.FundingSummaryReport, "Funding Summary Report")
        {
            _fileNameService = fileNameService;
            _fundingSummaryReportModelBuilder = fundingSummaryReportModelBuilder;
            _excelService = excelService;
            _fundingSummaryReportRenderService = fundingSummaryReportRenderService;
        }

        public virtual IEnumerable<Type> DependsOn
            => new[] 
            {
                DependentDataCatalog.Fm25,
                DependentDataCatalog.Fm35,
                DependentDataCatalog.Fm36,
                DependentDataCatalog.Fm81,
                DependentDataCatalog.Fm99,
                DependentDataCatalog.ReferenceData,
            };

        public async Task<IEnumerable<string>> GenerateAsync(
            IReportServiceContext reportServiceContext,
            IReportServiceDependentData reportsDependentData,
            CancellationToken cancellationToken)
        {
            var fundingSummaryReportModel = _fundingSummaryReportModelBuilder.Build(reportServiceContext, reportsDependentData);

            var fileName = _fileNameService.GetFilename(reportServiceContext, ReportName, OutputTypes.Excel);

            using (var workbook = _excelService.NewWorkbook())
            {
                workbook.Worksheets.Clear();

                var worksheet = _excelService.GetWorksheetFromWorkbook(workbook, "FundingSummaryReport");

                _fundingSummaryReportRenderService.Render(fundingSummaryReportModel, worksheet);

                await _excelService.SaveWorkbookAsync(workbook, fileName, reportServiceContext.Container, cancellationToken);
            }

            return new[] { fileName };
        }
    }
}
