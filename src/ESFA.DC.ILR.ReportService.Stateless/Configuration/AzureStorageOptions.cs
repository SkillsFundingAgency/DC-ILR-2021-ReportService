﻿using ESFA.DC.ILR1819.ReportService.Stateless.Interfaces;

namespace ESFA.DC.ILR1819.ReportService.Stateless.Configuration
{
    public class AzureStorageOptions : IAzureStorageOptions
    {
        public string AzureBlobConnectionString { get; set; }

        public string AzureBlobContainerName { get; set; }
    }
}
