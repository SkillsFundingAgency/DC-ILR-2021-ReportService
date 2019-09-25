using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports
{
    public class ReportFilterPropertyDefinition<T> : IReportFilterPropertyDefinition
    {
        public ReportFilterPropertyDefinition(string name)
        {
            Name = name;
            Type = typeof(T).FullName;
        }

        public string Name { get; }

        public string Type { get; }
    }
}
