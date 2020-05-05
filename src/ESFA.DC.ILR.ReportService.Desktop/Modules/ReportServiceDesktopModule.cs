using Autofac;
using ESFA.DC.ILR.Desktop.Interface;
using ESFA.DC.ILR.ReportService.Desktop.Context;
using ESFA.DC.ILR.ReportService.Desktop.Service;
using ESFA.DC.ILR.ReportService.Modules;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Desktop.Modules
{
    public class ReportServiceDesktopModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ReportServiceJobContextDesktopContext>().As<IReportServiceContext>();
            builder.RegisterType<ReportServiceContextFactory>().As<IReportServiceContextFactory<IDesktopContext>>();
            builder.RegisterType<DesktopFileNameService>().As<IFileNameService>();
            builder.RegisterType<DesktopJobContextMessageKeysMutator>().As<IJobContextMessageKeysMutator>();

            builder.RegisterModule<OrchestrationModule>();
            builder.RegisterModule<DataModule>();
            builder.RegisterModule<DesktopReportsModule>();
        }
    }
}
