using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Aspose.Cells;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.AdultFundingClaim
{
    public class AdultFundingClaimReport : AbstractReport, IReport
    {
        private readonly IFileNameService _fileNameService;
        private readonly IModelBuilder<AdultFundingClaimReportModel> _modelBuilder;
        private readonly IExcelService _excelService;
        private const int FisInfoRow = 8;
        public AdultFundingClaimReport(
            IFileNameService fileNameService,
            IModelBuilder<AdultFundingClaimReportModel> modelBuilder,
            IExcelService excelService
        )
            : base(ReportTaskNameConstants.AdultFundingClaimReport, "Adult Funding Claim Report")
        {
            _fileNameService = fileNameService;
            _modelBuilder = modelBuilder;
            _excelService = excelService;
        }

        public async Task<IEnumerable<string>> GenerateAsync(IReportServiceContext reportServiceContext, IReportServiceDependentData reportsDependentData,
            CancellationToken cancellationToken)
        {
            var fileName = _fileNameService.GetFilename(reportServiceContext, ReportName, OutputTypes.Excel);
            var model = _modelBuilder.Build(reportServiceContext, reportsDependentData);
            var workbook = _excelService.BindExcelTemplateToWorkbook(model, "AdultFundingClaimReportTemplate.xlsx", "AdultFundingClaim");
            RenderIndicativeMessage(workbook);
            await _excelService.SaveWorkbookAsync(workbook, fileName, reportServiceContext.Container, cancellationToken);
            return new[] { fileName };
        }

        public virtual IEnumerable<Type> DependsOn
            => new[]
            {
                DependentDataCatalog.Fm35,
                DependentDataCatalog.Fm99,
                DependentDataCatalog.ReferenceData
            };

        public virtual void RenderIndicativeMessage(Workbook workbook)
        {
            workbook.Worksheets[0].Cells.DeleteRow(FisInfoRow);
        }
    }
}
