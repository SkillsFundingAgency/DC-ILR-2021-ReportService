using System.Collections.Generic;

namespace ESFA.DC.ILR.ReportService.Service.Interface
{
    public interface IFilteredReport
    {
        string ReportName { get; }

        IEnumerable<IReportFilterPropertyDefinition> FilteredOn { get; }
    }
}
