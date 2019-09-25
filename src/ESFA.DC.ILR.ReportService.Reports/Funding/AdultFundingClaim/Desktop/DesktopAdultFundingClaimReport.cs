using Aspose.Cells;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.AdultFundingClaim.Desktop
{
    public class DesktopAdultFundingClaimReport : AdultFundingClaimReport
    {
        public DesktopAdultFundingClaimReport(
            IFileNameService fileNameService, 
            IModelBuilder<AdultFundingClaimReportModel> modelBuilder,
            IExcelService excelService) 
            : base(fileNameService, modelBuilder, excelService)
        {
        }

        public override void RenderIndicativeMessage(Workbook workbook)
        {
        }
    }
}
