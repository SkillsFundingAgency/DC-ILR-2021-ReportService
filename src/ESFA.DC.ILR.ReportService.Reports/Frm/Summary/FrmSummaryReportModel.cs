using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ReportService.Reports.Frm.Summary
{
    public class FrmSummaryReportModel
    {
        public string ProviderName { get; set; }
        public int UKPRN { get; set; }
        public string ILRFileName { get; set; }
        public string LastFileUpdate { get; set; }
        public string SecurityClassification { get; set; }
        public List<FrmSummaryReportTableRow> FundingRulesMonitoring { get; set; }

        public void GenerateTotalRow()
        {
            var row = new FrmSummaryReportTableRow()
            {
                Report = "Total",
                Title = "All Reports",
                NumberOfQueries = FundingRulesMonitoring.Sum(r => r.NumberOfQueries)
            };
            FundingRulesMonitoring.Add(row);
        }
    }

    public class FrmSummaryReportTableRow
    {
        public string Report { get; set; }
        public string Title { get; set; }
        public int NumberOfQueries { get; set; }
    }
}
