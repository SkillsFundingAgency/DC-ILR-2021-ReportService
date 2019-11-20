using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Stateless.Configuration
{
    public class DatabaseConfiguration : IDatabaseConfiguration
    {
        public string EasDbConnectionString { get; set; }

        public string IlrDbConnectionString { get; set; }

        public string PostcodesDbConnectionString { get; set; }
    }
}
