using System;
using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Desktop
{
    public class DesktopFundingSummaryReport : FundingSummaryReport
    {
        public DesktopFundingSummaryReport(
            IFileNameService fileNameService,
            IModelBuilder<IFundingSummaryReport> fundingSummaryReportModelBuilder,
            IExcelService excelService,
            IRenderService<IFundingSummaryReport> fundingSummaryReportRenderService) 
            : base(fileNameService, fundingSummaryReportModelBuilder, excelService, fundingSummaryReportRenderService)
        {
        }

        public override IEnumerable<Type> DependsOn
            => new[]
            {
                DependentDataCatalog.Fm25,
                DependentDataCatalog.Fm35,
                DependentDataCatalog.Fm36,
                DependentDataCatalog.Fm81,
                DependentDataCatalog.Fm99,
            };
    }
}
