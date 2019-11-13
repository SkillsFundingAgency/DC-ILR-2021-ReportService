using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.EAS1920.EF;
using ESFA.DC.EAS1920.EF.Interface;
using ESFA.DC.ILR.Constants;
using ESFA.DC.ILR.ReportService.Interface.Configuration;
using ESFA.DC.ILR.ReportService.Modules;
using ESFA.DC.ILR.ReportService.Service;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Stateless.Configuration;
using ESFA.DC.ILR.ReportService.Stateless.Context;
using ESFA.DC.ILR1920.DataStore.EF;
using ESFA.DC.ILR1920.DataStore.EF.Interface;
using ESFA.DC.ILR1920.DataStore.EF.Valid;
using ESFA.DC.ILR1920.DataStore.EF.Valid.Interface;
using ESFA.DC.IO.AzureStorage.Config.Interfaces;
using ESFA.DC.JobContextManager.Interface;
using ESFA.DC.JobContextManager.Model;
using ESFA.DC.JobContextManager.Model.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.ServiceFabric.Common.Config;
using ESFA.DC.ServiceFabric.Common.Config.Interface;
using Microsoft.EntityFrameworkCore;
using ExecutionContext = ESFA.DC.Logging.ExecutionContext;

namespace ESFA.DC.ILR.ReportService.Stateless.Handlers
{
    public sealed class MessageHandler : IMessageHandler<JobContextMessage>
    {
        private readonly ILifetimeScope _parentLifeTimeScope;
        private readonly StatelessServiceContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandler"/> class.
        /// Simple constructor for use by AutoFac testing, don't want to have to fake a @see StatelessServiceContext
        /// </summary>
        /// <param name="parentLifeTimeScope">AutoFac scope</param>
        public MessageHandler(ILifetimeScope parentLifeTimeScope)
        {
            _parentLifeTimeScope = parentLifeTimeScope;
            _context = null;
        }

        public MessageHandler(ILifetimeScope parentLifeTimeScope, StatelessServiceContext context)
        {
            _parentLifeTimeScope = parentLifeTimeScope;
            _context = context;
        }

        public async Task<bool> HandleAsync(JobContextMessage jobContextMessage, CancellationToken cancellationToken)
        {
            try
            {
                using (var childLifeTimeScope = GetChildLifeTimeScope(jobContextMessage))
                {
                    var executionContext = (ExecutionContext)childLifeTimeScope.Resolve<IExecutionContext>();
                    var versionInfo = childLifeTimeScope.Resolve<IVersionInfo>();

                    executionContext.JobId = jobContextMessage.JobId.ToString();
                    var logger = childLifeTimeScope.Resolve<ILogger>();
                    logger.LogDebug("Started Report Service");

                    ////Legacy
                    //var entryPoint = childLifeTimeScope.Resolve<LegacyEntryPoint>();
                    //var result = await entryPoint.Callback(new ReportServiceJobContextMessageContext(jobContextMessage), cancellationToken);

                    var entryPoint = childLifeTimeScope.Resolve<IEntryPoint>();

                    var result = await entryPoint.Callback(new ReportServiceJobContextMessageContext(jobContextMessage, versionInfo), cancellationToken);

                    logger.LogDebug($"Completed Report Service");
                    return true;
                }
            }
            catch (OutOfMemoryException oom)
            {
                Environment.FailFast("Report Service Out Of Memory", oom);
                throw;
            }
            catch (Exception ex)
            {
                ServiceEventSource.Current.ServiceMessage(_context, "Exception-{0}", ex.ToString());
                throw;
            }
        }

        public ILifetimeScope GetChildLifeTimeScope(JobContextMessage jobContextMessage)
        {
            return _parentLifeTimeScope.BeginLifetimeScope(c =>
            {
                c.RegisterInstance(jobContextMessage).As<IJobContextMessage>();

                var azureBlobStorageOptions = _parentLifeTimeScope.Resolve<IAzureStorageOptions>();
                c.RegisterInstance(new AzureStorageKeyValuePersistenceConfig(
                        azureBlobStorageOptions.AzureBlobConnectionString,
                        jobContextMessage.KeyValuePairs[ILRContextKeys.Container].ToString()))
                    .As<IAzureStorageKeyValuePersistenceServiceConfig>();

                switch (jobContextMessage.KeyValuePairs["CollectionName"].ToString())
                {
                    case "ILR1920":
                        c.RegisterModule<DataModule>();
                        break;

                    case "EAS":
                        c.RegisterModule<EasDataModule>();

                        // register Eas database
                        IServiceFabricConfigurationService serviceFabricConfigurationService = new ServiceFabricConfigurationService();
                        var databaseConfiguration = serviceFabricConfigurationService.GetConfigSectionAs<DatabaseConfiguration>("DatabaseConfiguration");

                        c.RegisterType<EasContext>().As<IEasdbContext>();
                        c.Register(container => new DbContextOptionsBuilder<EasContext>()
                            .UseSqlServer(databaseConfiguration.EasDbConnectionString)
                            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).Options).As<DbContextOptions<EasContext>>().SingleInstance();

                        c.RegisterType<ILR1920_DataStoreEntities>().As<IILR1920_DataStoreEntities>();
                        c.Register(container => new DbContextOptionsBuilder<ILR1920_DataStoreEntities>()
                            .UseSqlServer(databaseConfiguration.IlrDbConnectionString)
                            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).Options).As<DbContextOptions<ILR1920_DataStoreEntities>>().SingleInstance();

                        c.RegisterType<ILR1920_DataStoreEntitiesValid>().As<IILR1920_DataStoreEntitiesValid>();
                        c.Register(container => new DbContextOptionsBuilder<ILR1920_DataStoreEntitiesValid>()
                            .UseSqlServer(databaseConfiguration.IlrDbConnectionString)
                            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).Options).As<DbContextOptions<ILR1920_DataStoreEntitiesValid>>().SingleInstance();
                        break;
                }
            });
        }
    }
}
