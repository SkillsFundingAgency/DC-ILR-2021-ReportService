using System.Collections.Generic;
using Autofac;
using ESFA.DC.CsvService;
using ESFA.DC.CsvService.Interface;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ExcelService;
using ESFA.DC.ExcelService.Interface;
using ESFA.DC.FileService;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.Desktop.Interface;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Data;
using ESFA.DC.ILR.ReportService.Data.Providers;
using ESFA.DC.ILR.ReportService.Desktop.Context;
using ESFA.DC.ILR.ReportService.Desktop.Tests.Stubs;
using ESFA.DC.ILR.ReportService.Models.ReferenceData;
using ESFA.DC.ILR.ReportService.Reports;
using ESFA.DC.ILR.ReportService.Reports.Service;
using ESFA.DC.ILR.ReportService.Reports.Validation.Detail;
using ESFA.DC.ILR.ReportService.Reports.Validation.FrontEnd;
using ESFA.DC.ILR.ReportService.Reports.Validation.Interface;
using ESFA.DC.ILR.ReportService.Reports.Validation.Schema;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;
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
            builder.RegisterType<ReportServiceContextKeysMutator>().As<IReportServiceContextKeysMutator>();
            builder.RegisterType<ReportServiceContextFactory>().As<IReportServiceContextFactory<IDesktopContext>>();
            builder.RegisterType<FileSystemFileService>().As<IFileService>();
            
            builder.RegisterType<IlrReferenceDataProviderService>()
                .Keyed<IExternalDataProvider>(typeof(ReferenceDataRoot))
                .InstancePerLifetimeScope();

            builder.RegisterType<ValidIlrFileServiceProvider>()
                .Keyed<IExternalDataProvider>(typeof(IMessage))
                .InstancePerLifetimeScope();

            builder.RegisterType<IlrValidationErrorsProvider>()
                .Keyed<IExternalDataProvider>(typeof(List<ValidationErrors.Interface.Models.ValidationError>))
                .InstancePerLifetimeScope();
            
            builder.RegisterType<ReportsDependentDataPopulationService>().As<IReportsDependentDataPopulationService>();

            builder.RegisterType<JsonSerializationService>().As<IJsonSerializationService>();
            builder.RegisterType<XmlSerializationService>().As<IXmlSerializationService>();
            builder.RegisterType<DateTimeProvider.DateTimeProvider>().As<IDateTimeProvider>();

            builder.RegisterType<ReportServiceDependentData>().As<IReportServiceDependentData>();
            builder.RegisterType<FileNameService>().As<IFileNameService>();

            builder.RegisterType<LoggerStub>().As<ILogger>();

            // Builders 
            builder.RegisterType<ValidationErrorsDetailReportBuilder>().As<IValidationErrorsReportBuilder>();
            builder.RegisterType<ValidationSchemaErrorsReportBuilder>().As<IValidationSchemaErrorsReportBuilder>();

            //Reports
            builder.RegisterType<ValidationErrorsDetailReport>().As<IReport>();
            builder.RegisterType<ValidationSchemaErrorsReport>().As<IReport>();

            builder.RegisterType<FrontEndValidationReport>().As<IFrontEndValidationReport>();

            builder.RegisterType<CsvFileService>().As<ICsvFileService>();
            builder.RegisterType<ExcelFileService>().As<IExcelFileService>();
            builder.RegisterType<CreateZipService>().As<IZipService>();

            return builder;
        }
    }
   
}
