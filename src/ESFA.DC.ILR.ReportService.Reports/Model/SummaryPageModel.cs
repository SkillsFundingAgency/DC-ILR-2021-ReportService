using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Reports.Model.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Model
{
    public class SummaryPageModel : ISummaryPage
    {
        public Dictionary<string, string> HeaderData { get; set; }

        public Dictionary<string, string> FooterData { get; set; }
    }
}
