using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Desktop.Context
{
    public class ReportFilterQuery : IReportFilterQuery
    {
        public string ReportName { get; set; }

        public IEnumerable<IReportFilterPropertyQuery> Properties { get; set; }
    }
}
