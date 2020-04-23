using ESFA.DC.ExcelService.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.AdultFundingClaim.Desktop
{
    public class DesktopAdultFundingClaimReport : AdultFundingClaimReport
    {
        public DesktopAdultFundingClaimReport(
            IFileNameService fileNameService, 
            IModelBuilder<AdultFundingClaimReportModel> modelBuilder,
            IExcelFileService excelService) 
            : base(fileNameService, modelBuilder, excelService, true)
        {
        }
    }
}
