using System;
using System.Collections.Generic;
using ESFA.DC.ExcelService.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.AEBSTF;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Desktop
{
    public class DesktopAEBSTFFundingSummaryReport : AEBSTFFundingSummaryReport
    {
        public DesktopAEBSTFFundingSummaryReport(
            IFileNameService fileNameService,
            IModelBuilder<AEBSTFFundingSummaryReportModel> fundingSummaryReportModelBuilder,
            IExcelFileService excelService,
            IRenderService<IFundingSummaryReport> fundingSummaryReportRenderService) 
            : base(fileNameService, fundingSummaryReportModelBuilder, excelService, fundingSummaryReportRenderService)
        {
        }

        public override IEnumerable<Type> DependsOn
            => new[]
            {
                DependentDataCatalog.Fm35
            };
    }
}
