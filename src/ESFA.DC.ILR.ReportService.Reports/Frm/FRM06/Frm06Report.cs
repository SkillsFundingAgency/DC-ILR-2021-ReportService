using System;
using System.Collections.Generic;
using ESFA.DC.ExcelService.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Frm.FRM06
{
    public class Frm06Report : AbstractFundingRulesMonitoringReport<Frm06ReportModel>, IWorksheetReport
    {
        public Frm06Report(
            IExcelFileService excelService,
            IModelBuilder<IEnumerable<Frm06ReportModel>> fundingMonitoring06ModelBuilder,
            IRenderService<IEnumerable<Frm06ReportModel>> fundingReportMonitoringRenderService) 
            : base(excelService,
                fundingMonitoring06ModelBuilder,
                fundingReportMonitoringRenderService, 
                "TaskGenerateFundingRulesMonitoring06Report", 
                "FRM06")
        {
        }

        public virtual IEnumerable<Type> DependsOn
            => new[]
            {
                DependentDataCatalog.ValidIlr,
                DependentDataCatalog.Frm,
                DependentDataCatalog.ReferenceData
            };
    }
}
