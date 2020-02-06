using Autofac;
using ESFA.DC.EAS1920.EF;
using ESFA.DC.EAS1920.EF.Interface;
using ESFA.DC.ILR.ReportService.Data.Eas;
using ESFA.DC.ILR.ReportService.Data.Eas.Providers;
using ESFA.DC.ILR.ReportService.Reports;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR1920.DataStore.EF;
using ESFA.DC.ILR1920.DataStore.EF.Interface;
using ESFA.DC.ILR1920.DataStore.EF.Valid;
using ESFA.DC.ILR1920.DataStore.EF.Valid.Interface;
using ESFA.DC.ReferenceData.Postcodes.Model;
using ESFA.DC.ReferenceData.Postcodes.Model.Interface;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.ILR.ReportService.Modules
{
    public class EasDataModule : Module
    {
        private IDatabaseConfiguration _databaseConfiguration;

        public EasDataModule(IDatabaseConfiguration databaseConfiguration)
        {
            _databaseConfiguration = databaseConfiguration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            RegisterDatabases(builder);

            builder.RegisterType<IlrReferenceDataProvider>().Keyed<IExternalDataProvider>(DependentDataCatalog.ReferenceData);
            builder.RegisterType<ValidIlrProvider>().Keyed<IExternalDataProvider>(DependentDataCatalog.ValidIlr);
            builder.RegisterType<Fm25Provider>().Keyed<IExternalDataProvider>(DependentDataCatalog.Fm25);
            builder.RegisterType<Fm35Provider>().Keyed<IExternalDataProvider>(DependentDataCatalog.Fm35);
            builder.RegisterType<Fm36Provider>().Keyed<IExternalDataProvider>(DependentDataCatalog.Fm36);
            builder.RegisterType<Fm81Provider>().Keyed<IExternalDataProvider>(DependentDataCatalog.Fm81);
            builder.RegisterType<Fm99Provider>().Keyed<IExternalDataProvider>(DependentDataCatalog.Fm99);
            builder.RegisterType<EasProvider>().Keyed<IExternalDataProvider>(DependentDataCatalog.Eas);
            builder.RegisterType<JobContextMessageKeysMutator>().As<IJobContextMessageKeysMutator>();
        }

        private void RegisterDatabases(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<EasContext>().As<IEasdbContext>();            
            containerBuilder.Register(container => new DbContextOptionsBuilder<EasContext>()
                .UseSqlServer(_databaseConfiguration.EasDbConnectionString, sqlServerOptions => sqlServerOptions.CommandTimeout(600))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).Options).As<DbContextOptions<EasContext>>().SingleInstance();

            containerBuilder.RegisterType<PostcodesContext>().As<IPostcodesContext>();
            containerBuilder.Register(container => new DbContextOptionsBuilder<PostcodesContext>()
                .UseSqlServer(_databaseConfiguration.PostcodesDbConnectionString, sqlServerOptions => sqlServerOptions.CommandTimeout(600))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).Options).As<DbContextOptions<PostcodesContext>>().SingleInstance();

            containerBuilder.RegisterType<ILR1920_DataStoreEntities>().As<IILR1920_DataStoreEntities>();
            containerBuilder.Register(container => new DbContextOptionsBuilder<ILR1920_DataStoreEntities>()
                .UseSqlServer(_databaseConfiguration.IlrDbConnectionString, sqlServerOptions => sqlServerOptions.CommandTimeout(600))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).Options).As<DbContextOptions<ILR1920_DataStoreEntities>>().SingleInstance();

            containerBuilder.RegisterType<ILR1920_DataStoreEntitiesValid>().As<IILR1920_DataStoreEntitiesValid>();
            containerBuilder.Register(container => new DbContextOptionsBuilder<ILR1920_DataStoreEntitiesValid>()
                .UseSqlServer(_databaseConfiguration.IlrDbConnectionString, sqlServerOptions => sqlServerOptions.CommandTimeout(600))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).Options).As<DbContextOptions<ILR1920_DataStoreEntitiesValid>>().SingleInstance();
        }
    }
}
