using System.Collections.Generic;
using Autofac;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.FileService;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReportService.Desktop.Context;
using ESFA.DC.ILR.ReportService.Desktop.Context.Interface;
using ESFA.DC.ILR.ReportService.Desktop.Tests.Stubs;
using ESFA.DC.ILR.ReportService.Reports;
using ESFA.DC.ILR.ReportService.Reports.Builders;
using ESFA.DC.ILR.ReportService.Reports.Providers;
using ESFA.DC.ILR.ReportService.Reports.Reports;
using ESFA.DC.ILR.ReportService.Reports.Service;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Builders;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;
using ESFA.DC.ILR.ReportService.Service.Interface.Providers;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using ESFA.DC.Serialization.Xml;

namespace ESFA.DC.ILR.ReportService.Desktop.Tests
{
    public static class DIComposition
    {
        public static ContainerBuilder BuildContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<EntryPoint>().As<IEntryPoint>();
            builder.RegisterType<ReportServiceJobContextDesktopContext>().As<IReportServiceContext>();
            builder.RegisterType<ReportServiceContextFactory>().As<IReportServiceContextFactory>();
            builder.RegisterType<FileSystemFileService>().As<IFileService>();
            builder.RegisterType<IlrFileServiceProvider>().As<IFileProviderService<IMessage>>().InstancePerLifetimeScope();
            builder.RegisterType<IlrReferenceDataProviderService>().As<IFileProviderService<ReferenceDataRoot>>().InstancePerLifetimeScope();
            builder.RegisterType<IlrValidationErrorsProvider>().As<IFileProviderService<List<ValidationErrors.Interface.Models.ValidationError>>>().InstancePerLifetimeScope();
            builder.RegisterType<ValidationErrorsReportBuilder>().As<IValidationErrorsReportBuilder>().InstancePerLifetimeScope();
            builder.RegisterType<ValidationSchemaErrorsReportBuilder>().As<IValidationSchemaErrorsReportBuilder>().InstancePerLifetimeScope();
            builder.RegisterType<JsonSerializationService>().As<IJsonSerializationService>();
            builder.RegisterType<XmlSerializationService>().As<IXmlSerializationService>();
            builder.RegisterType<DateTimeProvider.DateTimeProvider>().As<IDateTimeProvider>();
            builder.RegisterType<ValueProvider>().As<IValueProvider>();
            builder.RegisterType<LoggerStub>().As<ILogger>();

            //Reports
            builder.RegisterType<ValidationErrorsReport>().As<IReport>().InstancePerLifetimeScope();
            builder.RegisterType<ValidationSchemaErrorsReport>().As<IReport>().InstancePerLifetimeScope();

            builder.RegisterType<CsvService>().As<ICsvService>();

            return builder;
        }
    }
   
}
