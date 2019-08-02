﻿using System.IO;
using System.Linq;
using Aspose.Cells;
using ESFA.DC.ILR.ReportService.Reports.Funding;
using ESFA.DC.ILR.ReportService.Reports.Funding.DevolvedFundingSummary;
using ESFA.DC.ILR.ReportService.Reports.Funding.DevolvedFundingSummary.Model;
using ESFA.DC.ILR.ReportService.Reports.Funding.DevolvedFundingSummary.Model.Interface;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding.DevolvedFunding
{
    public class DevolvedFundingSummaryreportRenderServiceTests
    {
        [Fact]
        public void Render()
        {
            var workbook = new Workbook();

            var currentPeriod = 12;

            var fundingSummaryReport =
                new DevolvedAdultEducationFundingSummaryReportModel("105", 1000000, "Provider ABC", "ILR-10000000-1920-20191204-164917-01.xml", "ILR-10000000-1920-20191204-164916-01.xml", Enumerable.Range(1, 2)
                    .Select(l => (IDevolvedAdultEducationFundingCategory)new DevolvedAdultEducationFundingCategory("Funding Category Title", currentPeriod, Enumerable.Range(1, 5)
                        .Select(k => (IDevolvedAdultEducationFundLineGroup)new DevolvedAdultEducationFundLineGroup("Funding Sub Category Title", currentPeriod, FundingDataSources.FM35, new string[] { }, null)
                        {
                                        FundLines = Enumerable.Range(0, 5)
                                            .Select(j => (IDevolvedAdultEducationFundLine)new DevolvedAdultEducationFundLine(currentPeriod, "Title", 1.1111m, 2.2222m, 3.3333m,
                                                4.4444m,
                                                5.5555m, 6.6666m, 7.7777m, 8.8888m, 9.9999m, 10.1010m, 11.1111m, 12.1212m))
                                            .ToList()
                        }).ToList()
                        )).ToList());

            var worksheet = workbook.Worksheets[0];

            NewService().Render(fundingSummaryReport, worksheet);

            Directory.CreateDirectory("Output");
            workbook.Save("Output/DevolvedFundingSummaryReport.xlsx");
        }

        private DevolvedFundingSummaryReportRenderService NewService()
        {
            return new DevolvedFundingSummaryReportRenderService();
        }
    }
}
