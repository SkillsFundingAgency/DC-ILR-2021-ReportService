using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Aspose.Cells;
using ESFA.DC.ExcelService.Interface;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Frm
{
    public abstract class AbstractFundingRulesMonitoringReport<TModel> : AbstractReport
    {
        private readonly IExcelFileService _excelService;
        private readonly IModelBuilder<IEnumerable<TModel>> _fundingRuleMonitoringReportModelBuilder;
        private readonly IRenderService<IEnumerable<TModel>> _fundingRuleMonitoringRenderService;

        protected AbstractFundingRulesMonitoringReport(
            IExcelFileService excelService,
            IModelBuilder<IEnumerable<TModel>> fundingRulesMonitoringModelBuilder,
            IRenderService<IEnumerable<TModel>> fundingRulesMonitoringRenderService,
            string taskName,
            string fileName) 
            : base(taskName, fileName)
        {
            _excelService = excelService;
            _fundingRuleMonitoringRenderService = fundingRulesMonitoringRenderService;
            _fundingRuleMonitoringReportModelBuilder = fundingRulesMonitoringModelBuilder;
        }

        public virtual void Generate(Workbook workbook, IReportServiceContext reportServiceContext, IReportServiceDependentData reportsDependentData, CancellationToken cancellationToken)
        {
            var fundingReportMonitoringModels = _fundingRuleMonitoringReportModelBuilder.Build(reportServiceContext, reportsDependentData).ToList();

            if (fundingReportMonitoringModels.Any())
            {
                var worksheet = _excelService.GetWorksheetFromWorkbook(workbook, ReportName);

                _fundingRuleMonitoringRenderService.Render(fundingReportMonitoringModels, worksheet);
            }
        }
    }
}
