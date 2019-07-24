using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using Autofac;
using Autofac.Integration.ServiceFabric;
using ESFA.DC.ILR.ReportService.Service;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Stateless.Configuration;
using ESFA.DC.ServiceFabric.Common.Config;
using ESFA.DC.ServiceFabric.Common.Config.Interface;


namespace ESFA.DC.ILR.ReportService.Stateless
{
    public static class Program
    {
        /// <summary>
        /// This is the entry point of the service host process.#d
        /// </summary>
        public static void Main()
        {
            try
            {
                IServiceFabricConfigurationService serviceFabricConfigurationService = new ServiceFabricConfigurationService();

                // License Aspose.Cells
                SoftwareLicenceSection softwareLicenceSection = serviceFabricConfigurationService.GetConfigSectionAs<SoftwareLicenceSection>(nameof(SoftwareLicenceSection));
                if (!string.IsNullOrEmpty(softwareLicenceSection.AsposeLicence))
                {
                    using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(softwareLicenceSection.AsposeLicence.Replace("&lt;", "<").Replace("&gt;", ">"))))
                    {
                        new Aspose.Cells.License().SetLicense(ms);
                    }
                }

                // Setup Autofac
                ContainerBuilder builder = DIComposition.BuildContainer(serviceFabricConfigurationService);

                // Register the Autofac magic for Service Fabric support.
                builder.RegisterServiceFabricSupport();

                // Register the stateless service.
                builder.RegisterStatelessService<ServiceFabric.Common.Stateless>("ESFA.DC.ILR1920.ReportService.StatelessType");

                using (var container = builder.Build())
                {
                    var legacyEntryPoint = container.Resolve<LegacyEntryPoint>();
                    var entryPoint = container.Resolve<IEntryPoint>();
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
