using ESFA.DC.ILR.ReportService.Stateless.Interfaces;

namespace ESFA.DC.ILR.ReportService.Stateless.Configuration
{
    public class AzureStorageOptions : IAzureStorageOptions
    {
        public string AzureBlobConnectionString { get; set; }

        public string AzureBlobContainerName { get; set; }
    }
}
