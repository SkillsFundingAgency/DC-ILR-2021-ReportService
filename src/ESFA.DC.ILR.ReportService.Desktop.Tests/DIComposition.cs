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
using ESFA.DC.ILR.ReportService.Service.Model;
using ESFA.DC.ILR.ReportService.Service.Model.Interface;
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
            
            builder.RegisterType<IlrReferenceDataProviderService>()
                .Keyed<IExternalDataProvider>(typeof(ReferenceDataRoot))
                .InstancePerLifetimeScope();

            builder.RegisterType<IlrFileServiceProvider>()
                .Keyed<IExternalDataProvider>(typeof(IMessage))
                .InstancePerLifetimeScope();

            builder.RegisterType<IlrValidationErrorsProvider>()
                .Keyed<IExternalDataProvider>(typeof(List<ValidationErrors.Interface.Models.ValidationError>))
                .InstancePerLifetimeScope();
            
            builder.RegisterType<ReportsDependentDataPopulationService>().As<IReportsDependentDataPopulationService>();

            builder.RegisterType<JsonSerializationService>().As<IJsonSerializationService>();
            builder.RegisterType<XmlSerializationService>().As<IXmlSerializationService>();
            builder.RegisterType<DateTimeProvider.DateTimeProvider>().As<IDateTimeProvider>();
            builder.RegisterType<ValueProvider>().As<IValueProvider>();

            builder.RegisterType<ReportServiceDependentData>().As<IReportServiceDependentData>();

            builder.RegisterType<LoggerStub>().As<ILogger>();

            // Builders 
            builder.RegisterType<ValidationErrorsReportBuilder>().As<IValidationErrorsReportBuilder>().InstancePerLifetimeScope();
            builder.RegisterType<ValidationSchemaErrorsReportBuilder>().As<IValidationSchemaErrorsReportBuilder>().InstancePerLifetimeScope();

            //Reports
            builder.RegisterType<ValidationErrorsReport>().As<IReport>().InstancePerLifetimeScope();
            builder.RegisterType<ValidationSchemaErrorsReport>().As<IReport>().InstancePerLifetimeScope();

            builder.RegisterType<CsvService>().As<ICsvService>();

            return builder;
        }
    }
   
}
