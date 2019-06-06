namespace ESFA.DC.ILR.ReportService.Stateless.Interfaces
{
    public interface IAzureStorageOptions
    {
        string AzureBlobConnectionString { get; set; }

        string AzureBlobContainerName { get; set; }
    }
}