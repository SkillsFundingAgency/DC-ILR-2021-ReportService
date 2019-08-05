﻿using Autofac;
using ESFA.DC.ILR.ReportService.Data.Providers;
using ESFA.DC.ILR.ReportService.Reports;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Modules
{
    public class DataModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<IlrReferenceDataProviderService>().Keyed<IExternalDataProvider>(DependentDataCatalog.ReferenceData);
            builder.RegisterType<IlrFileServiceProvider>().Keyed<IExternalDataProvider>(DependentDataCatalog.Ilr);
            builder.RegisterType<LooseIlrFileServiceProvider>().Keyed<IExternalDataProvider>(DependentDataCatalog.LooseIlr);
            builder.RegisterType<IlrValidationErrorsProvider>().Keyed<IExternalDataProvider>(DependentDataCatalog.ValidationErrors);
            builder.RegisterType<Fm25Provider>().Keyed<IExternalDataProvider>(DependentDataCatalog.Fm25);
            builder.RegisterType<Fm35Provider>().Keyed<IExternalDataProvider>(DependentDataCatalog.Fm35);
            builder.RegisterType<Fm36Provider>().Keyed<IExternalDataProvider>(DependentDataCatalog.Fm36);
            builder.RegisterType<Fm81Provider>().Keyed<IExternalDataProvider>(DependentDataCatalog.Fm81);
            builder.RegisterType<Fm99Provider>().Keyed<IExternalDataProvider>(DependentDataCatalog.Fm99);

            builder.RegisterType<ReportsDependentDataPopulationService>().As<IReportsDependentDataPopulationService>();
        }
    }
}
