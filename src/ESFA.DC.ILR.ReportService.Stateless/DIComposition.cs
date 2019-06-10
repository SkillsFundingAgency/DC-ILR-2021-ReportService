﻿using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Features.AttributeFilters;
using ESFA.DC.DASPayments.EF;
using ESFA.DC.DASPayments.EF.Interfaces;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.FileService;
using ESFA.DC.FileService.Config;
using ESFA.DC.FileService.Config.Interface;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.ReportService.Interface.Builders;
using ESFA.DC.ILR.ReportService.Interface.Builders.PeriodEnd;
using ESFA.DC.ILR.ReportService.Interface.Configuration;
using ESFA.DC.ILR.ReportService.Interface.DataMatch;
using ESFA.DC.ILR.ReportService.Interface.Provider;
using ESFA.DC.ILR.ReportService.Interface.Reports;
using ESFA.DC.ILR.ReportService.Interface.Service;
using ESFA.DC.ILR.ReportService.Service;
using ESFA.DC.ILR.ReportService.Service.Builders;
using ESFA.DC.ILR.ReportService.Service.Builders.PeriodEnd;
using ESFA.DC.ILR.ReportService.Service.BusinessRules;
using ESFA.DC.ILR.ReportService.Service.Commands.AppsIndicativeEarnings;
using ESFA.DC.ILR.ReportService.Service.Provider;
using ESFA.DC.ILR.ReportService.Service.Reports;
using ESFA.DC.ILR.ReportService.Service.Reports.PeriodEnd;
using ESFA.DC.ILR.ReportService.Service.Service;
using ESFA.DC.ILR.ReportService.Service.Service.DataMatch;
using ESFA.DC.ILR1819.DataStore.EF;
using ESFA.DC.ILR1819.DataStore.EF.Interface;
using ESFA.DC.ILR1819.DataStore.EF.Valid;
using ESFA.DC.ILR1819.DataStore.EF.Valid.Interface;
using ESFA.DC.ILR.ReportService.Stateless.Configuration;
using ESFA.DC.ILR.ReportService.Stateless.Handlers;
using ESFA.DC.ILR.ReportService.Stateless.Interfaces;
using ESFA.DC.IO.AzureStorage;
using ESFA.DC.IO.AzureStorage.Config.Interfaces;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.JobContextManager;
using ESFA.DC.JobContextManager.Interface;
using ESFA.DC.JobContextManager.Model;
using ESFA.DC.Mapping.Interface;
using ESFA.DC.ReferenceData.FCS.Model;
using ESFA.DC.ReferenceData.FCS.Model.Interface;
using ESFA.DC.ServiceFabric.Common.Config.Interface;
using ESFA.DC.ServiceFabric.Common.Modules;
using Microsoft.EntityFrameworkCore;
using VersionInfo = ESFA.DC.ILR.ReportService.Stateless.Configuration.VersionInfo;

