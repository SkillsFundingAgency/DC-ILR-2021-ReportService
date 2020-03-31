namespace ESFA.DC.ILR.ReportService.Service.Interface
{
    public interface IDatabaseConfiguration
    {
        string EasDbConnectionString { get; set; }

        string IlrDbConnectionString { get; set; }

        string PostcodesDbConnectionString { get; set; }
    }
}
