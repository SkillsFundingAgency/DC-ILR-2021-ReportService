using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports
{
    public class ReportFilterDefinition : IReportFilterDefinition
    {
        public string ReportName { get; set; }

        public IEnumerable<IReportFilterPropertyDefinition> Properties { get; set; }
    }
}
