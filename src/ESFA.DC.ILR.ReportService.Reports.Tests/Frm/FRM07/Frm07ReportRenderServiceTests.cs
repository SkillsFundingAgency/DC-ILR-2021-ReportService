using System.IO;
using System.Linq;
using Aspose.Cells;
using ESFA.DC.ILR.ReportService.Reports.Frm.FRM07;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Frm.FRM07
{
    public class Frm07ReportRenderServiceTests
    {
        [Fact]
        public void Render()
        {
            var workbook = new Workbook();
            var worksheet = workbook.Worksheets[0];

            var frm07ReportModels = Enumerable.Range(1, 5).Select(l => new Frm07ReportModel
            {
                LearnAimRef = l.ToString(),
                UKPRN = 1000000 + l,
                OrgName = $"Org{l}"
            });

            NewService().Render(frm07ReportModels, worksheet);

            Directory.CreateDirectory("Output");
            workbook.Save("Output/FRM07_FundingRuleMonitoringReport.xlsx");
        }

        private Frm07ReportRenderService NewService()
        {
            return new Frm07ReportRenderService();
        }
    }
}
