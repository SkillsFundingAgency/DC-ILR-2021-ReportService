using Autofac;
using Autofac.Features.AttributeFilters;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.FileService;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.ReportService.Desktop.Context;
using ESFA.DC.ILR.ReportService.Desktop.Context.Interface;
using ESFA.DC.ILR.ReportService.Reports;
using ESFA.DC.ILR.ReportService.Reports.Providers;
using ESFA.DC.ILR.ReportService.Reports.Reports;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Providers;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using ESFA.DC.Serialization.Xml;

namespace ESFA.DC.ILR.ReportService.Desktop.Modules
{
    public class ReportServieDesktopModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<EntryPoint>().As<IEntryPoint>();
            builder.RegisterType<ReportServiceJobContextDesktopContext>().As<IReportServiceContext>();
            builder.RegisterType<ReportServiceContextFactory>().As<IReportServiceContextFactory>();
            builder.RegisterType<FileSystemFileService>().As<IFileService>();
            builder.RegisterType<IlrFileServiceProvider>().As<IIlrProviderService>();
            builder.RegisterType<JsonSerializationService>().As<IJsonSerializationService>();
            builder.RegisterType<XmlSerializationService>().As<IXmlSerializationService>();
            builder.RegisterType<DateTimeProvider.DateTimeProvider>().As<IDateTimeProvider>();
            builder.RegisterType<ValueProvider>().As<IValueProvider>();

            //Reports
            builder.RegisterType<ValidationErrorsReport>().As<IReport>()
                .WithAttributeFiltering()
                .InstancePerLifetimeScope();
        }
    }
}
