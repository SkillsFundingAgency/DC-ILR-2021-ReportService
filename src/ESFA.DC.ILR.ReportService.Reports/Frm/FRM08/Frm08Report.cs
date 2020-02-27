using System;
using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;

namespace ESFA.DC.ILR.ReportService.Reports.Frm.FRM08
{
    public class Frm08Report : AbstractFundingRulesMonitoringReport<Frm08ReportModel>, IWorksheetReport
    {
        public Frm08Report(
            IExcelService excelService,
            IModelBuilder<IEnumerable<Frm08ReportModel>> fundingRulesMonitoringModelBuilder,
            IRenderService<IEnumerable<Frm08ReportModel>> fundingRulesMonitoringRenderService) 
            : base(excelService,
                fundingRulesMonitoringModelBuilder,
                fundingRulesMonitoringRenderService,
                "TaskGenerateFundingRulesMonitoring08Report",
                "FRM08")
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
