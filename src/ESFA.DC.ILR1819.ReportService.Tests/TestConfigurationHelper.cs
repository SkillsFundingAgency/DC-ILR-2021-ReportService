using ESFA.DC.ILR1819.ReportService.Model.Configuration;
using ESFA.DC.ILR1819.ReportService.Stateless.Configuration;
using ESFA.DC.ServiceFabric.Helpers.Interfaces;

namespace ESFA.DC.ILR1819.ReportService.Tests
{
    public sealed class TestConfigurationHelper : IConfigurationHelper
    {
        public T GetSectionValues<T>(string sectionName)
        {
            switch (sectionName)
            {
                case "RedisSection":
                    return (T)(object)new RedisOptions();
                case "LarsSection":
                    return (T)(object)new LarsConfiguration();
                case "TopicAndTaskSection":
                    return (T)(object)new TopicAndTaskSectionOptions();
                case "AzureStorageSection":
                    return (T)(object)new AzureStorageOptions();
                case "ServiceBusSettings":
                    return (T)(object)new ServiceBusOptions();
                case "VersionSection":
                    return (T)(object)new VersionInfo();
                case "LoggerSection":
                    return (T)(object)new LoggerOptions();
                case "JobStatusSection":
                    return (T)(object)new JobStatusQueueOptions();
                case "OrgSection":
                    return (T)(object)new OrgConfiguration();
            }

            return default(T);
        }
    }
}
