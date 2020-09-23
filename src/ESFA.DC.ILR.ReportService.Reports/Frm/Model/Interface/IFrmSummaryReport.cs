using ESFA.DC.ILR.ReportService.Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.ILR.ReportService.Reports.Frm.Model.Interface
{
    public interface IFrmSummaryReport
    {
        IDictionary<string, string> HeaderData { get; }
        IList<IFrmSummaryTableRow> SummaryTable { get; set; }
        int TotalRowCount { get; }
    }
}
