using System;
using System.Collections.Generic;
using ESFA.DC.ExcelService.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Frm.FRM15
{
    public class Frm15Report : AbstractFundingRulesMonitoringReport<Frm15ReportModel>, IFrmWorksheetReport
    {
        public Frm15Report(
            IExcelFileService excelService,
            IModelBuilder<IEnumerable<Frm15ReportModel>> fundingMonitoring06ModelBuilder,
            IRenderService<IEnumerable<Frm15ReportModel>> fundingReportMonitoringRenderService)
            : base(excelService,
                fundingMonitoring06ModelBuilder,
                fundingReportMonitoringRenderService,
                "TaskGenerateFundingRulesMonitoring15Report",
                "FRM15",
                "End Point Assessment Organisations")
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