namespace ESFA.DC.ILR.ReportService.Stateless
{
    public static class DIComposition
    {
        public static ContainerBuilder BuildContainer(IServiceFabricConfigurationService serviceFabricConfigurationService)
        {
            var containerBuilder = new ContainerBuilder();

            var statelessServiceConfiguration = serviceFabricConfigurationService.GetConfigSectionAsStatelessServiceConfiguration();

            var reportServiceConfiguration = serviceFabricConfigurationService.GetConfigSectionAs<ReportServiceConfiguration>("ReportServiceConfiguration");
            containerBuilder.RegisterInstance(reportServiceConfiguration).As<IReportServiceConfiguration>();

            // register azure blob storage service
            var azureBlobStorageOptions = serviceFabricConfigurationService.GetConfigSectionAs<AzureStorageOptions>("AzureStorageSection");
            containerBuilder.RegisterInstance(azureBlobStorageOptions).As<IAzureStorageOptions>();
            containerBuilder.Register(c =>
                    new AzureStorageKeyValuePersistenceConfig(
                        azureBlobStorageOptions.AzureBlobConnectionString,
                        azureBlobStorageOptions.AzureBlobContainerName))
                .As<IAzureStorageKeyValuePersistenceServiceConfig>().SingleInstance();

            containerBuilder.RegisterType<AzureStorageKeyValuePersistenceService>()
                .As<IStreamableKeyValuePersistenceService>()
                .InstancePerLifetimeScope();

            var azureStorageFileServiceConfiguration = new AzureStorageFileServiceConfiguration()
            {
                ConnectionString = azureBlobStorageOptions.AzureBlobConnectionString,
            };

            containerBuilder.RegisterInstance(azureStorageFileServiceConfiguration).As<IAzureStorageFileServiceConfiguration>();
            containerBuilder.RegisterType<AzureStorageFileService>().As<IFileService>();

            containerBuilder.RegisterModule(new StatelessServiceModule(statelessServiceConfiguration));
            containerBuilder.RegisterModule<SerializationModule>();

            var versionInfo = serviceFabricConfigurationService.GetConfigSectionAs<VersionInfo>("VersionSection");
            containerBuilder.RegisterInstance(versionInfo).As<IVersionInfo>().SingleInstance();

            // register message mapper
            containerBuilder.RegisterType<DefaultJobContextMessageMapper<JobContextMessage>>().As<IMapper<JobContextMessage, JobContextMessage>>();

            // register MessageHandler
            containerBuilder.RegisterType<MessageHandler>().As<IMessageHandler<JobContextMessage>>().InstancePerLifetimeScope();

            containerBuilder.RegisterType<EntryPoint>().WithAttributeFiltering().InstancePerLifetimeScope();
            containerBuilder.RegisterType<ZipService>().As<IZipService>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<ReportsProvider>().As<IReportsProvider>().InstancePerLifetimeScope();

            containerBuilder.RegisterType<ILR1819_DataStoreEntitiesValid>().As<IIlr1819ValidContext>();
            containerBuilder.Register(context =>
                {
                    var optionsBuilder = new DbContextOptionsBuilder<ILR1819_DataStoreEntitiesValid>();
                    optionsBuilder.UseSqlServer(
                        reportServiceConfiguration.ILRDataStoreValidConnectionString,
                        options => options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>()));

                    return optionsBuilder.Options;
                })
                .As<DbContextOptions<ILR1819_DataStoreEntitiesValid>>()
                .SingleInstance();

            containerBuilder.RegisterType<ILR1819_DataStoreEntities>().As<IIlr1819RulebaseContext>();
            containerBuilder.Register(context =>
                {
                    var optionsBuilder = new DbContextOptionsBuilder<ILR1819_DataStoreEntities>();
                    optionsBuilder.UseSqlServer(
                        reportServiceConfiguration.ILRDataStoreConnectionString,
                        options => options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>()));

                    return optionsBuilder.Options;
                })
                .As<DbContextOptions<ILR1819_DataStoreEntities>>()
                .SingleInstance();

