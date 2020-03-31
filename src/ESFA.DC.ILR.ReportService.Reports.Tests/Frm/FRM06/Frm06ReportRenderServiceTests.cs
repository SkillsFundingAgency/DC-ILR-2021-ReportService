﻿using System.IO;
using System.Linq;
using Aspose.Cells;
using ESFA.DC.ILR.ReportService.Reports.Frm.FRM06;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Frm.FRM06
{
    public class Frm06ReportRenderServiceTests
    {
        [Fact]
        public void Render()
        {
            var workbook = new Workbook();
            var worksheet = workbook.Worksheets[0];

            var frm06ReportModels = Enumerable.Range(1, 5).Select(l => new Frm06ReportModel
            {
                LearnAimRef = l.ToString(),
                UKPRN = 1000000 + l,
                OrgName = $"Org{l}"
            });

            NewService().Render(frm06ReportModels, worksheet);

            Directory.CreateDirectory("Output");
            workbook.Save("Output/FRM06_FundingRuleMonitoringReport.xlsx");
        }

        private Frm06ReportRenderService NewService()
        {
            return new Frm06ReportRenderService();
        }
    }
}