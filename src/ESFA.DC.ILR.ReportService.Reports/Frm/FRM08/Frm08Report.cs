using System;
using System.Collections.Generic;
using ESFA.DC.ExcelService.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Frm.FRM08
{
    public class Frm08Report : AbstractFundingRulesMonitoringReport<Frm08ReportModel>, IFrmWorksheetReport
    {
        public Frm08Report(
            IExcelFileService excelService,
            IModelBuilder<IEnumerable<Frm08ReportModel>> fundingRulesMonitoringModelBuilder,
            IRenderService<IEnumerable<Frm08ReportModel>> fundingRulesMonitoringRenderService) 
            : base(excelService,
                fundingRulesMonitoringModelBuilder,
                fundingRulesMonitoringRenderService,
                "TaskGenerateFundingRulesMonitoring08Report",
                "FRM08",
                "Breaks In Learning: Duration")
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
