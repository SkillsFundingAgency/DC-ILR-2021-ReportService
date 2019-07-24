using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.DevolvedFundingSummary
{
    public class DevolvedAdultEducationFundingSummaryReport : AbstractReport, IReport
    {
        public DevolvedAdultEducationFundingSummaryReport() 
            : base(ReportTaskNameConstants.DevolvedAdultEducationFundingSummaryReport, "Devolved Adult Education Funding Summary Report")
        {

        }

        public async Task<IEnumerable<string>> GenerateAsync(IReportServiceContext reportServiceContext, IReportServiceDependentData reportsDependentData, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Type> DependsOn { get; }
    }
}
