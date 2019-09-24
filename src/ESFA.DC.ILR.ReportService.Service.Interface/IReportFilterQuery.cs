using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.ILR.ReportService.Service.Interface
{
    public interface IReportFilterQuery
    {
        string ReportName { get; }

        IEnumerable<IReportFilterPropertyQuery> Properties { get; }
    }
}
