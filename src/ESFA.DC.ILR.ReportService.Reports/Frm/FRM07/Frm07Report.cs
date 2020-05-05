using System;
using System.Collections.Generic;
using ESFA.DC.ExcelService.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Frm.FRM07
{
    public class Frm07Report : AbstractFundingRulesMonitoringReport<Frm07ReportModel>, IWorksheetReport
    {
        public Frm07Report(
            IExcelFileService excelService,
            IModelBuilder<IEnumerable<Frm07ReportModel>> fundingRulesMonitoringModelBuilder,
            IRenderService<IEnumerable<Frm07ReportModel>> fundingRulesMonitoringRenderService)
            : base(excelService,
                fundingRulesMonitoringModelBuilder,
                fundingRulesMonitoringRenderService,
                "TaskGenerateFundingRulesMonitoring07Report",
                "FRM07")
        {
        }

        public virtual IEnumerable<Type> DependsOn
            => new[]
            {
                DependentDataCatalog.ValidIlr,
                DependentDataCatalog.ReferenceData
            };
    }
}
