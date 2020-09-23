using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.ILR.ReportService.Service.Interface
{
    public interface IFrmSummaryTableRow
    {
        string Report { get; }
        string Title { get; }
        int NumberOfQueries { get; }
    }
}
