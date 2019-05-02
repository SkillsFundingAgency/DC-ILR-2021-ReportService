using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.ILR.ReportService.Interface.Provider;
using ESFA.DC.ILR.ReportService.Service.Provider;
using ESFA.DC.ILR1819.ReportService.Stateless;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.JobContextManager.Interface;
using ESFA.DC.JobContextManager.Model;
using ESFA.DC.JobContextManager.Model.Interface;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Tests.AutoFac
{
    public sealed class AutoFacTest
    {
        [Fact]
        public async Task TestRegistrations()
        {
            try
            {
                JobContextMessage jobContextMessage =
                    new JobContextMessage(
                        1,
                        new ITopicItem[] { new TopicItem("SubscriptionName", new List<ITaskItem>()) },
                        0,
                        DateTime.UtcNow);
                jobContextMessage.KeyValuePairs[JobContextMessageKey.Container] = "Container";
                CancellationTokenSource cts = new CancellationTokenSource();
                cts.Cancel();

                ContainerBuilder containerBuilder = DIComposition.BuildContainer(new TestConfigurationHelper());

                IContainer c = containerBuilder.Build();
                using (var lifeTime = c.BeginLifetimeScope())
                {
                    var messageHandler = lifeTime.Resolve<IMessageHandler<JobContextMessage>>();
                    bool ret = await messageHandler.HandleAsync(jobContextMessage, cts.Token);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Assert.Null(ex);
            }
        }

        [Theory]
        [InlineData("ILR1819", typeof(FM36FileServiceProvider), true)]
        [InlineData("ILR1819", typeof(FM36SqlProviderService), false)]
        [InlineData("EAS", typeof(FM36SqlProviderService), true)]
        [InlineData("EAS", typeof(FM36FileServiceProvider), false)]
        public void TestServicesRegisteredByCollectionName(string collectionName, Type serviceType, bool expectation)
        {
            ContainerBuilder cb = DIComposition.BuildContainer(new TestConfigurationHelper());
            DIComposition.RegisterServicesByCollectionName(collectionName, cb);
            var container = cb.Build();
            container.Resolve<IFM36ProviderService>().GetType()?.Equals(serviceType).Should().Be(expectation);
        }
    }
}
