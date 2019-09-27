namespace ESFA.DC.ILR.ReportService.Interface.Configuration
{
    public interface IAzureStorageOptions
    {
        string AzureBlobConnectionString { get; set; }

        string AzureBlobContainerName { get; set; }
    }
}
