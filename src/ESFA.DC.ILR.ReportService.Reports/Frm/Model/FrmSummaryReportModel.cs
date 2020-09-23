using ESFA.DC.ILR.ReportService.Reports.Frm.Model;
using ESFA.DC.ILR.ReportService.Reports.Frm.Model.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ReportService.Reports.Frm
{
    public class FrmSummaryReportModel : IFrmSummaryReport
    {
        public FrmSummaryReportModel(IDictionary<string, string> headerData)
        {
            HeaderData = headerData;
        }
        public IDictionary<string, string> HeaderData { get; }

        public IList<IFrmSummaryTableRow> SummaryTable { get; set; }

        public int TotalRowCount => SummaryTable.Sum(x => x.NumberOfQueries);
    }
}
