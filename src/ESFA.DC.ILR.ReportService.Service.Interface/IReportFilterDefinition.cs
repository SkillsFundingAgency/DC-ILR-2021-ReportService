using System.Collections.Generic;

namespace ESFA.DC.ILR.ReportService.Service.Interface
{
    public interface IReportFilterDefinition
    {
        string ReportName { get; }

        IEnumerable<IReportFilterPropertyDefinition> Properties { get; }
    }
}
