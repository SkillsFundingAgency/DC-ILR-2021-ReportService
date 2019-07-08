using System.IO;
using System.Linq;
using Aspose.Cells;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model.Interface;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding.FundingSummary
{
    public class FundingSummaryReportRenderServiceTests
    {
        [Fact]
        public void Render()
        {
            var workbook = new Workbook();

            var fundingSummaryReport = new FundingSummaryReport(Enumerable.Range(1, 5)
                .Select(l => (IFundingCategory)new FundingCategory("Title", "FCTitle", "CFCTitle", Enumerable.Range(1, 10)
                    .Select(k => (IFundingSubCategory)new FundingSubCategory("Funding Sub Category SubTitle", "Funding SubCategory Title", Enumerable.Range(1, 3)
                        .Select(i => (IFundLineGroup)new FundLineGroup("FundLineGroup", Enumerable.Range(0, 5)
                            .Select(j => (IFundLine)new FundLine(12, "Title", 1.1111m, 2.2222m, 3.3333m, 4.4444m, 5.5555m, 6.6666m, 7.7777m, 8.8888m, 9.9999m, 10.1010m, 11.1111m, 12.1212m))
                            .ToList()))
                        .ToList()))
                    .ToList()))
                .ToList());

            var worksheet = workbook.Worksheets[0];

            worksheet = NewService().Render(fundingSummaryReport, worksheet);
            
            Directory.CreateDirectory("Output");
            workbook.Save("Output/FundingSummaryReport.xlsx");
        }
        
        private FundingSummaryReportRenderService NewService()
        {
            return new FundingSummaryReportRenderService();
        }
    }
}
