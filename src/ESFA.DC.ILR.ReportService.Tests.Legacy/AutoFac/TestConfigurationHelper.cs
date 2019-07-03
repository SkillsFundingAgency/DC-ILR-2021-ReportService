using ESFA.DC.ILR.ReportService.Interface.Configuration;
using ESFA.DC.ILR.ReportService.Model.Configuration;
using ESFA.DC.ILR1819.ReportService.Stateless.Configuration;
using ESFA.DC.ServiceFabric.Helpers.Interfaces;

namespace ESFA.DC.ILR.ReportService.Tests.AutoFac
{
    public sealed class TestConfigurationHelper : IConfigurationHelper
    {
        public T GetSectionValues<T>(string sectionName)
        {
            switch (sectionName)
            {
                case "LarsSection":
                    return (T)(object)new LarsConfiguration();
                case "AzureStorageSection":
                    return (T)(object)new AzureStorageOptions
                    {
                        AzureBlobConnectionString = "AzureBlobConnectionString",
                        AzureBlobContainerName = "AzureBlobContainerName"
                    };
                case "ServiceBusSettings":
                    return (T)(object)new ServiceBusOptions()
                    {
                        AuditQueueName = "AuditQueueName",
                        ReportingSubscriptionName = "ReportingSubscriptionName",
                        ServiceBusConnectionString = "ServiceBusConnectionString",
                        TopicName = "TopicName"
                    };
                case "VersionSection":
                    return (T)(object)new VersionInfo
                    {
                        ServiceReleaseVersion = "ServiceReleaseVersion"
                    };
                case "LoggerSection":
                    return (T)(object)new LoggerOptions
                    {
                        LoggerConnectionstring = "Server=.;Database=myDataBase;User Id=myUsername;Password = myPassword;"
                    };
                case "JobStatusSection":
                    return (T)(object)new JobStatusQueueOptions();
                case "OrgSection":
                    return (T)(object)new OrgConfiguration();
                case "CollectionsManagementSection":
                    return (T)(object)new CollectionsManagementConfiguration
                    {
                        CollectionsManagementConnectionString = "Server=.;Database=myDataBase;User Id=myUsername;Password = myPassword;"
                    };
                case "DasCommitmentsSection":
                    return (T)(object)new DasCommitmentsConfiguration
                    {
                        DasCommitmentsConnectionString =
                            "Server=.;Database=myDataBase;User Id=myUsername;Password = myPassword;"
                    };
                case "EasSection":
                    return (T)(object)new EasConfiguration()
                    {
                        EasConnectionString = "data source=(local);initial catalog=Easdb;integrated security=True;multipleactiveresultsets=True;Connect Timeout=90"
                    };
                case "LargeEmployerSection":
                    return (T)(object)new LargeEmployerConfiguration
                    {
                        LargeEmployerConnectionString =
                            "Server=.;Database=myDataBase;User Id=myUsername;Password = myPassword;"
                    };
                case "DASPaymentsSection":
                    return (T)(object)new DASPaymentsConfiguration
                    {
                        DASPaymentsConnectionString =
                            "Server=.;Database=DASPayments;integrated security=True;"
                    };
                case "PostcodeSection":
                    return (T)(object)new PostcodeConfiguration
                    {
                        PostcodeConnectionString =
                            "Server=.;Database=myDataBase;User Id=myUsername;Password = myPassword;"
                    };
                case "DataStoreSection":
                    return (T)(object)new DataStoreConfiguration()
                    {
                        ILRDataStoreConnectionString =
                            "Server=.;Database=ilr1819_DataStore;integrated security=True;",
                        ILRDataStoreValidConnectionString =
                            "Server=.;Database=ilr1819_DataStore;integrated security=True;"
                    };
                case "IlrValidationErrorsSection":
                    return (T)(object)new IlrValidationErrorsConfiguration();
            }

            return default(T);
        }
    }
}