using System;
using System.Collections.Generic;
using ESFA.DC.ExcelService.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Frm.FRM09
{
    public class Frm09Report : AbstractFundingRulesMonitoringReport<Frm09ReportModel>, IFrmWorksheetReport
    {
        public Frm09Report(
            IExcelFileService excelService,
            IModelBuilder<IEnumerable<Frm09ReportModel>> fundingRulesMonitoringModelBuilder, 
            IRenderService<IEnumerable<Frm09ReportModel>> fundingRulesMonitoringRenderService
            ) : base(excelService,
            fundingRulesMonitoringModelBuilder,
            fundingRulesMonitoringRenderService,
            "TaskGenerateFundingRulesMonitoring09Report",
            "FRM09",
            "Transfers with no return")
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
