using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.FundingClaim
{
    public class FundingClaimReport : AbstractReport, IReport, IFilteredReport
    {
        private readonly IFileNameService _fileNameService;
        private readonly IModelBuilder<FundingClaimReportModel> _modelBuilder;
        private readonly IExcelService _excelService;

        public const string ReportNameConstant = @"16-19 Funding Claim Report";
        public const string ReferenceDateFilterPropertyName = @"Reference Date";

        public FundingClaimReport(
            IFileNameService fileNameService,
            IModelBuilder<FundingClaimReportModel> modelBuilder,
            IExcelService excelService)
            : base(ReportTaskNameConstants.FundingClaim1619Report, ReportNameConstant)
        {
            _fileNameService = fileNameService;
            _modelBuilder = modelBuilder;
            _excelService = excelService;
        }

        public virtual IEnumerable<Type> DependsOn
            => new[]
            {
                DependentDataCatalog.Fm25,
                DependentDataCatalog.ValidIlr,
                DependentDataCatalog.ReferenceData
            };

        public IReportFilterDefinition Filter => new ReportFilterDefinition()
        {
            ReportName = ReportName,
            Properties = new IReportFilterPropertyDefinition[]
            {
                new ReportFilterPropertyDefinition<DateTime?>(ReferenceDateFilterPropertyName),
            }
        };

        public async Task<IEnumerable<string>> GenerateAsync(IReportServiceContext reportServiceContext, IReportServiceDependentData reportsDependentData, CancellationToken cancellationToken)
        {
            var fileName = _fileNameService.GetFilename(reportServiceContext, ReportName, OutputTypes.Excel);
            var model = _modelBuilder.Build(reportServiceContext, reportsDependentData);

            var workbook = _excelService.BindExcelTemplateToWorkbook(model, "FundingClaim1619ReportTemplate.xlsx", "FundingClaim");

            await _excelService.SaveWorkbookAsync(workbook, fileName, reportServiceContext.Container, cancellationToken);

            return new[] { fileName };
        }
    }
}
