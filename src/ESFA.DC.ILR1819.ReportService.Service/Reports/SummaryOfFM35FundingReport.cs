using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Reports;
using ESFA.DC.ILR1819.ReportService.Model.Report;
using ESFA.DC.JobContext.Interface;

namespace ESFA.DC.ILR1819.ReportService.Service.Reports
{
    public class SummaryOfFM35FundingReport : AbstractReportBuilder, IReport
    {
        public SummaryOfFM35FundingReport(IDateTimeProvider dateTimeProvider)
            : base(dateTimeProvider)
        {
            ReportName = "foo";
        }

        public ReportType ReportType => ReportType.SummaryOfFM35Funding;

        public Task GenerateReport(IJobContextMessage jobContextMessage, ZipArchive archive, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
