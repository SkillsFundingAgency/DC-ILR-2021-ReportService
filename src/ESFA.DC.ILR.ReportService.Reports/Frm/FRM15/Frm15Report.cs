using System;
using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;

namespace ESFA.DC.ILR.ReportService.Reports.Frm.FRM15
{
    public class Frm15Report : AbstractFundingRulesMonitoringReport<Frm15ReportModel>, IWorksheetReport
    {
        public Frm15Report(
            IExcelService excelService,
            IModelBuilder<IEnumerable<Frm15ReportModel>> fundingMonitoring06ModelBuilder,
            IRenderService<IEnumerable<Frm15ReportModel>> fundingReportMonitoringRenderService)
            : base(excelService,
                fundingMonitoring06ModelBuilder,
                fundingReportMonitoringRenderService,
                "TaskGenerateFundingRulesMonitoring15Report",
                "FRM15")
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
