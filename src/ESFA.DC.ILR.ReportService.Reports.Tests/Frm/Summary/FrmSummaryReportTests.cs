using ESFA.DC.ILR.ReportService.Reports.Frm.Summary;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Frm.Summary
{
    public class FrmSummaryReportTests
    {
        [Fact]
        public void GenerateTotalRow()
        {
            var list = new List<FrmSummaryReportTableRow>() {
                new FrmSummaryReportTableRow
                {
                    Title = "FRM06",
                    Report = "FRM06",
                    NumberOfQueries = 2
                },
                 new FrmSummaryReportTableRow
                {
                    Title = "FRM07",
                    Report = "FRM07",
                    NumberOfQueries = 2
                }
            };
        }
    }
}
