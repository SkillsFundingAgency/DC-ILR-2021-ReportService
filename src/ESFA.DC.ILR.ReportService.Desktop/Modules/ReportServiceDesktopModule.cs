﻿using System.Collections.Generic;
using Autofac;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReportService.Data.Providers;
using ESFA.DC.ILR.ReportService.Desktop.Context;
using ESFA.DC.ILR.ReportService.Desktop.Context.Interface;
using ESFA.DC.ILR.ReportService.Reports;
using ESFA.DC.ILR.ReportService.Reports.Builders;
using ESFA.DC.ILR.ReportService.Reports.Reports;
using ESFA.DC.ILR.ReportService.Reports.Service;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface.Builders;
using ESFA.DC.ILR.ReportService.Service.Interface.Output;
using ESFA.DC.ILR.ReportService.Service.Model;
using ESFA.DC.ILR.ReportService.Service.Model.Interface;
using ESFA.DC.ILR.ValidationErrors.Interface.Models;

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
            // Builders 
            builder.RegisterType<ValidationErrorsReportBuilder>().As<IValidationErrorsReportBuilder>().InstancePerLifetimeScope();
            builder.RegisterType<ValidationSchemaErrorsReportBuilder>().As<IValidationSchemaErrorsReportBuilder>().InstancePerLifetimeScope();

            //Reports
            builder.RegisterType<ValidationErrorsReport>().As<IReport>();
            builder.RegisterType<ValidationSchemaErrorsReport>().As<IReport>();

            builder.RegisterType<CsvService>().As<ICsvService>();
        }
    }
}
