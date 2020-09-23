using ESFA.DC.ILR.ReportService.Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.ILR.ReportService.Reports.Frm.Model
{
    public class FrmSummaryTableRow : IFrmSummaryTableRow
    {
        public string Report { get; set; }
        public string Title { get; set; }
        public int NumberOfQueries { get; set; }
    }
}
