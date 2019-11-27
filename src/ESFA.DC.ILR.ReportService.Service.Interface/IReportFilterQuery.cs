using System.Collections.Generic;

namespace ESFA.DC.ILR.ReportService.Service.Interface
{
    public interface IReportFilterQuery
    {
        string ReportName { get; }

        IEnumerable<IReportFilterPropertyQuery> Properties { get; }
    }
}
