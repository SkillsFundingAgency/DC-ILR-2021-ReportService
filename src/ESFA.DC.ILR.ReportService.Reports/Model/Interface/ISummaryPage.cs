using System.Collections.Generic;

namespace ESFA.DC.ILR.ReportService.Reports.Model.Interface
{
    public interface ISummaryPage
    {
        Dictionary<string, string> HeaderData { get; }

        Dictionary<string, string> FooterData { get; }
    }
}
