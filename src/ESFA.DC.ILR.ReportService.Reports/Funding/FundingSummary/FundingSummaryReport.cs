using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary
{
    public class FundingSummaryReport : AbstractReport, IReport
    {
        public FundingSummaryReport()
            : base(ReportTaskNameConstants.FundingSummaryReport, "Funding Summary Report")
        {
        }

        public IEnumerable<Type> DependsOn
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> GenerateAsync(
            IReportServiceContext reportServiceContext,
            IReportServiceDependentData reportsDependentData,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
