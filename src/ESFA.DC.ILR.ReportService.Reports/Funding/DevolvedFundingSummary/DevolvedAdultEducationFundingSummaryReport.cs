using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Funding.DevolvedFundingSummary.Model.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.DevolvedFundingSummary
{
    public class DevolvedAdultEducationFundingSummaryReport : AbstractReport, IReport
    {
        private readonly IFileNameService _fileNameService;
        private readonly IModelBuilder<IDevolvedAdultEducationFundingSummaryReport> _devolvedFundingSummaryReportBuilder;
        private readonly IExcelService _excelService;
        private readonly IRenderService<IDevolvedAdultEducationFundingSummaryReport> _devolvedFundingSummaryReportRenderService;

        public DevolvedAdultEducationFundingSummaryReport(
            IFileNameService fileNameService,
            IModelBuilder<IDevolvedAdultEducationFundingSummaryReport> devolvedFundingSummaryReportBuilder,
            IExcelService excelService,
            IRenderService<IDevolvedAdultEducationFundingSummaryReport> devolvedFundingSummaryReportRenderService) 
            : base(ReportTaskNameConstants.DevolvedAdultEducationFundingSummaryReport, "Devolved Adult Education Funding Summary Report")
        {
            _fileNameService = fileNameService;
            _devolvedFundingSummaryReportBuilder = devolvedFundingSummaryReportBuilder;
            _excelService = excelService;
            _devolvedFundingSummaryReportRenderService = devolvedFundingSummaryReportRenderService;
        }

        public IEnumerable<Type> DependsOn
            => new[]
            {
                DependentDataCatalog.Fm35
            };

        public async Task<IEnumerable<string>> GenerateAsync(
            IReportServiceContext reportServiceContext,
            IReportServiceDependentData reportsDependentData,
            CancellationToken cancellationToken)
        {
            var fundingSummaryReportModel = _devolvedFundingSummaryReportBuilder.Build(reportServiceContext, reportsDependentData);

            var fileName = _fileNameService.GetFilename(reportServiceContext, FileName, OutputTypes.Excel);

            using (var workbook = _excelService.NewWorkbook())
            {
                var workbookIndex = 0;

                foreach (var fundingArea in fundingSummaryReportModel.DevolvedFundingAreas)
                {
                    var worksheet = _excelService.GetWorksheetFromWorkbook(workbook, workbookIndex++);

                    _devolvedFundingSummaryReportRenderService.Render(fundingSummaryReportModel, worksheet);
                }

                await _excelService.SaveWorkbookAsync(workbook, fileName, reportServiceContext.Container, cancellationToken);
            }

            return new[] { fileName };
        }
    }
}
