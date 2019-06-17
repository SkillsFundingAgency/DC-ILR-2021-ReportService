using Autofac;
using ESFA.DC.ILR.ReportService.Desktop.Context;
using ESFA.DC.ILR.ReportService.Desktop.Context.Interface;
using ESFA.DC.ILR.ReportService.Reports;
using ESFA.DC.ILR.ReportService.Reports.Providers;
using ESFA.DC.ILR.ReportService.Reports.Reports;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Providers;

namespace ESFA.DC.ILR.ReportService.Desktop.Modules
{
    public class ReportServieDesktopModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<EntryPoint>().As<IEntryPoint>();
            builder.RegisterType<ReportServiceJobContextDesktopContext>().As<IReportServiceContext>();
            builder.RegisterType<ReportServiceContextFactory>().As<IReportServiceContextFactory>();
            builder.RegisterType<IlrFileServiceProvider>().As<IIlrProviderService>();

            //Reports
            builder.RegisterType<ValidationErrorsReport>().As<IReport>();
        }
    }
}
