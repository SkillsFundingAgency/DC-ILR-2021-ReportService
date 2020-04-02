using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Desktop.Context
{
    public class ReportFilterPropertyQuery : IReportFilterPropertyQuery
    {
        public string PropertyName { get; set; }

        public object Value { get; set; }
    }
}
