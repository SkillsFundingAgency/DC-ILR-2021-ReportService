using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Autofac;
using Autofac.Integration.ServiceFabric;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ServiceFabric.Common.Config;
using ESFA.DC.ServiceFabric.Common.Config.Interface;

namespace ESFA.DC.ILR.ReportService.Stateless
{
    public static class Program
    {
        public static void Main()
        {
            try
            {
                IServiceFabricConfigurationService serviceFabricConfigurationService = new ServiceFabricConfigurationService();

                // Setup Autofac
                ContainerBuilder builder = DIComposition.BuildContainer(serviceFabricConfigurationService);

                // Register the Autofac magic for Service Fabric support.
                builder.RegisterServiceFabricSupport();

                // Register the stateless service.
                builder.RegisterStatelessService<ServiceFabric.Common.Stateless>("ESFA.DC.ILR2021.ReportService.StatelessType");

                using (var container = builder.Build())
                {
                    //var entryPoint = container.Resolve<IEntryPoint>();
                    var reports = container.Resolve<IEnumerable<IReport>>();

                    ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(ServiceFabric.Common.Stateless).Name);

                    // Prevents this host process from terminating so services keep running.
                    Thread.Sleep(Timeout.Infinite);
                }
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e + Environment.NewLine + (e.InnerException?.ToString() ?? "No inner exception"));
                throw;
            }
        }
    }
}
