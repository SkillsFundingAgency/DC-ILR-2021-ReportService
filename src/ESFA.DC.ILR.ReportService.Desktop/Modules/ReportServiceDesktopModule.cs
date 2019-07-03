using System.Collections.Generic;
using Autofac;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReportService.Data.Providers;
using ESFA.DC.ILR.ReportService.Desktop.Context;
using ESFA.DC.ILR.ReportService.Desktop.Context.Interface;
using ESFA.DC.ILR.ReportService.Reports;
using ESFA.DC.ILR.ReportService.Reports.Interface;
using ESFA.DC.ILR.ReportService.Reports.Service;
using ESFA.DC.ILR.ReportService.Reports.Validation;
using ESFA.DC.ILR.ReportService.Reports.Validation.Detail;
using ESFA.DC.ILR.ReportService.Reports.Validation.FrontEnd;
using ESFA.DC.ILR.ReportService.Reports.Validation.Interface;
using ESFA.DC.ILR.ReportService.Reports.Validation.Schema;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;

namespace ESFA.DC.ILR.ReportService.Desktop.Modules
{
    public class ReportServiceDesktopModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<EntryPoint>().As<IEntryPoint>();
            builder.RegisterType<ReportServiceJobContextDesktopContext>().As<IReportServiceContext>();
            builder.RegisterType<ReportServiceContextFactory>().As<IReportServiceContextFactory>();

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

            builder.RegisterType<ReportServiceDependentData>().As<IReportServiceDependentData>();
            builder.RegisterType<FileNameService>().As<IFileNameService>();

            // Builders 
            builder.RegisterType<ValidationErrorsDetailReportBuilder>().As<IValidationErrorsReportBuilder>().InstancePerLifetimeScope();
            builder.RegisterType<ValidationSchemaErrorsReportBuilder>().As<IValidationSchemaErrorsReportBuilder>().InstancePerLifetimeScope();

            //Reports
            builder.RegisterType<ValidationErrorsDetailReport>().As<IReport>();
            builder.RegisterType<ValidationSchemaErrorsReport>().As<IReport>();
            builder.RegisterType<FrontEndValidationReport>().As<IFrontEndValidationReport>();

            builder.RegisterType<CsvService>().As<ICsvService>();
        }
    }
}