            containerBuilder.RegisterType<DASPaymentsContext>().As<IDASPaymentsContext>();
            containerBuilder.Register(context =>
                {
                    var optionsBuilder = new DbContextOptionsBuilder<DASPaymentsContext>();
                    optionsBuilder.UseSqlServer(
                        reportServiceConfiguration.DASPaymentsConnectionString,
                        options => options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>()));

                    return optionsBuilder.Options;
                })
                .As<DbContextOptions<DASPaymentsContext>>()
                .SingleInstance();

            containerBuilder.RegisterType<FcsContext>().As<IFcsContext>();
            containerBuilder.Register(context =>
                {
                    var optionsBuilder = new DbContextOptionsBuilder<FcsContext>();
                    optionsBuilder.UseSqlServer(
                        reportServiceConfiguration.FCSConnectionString,
                        options => options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>()));

                    return optionsBuilder.Options;
                })
                .As<DbContextOptions<FcsContext>>()
                .SingleInstance();

            containerBuilder.RegisterType<DateTimeProvider.DateTimeProvider>().As<IDateTimeProvider>().InstancePerLifetimeScope();

            RegisterReports(containerBuilder);
            RegisterServices(containerBuilder);
            RegisterBuilders(containerBuilder);
            RegisterRules(containerBuilder);
            RegisterCommands(containerBuilder);

           return containerBuilder;
        }

        public static void RegisterServicesByCollectionName(string collectionName, ContainerBuilder containerBuilder)
        {
            if (collectionName.Equals("ILR1819", StringComparison.OrdinalIgnoreCase))
            {
                RegisterILRServices(containerBuilder);
            }
            else
            {
                RegisterNonILRServices(containerBuilder);
            }
        }

        private static void RegisterILRServices(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<FM36FileServiceProvider>().As<IFM36ProviderService>()
                .WithAttributeFiltering()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<FM36FileServiceProvider>().As<IFM36NonContractedActivityProviderService>()
                .WithAttributeFiltering()
                .InstancePerLifetimeScope();
        }

        private static void RegisterNonILRServices(ContainerBuilder containerBuilder)
        {
           containerBuilder.RegisterType<FM36SqlProviderService>().As<IFM36ProviderService>()
                .WithAttributeFiltering()
                .InstancePerLifetimeScope();
        }

        private static void RegisterReports(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<AllbOccupancyReport>().As<IReport>()
                .WithAttributeFiltering()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<MainOccupancyReport>().As<IReport>()
                .WithAttributeFiltering()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<FundingSummaryReport>().As<IReport>()
                .WithAttributeFiltering()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<AdultFundingClaimReport>().As<IReport>()
                .WithAttributeFiltering()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<SummaryOfFunding1619Report>().As<IReport>()
                .WithAttributeFiltering()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<MathsAndEnglishReport>().As<IReport>()
                .WithAttributeFiltering()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<ValidationErrorsReport>().As<IReport>()
                .WithAttributeFiltering()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<SummaryOfFm35FundingReport>().As<IReport>()
                .WithAttributeFiltering()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<AppsAdditionalPaymentsReport>().As<IReport>()
                .WithAttributeFiltering()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<NonContractedAppsActivityReport>().As<IReport>()
                .WithAttributeFiltering()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<ILRDataQualityReport>().As<IReport>()
                .WithAttributeFiltering()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<AppsIndicativeEarningsReport>().As<IReport>()
                .WithAttributeFiltering()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<DataMatchReport>().As<IReport>()
                .WithAttributeFiltering()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<TrailblazerEmployerIncentivesReport>().As<IReport>()
                .WithAttributeFiltering()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<FundingClaim1619Report>().As<IReport>()
                .WithAttributeFiltering()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<AppsCoInvestmentContributionsReport>().As<IReport>()
                .WithAttributeFiltering()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<AppsMonthlyPaymentReport>().As<IReport>()
                .WithAttributeFiltering()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<AppsDataMatchMonthEndReport>().As<IReport>()
                .WithAttributeFiltering()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<FundingSummaryPeriodEndReport>().As<IReport>()
                .WithAttributeFiltering()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<TrailblazerAppsOccupancyReport>().As<IReport>()
                .WithAttributeFiltering()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<HNSDetailReport>().As<IReport>()
                .WithAttributeFiltering()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<SummaryOfFm35FundingReport>().As<IReport>()
                .WithAttributeFiltering()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<HNSSummaryReport>().As<IReport>()
                .WithAttributeFiltering()
                .InstancePerLifetimeScope();

            containerBuilder.Register(c => new List<IReport>(c.Resolve<IEnumerable<IReport>>()))
                .As<IList<IReport>>();
        }

        private static void RegisterServices(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<FM36FileServiceProvider>().As<IFM36ProviderService>()
                .WithAttributeFiltering()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<FM36FileServiceProvider>().As<IFM36NonContractedActivityProviderService>()
                .WithAttributeFiltering()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<IlrFileServiceProvider>().As<IIlrProviderService>()
                .WithAttributeFiltering()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<IlrMetadataProviderService>().As<IIlrMetadataProviderService>()
                .WithAttributeFiltering()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<IlrPeriodEndProviderService>().As<IIlrPeriodEndProviderService>()
                .WithAttributeFiltering()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<DASPaymentsProviderService>().As<IDASPaymentsProviderService>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<LarsProviderService>().As<ILarsProviderService>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<FCSProviderService>().As<IFCSProviderService>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<AllbProviderService>().As<IAllbProviderService>()
                .WithAttributeFiltering()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<FM25ProviderService>().As<IFM25ProviderService>()
                .WithAttributeFiltering()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<FM35ProviderService>().As<IFM35ProviderService>()
                .WithAttributeFiltering()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<FM36PeriodEndProviderService>().As<IFM36PeriodEndProviderService>()
                .WithAttributeFiltering()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<FM81TrailBlazerProviderService>().As<IFM81TrailBlazerProviderService>()
                .WithAttributeFiltering()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<ValidLearnersService>().As<IValidLearnersService>()
                .WithAttributeFiltering()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<InvalidLearnersService>().As<IInvalidLearnersService>()
                .WithAttributeFiltering()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<StringUtilitiesService>().As<IStringUtilitiesService>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<IntUtilitiesService>().As<IIntUtilitiesService>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<OrgProviderService>().As<IOrgProviderService>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<PeriodProviderService>().As<IPeriodProviderService>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<DasCommitmentsService>().As<IDasCommitmentsService>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<EasProviderService>().As<IEasProviderService>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<PostcodeProviderService>().As<IPostcodeProviderService>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<LargeEmployerProviderService>().As<ILargeEmployerProviderService>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<ExcelStyleProvider>().As<IExcelStyleProvider>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<CacheProviderService<ILR.FundingService.FM35.FundingOutput.Model.Output.LearningDelivery[]>>().As<ICacheProviderService<ILR.FundingService.FM35.FundingOutput.Model.Output.LearningDelivery[]>>()
                .InstancePerDependency();
            containerBuilder.RegisterType<CacheProviderService<ILR.FundingService.FM36.FundingOutput.Model.Output.LearningDelivery[]>>().As<ICacheProviderService<ILR.FundingService.FM36.FundingOutput.Model.Output.LearningDelivery[]>>()
                .InstancePerDependency();
            containerBuilder.RegisterType<CacheProviderService<ILR.FundingService.FM81.FundingOutput.Model.Output.LearningDelivery[]>>().As<ICacheProviderService<ILR.FundingService.FM81.FundingOutput.Model.Output.LearningDelivery[]>>()
                .InstancePerDependency();

            containerBuilder.RegisterType<ValueProvider>().As<IValueProvider>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<ValidationStageOutputCache>().As<IValidationStageOutputCache>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<ValidationErrorsService>().As<IValidationErrorsService>()
                .InstancePerLifetimeScope();
        }

        private static void RegisterBuilders(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<MathsAndEnglishModelBuilder>().As<IMathsAndEnglishModelBuilder>()
                .InstancePerLifetimeScope();
            containerBuilder.RegisterType<MainOccupancyReportModelBuilder>().As<IMainOccupancyReportModelBuilder>()
                .InstancePerLifetimeScope();
            containerBuilder.RegisterType<AppsAdditionalPaymentsModelBuilder>().As<IAppsAdditionalPaymentsModelBuilder>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<NonContractedAppsActivityModelBuilder>().As<INonContractedAppsActivityModelBuilder>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<AppsMonthlyPaymentModelBuilder>().As<IAppsMonthlyPaymentModelBuilder>()
                .InstancePerLifetimeScope();
            containerBuilder.RegisterType<AppsDataMatchMonthEndModelBuilder>().As<IAppsDataMatchMonthEndModelBuilder>()
                .InstancePerLifetimeScope();
            containerBuilder.RegisterType<FundingSummaryPeriodEndModelBuilder>().As<IFundingSummaryPeriodEndModelBuilder>()
                .InstancePerLifetimeScope();
            containerBuilder.RegisterType<AppsIndicativeEarningsModelBuilder>().As<IAppsIndicativeEarningsModelBuilder>()
                .InstancePerLifetimeScope();
            containerBuilder.RegisterType<DasCommitmentBuilder>().As<IDasCommitmentBuilder>()
                .InstancePerLifetimeScope();
            containerBuilder.RegisterType<Fm25Builder>().As<IFm25Builder>()
                .InstancePerLifetimeScope();
            containerBuilder.RegisterType<Fm35Builder>().As<IFm35Builder>()
                .InstancePerLifetimeScope();
            containerBuilder.RegisterType<Fm36Builder>().As<IFm36Builder>()
                .InstancePerLifetimeScope();
            containerBuilder.RegisterType<Fm81Builder>().As<IFm81Builder>()
                .InstancePerLifetimeScope();
            containerBuilder.RegisterType<AllbBuilder>().As<IAllbBuilder>()
                .InstancePerLifetimeScope();
            containerBuilder.RegisterType<TotalBuilder>().As<ITotalBuilder>()
                .InstancePerLifetimeScope();
            containerBuilder.RegisterType<EasBuilder>().As<IEasBuilder>()
                .InstancePerLifetimeScope();
            containerBuilder.RegisterType<DatalockValidationResultBuilder>().As<IDatalockValidationResultBuilder>()
                .InstancePerLifetimeScope();
            containerBuilder.RegisterType<AdultFundingClaimBuilder>().As<IAdultFundingClaimBuilder>()
                .InstancePerLifetimeScope();
            containerBuilder.RegisterType<TrailblazerEmployerIncentivesModelBuilder>().As<ITrailblazerEmployerIncentivesModelBuilder>()
                .InstancePerLifetimeScope();
            containerBuilder.RegisterType<TrailblazerAppsOccupancyModelBuilder>().As<ITrailblazerAppsOccupancyModelBuilder>()
                .InstancePerLifetimeScope();
            containerBuilder.RegisterType<HNSReportModelBuilder>().As<IHNSReportModelBuilder>()
                .InstancePerLifetimeScope();
            containerBuilder.RegisterType<FundingSummaryPeriodEndModelBuilder>().As<IFundingSummaryPeriodEndModelBuilder>()
                .InstancePerLifetimeScope();
            containerBuilder.RegisterType<HNSSummaryModelBuilder>().As<IHNSSummaryModelBuilder>()
                .InstancePerLifetimeScope();
        }

        private static void RegisterRules(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<MathsAndEnglishFm25Rules>().As<IMathsAndEnglishFm25Rules>()
                .InstancePerLifetimeScope();
        }

        private static void RegisterCommands(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<AppsIndicativeAugustCommand>().As<IAppsIndicativeCommand>()
                .InstancePerLifetimeScope();
            containerBuilder.RegisterType<AppsIndicativeSeptemberCommand>().As<IAppsIndicativeCommand>()
                .InstancePerLifetimeScope();
            containerBuilder.RegisterType<AppsIndicativeOctoberCommand>().As<IAppsIndicativeCommand>()
                .InstancePerLifetimeScope();
            containerBuilder.RegisterType<AppsIndicativeNovemberCommand>().As<IAppsIndicativeCommand>()
                .InstancePerLifetimeScope();
            containerBuilder.RegisterType<AppsIndicativeDecemberCommand>().As<IAppsIndicativeCommand>()
                .InstancePerLifetimeScope();
            containerBuilder.RegisterType<AppsIndicativeJanuaryCommand>().As<IAppsIndicativeCommand>()
                .InstancePerLifetimeScope();
            containerBuilder.RegisterType<AppsIndicativeFebruaryCommand>().As<IAppsIndicativeCommand>()
                .InstancePerLifetimeScope();
            containerBuilder.RegisterType<AppsIndicativeMarchCommand>().As<IAppsIndicativeCommand>()
                .InstancePerLifetimeScope();
            containerBuilder.RegisterType<AppsIndicativeAprilCommand>().As<IAppsIndicativeCommand>()
                .InstancePerLifetimeScope();
            containerBuilder.RegisterType<AppsIndicativeMayCommand>().As<IAppsIndicativeCommand>()
                .InstancePerLifetimeScope();
            containerBuilder.RegisterType<AppsIndicativeJuneCommand>().As<IAppsIndicativeCommand>()
                .InstancePerLifetimeScope();
            containerBuilder.RegisterType<AppsIndicativeJulyCommand>().As<IAppsIndicativeCommand>()
                .InstancePerLifetimeScope();

            containerBuilder.Register(c => new List<IAppsIndicativeCommand>(c.Resolve<IEnumerable<IAppsIndicativeCommand>>()))
                .As<IList<IAppsIndicativeCommand>>();
        }
    }
}
